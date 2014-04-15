//#define SIP_MODE // Uncomment to use SIP.

using System;
using System.IO;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Core = Microsoft.Office.Core;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using PPTPaneManagement;
using PowerPointRegions;
#if SIP_MODE
using PPTPropertyManagement;
#endif
using System.Collections;

namespace PPTDeckBuilder
{
	/// <summary>
	/// The PPTSlideLoader gets the powerpoint slides from a file and transforms them into the appropriate format.
	/// </summary>
	public class PPTSlideLoader 
	{
		#region Constants

		public const string DEFAULT_IMAGE_FORMAT = "EMF";
		public static readonly Size DEFAULT_IMAGE_SIZE = new Size(0, 0);

		#endregion

		#region Type defns.

		public enum MaximumImageSizeType { Large, Medium, Small };

		#endregion

		#region Members

		private string myImageFormat = DEFAULT_IMAGE_FORMAT;
		private bool myHighResolutionImage = true;
		private MaximumImageSizeType myMaximumImageSize = MaximumImageSizeType.Large;

		private Size myViewerSize;
		private int myDefaultCommentMenuType;

		// Keep track of the number of files loaded so that we can use separate directories
		private int myFilesLoaded;

		// We need a directory for the temporary files.  There has been a problem with not being able to close 
		// ppt (or at least, not free up resources).  The current attempt will be store in a temporary directory
		// which is cleared before storing the files (this means that the number of files should not
		// build up over time, even if some garbage is left
		private string myTemporaryDirectoryPath;

		private SlideViewer.Slide[] mySlideArray;

		private PPTLibrary.PPT myPPT;

		/// <summary>
		/// Manager of pane visibility. If null, loader assumes no pane visibility functionality.
		/// </summary>
		private PPTPaneManagement.PPTPaneManager myPaneManager;

#if SIP_MODE
		/// <summary>
		/// Manager of widget properties. If null, loader assumes no widgets.
		/// </summary>
		private PPTPropertyManagement.PPTPropertyManager myPropertyManager;
#endif

		#endregion

		#region Public Properties

		public SlideViewer.Slide[] SlideArray 
		{
			get { return this.mySlideArray; }
		}

		/// <summary>
		/// The number of slides stored in the deck.  A slide count of zero is used as an error condition
		/// </summary>
		public int SlideCount 
		{
			get 
			{ 
				if (this.mySlideArray == null)
					return 0;
				else
					return this.mySlideArray.Length;
			}
		}

		public string ImageFormat 
		{
			get { return this.myImageFormat; }
			set { this.myImageFormat = value; }
		}

		public bool HighResolutionImage 
		{
			get { return this.myHighResolutionImage; }
			set { this.myHighResolutionImage = value; }
		}

		public MaximumImageSizeType MaximumImageSize 
		{
			get { return this.myMaximumImageSize; }
			set { this.myMaximumImageSize = value; }
		}

		#endregion

		#region Internals

		private int SlideSizeCutoff(MaximumImageSizeType size)
		{
			switch (size)
			{
				case MaximumImageSizeType.Large:
					return 600000;
				case MaximumImageSizeType.Medium:
					return 300000;
				case MaximumImageSizeType.Small:
					return 100000;
			}
			return 0;
		}

