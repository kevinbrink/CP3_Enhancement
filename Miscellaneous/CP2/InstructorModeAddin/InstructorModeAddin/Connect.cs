using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace InstructorModeAddin
{
	using System;
	using Microsoft.Office.Core;
	using Extensibility;
	using System.Runtime.InteropServices;
	

	#region Read me for Add-in installation and setup information.
	// When run, the Add-in wizard prepared the registry for the Add-in.
	// At a later time, if the Add-in becomes unavailable for reasons such as:
	//   1) You moved this project to a computer other than which is was originally created on.
	//   2) You chose 'Yes' when presented with a message asking if you wish to remove the Add-in.
	//   3) Registry corruption.
	// you will need to re-register the Add-in by building the MyAddin21Setup project 
	// by right clicking the project in the Solution Explorer, then choosing install.
	#endregion
	
	/// <summary>
	///   The object for implementing an Add-in.
	/// </summary>
	/// <seealso class='IDTExtensibility2' />
	[GuidAttribute("A1A10B58-0B8A-4D3A-B3FA-8E372C29828C"), ProgId("InstructorModeAddin.Connect")]
	public class Connect : Object, Extensibility.IDTExtensibility2
	{
		private PPTPaneManagement.PPTPaneManager myPaneManager;
		private PPTDeckBuilder.PPTDeckBuilder myDeckBuilder;
		private PPTLibrary.PPT myPPT;

		/// <summary>
		///		Implements the constructor for the Add-in object.
		///		Place your initialization code within this method.
		/// </summary>
		public Connect()
		{
		}

		/// <summary>
		///      Implements the OnConnection method of the IDTExtensibility2 interface.
		///      Receives notification that the Add-in is being loaded.
		/// </summary>
		/// <param term='application'>
		///      Root object of the host application.
		/// </param>
		/// <param term='connectMode'>
		///      Describes how the Add-in is being loaded.
		/// </param>
		/// <param term='addInInst'>
		///      Object representing this Add-in.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, Extensibility.ext_ConnectMode connectMode, object addInInst, ref System.Array custom)
		{
			if (!(application is PowerPoint.Application))
				throw new ArgumentException("must be a PowerPoint Application object.. perhaps you are running the add-in with the wrong application?", "application");
			// Start PPT.
			this.myPPT = new PPTLibrary.PPT((PowerPoint.Application)application);
			this.myPPT.CloseOnExit = PPTLibrary.PPT.CloseConditions.Never; // I don't trust PPT objects to close PowerPoint correctly.
			
			this.myPaneManager = new PPTPaneManagement.PPTPaneManager(this.myPPT);
	 
			this.myDeckBuilder = new PPTDeckBuilder.PPTDeckBuilder(this.myPPT, this.myPaneManager);

			applicationObject = application;
			addInInstance = addInInst;
		}

		/// <summary>
		///     Implements the OnDisconnection method of the IDTExtensibility2 interface.
		///     Receives notification that the Add-in is being unloaded.
		/// </summary>
		/// <param term='disconnectMode'>
		///      Describes how the Add-in is being unloaded.
		/// </param>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(Extensibility.ext_DisconnectMode disconnectMode, ref System.Array custom)
		{
			if (this.myPaneManager != null)
				this.myPaneManager.Hide();
			this.myPaneManager = null;
			this.myDeckBuilder = null;
			this.myPPT = null;
		}

		/// <summary>
		///      Implements the OnAddInsUpdate method of the IDTExtensibility2 interface.
		///      Receives notification that the collection of Add-ins has changed.
		/// </summary>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnAddInsUpdate(ref System.Array custom)
		{
		}

		/// <summary>
		///      Implements the OnStartupComplete method of the IDTExtensibility2 interface.
		///      Receives notification that the host application has completed loading.
		/// </summary>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref System.Array custom)
		{
		}

		/// <summary>
		///      Implements the OnBeginShutdown method of the IDTExtensibility2 interface.
		///      Receives notification that the host application is being unloaded.
		/// </summary>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref System.Array custom)
		{
		}
		
		
		private object applicationObject;
		private object addInInstance;
	}
}