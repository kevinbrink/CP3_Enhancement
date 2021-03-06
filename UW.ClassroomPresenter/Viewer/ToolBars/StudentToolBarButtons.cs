using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

using Microsoft.VisualBasic;

using UW.ClassroomPresenter.Model;
using UW.ClassroomPresenter.Model.Network;
using UW.ClassroomPresenter.Model.Presentation;
using UW.ClassroomPresenter.Network.Messages.Presentation;
using UW.ClassroomPresenter.Model.Viewer;
using UW.ClassroomPresenter.Viewer.Slides;
using UW.ClassroomPresenter.Viewer.Menus;
using System.Collections;


namespace UW.ClassroomPresenter.Viewer.ToolBars {
    /// <summary>
    /// Represents the tool bar buttons that are associated with the student view
    /// Namely, the student submissions button.
    /// </summary>
    public class StudentToolBarButtons {
        /// <summary>
        /// The link to the PresenterModel modified by this class
        /// </summary>
        private readonly PresenterModel m_Model;

        //private MyForm qpDialog;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model">The PresenterModel object to associate with these buttons</param>
        public StudentToolBarButtons(PresenterModel model) {
            this.m_Model = model;
           
        }
        /// <summary>
        /// Make all the buttons for this tool bar and add them to the bar
        /// </summary>
        /// <param name="parent">The toolbar to add the button to</param>
        /// <param name="dispatcher">The event queue to dispatch message onto</param>
        public void MakeButtons(ToolStrip parent, ControlEventQueue dispatcher) {
            ParticipantToolBarButton submit;

            submit = new SubmitStudentSubmissionToolBarButton( dispatcher, this.m_Model );
            submit.AutoSize = false;
            submit.Width = 68;
            submit.Height = 44;
            submit.Image = UW.ClassroomPresenter.Properties.Resources.submission;

            parent.Items.Add( submit );

            //parent.Items.Add( new StudentQuickPollToolBarButton( dispatcher, this.m_Model, Strings.QuickPollYes, "Yes" ) );
            //parent.Items.Add( new StudentQuickPollToolBarButton( dispatcher, this.m_Model, Strings.QuickPollNo, "No" ) );
            //parent.Items.Add( new StudentQuickPollToolBarButton( dispatcher, this.m_Model, Strings.QuickPollBoth, "Both" ) );
            //parent.Items.Add( new StudentQuickPollToolBarButton( dispatcher, this.m_Model, Strings.QuickPollNeither, "Neither" ) );
            //parent.Items.Add( new StudentQuickPollToolBarButton( dispatcher, this.m_Model, "A", "A" ) );
            //parent.Items.Add( new StudentQuickPollToolBarButton( dispatcher, this.m_Model, "B", "B" ) );
            //parent.Items.Add( new StudentQuickPollToolBarButton( dispatcher, this.m_Model, "C", "C" ) );
            //parent.Items.Add( new StudentQuickPollToolBarButton( dispatcher, this.m_Model, "D", "D" ) );
            //parent.Items.Add( new StudentQuickPollToolBarButton( dispatcher, this.m_Model, "E", "E" ) );
        }

        /// <summary>
        /// Make all the buttons for this tool bar and add them to the bar
        /// </summary>
        /// <param name="main">The main toolbar to add the button to</param>
        /// <param name="extra">The extra toolbar to add the button to</param>
        /// <param name="dispatcher">The event queue to dispatch message onto</param>
        public void MakeButtons(ToolStrip main, ToolStrip extra, ControlEventQueue dispatcher) {
            ParticipantToolBarButton submit; // yes, no, both, neither, a, b, c, d, e;

            new StudentQuickPollToolBarButton(dispatcher, this.m_Model, "test", "test");
            submit = new SubmitStudentSubmissionToolBarButton( dispatcher, this.m_Model );
            submit.AutoSize = false;
            submit.Width = 54;
            submit.Height = 44;
            submit.Image = UW.ClassroomPresenter.Properties.Resources.submission;

            extra.Items.Add( submit );

            //yes = new StudentQuickPollToolBarButton(dispatcher, this.m_Model, Strings.QuickPollYes, "Yes");
            //yes.AutoSize = false;
            //yes.Width = 54;
            //yes.Height = 32;

            //main.Items.Add( yes );

            //no = new StudentQuickPollToolBarButton(dispatcher, this.m_Model, Strings.QuickPollNo, "No");
            //no.AutoSize = false;
            //no.Width = 54;
            //no.Height = 32;

            //main.Items.Add( no );

            //both = new StudentQuickPollToolBarButton(dispatcher, this.m_Model, Strings.QuickPollBoth, "Both");
            //both.AutoSize = false;
            //both.Width = 54;
            //both.Height = 32;

            //main.Items.Add( both );

            //neither = new StudentQuickPollToolBarButton( dispatcher, this.m_Model, Strings.QuickPollNeither, "Neither" );
            //neither.AutoSize = false;
            //neither.Width = 54;
            //neither.Height = 32;

            //main.Items.Add( neither );

            //a = new StudentQuickPollToolBarButton(dispatcher, this.m_Model, "A", "A");
            //a.AutoSize = false;
            //a.Width = 54;
            //a.Height = 18;

            //extra.Items.Add( a );

            //b = new StudentQuickPollToolBarButton(dispatcher, this.m_Model, "B", "B");
            //b.AutoSize = false;
            //b.Width = 54;
            //b.Height = 18;

            //extra.Items.Add( b );

            //c = new StudentQuickPollToolBarButton(dispatcher, this.m_Model, "C", "C");
            //c.AutoSize = false;
            //c.Width = 54;
            //c.Height = 18;

            //extra.Items.Add( c );

            //d = new StudentQuickPollToolBarButton(dispatcher, this.m_Model, "D", "D");
            //d.AutoSize = false;
            //d.Width = 54;
            //d.Height = 18;

            //extra.Items.Add( d );

            //e = new StudentQuickPollToolBarButton(dispatcher, this.m_Model, "E", "E");
            //e.AutoSize = false;
            //e.Width = 54;
            //e.Height = 18;

            //extra.Items.Add( e );
        }

