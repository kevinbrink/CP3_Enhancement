//#define SIP_MODE // Uncomment to use SIP.

using System;
using Core = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;

namespace PPTDeckBuilder
{
	public class PPTDeckBuilder
	{
		#region Constants

		public const string EXTENSION = "csd";
		public static readonly System.Drawing.Size DEFAULT_SIZE = new System.Drawing.Size(720, 540);
		public const int DEFAULT_COMMENT_TYPE = SlideViewer.CommentMenuType.CSE142Menu;

		#endregion

		#region Members

		/// <summary>
		/// The convenience object used to access PPT Automation functionality.
		/// </summary>
		private PPTLibrary.PPT myPPT;

		private Core.CommandBarButton myExportButton;
		private Core.CommandBarButton myFeedbackButton;

		private PPTSlideLoader myLoader;

		private SlideViewer.FeedbackMenuList myFeedbackMenuList = null;

		#endregion

		#region Public methods

		/// <summary>
		/// Load a feedback menu from a file.
		/// </summary>
		/// <returns>true iff a menu is successfully loaded.</returns>
		public bool LoadFeedbackMenu()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Feedback Menu Files (*.xml)|*.xml|All files (*.*)|*.*" ;
			if(openFileDialog.ShowDialog() == DialogResult.OK) 
			{
				string fileName = openFileDialog.FileName;
				try 
				{
					XmlDocument xmlDoc = new XmlDocument();
					xmlDoc.Load(fileName);
					this.myFeedbackMenuList = new SlideViewer.FeedbackMenuList(xmlDoc.DocumentElement);
					foreach (SlideViewer.FeedbackMenu menu in this.myFeedbackMenuList)
					{
						MessageBox.Show("Loaded \"" + menu.Name + "\" as default menu.");
						break;
					}
				}
				catch (XmlException e) 
				{
					MessageBox.Show("Error in parsing XML \n\r" + e.Message);
					return false;
				}
			}
			else
				return false;

			return true;
		}

		/// <summary>
		/// Save the active presentation, prompting the user for a filename.
		/// </summary>
		/// <returns>true iff successfully saved</returns>
		/// <exception cref="UnauthorizedAccessException">if there is no active presentation</exception>
		public bool SavePresentation()
		{
			//First check to see if there is an active presentation to save
			PowerPoint.Presentation presentation = this.myPPT.GetActivePresentation();
			if (presentation == null) 
				throw new UnauthorizedAccessException("no active presentation to save.");

			return this.SavePresentation(presentation);
		}

		[DllImport("user32.dll")]
		public static extern bool BringWindowToTop(int hWnd);