        protected Size CompressedImageSize(MaximumImageSizeType size, int width, int height)
        {
            int width_out = 0;
            int height_out = 0;
            int max_size = 0;

            // Check for divide by zero
            if( height == 0 )
                return new Size(0,0);

            // Determine how big we want the resulting image to be
            switch (size)
            {
                case MaximumImageSizeType.Large:
                    max_size = 600*450;
                    break;
                case MaximumImageSizeType.Medium:
                    max_size = 452*339;
                    break;
                case MaximumImageSizeType.Small:
                    max_size = 300*225;
                    break;
                default:
                    return new Size(0,0);
            }

            // Scale the width and height to maintain aspect ratio but be no bigger than max_size
            width_out = (int)Math.Sqrt( (float)max_size * ((float)width/(float)height) );
            height_out = (int)((float)width_out / ((float)width/(float)height));

            return new Size(width_out, height_out);
        }

#if SIP_MODE
		private SlideViewer.Slide.WidgetInfo GetWidgetInfo(PowerPoint.Shape shape)
		{
			// Get the modes.
			string[] stringModes = this.myPaneManager.GetModes(shape);
			SlideViewer.SlideMode[] modes = new SlideViewer.SlideMode[stringModes.Length];
			for (int i = 0; i < stringModes.Length; i++)
			{
				if (stringModes[i] == "")
					modes[i] = SlideViewer.SlideMode.Default;
				else
					modes[i] = new SlideViewer.SlideMode(stringModes[i]);
			}

			// Get the property names.
			string[] names = this.myPropertyManager.GetProperties(shape);
			
			// Get the property types/values/queries.
			SlideViewer.Slide.WidgetPropertyTypeInfo[] types = 
				new SlideViewer.Slide.WidgetPropertyTypeInfo[names.Length];
			object[] values = new object[names.Length];
			string[] queries = new string[names.Length];
			for (int i = 0; i < names.Length; i++)
			{
				PropertyType type = this.myPropertyManager.GetPropertyType(shape, names[i]);
				ValueObject vo = (ValueObject)this.myPropertyManager.GetPropertyValue(shape, names[i]);
				if (vo == null)
					vo = new ValueObject(null, null);

				int freqAsInt = 2;
				switch (type.UpdateFrequency)
				{
					case UpdateFrequency.OFTEN:
						freqAsInt = 2;
						break;
					case UpdateFrequency.ON_EVENT:
						freqAsInt = 1;
						break;
					case UpdateFrequency.ONCE:
						freqAsInt = 0;
						break;
				}

				// Manufacture a type.
				types[i] = new SlideViewer.Slide.WidgetPropertyTypeInfo(
					type.IsInput, 
					!type.IsScalar, 
					freqAsInt, 
					vo.Query != null);

				// Manufacture a value, creating a default if necessary.
				if (vo.Value == null)
					values[i] = type.IsScalar ? (object)"" : (object)new string[] { };
				else
					values[i] = vo.Value;

				// Manufacture a query.
				queries[i] = vo.Query; // allowed to be null.
			}

			return new SlideViewer.Slide.WidgetInfo(modes, names, types, values, queries);
		}

		private bool IsWidget(PowerPoint.Shape shape)
		{
			// Use the "name" property to decide whether this is a widget or not.
			return this.myPropertyManager != null &&
				this.myPropertyManager.HasProperty(shape, "name");
		}
#endif