        #region ParticipantToolBarButton class

        /// <summary>
        /// A utility class which keeps its abstract <see cref="Participant"/> property in
        /// sync with that of the associated <see cref="PresenterModel"/>.
        /// Subclasses can define their own setter for the <c>Presenter</c> property and
        /// handle changes as they wish.
        /// </summary>
        private abstract class ParticipantToolBarButton : ToolStripButton {
            /// <summary>
            /// The event queue
            /// </summary>
            protected readonly ControlEventQueue m_EventQueue;
            /// <summary>
            /// The role changed dispatcher
            /// </summary>
            private readonly EventQueue.PropertyEventDispatcher m_RoleChangedDispatcher;

            /// <summary>
            /// The presenter model
            /// </summary>
            private readonly PresenterModel m_Model;
            /// <summary>
            /// True if disposed
            /// </summary>
            private bool m_Disposed;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="dispatcher">The event queue</param>
            /// <param name="model">The model</param>
            protected ParticipantToolBarButton(ControlEventQueue dispatcher, PresenterModel model) {
                this.m_EventQueue = dispatcher;
                this.m_Model = model;

                this.m_RoleChangedDispatcher = new EventQueue.PropertyEventDispatcher(this.m_EventQueue, new PropertyEventHandler(this.HandleRoleChanged));
                this.m_Model.Participant.Changed["Role"].Add(this.m_RoleChangedDispatcher.Dispatcher);
            }

            /// <summary>
            /// Releases all resources
            /// </summary>
            /// <param name="disposing">True if truely disposing</param>
            protected override void Dispose(bool disposing) {
                if(this.m_Disposed) return;
                try {
                    if(disposing) {
                        this.m_Model.Participant.Changed["Role"].Remove(this.m_RoleChangedDispatcher.Dispatcher);
                    }
                } finally {
                    base.Dispose(disposing);
                }
                this.m_Disposed = true;
            }

            /// <summary>
            /// Handles changes to the role
            /// </summary>
            protected abstract RoleModel Role { get; set; }

            /// <summary>
            /// Handle changes to the role
            /// </summary>
            /// <param name="sender">The event sender</param>
            /// <param name="args">The event args</param>
            private void HandleRoleChanged(object sender, PropertyEventArgs args) {
                this.InitializeRole();
            }

            /// <summary>
            /// Sets the <see cref="Role"/> property to the current
            /// <see cref="ParticipantModel.Role">Role</see> of
            /// the <see cref="ParticipantModel"/> of the associated <see cref="PresenterModel"/>.
            /// </summary>
            /// <remarks>
            /// This method should be called once from a subclass's constructor,
            /// and may be called any number of times thereafter.
            /// <para>
            /// This method should be called from the <see cref="m_EventQueue"/> or
            /// the control's creator thread.
            /// </para>
            /// </remarks>
            protected void InitializeRole() {
                using(Synchronizer.Lock(this.m_Model.Participant.SyncRoot)) {
                    this.Role = this.m_Model.Participant.Role;
                    this.Visible = (this.m_Model.Participant.Role is StudentModel);
                }
            }
        }

        #endregion

        #region SubmitStudentSubmissionToolBarButton class

        /// <summary>
        /// This class represents the button that submits the student submission 
        /// and ensures that the correct thing is displayed on the screen.
        /// </summary>
        private class SubmitStudentSubmissionToolBarButton : ParticipantToolBarButton {
            /// <summary>
            /// The current role
            /// </summary>
            private RoleModel m_Role;
            /// <summary>
            /// The presenter model
            /// </summary>
            private readonly PresenterModel m_Model;
            /// <summary>
            /// True if disposed
            /// </summary>
            private bool m_Disposed;

            private readonly EventQueue.PropertyEventDispatcher m_AcceptingSSubsChangedDispatcher;
            /// <summary>
            /// Property dispatcher
            /// </summary>
            private readonly IDisposable m_CurrentPresentationDispatcher;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="dispatcher">The event queue</param>
            /// <param name="model">The presenter model</param>
            public SubmitStudentSubmissionToolBarButton(ControlEventQueue dispatcher, PresenterModel model)
                : base(dispatcher, model) {
                this.m_Model = model;
                this.Name = "StudentSubmissionToolBarButton";
                this.ToolTipText = "Ask the instructor a question";
                
                // This is the listener that we will attatch to the AcceptingStudentSubmissions field
                // once we've got the current presentation.  We can't do this directly because
                // when this button is created there is no presentation yet.
                this.m_AcceptingSSubsChangedDispatcher = new EventQueue.PropertyEventDispatcher(this.m_EventQueue, new PropertyEventHandler(this.HandleAcceptingSSubsChanged));

                // Listen for the Presentation.
                // Don't call the event handler immediately (use Listen instead of ListenAndInitialize)
                // because it needs to be delayed until Initialize() below.
                this.m_CurrentPresentationDispatcher =
                    this.m_Model.Workspace.CurrentPresentation.Listen(dispatcher,
                    delegate(Property<PresentationModel>.EventArgs args) {
                        if (args.Old != null) {
                            if (args.Old.Owner != null) {
                                using (Synchronizer.Lock(args.Old.Owner)) {
                                    ((InstructorModel)(((PresentationModel)(args.Old)).Owner.Role)).Changed["AcceptingStudentSubmissions"].Remove(this.m_AcceptingSSubsChangedDispatcher.Dispatcher);
                                }
                            }
                         }
                        ParticipantModel participant = null;
                        using (Synchronizer.Lock(this.m_Model.Workspace.CurrentPresentation.SyncRoot)) {
                            if (this.m_Model.Workspace.CurrentPresentation.Value != null) {
                                using (Synchronizer.Lock(this.m_Model.Workspace.CurrentPresentation.Value.SyncRoot)) {
                                    participant = this.m_Model.Workspace.CurrentPresentation.Value.Owner;
                                }
                            }
                        }
                        if (participant != null) {
                            using (Synchronizer.Lock(participant.SyncRoot)) {
                                ((InstructorModel)(participant.Role)).Changed["AcceptingStudentSubmissions"].Add(this.m_AcceptingSSubsChangedDispatcher.Dispatcher);
                            }
                        }
                        this.HandleAcceptingSSubsChanged(null, null);                                   
                    });

                base.InitializeRole();
            }