		/// <summary>
		/// Save the given presentation, prompting the user for a filename.
		/// </summary>
		/// <returns>true iff successfully saved</returns>
		/// <param name="presentation">a non-null, open presentation</param>
		/// <exception cref="UnauthorizedAccessException">if there is no active presentation</exception>		
		/// <exception cref="ArgumentNullException">if presentation is null</exception>
		public bool SavePresentation(PowerPoint.Presentation presentation)
		{
			if (presentation == null) throw new ArgumentNullException("presentation");

			System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			saveFileDialog.Filter = "Conferencing slide deck files (*.csd)|*.csd" ;
			saveFileDialog.FileName = System.IO.Path.ChangeExtension(presentation.Name, EXTENSION);

			// Note: .NET v1.1 seems to have fixed the popup problem (or possibly using PPT's IntPtr has).
			DialogResult result = saveFileDialog.ShowDialog(System.Windows.Forms.Form.FromHandle((System.IntPtr)this.myPPT.App.HWND)); //PPTLibrary.DialogMessageWrapper.ShowDialog(saveFileDialog);
			if (result == System.Windows.Forms.DialogResult.OK ||
				result == System.Windows.Forms.DialogResult.Yes)
			{
				string message = null;
				try
				{
					this.SavePresentation(presentation, saveFileDialog.FileName);
				}
				catch (System.Security.SecurityException e) { message = "Permission problem: " + e.Message; }
				catch (System.IO.DirectoryNotFoundException) { message = "Directory not found."; }
				catch (System.IO.IOException e) { message = "Problem creating file: " + e.Message; }
				catch (UnauthorizedAccessException) { message = "File was readonly."; }
				if (message != null)
				{
					System.Windows.Forms.MessageBox.Show(message, "Error saving presentation", 
						System.Windows.Forms.MessageBoxButtons.OK,
						System.Windows.Forms.MessageBoxIcon.Error);
					return false;
				}
				else
					return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Save the given presentation to the given file name.
		/// </summary>
		/// <param name="presentation">non-null presentation to save</param>
		/// <param name="fileName">non-null, writable filename</param>
		/// <remarks>may throw any of the same exceptions that File.Create throws</remarks>
		public void SavePresentation(PowerPoint.Presentation presentation, string fileName)
		{
			// TODO: error handling
			System.IO.Stream stream = System.IO.File.Create(fileName);
			IFormatter formatter = new BinaryFormatter();
            SlideViewer.SlideDeck sd = this.MakeSlideDeck(presentation, fileName);
            SlideViewer.CSDDocument csd = new SlideViewer.CSDDocument();
            csd.SetInfo(sd);
            formatter.Serialize(stream, csd);
			stream.Close();
		}

		#endregion

		#region Internals

		private void ConfigureButton()
		{
			this.myExportButton.Enabled = 
				this.myPPT.GetActivePresentation() != null && 
				this.myPPT.GetActiveWindow() != null;
		}

		private SlideViewer.SlideDeck MakeSlideDeck(PowerPoint.Presentation presentation, string fileName)
		{
			this.myLoader.LoadSlidesFromPresentation(presentation);
			if (this.myFeedbackMenuList != null)
			{
				SlideViewer.FeedbackMenu defaultMenu = null;
				this.myFeedbackMenuList.Reset();
				foreach (SlideViewer.FeedbackMenu menu in this.myFeedbackMenuList)
				{
					defaultMenu = menu;
					break;
				}
				if (defaultMenu != null)
					foreach (SlideViewer.Slide slide in this.myLoader.SlideArray)
					{
						slide.FeedbackMenu = defaultMenu;
						slide.DefaultFeedbackMenu = defaultMenu;
					}
			}
			return new SlideViewer.PresentationDeck(new SlideViewer.SlideArray(this.myLoader.SlideArray), fileName, "");
		}

		#endregion

		#region Constructor
#if SIP_MODE
		/// <summary>
		/// Creates the PPT automation component which handles exporting CSD decks from PPT.
		/// </summary>
		/// <param name="ppt">non-null ppt library accessor</param>
		/// <param name="paneManager">pane manager or null if no pane management is to be used</param>
		/// <param name="propertyManager">widget property manager or null if the builder should assume no widgets exist</param>
		public PPTDeckBuilder(PPTLibrary.PPT ppt, PPTPaneManagement.PPTPaneManager paneManager,
			PPTPropertyManagement.PPTPropertyManager propertyManager)
#else

		public const int PPT_MENU_BAR_ID = 37; // Determined by experimentation; should hold regardless of language.
		public const int PPT_FILE_ID = 30002;  // Determined by experimentation; should hold regardless of language.
		public const int PPT_SAVE_AS_ID = 748; // Determined by experimentation; should hold regardless of language.

		/// <summary>
		/// Creates the PPT automation component which handles exporting CSD decks from PPT.
		/// </summary>
		/// <param name="ppt">non-null ppt library accessor</param>
		/// <param name="paneManager">pane manager or null if no pane management is to be used</param>
		public PPTDeckBuilder(PPTLibrary.PPT ppt, PPTPaneManagement.PPTPaneManager paneManager)
#endif
		{
			if (ppt == null) throw new ArgumentNullException("ppt");

			// Hook up to arguments.
			this.myPPT = ppt;

			// Prepare the loader.
			this.myLoader = new PPTSlideLoader(DEFAULT_SIZE, DEFAULT_COMMENT_TYPE, this.myPPT, paneManager
#if SIP_MODE
				, propertyManager
#endif
				);

			// Find the menubar. 
			Core.CommandBar menu = null;
			foreach (Core.CommandBar cb in this.myPPT.App.CommandBars)
				if (cb.Id == PPT_MENU_BAR_ID)
				{
					menu = cb;
					break;
				}
			if (menu == null) 
				throw new Exception("PPTDeckBuilderAddin: Unable to find the PowerPoint menu bar.");

			// Find the file menu.
			Core.CommandBarPopup fileMenu = null;
			foreach (Core.CommandBarControl bar in menu.Controls)
				if (bar.Id == PPT_FILE_ID)
				{
					fileMenu = (Core.CommandBarPopup)bar;
					break;
				}
			if (fileMenu == null) 
				throw new Exception("PPTDeckBuilderAddin: Unable to find the PowerPoint file menu.");

			// Find the Save As.. button.
			// And see if the export to CSD button is already there.
			Core.CommandBarControl saveAs = null;
			foreach (Core.CommandBarControl control in fileMenu.Controls)
			{
				if (control.Id == PPT_SAVE_AS_ID)
					saveAs = control;
				if (control.Caption == "&Export to CSD...")
					control.Delete(false); // Delete permanently.
				if (control.Caption == "Load &Feedback Menu...")
					control.Delete(false);
			}

			// Add and configure the export to CSD button...
			this.myExportButton = (Core.CommandBarButton)fileMenu.Controls.Add(
				Core.MsoControlType.msoControlButton, 
				System.Reflection.Missing.Value,                // Not a built in button: no ID.
				System.Reflection.Missing.Value,                // No parameters used.
				(saveAs == null) ? (object)System.Reflection.Missing.Value : (object)(saveAs.Index + 1), // Place just after Save As... if possible.
				true);                                          // Temporary (disappear on closing PPT).
			this.myExportButton.Caption = "&Export to CSD...";
			this.myExportButton.DescriptionText = "Export the current presentation to the Conferencing Slide Deck format.";
			this.myExportButton.TooltipText = this.myExportButton.DescriptionText;
			this.myExportButton.Click += new Core._CommandBarButtonEvents_ClickEventHandler(this.HandleCSDButtonClick);
			this.myExportButton.Visible = true;
			this.ConfigureButton();

			// Add and configure the load feedback menu button...
			this.myFeedbackButton = (Core.CommandBarButton)fileMenu.Controls.Add(
				Core.MsoControlType.msoControlButton, 
				System.Reflection.Missing.Value,                // Not a built in button: no ID.
				System.Reflection.Missing.Value,                // No parameters used.
				(object)(this.myExportButton.Index + 1), // Place just after Export to CSD
				true);                                          // Temporary (disappear on closing PPT).
			this.myFeedbackButton.Caption = "Load &Feedback Menu...";
			this.myFeedbackButton.DescriptionText = "Load a feedback menu to associate with this deck.";
			this.myFeedbackButton.TooltipText = this.myFeedbackButton.DescriptionText;
			this.myFeedbackButton.Click += new Core._CommandBarButtonEvents_ClickEventHandler(this.HandleFeedbackButtonClick);
			this.myFeedbackButton.Visible = true;
			this.ConfigureButton();

			// Hook up to events to enable/disable the export button.
			this.myPPT.App.PresentationClose += new PowerPoint.EApplication_PresentationCloseEventHandler(this.HandlePresentation);
			this.myPPT.App.PresentationOpen += new PowerPoint.EApplication_PresentationOpenEventHandler(this.HandlePresentation);
			this.myPPT.App.WindowActivate += new PowerPoint.EApplication_WindowActivateEventHandler(this.HandleWindow);
			this.myPPT.App.WindowDeactivate += new PowerPoint.EApplication_WindowDeactivateEventHandler(this.HandleWindow);
		}

		#endregion

		#region Event Handlers

		private void HandleCSDButtonClick(Core.CommandBarButton button, ref bool cancel) 
		{
			this.SavePresentation();
			GC.Collect();
		}

		private void HandleFeedbackButtonClick(Core.CommandBarButton button, ref bool cancel) 
		{
			this.LoadFeedbackMenu();
		}

		private void HandleWindow(PowerPoint.Presentation p, PowerPoint.DocumentWindow w) { this.ConfigureButton(); }
		private void HandlePresentation(PowerPoint.Presentation p) { this.ConfigureButton(); }

		#endregion

		#region Test Main

		public static void Main(string[] args)
		{
			PPTLibrary.PPT ppt = new PPTLibrary.PPT();
			PPTPaneManagement.PPTPaneManager paneMgr = new PPTPaneManagement.PPTPaneManager(ppt);
			PPTDeckBuilder builder = new PPTDeckBuilder(ppt, paneMgr
#if SIP_MODE
				, null
#endif
				);

			ppt.App.Activate();

			Console.ReadLine();
		}

		#endregion
	}
}