		// Store the slides and the jpgs of a presentation into the arrays
		// Here is a problem to fix - slide export seems to leave the file open, so if a file 
		// name is reused, then there is a currently in use error.  The ugly fix is to keep generating new
		// file names.  Attempts to close the application / presentation don't seem to fix this problem.
		//
		// The images come from exporting each slide to an internal image format (see DEFAULT_IMAGE_FORMAT) and reading back in.
		//
		// We create a new directory each time we call BSD, so we pass in a generation number
		private void BuildSlideDeck(PowerPoint.Presentation ppPresentation,  int loadNumber, bool constructGeometry) 
		{
			// Prep the temporary storage area.
			string dirPath = this.myTemporaryDirectoryPath +"\\pptFiles" + loadNumber + "\\" + System.Diagnostics.Process.GetCurrentProcess().Id;
			string filePath = dirPath +"\\";
			Directory.CreateDirectory(dirPath);

			// Grab some ppt info.
			PowerPoint.Slides ppSlides = ppPresentation.Slides;
			int nSlides = ppSlides.Count;

			// Degenerate case.
			if (nSlides == 0)
				return;				// If there are no slides, we already have an empty deck
			
			// Note: throughout this process, be wary of invisible shapes.

			IList /* of PowerPoint.Shape */ widgets = new ArrayList();
			IList[] /* of string/mode */ slideModes = new IList[nSlides];
			IList /* of string/mode */ allModes = new ArrayList();
			SlideViewer.Slide[] slides = new SlideViewer.Slide[nSlides];

			// Record the current mode to restore it later; null if no pane manager.
			string currentMode = this.myPaneManager == null ? null : this.myPaneManager.Mode;

			// In parallel:
			// Note all widgets so they can be hidden. Enter the widgets as they go.
			// Loop through the slides, determining which mode we might want each slide in.
			Stack /* of PowerPoint.Shape */ shapes = new Stack();
			for (int i = 0; i < nSlides; i++)
			{
				slides[i] = new SlideViewer.Slide();
				slideModes[i] = new ArrayList();
				slideModes[i].Add("");  // Every slide is required to have a default mode
				IList /* of SlideViewer.Slide.WidgetInfo */ widgetInfos = new ArrayList();

				foreach (PowerPoint.Shape shape in ppSlides[i+1].Shapes)
					shapes.Push(shape);

				// Iterate over the shapes, throwing groups back into the list.
				while (shapes.Count > 0)
				{
					PowerPoint.Shape shape = (PowerPoint.Shape)shapes.Pop();
					PowerPoint.GroupShapes subgroup = this.myPPT.GetGroupItems(shape);
					if (subgroup == null)
					{
						// It's a leaf shape.

						// Ignore invisible objects EXCEPT restricted objects 
						// (which are only invisible b/c they are not in mode).
						if (shape.Visible != Core.MsoTriState.msoTrue && 
							(this.myPaneManager == null || !this.myPaneManager.IsRestricted(shape)))
							continue;
						
#if SIP_MODE
						if (this.IsWidget(shape))
						{
							// It's a widget.
							widgets.Add(shape);
							widgetInfos.Add(this.GetWidgetInfo(shape));
						}
						else
#endif
						{
							// It's not a widget.
							if (this.myPaneManager != null)
								foreach (string mode in this.myPaneManager.GetModes(shape))
									if (!slideModes[i].Contains(mode))
										slideModes[i].Add(mode);
						}
					}
					else
					{
						// It's a group.
						foreach (PowerPoint.Shape subshape in subgroup)
							shapes.Push(subshape);
					}
				}

				// Put all collected information into the slide.
				foreach (string mode in slideModes[i])
					if (!allModes.Contains(mode))
						allModes.Add(mode);
#if SIP_MODE
				SlideViewer.Slide.WidgetInfo[] infos = new SlideViewer.Slide.WidgetInfo[widgetInfos.Count];
				widgetInfos.CopyTo(infos, 0);
				slides[i].Widgets = infos;
#endif
			}

			// In sequence, set the mode to each relevant mode, skipping to the slides that need entries in
			// that mode and entering them.
			int slidesTooBig = 0;
			if (allModes.Contains(currentMode))
			{
				// Move it to the start to avoid switching an extra time.
				// (Counts on assignment of a mode that's already in place being a no-op.)
				int index = allModes.IndexOf(currentMode);
				allModes[index] = allModes[0];
				allModes[0] = currentMode;
			}

			if (this.myPaneManager != null)
				this.myPaneManager.Mode = "";
			RegionBuilder regionBuilder = new RegionBuilder(ppPresentation);
			RegionTreeToOutlinesConverter regionConverter = new RegionTreeToOutlinesConverter(ppPresentation.SlideMaster.Width, ppPresentation.SlideMaster.Height);

			foreach (string mode in allModes)
			{
				if (this.myPaneManager != null)
					this.myPaneManager.Mode = mode;
				foreach (PowerPoint.Shape shape in widgets)
					shape.Visible = Core.MsoTriState.msoFalse;
				for (int i = 0; i < nSlides; i++)
					if (slideModes[i].Contains(mode))
					{
						PowerPoint.Slide ppSlide = ppSlides[i+1];  // Get the powerpoint slide
						string imageFileName = filePath + "PPTempFile" + (i+1) + "." + mode + "." + ImageFormat;
						Image image = ConstructSlideImage(ppSlide, imageFileName, ImageFormat, 
							this.HighResolutionImage ? 0 : 400, this.HighResolutionImage ? 0 : 300, ref slidesTooBig);
						if (mode == "")
						{
							slides[i].SetImage(SlideViewer.SlideMode.Default, image);
							// Set the title in the (required) default mode.
							slides[i].Title = String.Format("{0,5}", (i+1) + ". ") + GetSlideTitle(ppSlides[i+1]);
							// TODO: ensure that the actual construction work is done while in an appropriate mode (default?)
							if (constructGeometry)
							{
								SlideViewer.Outline[] outlines = regionConverter.ToFlatOutlines(regionBuilder.SlideTrees[i]);
								SlideViewer.SlideGeometry geom = new SlideViewer.SlideGeometry(outlines);
								geom.CommentMenuType = this.myDefaultCommentMenuType;
								slides[i].CommentMenuType = this.myDefaultCommentMenuType;
								slides[i].Geometry = geom;
							}
						}
						else
							slides[i].SetImage(new SlideViewer.SlideMode(mode), image);
					}
			}
			if (slidesTooBig > 0)
				MessageBox.Show("Oversized slides\r\n"+slidesTooBig + " slides have been compressed");

			// Restore all widgets (that were noted) to visibility.
			foreach (PowerPoint.Shape shape in widgets)
				shape.Visible = Core.MsoTriState.msoTrue;
			
			// Restore the original mode.
			if (this.myPaneManager != null)
				this.myPaneManager.Mode = currentMode;
			
			// Record the generated slides.
			this.mySlideArray = slides;
		}

		// HACK: this could be quite a big performance hit to find out the image size!!!
		private long ImageSize(Image image)
		{
			// Now we need to know how big this is when it is serialized
			MemoryStream memoryStream = new MemoryStream();				
			IFormatter formatter = new BinaryFormatter();
			formatter.Serialize(memoryStream, new SlideViewer.SerializableImage(image));
			return memoryStream.Length;
		}