            protected void Initialize() {
                using (Synchronizer.Lock(this.m_Model.Workspace.CurrentPresentation.SyncRoot)) {
                    using (Synchronizer.Lock(this.m_Model.Workspace.CurrentPresentation.Value.SyncRoot)) {
                        using (Synchronizer.Lock(this.m_Model.Workspace.CurrentPresentation.Value.Owner.SyncRoot)) {
                            ((InstructorModel)(this.m_Model.Workspace.CurrentPresentation.Value.Owner.Role)).Changed["AcceptingStudentSubmissions"].Add(this.m_AcceptingSSubsChangedDispatcher.Dispatcher);
                           
                        }
                    }
                }
                this.HandleAcceptingSSubsChanged(null, null);
            }



            /// <summary>
            /// Release all resources
            /// </summary>
            /// <param name="disposing">True if truely disposing</param>
            protected override void Dispose(bool disposing) {
                if (this.m_Disposed) return;
                try {
                    if (disposing) {
                        // Unregister event listeners 
                        ParticipantModel participant = null;
                        using (Synchronizer.Lock(this.m_Model.Workspace.CurrentPresentation.SyncRoot)) {
                            if (this.m_Model.Workspace.CurrentPresentation.Value != null) {
                                using (Synchronizer.Lock(this.m_Model.Workspace.CurrentPresentation.Value.SyncRoot)) {
                                    participant = this.m_Model.Workspace.CurrentPresentation.Value.Owner;
                                }
                            }
                        }
                        if (participant != null) {
                            using (Synchronizer.Lock(participant.SyncRoot)) {
                                ((InstructorModel)(participant.Role)).Changed["AcceptingStudentSubmissions"].Remove(this.m_AcceptingSSubsChangedDispatcher.Dispatcher);
                            }
                        }
                        this.Role = null;
                    }
                }
                finally {
                    base.Dispose(disposing);
                }
                this.m_Disposed = true;
            }

            /// <summary>
            /// Adds or removes any event handlers when the role is changed
            /// </summary>
            protected override RoleModel Role {
                get { return this.m_Role; }
                set { this.m_Role = value; }
            }

            /// <summary>
            /// Handle the button being clicked
            /// </summary>
            /// <param name="args">The event args</param>
            protected override void OnClick(EventArgs args) {
                if (this.Role is StudentModel)
                {

                    /* The following code was added/modified by Gabriel Martin and Eric Dodds on Nov 8, 2014 */

                    using (Synchronizer.Lock(this.m_Model.ViewerState.SyncRoot))
                    {
                        using (Synchronizer.Lock(this.m_Model.Workspace.CurrentPresentation.SyncRoot))
                        {
                            using (Synchronizer.Lock(this.m_Model.Workspace.CurrentPresentation.Value.SyncRoot))
                            {
                                /* Retrieve desired question from student through the use of an InputBox */
                                string input = Interaction.InputBox("Question Submission", "Enter your question below.");

                                /* Create a new QuestionModel and assign it to the StudentQuestion of the PresenterModel */
                                this.m_Model.StudentQuestion = new QuestionModel(new Guid("{1afc601e-e601-43f9-86d4-06ad71238b29}"), input);
                            }
                        }
                        this.m_Model.ViewerState.StudentSubmissionSignal = !this.m_Model.ViewerState.StudentSubmissionSignal; /* Change the submission signal */
                    }
                }
                base.OnClick(args);
            }
            /// <summary>
            /// Handle AcceptingStudentSubmissions changing (disable when false)
            /// </summary>
            /// <param name="sender">The event sender</param>
            /// <param name="args">The arguments</param>
            private void HandleAcceptingSSubsChanged(object sender, PropertyEventArgs args) {
                InstructorModel instructor = null;
                ParticipantModel participant = null;
                using (Synchronizer.Lock(this.m_Model.Workspace.CurrentPresentation.SyncRoot)) {
                    if (this.m_Model.Workspace.CurrentPresentation.Value != null) {
                        using (Synchronizer.Lock(this.m_Model.Workspace.CurrentPresentation.Value.SyncRoot)) {
                            participant = this.m_Model.Workspace.CurrentPresentation.Value.Owner;
                        }
                    }
                }
                if (participant != null) {
                    using (Synchronizer.Lock(participant.SyncRoot)) {
                        if (participant.Role != null) {
                            instructor = participant.Role as InstructorModel;
                        }
                    }
                }
                if (instructor != null) {
                    using (Synchronizer.Lock(instructor.SyncRoot)) {
                        this.Enabled = instructor.AcceptingStudentSubmissions;
                    }
                }

            }
        }

        #endregion

        #region StudentQuickPollButton class

        /// <summary>
        /// This class represents the button that submits the student submission 
        /// and ensures that the correct thing is displayed on the screen.
        /// </summary>
        private class StudentQuickPollToolBarButton : ParticipantToolBarButton {

