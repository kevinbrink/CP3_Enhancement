// $Id: StudentSubmissionNetworkService.cs 606 2005-08-26 19:42:21Z pediddle $

using System;
using System.Diagnostics;
using System.Threading;

using Microsoft.Ink;

using UW.ClassroomPresenter.Model;
using UW.ClassroomPresenter.Model.Network;
using UW.ClassroomPresenter.Model.Workspace;
using UW.ClassroomPresenter.Model.Presentation;

using UW.ClassroomPresenter.Network.Messages.Network;
using UW.ClassroomPresenter.Network.Messages.Presentation;
using UW.ClassroomPresenter.Model.Viewer;

namespace UW.ClassroomPresenter.Network.Messages {
    /// <summary>
    /// This component is responsible for listening to indications from the UI 
    /// that the current slide ink is supposed to be sent as a student submission
    /// </summary>
    public class StudentSubmissionNetworkService : IDisposable {
        #region Private Members

        /// <summary>
        /// The sending queue onto which all network messages are sent
        /// </summary>
        private readonly SendingQueue m_Sender;

        /// <summary>
        /// The presenter model
        /// </summary>
        private readonly PresenterModel m_Model;

        /// <summary>
        /// The event dispatcher that is used to marshal change events
        /// </summary>
        private readonly EventQueue.PropertyEventDispatcher m_SendChangeDispatcher;

        /// <summary>
        /// Flag indicating if the object has been collected or not
        /// </summary>
        private bool m_Disposed;

        #endregion

        #region Construction

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sender">The network queue to send messages on</param>
        /// <param name="model">The presenter model</param>
        public StudentSubmissionNetworkService(SendingQueue sender, PresenterModel model) {
            this.m_Sender = sender;
            this.m_Model = model;

            // Setup the event listener for this 
            this.m_SendChangeDispatcher = new EventQueue.PropertyEventDispatcher( this.m_Sender, new PropertyEventHandler(this.HandleSendSubmission) );
            
            //FV: Not locking here resolves a lock order warning.
            //using( Synchronizer.Lock( this.m_Model.ViewerState.SyncRoot ) ) {
                this.m_Model.ViewerState.Changed["StudentSubmissionSignal"].Add( this.m_SendChangeDispatcher.Dispatcher );
            //}
        }

        #endregion

        #region Submissions

        /// <summary>
        /// Send the question to the instructor
        /// </summary>
        /// <param name="sender">The object which sent this event, i.e. this class</param>
        /// <param name="args">The parameters for the property</param>
        private void HandleSendSubmission( object sender, PropertyEventArgs args ) {

            /* The following code was modified by Gabriel Martin on Oct 29, 2014 */

            using (Synchronizer.Lock(SubmissionStatusModel.GetInstance().SyncRoot)) {
                SubmissionStatusModel.GetInstance().SubmissionStatus = SubmissionStatusModel.Status.NotReceived;
            }
            
            /* Question message */
            UW.ClassroomPresenter.Network.Messages.Message ques;

            /* Build the message that we're going to send to the instructor */
            using( this.m_Model.Workspace.Lock() ) {
                /* The question which we'll be sending */
                ques = new QuestionInformationMessage(this.m_Model.StudentQuestion);
                ques.Group = Groups.Group.Submissions;

                /* Send the message */
                this.m_Sender.Send(ques);
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Destructor
        /// </summary>
        ~StudentSubmissionNetworkService() {
            this.Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources associated with this component
        /// </summary>
        public void Dispose() {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Internal version of dispose that does all the real work
        /// </summary>
        /// <param name="disposing">True if we are in the process of disposing</param>
        protected virtual void Dispose(bool disposing) {
            if(this.m_Disposed) return;
            if(disposing) {
                using( Synchronizer.Lock( this.m_Model.ViewerState.SyncRoot ) ) {
                    this.m_Model.ViewerState.Changed["StudentSubmissionSignal"].Remove( this.m_SendChangeDispatcher.Dispatcher );
                }
            }
            this.m_Disposed = true;
        }

        #endregion
    }
}