		private Image ConstructSlideImage(PowerPoint._Slide ppSlide, 
			string imageFileName, string imageFormatString, int imageWidth, int imageHeight, ref int errorCount)
		{								
			// Converting slide formats is done through exporting to a file and reading in
			ppSlide.Export(imageFileName, imageFormatString, imageWidth, imageHeight);
			Image image = Image.FromFile(imageFileName);
			long size1 = ImageSize(image);

			if (size1 <= SlideSizeCutoff(this.MaximumImageSize))		
				return image;
			// Bitmap was too big, so let's make a low res version
			errorCount++;

			Size compressedSize = CompressedImageSize(this.MaximumImageSize, imageWidth, imageHeight);

			ppSlide.Export(imageFileName + "x.png", ".png", compressedSize.Width, compressedSize.Height);
			Bitmap bitmap2 = new Bitmap(imageFileName + "x.png");

			long size2 = ImageSize(bitmap2);
			return bitmap2;
		}

		protected string GetSlideTitle(PowerPoint._Slide ppSlide)
		{
			// TO get the title, we will look in the first rectangle with text
			// This seems to be standard operation for PowerPoint.
			for (int i = 1; i <= ppSlide.Shapes.Count; i++)
			{
				PowerPoint.Shape shape = ppSlide.Shapes[i];
				if (shape.HasTextFrame == Core.MsoTriState.msoTrue)
				{
					if (shape.TextFrame == null || 
						shape.TextFrame.HasText != Core.MsoTriState.msoTrue ||
						shape.TextFrame.TextRange == null || 
						shape.TextFrame.TextRange.Text == null)
						continue;
					string title = shape.TextFrame.TextRange.Text;
					if (title == "")
						continue;
					return CleanString(title);				// Need to make sure all the characters are printable
				}
			}
			return ppSlide.Name;

		}

		// Make sure all characters in a string are printable - replace any out of range characters by a space
		private string CleanString(string str)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder(str);
			for (int i = 0; i < str.Length; i++)
			{
				char c = str[i];
				if (! (Char.IsLetterOrDigit(c) || Char.IsPunctuation(c) || Char.IsSymbol(c)))
					sb[i] = ' ';
			}
			return sb.ToString();
		}

		#endregion

		#region Constructors

#if SIP_MODE
		/// <param name="ppt">non-null ppt library accessor</param>
		/// <param name="paneManager">pane manager or null if no pane management is to be used</param>
		/// <param name="propertyManager">widget property manager or null if the loader should assume no widgets exist</param>
		public PPTSlideLoader(Size viewerSize, int commentMenuType, PPTLibrary.PPT ppt, 
			PPTPaneManager paneManager, PPTPropertyManager propertyManager) 
#else
		/// <param name="ppt">non-null ppt library accessor</param>
		/// <param name="paneManager">pane manager or null if no pane management is to be used</param>
		public PPTSlideLoader(Size viewerSize, int commentMenuType, PPTLibrary.PPT ppt, PPTPaneManagement.PPTPaneManager paneManager) 
#endif
		{
			if (ppt == null) throw new ArgumentNullException("ppt");

			this.myPPT = ppt;
			this.myPaneManager = paneManager;
#if SIP_MODE
			this.myPropertyManager = propertyManager;
#endif

			this.myViewerSize = viewerSize;
			this.myDefaultCommentMenuType = commentMenuType;

			this.myFilesLoaded = 0;

			this.myTemporaryDirectoryPath =  Path.GetTempPath() + "\\DeckBuilderTempFiles";
			try 
			{
				if (Directory.Exists(this.myTemporaryDirectoryPath))
					Directory.Delete(this.myTemporaryDirectoryPath, true);		// Delete directory to clean up old files
				Directory.CreateDirectory(this.myTemporaryDirectoryPath);

			}
			catch (IOException e)
			{
				Console.WriteLine(e.Message);
			}

		}

		#endregion

		#region Public methods

		// Load the slides from a file - if successful then the object records the deck of images
		public void LoadSlidesFromFile(string fileName)
		{
			this.mySlideArray = new SlideViewer.Slide[0];

			// Abracadabra
			PowerPoint.Presentation ppPres = this.myPPT.App.Presentations.Open(fileName, Core.MsoTriState.msoTrue,
				Core.MsoTriState.msoFalse, Core.MsoTriState.msoFalse);

			this.LoadSlidesFromPresentation(ppPres);

			ppPres.Close();
			ppPres = null;
		}

		public void LoadSlidesFromPresentation(PowerPoint.Presentation ppPres)
		{
			this.mySlideArray = new SlideViewer.Slide[0];
			BuildSlideDeck(ppPres, this.myFilesLoaded, true);	// Build the deck from the powerpoint slides - maybe this 
			this.myFilesLoaded++;
			GC.Collect();
		}

		#endregion
	}
}