            #region Private Members
            public MyForm qpDialog;
            /// <summary>
            /// The current role
            /// </summary>
            private RoleModel m_Role;
            /// <summary>
            /// The presenter model
            /// </summary>
            private readonly PresenterModel m_Model;
            /// <summary>
            /// True if disposed
            /// </summary>
            private bool m_Disposed;

            /// <summary>
            /// The value of the button
            /// </summary>
            private string m_Value;

            /// <summary>
            /// Event dispatcher to handle when the AcceptingQuickPollSubmissions changes
            /// </summary>
            private readonly EventQueue.PropertyEventDispatcher m_QuickPollChangedDispatcher;

            /// <summary>
            /// Event dispatcher
            /// </summary>
            private readonly EventQueue.PropertyEventDispatcher m_CurrentQuickPollResultChangedDispatcher;
            /// <summary>
            /// Event dispatcher
            /// </summary>
            private readonly EventQueue.PropertyEventDispatcher m_CurrentQuickPollResultStringChangedDispatcher;
            private QuickPollResultModel m_CurrentResult;

            /// <summary>
            /// Property dispatcher to handle when the current presentation changes
            /// </summary>
            private readonly IDisposable m_CurrentPresentationDispatcher;
            
            #endregion
       
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="dispatcher">The event queue</param>
            /// <param name="model">The presenter model</param>
            public StudentQuickPollToolBarButton( ControlEventQueue dispatcher, PresenterModel model, string name, string value )
                : base( dispatcher, model ) {
                this.m_Model = model;
                this.m_Value = "test";
               /* this.Name = name;
                this.ToolTipText = name;
                this.Text = name;
                this.DisplayStyle = ToolStripItemDisplayStyle.Text;*/
               
                    // Handle the changed result string
                this.m_CurrentQuickPollResultStringChangedDispatcher = new EventQueue.PropertyEventDispatcher( this.m_EventQueue, new PropertyEventHandler( this.HandleQuickPollResultChanged ) );
           
                // This listener listens to changes in the CurrentStudentQuickPollResult field of the 
                // PresenterModel, when this changes we need to update the UI and attach a new listener
                // to the ResultString of the model so that we can update the UI.
                this.m_CurrentQuickPollResultChangedDispatcher = new EventQueue.PropertyEventDispatcher( this.m_EventQueue, new PropertyEventHandler( this.HandleQuickPollResultChanged ) );
                this.m_Model.Changed["CurrentStudentQuickPollResult"].Add( this.m_CurrentQuickPollResultChangedDispatcher.Dispatcher );
                using( Synchronizer.Lock( this.m_Model.SyncRoot)  ) {
                    this.m_CurrentResult = this.m_Model.CurrentStudentQuickPollResult;
                }
                this.HandleQuickPollResultChanged( null, null );



                // This is the listener that we will attatch to the AcceptingStudentSubmissions field
                // once we've got the current presentation.  We can't do this directly because
                // when this button is created there is no presentation yet.
                this.m_QuickPollChangedDispatcher = new EventQueue.PropertyEventDispatcher( this.m_EventQueue, new PropertyEventHandler( this.HandleQuickPollChanged ) );

                // Listen for the Presentation.
                // Don't call the event handler immediately (use Listen instead of ListenAndInitialize)
                // because it needs to be delayed until Initialize() below.
                this.m_CurrentPresentationDispatcher =
                    this.m_Model.Workspace.CurrentPresentation.Listen( dispatcher,
                    delegate( Property<PresentationModel>.EventArgs args ) {
                        // Remove any previous listener
                        if( args.Old != null ) {
                            if( args.Old.Owner != null ) {
                                using( Synchronizer.Lock( args.Old.Owner ) ) {
                                    ((InstructorModel)(((PresentationModel)(args.Old)).Owner.Role)).Changed["AcceptingQuickPollSubmissions"].Remove( this.m_QuickPollChangedDispatcher.Dispatcher );
                                }
                            }
                        }

                        // Get the new InstructorModel
                        ParticipantModel participant = null;
                        using( Synchronizer.Lock( this.m_Model.Workspace.CurrentPresentation.SyncRoot ) ) {
                            if( this.m_Model.Workspace.CurrentPresentation.Value != null ) {
                                using( Synchronizer.Lock( this.m_Model.Workspace.CurrentPresentation.Value.SyncRoot ) ) {
                                    participant = this.m_Model.Workspace.CurrentPresentation.Value.Owner;
                                }
                            }
                        }

                        // Attach to the new AcceptingQuickPollSubmissions
                        if( participant != null ) {
                            using( Synchronizer.Lock( participant.SyncRoot ) ) {
                                ((InstructorModel)(participant.Role)).Changed["AcceptingQuickPollSubmissions"].Add( this.m_QuickPollChangedDispatcher.Dispatcher );
                            }
                        }

                        // Update the UI
                       
                       this.HandleQuickPollChanged( null, null );
                    } );

                base.InitializeRole();
            }

            protected void Initialize() {
                using( Synchronizer.Lock( this.m_Model.Workspace.CurrentPresentation.SyncRoot ) ) {
                    using( Synchronizer.Lock( this.m_Model.Workspace.CurrentPresentation.Value.SyncRoot ) ) {
                        using( Synchronizer.Lock( this.m_Model.Workspace.CurrentPresentation.Value.Owner.SyncRoot ) ) {
                            ((InstructorModel)(this.m_Model.Workspace.CurrentPresentation.Value.Owner.Role)).Changed["AcceptingQuickPollSubmissions"].Add( this.m_QuickPollChangedDispatcher.Dispatcher );

                        }
                    }
                }
                this.HandleQuickPollChanged( null, null );
            }



            /// <summary>
            /// Release all resources
            /// </summary>
            /// <param name="disposing">True if truely disposing</param>
            protected override void Dispose( bool disposing ) {
                if( this.m_Disposed ) return;
                try {
                    if( disposing ) {
                        // Unregister event listeners
                        this.m_CurrentResult = null;
                        this.m_Model.Changed["CurrentStudentQuickPollResult"].Remove( this.m_CurrentQuickPollResultChangedDispatcher.Dispatcher );

                        ParticipantModel participant = null;
                        using( Synchronizer.Lock( this.m_Model.Workspace.CurrentPresentation.SyncRoot ) ) {
                            if( this.m_Model.Workspace.CurrentPresentation.Value != null ) {
                                using( Synchronizer.Lock( this.m_Model.Workspace.CurrentPresentation.Value.SyncRoot ) ) {
                                    participant = this.m_Model.Workspace.CurrentPresentation.Value.Owner;
                                }
                            }
                        }
                        if( participant != null ) {
                            using( Synchronizer.Lock( participant.SyncRoot ) ) {
                                ((InstructorModel)(participant.Role)).Changed["AcceptingQuickPollSubmissions"].Remove( this.m_QuickPollChangedDispatcher.Dispatcher );
                            }
                        }
                        this.Role = null;
                    }
                } finally {
                    base.Dispose( disposing );
                }
                this.m_Disposed = true;
            }

            /// <summary>
            /// Adds or removes any event handlers when the role is changed
            /// </summary>
            protected override RoleModel Role {
                get { return this.m_Role; }
                set { this.m_Role = value; }
            }

            /// <summary>
            /// Create a new QuickPollResult and add it to the appropriate places 
            /// </summary>
            /// <param name="owner">The current participant</param>
            /// <param name="result">The result string</param>
            private void CreateNewQuickPollResult( ParticipantModel owner, string result ) {
                using( Synchronizer.Lock( this.m_Model ) ) {
                    // Create the QuickPollResultModel
                    using( Synchronizer.Lock( owner.SyncRoot ) ) {
                        this.m_Model.CurrentStudentQuickPollResult = new QuickPollResultModel( owner.Guid, result );
                    }

                    // Add to the QuickPollResults
                    using( this.m_Model.Workspace.Lock() ) {
                        if( ~this.m_Model.Workspace.CurrentPresentation != null ) {
                            using( Synchronizer.Lock( (~this.m_Model.Workspace.CurrentPresentation).SyncRoot ) ) {
                                if( (~this.m_Model.Workspace.CurrentPresentation).QuickPoll != null ) {
                                    using( Synchronizer.Lock( (~this.m_Model.Workspace.CurrentPresentation).QuickPoll.SyncRoot ) ) {
                                        if( (~this.m_Model.Workspace.CurrentPresentation).QuickPoll.QuickPollResults != null ) {
                                            (~this.m_Model.Workspace.CurrentPresentation).QuickPoll.QuickPollResults.Add( this.m_Model.CurrentStudentQuickPollResult );
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Handle the button being clicked
            /// </summary>
            /// <param name="args">The event args</param>
            protected override void OnClick( EventArgs args ) {
                // Only do this if we are a student
                if( this.Role is StudentModel ) {
                    using( Synchronizer.Lock( this.m_Model ) ) {
                        if( this.m_Model.CurrentStudentQuickPollResult != null ) {
                            // Update the existing QuickPollResultModel
                            using( Synchronizer.Lock( this.m_Model.CurrentStudentQuickPollResult.SyncRoot ) ) {
                                this.m_Model.CurrentStudentQuickPollResult.ResultString = this.m_Value;
                            }
                        } else {
                            // Create a new QuickPollResultModel and put it in all the right places
                            CreateNewQuickPollResult( this.m_Model.Participant, this.m_Value );
                        }
                    }
                }
                base.OnClick( args );
            }

            /// <summary>
            /// Handle AcceptingQuickPollSubmissions changing (disable when false)
            /// </summary>
            /// <param name="sender">The event sender</param>
            /// <param name="args">The arguments</param>
            private void HandleQuickPollChanged( object sender, PropertyEventArgs args ) {

                InstructorModel instructor = null;
                ParticipantModel participant = null;
               /*
                // Determine if the button should be visible based on the quick poll style
                bool IsValidButton = false;
                using( this.m_Model.Workspace.Lock() ) {
                    if( ~this.m_Model.Workspace.CurrentPresentation != null ) {
                        using( Synchronizer.Lock( (~this.m_Model.Workspace.CurrentPresentation).SyncRoot ) ) {
                            if( (~this.m_Model.Workspace.CurrentPresentation).QuickPoll != null ) {
                                using( Synchronizer.Lock( (~this.m_Model.Workspace.CurrentPresentation).QuickPoll.SyncRoot ) ) {
                                    if( UW.ClassroomPresenter.Model.Presentation.QuickPollModel.GetVoteStringsFromStyle( (~this.m_Model.Workspace.CurrentPresentation).QuickPoll.PollStyle ).Contains( this.m_Value ) ) 
                                    {
                                        IsValidButton = true;
                                    }
                                }
                            }
                        }
                    }
                }
                    */

                //Ensure the user is a student
                    if (Role is StudentModel)
                    {
                        using (Synchronizer.Lock(this.m_Model.Workspace.CurrentPresentation.SyncRoot))
                        {
                            //Ensure there is a current presentation
                            if (this.m_Model.Workspace.CurrentPresentation.Value != null)
                            {
                                using (Synchronizer.Lock(this.m_Model.Workspace.CurrentPresentation.Value.SyncRoot))
                                {
                                    //Get the participant from the current presentation
                                    participant = this.m_Model.Workspace.CurrentPresentation.Value.Owner;
                                }
                            }
                        }
                        if (participant != null)
                        {
                            using (Synchronizer.Lock(participant.SyncRoot))
                            {
                                if (participant.Role != null)
                                {
                                    //get the instructor from the participant
                                    instructor = participant.Role as InstructorModel;
                                }
                            }
                        }
                        if (instructor != null)
                        {
                            using (Synchronizer.Lock(instructor.SyncRoot))
                            {
                                //Check to see if the instructor is accepting poll submissions
                                if (instructor.AcceptingQuickPollSubmissions)
                                {
                                    using (this.m_Model.Workspace.Lock())
                                    {
                                        if (~this.m_Model.Workspace.CurrentPresentation != null)
                                        {
                                            using (Synchronizer.Lock((~this.m_Model.Workspace.CurrentPresentation).SyncRoot))
                                            {
                                                //Ensure there is a current quickpoll
                                                if ((~this.m_Model.Workspace.CurrentPresentation).QuickPoll != null)
                                                {
                                                    using (Synchronizer.Lock((~this.m_Model.Workspace.CurrentPresentation).QuickPoll.SyncRoot))
                                                    {
                                                        //Create the new dialog with the instructors quick poll information
                                                        qpDialog = new MyForm(this.m_Model, this.m_Role, (~this.m_Model.Workspace.CurrentPresentation).QuickPoll.instructorQA.ToArray(), (~this.m_Model.Workspace.CurrentPresentation).QuickPoll.PollStyle);
                                                        //SHow the quick poll dialog to the student
                                                        qpDialog.Show();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //If a current quick poll submission is ongoing
                                    if (qpDialog != null)
                                    {
                                        //Close the dialog
                                        qpDialog.Hide();
                                        using( Synchronizer.Lock( this.m_Model.SyncRoot ) ) {
                                            this.m_Model.CurrentStudentQuickPollResult = null;
                                        }
                                    }
                                }
                            }
                        }
                    }
                // Get the instructor model and set the visibility to it's value
                /*if( Role is StudentModel ) {
                    using( Synchronizer.Lock( this.m_Model.Workspace.CurrentPresentation.SyncRoot ) ) {
                        if( this.m_Model.Workspace.CurrentPresentation.Value != null ) {
                            using( Synchronizer.Lock( this.m_Model.Workspace.CurrentPresentation.Value.SyncRoot ) ) {
                                participant = this.m_Model.Workspace.CurrentPresentation.Value.Owner;
                            }
                        }
                    }
                    if( participant != null ) {
                        using( Synchronizer.Lock( participant.SyncRoot ) ) {
                            if( participant.Role != null ) {
                                instructor = participant.Role as InstructorModel;
                            }
                        }
                    }
                    if( instructor != null ) {
                        using( Synchronizer.Lock( instructor.SyncRoot ) ) {
                            this.Enabled = instructor.AcceptingQuickPollSubmissions & IsValidButton;
                            this.Visible = false;
                            //this.Visible = instructor.AcceptingQuickPollSubmissions & IsValidButton;
                            if (instructor.AcceptingQuickPollSubmissions & IsValidButton)
                            {

                                using (this.m_Model.Workspace.Lock())
                                {
                                    if (~this.m_Model.Workspace.CurrentPresentation != null)
                                    {
                                        using (Synchronizer.Lock((~this.m_Model.Workspace.CurrentPresentation).SyncRoot))
                                        {
                                            if ((~this.m_Model.Workspace.CurrentPresentation).QuickPoll != null)
                                            {
                                                using (Synchronizer.Lock((~this.m_Model.Workspace.CurrentPresentation).QuickPoll.SyncRoot))
                                                {
                                                    if (UW.ClassroomPresenter.Model.Presentation.QuickPollModel.GetVoteStringsFromStyle((~this.m_Model.Workspace.CurrentPresentation).QuickPoll.PollStyle).Contains(this.m_Value)
                                                 )
                                                    {
                                                 
                                                        MyForm pollDialog = new MyForm(this.m_Model, this.m_Role, (~this.m_Model.Workspace.CurrentPresentation).QuickPoll.instructorQA.ToArray(), (~this.m_Model.Workspace.CurrentPresentation).QuickPoll.PollStyle);
                                                        pollDialog.Show();
                                                        
                                                       /* 
                                                        MessageBox.Show(Convert.ToString((~this.m_Model.Workspace.CurrentPresentation).QuickPoll.queryStudent));
                                                        if ((~this.m_Model.Workspace.CurrentPresentation).QuickPoll.queryStudent==true)
                                                        {
                                                            (~this.m_Model.Workspace.CurrentPresentation).QuickPoll.queryStudent = false;
                                                            foreach (String q in (~this.m_Model.Workspace.CurrentPresentation).QuickPoll.instructorQA)
                                                            {
                                                                MessageBox.Show(q);
                                                            }
                                                            
                                                        }
                                                        MessageBox.Show(Convert.ToString((~this.m_Model.Workspace.CurrentPresentation).QuickPoll.queryStudent));
                                                        
                                                        }
                                                }
                                            }
                                        }
                                    }
                                }
                      
                                
                            }*/
                          /*  if( instructor.AcceptingQuickPollSubmissions == false ) {
                                this.Checked = false;
                                using( Synchronizer.Lock( this.m_Model.SyncRoot ) ) {
                                    this.m_Model.CurrentStudentQuickPollResult = null;
                                }
                            }*/
                           
                        }
                    
                
            

            /// <summary>
            /// Handles when the value of the CurrentQuickPollResult changes
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="args"></param>
            private void HandleQuickPollResultChanged( object sender, PropertyEventArgs args ) {
                if( this.m_CurrentResult != null ) {
                    this.m_CurrentResult.Changed["ResultString"].Remove( this.m_CurrentQuickPollResultStringChangedDispatcher.Dispatcher );
                }

                using( Synchronizer.Lock( this.m_Model.SyncRoot ) ) {
                    this.m_CurrentResult = this.m_Model.CurrentStudentQuickPollResult;
                    if( m_CurrentResult != null ) {
                        this.m_CurrentResult.Changed["ResultString"].Add( this.m_CurrentQuickPollResultStringChangedDispatcher.Dispatcher );
                    }
                }

                this.HandleQuickPollValueChanged( null, null );
            }

            /// <summary>
            /// Handles the value of quickpoll being changed
            /// </summary>
            /// <param name="sender">The event sender</param>
            /// <param name="args">The arguments</param>
            private void HandleQuickPollValueChanged( object sender, PropertyEventArgs args ) {
                using( Synchronizer.Lock( this.m_Model.SyncRoot ) ) {
                    if( this.m_Model.CurrentStudentQuickPollResult != null ) {
                        using( Synchronizer.Lock( this.m_Model.CurrentStudentQuickPollResult.SyncRoot ) ) {
                            this.Checked = (this.m_Model.CurrentStudentQuickPollResult.ResultString == this.m_Value);
                        }
                    }
                }
            }

        }

        #endregion


        #region Quickpoll Dialog Class

        /// <summary>
        /// Handles quickpoll UI on the student side
        /// Worked on by Matt and Vince
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="args">The arguments</param>

        class MyForm : System.Windows.Forms.Form
        {


            private Label question;
            private GroupBox checkgroup;
            private readonly PresenterModel m_Model;
            private readonly RoleModel m_role;
            private Hashtable previous;

            /// <summary>
            /// Handles the value of quickpoll being changed
            /// </summary>
            /// <param name="sender">The event sender</param>
            /// <param name="e">The arguments</param>
            void radioButtons_CheckedChanged(object sender, EventArgs e)
            {
                //interpret sender as the radio button that it is
                RadioButton rb = sender as RadioButton;
                
                //get the text associated with the selected radio button 
                
                if (rb != null)
                {
                    //make sure button is checked
                    if (rb.Checked)
                    {
                        // Only do this if we are a student
                        if (this.m_role is StudentModel)
                        {
                            using (Synchronizer.Lock(this.m_Model))
                            {
                                //see if we already have a result
                                if (this.m_Model.CurrentStudentQuickPollResult != null)
                                {
                                    // Update the existing QuickPollResultModel
                                    using (Synchronizer.Lock(this.m_Model.CurrentStudentQuickPollResult.SyncRoot))
                                    {
                                        //set the ResultString
                                        this.m_Model.CurrentStudentQuickPollResult.ResultString = rb.Text;
                                    }
                                }
                                else
                                {
                                    // Create a new QuickPollResultModel and set the ResultString to the text associated with the selected radio button                                 
                                    CreateNewQuickPollResult(this.m_Model.Participant, rb.Text);
                                }
                            }
                        }
                        base.OnClick(e);
                    }
                }
            }

            /// <summary>
            /// Handles the value of quickpoll being changed
            /// modified by Mark Friedrich on Dec 1 2014
            /// </summary>
            /// <param name="sender">The event sender</param>
            /// <param name="e">The arguments</param>
            void checkButtons_CheckedChanged(object sender, EventArgs e)
            {
                if (this.m_role is StudentModel)
                {
                    using (Synchronizer.Lock(this.m_Model))
                    {
                        // if previous is empty populate it with possible results
                        if (previous.Count == 0)
                        {
                            foreach (CheckBox c in checkgroup.Controls)
                            {
                                previous.Add(c.Text, null);
                            }
                        }

                        // loop through all checkboxes and determine if they are checked or not
                        foreach (CheckBox c in checkgroup.Controls)
                        {                                
                            if (c.Checked)
                            {
                                if (previous[c.Text] == null) // if the entry doesn't exist create it
                                {
                                    if (this.m_Model.CurrentStudentQuickPollResult != null) // if no CurrentStudentQuickPollResult exists we need to create one
                                    {
                                        using (Synchronizer.Lock(this.m_Model.CurrentStudentQuickPollResult.SyncRoot))
                                        {
                                            CreateNewQuickPollResult(this.m_Model.Participant, c.Text);
                                        }
                                    }
                                    else
                                    {
                                        CreateNewQuickPollResult(this.m_Model.Participant, c.Text); // replace existing CurrentStudentQuickPollResult
                                    }
                                }
                            }
                            else if (!c.Checked)
                            {
                                if (previous[c.Text] != null) // if the entry exist create remove it
                                {
                                    using (this.m_Model.Workspace.Lock())
                                    {
                                        using (Synchronizer.Lock((~this.m_Model.Workspace.CurrentPresentation).SyncRoot))
                                        {
                                            using (Synchronizer.Lock((~this.m_Model.Workspace.CurrentPresentation).QuickPoll.SyncRoot))
                                            {
                                                using (Synchronizer.Lock((~this.m_Model.Workspace.CurrentPresentation).QuickPoll.QuickPollResults))
                                                {
                                                    (~this.m_Model.Workspace.CurrentPresentation).QuickPoll.QuickPollResults.Remove((QuickPollResultModel)previous[c.Text]); // remove result localy
                                                    CreateNewQuickPollResult(this.m_Model.Participant, c.Text); // create the same entry to trigger professor side removal of entry
                                                    (~this.m_Model.Workspace.CurrentPresentation).QuickPoll.QuickPollResults.Remove((QuickPollResultModel)previous[c.Text]); // remove previously created entry from local results
                                                }
                                            }
                                        }
                                    }
                                    previous[c.Text] = null; // remove entry from hashtable
                                }
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Create a new QuickPollResult and add set the result string
            /// modified by Mark Friedrich on Dec 1 2014
            /// </summary>
            /// <param name="owner">The current participant</param>
            /// <param name="result">The result string</param>
            private void CreateNewQuickPollResult(ParticipantModel owner, string result)
            {
                using (Synchronizer.Lock(this.m_Model))
                {
                    // Create the QuickPollResultModel
                    using (Synchronizer.Lock(owner.SyncRoot))
                    {

                        if (previous.Count != 0) // determine if it's mutliple choice
                        {
                            QuickPollResultModel tmp = new QuickPollResultModel(owner.Guid, result);
                            this.m_Model.CurrentStudentQuickPollResult = tmp;
                            previous[result] = tmp;
                        }
                        else
                        {
                            this.m_Model.CurrentStudentQuickPollResult = new QuickPollResultModel(owner.Guid, result);
                        }
                    }

                    // Add to the QuickPollResults
                    using (this.m_Model.Workspace.Lock())
                    {
                        if (~this.m_Model.Workspace.CurrentPresentation != null)
                        {
                            using (Synchronizer.Lock((~this.m_Model.Workspace.CurrentPresentation).SyncRoot))
                            {
                                if ((~this.m_Model.Workspace.CurrentPresentation).QuickPoll != null)
                                {
                                    using (Synchronizer.Lock((~this.m_Model.Workspace.CurrentPresentation).QuickPoll.SyncRoot))
                                    {
                                        if ((~this.m_Model.Workspace.CurrentPresentation).QuickPoll.QuickPollResults != null)
                                        {
                                            //add the results to the quickpoll
                                            (~this.m_Model.Workspace.CurrentPresentation).QuickPoll.QuickPollResults.Add(this.m_Model.CurrentStudentQuickPollResult);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// generates the quickpoll form
            /// Worked on by Matt and Vince
            /// </summary>
            /// <param name="model">The presenter model</param>
            /// <param name="role">The user role</param>
            /// <param name="quickpollText">The Array containing the Question and Answers (InstructorQA)</param>
            /// <param name="quickPollStyle">The poll style</param>

            public MyForm(PresenterModel model, RoleModel role, String[] quickpollText, QuickPollModel.QuickPollStyle quickPollStyle)
            {
                this.m_role = role;

                this.m_Model = model;
                Size = new Size(400, 250);
                FormBorderStyle = FormBorderStyle.FixedDialog;
                Text = "Quickpoll";
                //Gets the question from quickpoll text from array (index 0 for question)
                question = new Label();
                question.Location = new Point(10, 10);
                question.Size = new Size(380, 30);
                question.Text = Convert.ToString(quickpollText.GetValue(0));
                Controls.Add(question);
                previous = new Hashtable();


                int yValue = 10;
                int yValueIncrement = 30;

                checkgroup = new GroupBox();
                checkgroup.Location = new Point(10, 40);
                checkgroup.Size = new Size(380, 160);
                Controls.Add(checkgroup);
                if ((quickpollText.GetValue(1).ToString().ToLower().Equals("true") && quickpollText.GetValue(2).ToString().ToLower().Equals("false"))
                    || (quickpollText.GetValue(1).ToString().ToLower().Equals("false")&& quickpollText.GetValue(2).ToString().ToLower().Equals("true"))
                    )
                {
                    List<String> TrueFalse = new List<String>();
                    TrueFalse.Add(quickpollText.GetValue(1).ToString());
                    TrueFalse.Add(quickpollText.GetValue(2).ToString());

                    // for each answer in the yesNo string-list create a radio button and add it to the group
                    foreach (string answerText in TrueFalse)
                    {
                        RadioButton answer = new RadioButton();
                        answer.Location = new Point(10, yValue);
                        yValue += yValueIncrement;
                        answer.Size = new Size(360, 25);
                        answer.Text = answerText;
                        answer.CheckedChanged += radioButtons_CheckedChanged;
                        checkgroup.Controls.Add(answer);
                    }

                }
                //check the polly style is not yes no both/neither
                else if (quickPollStyle != QuickPollModel.QuickPollStyle.YesNo 
                    && quickPollStyle != QuickPollModel.QuickPollStyle.YesNoBoth 
                    && quickPollStyle != QuickPollModel.QuickPollStyle.YesNoNeither )
                {
                    //if multiple choice style 
                    //then for every question string create a radio button with the question,
                    //register checked change and add the button to the group
                    for (int i = 1; i < quickpollText.Length; i++)
                    {
                        CheckBox answer = new CheckBox();
                        answer.Location = new Point(10, yValue);
                        yValue += yValueIncrement;
                        answer.Size = new Size(360, 25);
                        answer.Text = Convert.ToString(quickpollText.GetValue(i));
                        answer.CheckedChanged += new EventHandler(checkButtons_CheckedChanged);

                        checkgroup.Controls.Add(answer);
                    }
                }
                else
                {
                    //If yes no then we create a list of string to contain and add the options
                    List<String> yesNo = new List<String>();
                    yesNo.Add("Yes");
                    yesNo.Add("No");

                    //depending on poll style add extra string
                    if (quickPollStyle == QuickPollModel.QuickPollStyle.YesNoBoth)
                        yesNo.Add("Both");
                    else if (quickPollStyle == QuickPollModel.QuickPollStyle.YesNoNeither)
                        yesNo.Add("Neither");

                    // for each answer in the yesNo string-list create a radio button and add it to the group
                    foreach (string answerText in yesNo)
                    {
                        RadioButton answer = new RadioButton();
                        answer.Location = new Point(10, yValue);
                        yValue += yValueIncrement;
                        answer.Size = new Size(360, 25);
                        answer.Text = answerText;
                        answer.CheckedChanged += radioButtons_CheckedChanged;
                        checkgroup.Controls.Add(answer);
                    }

                }

            }

        }

        #endregion
    }
}

