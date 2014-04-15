// $Id: RoleMessages.cs 1554 2008-02-29 18:55:05Z cmprince $

using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Runtime.Serialization;

using UW.ClassroomPresenter.Model;
using UW.ClassroomPresenter.Model.Network;
using UW.ClassroomPresenter.Model.Presentation;
using UW.ClassroomPresenter.Network.Messages.Presentation;
using UW.ClassroomPresenter.Misc;

namespace UW.ClassroomPresenter.Network.Messages.Network {

    [Serializable]
    public abstract class RoleMessage : Message {
        protected RoleMessage(RoleModel role) : base(role.Id) {
            this.Target = role;
        }

        protected override bool UpdateTarget(ReceiveContext context) {
            using(Synchronizer.Lock(context.Participant.SyncRoot)) {
                context.Participant.Role = this.Target as RoleModel;
            }

            return false;
        }

        public static RoleMessage ForRole(RoleModel role) {
            if(role is InstructorModel) {
                return new InstructorMessage((InstructorModel) role);
            } else if(role is StudentModel) {
                return new StudentMessage((StudentModel) role);
            } else if(role is PublicModel) {
                return new PublicMessage((PublicModel) role);
            } else {
                return null;
            }
        }
    }

    [Serializable]
    public class InstructorMessage : RoleMessage {
        public readonly bool AcceptingStudentSubmissions;
        public readonly bool ForcingStudentNavigationLock;
        public readonly Int64 InstructorClockTicks;

        public InstructorMessage(InstructorModel role) : base(role) {
            using(Synchronizer.Lock(role.SyncRoot)) {
                this.AcceptingStudentSubmissions = role.AcceptingStudentSubmissions;
                // Need to add as an extension to maintain backward compatability
                this.Extension = new ExtensionWrapper( role.AcceptingQuickPollSubmissions, new Guid("{65A946F4-D1C5-426b-96DB-7AF4863CE296}") );
                this.ForcingStudentNavigationLock = role.ForcingStudentNavigationLock;
                this.InstructorClockTicks = UW.ClassroomPresenter.Misc.AccurateTiming.Now;
            }
        }

        protected override bool UpdateTarget(ReceiveContext context) {
            InstructorModel role = this.Target as InstructorModel;
            if(role == null)
                this.Target = role = new InstructorModel(((Guid) this.TargetId));

            using(Synchronizer.Lock(role.SyncRoot)) {
                role.AcceptingStudentSubmissions = this.AcceptingStudentSubmissions;
                // Deserialize the extension
                ExtensionWrapper extension = this.Extension as ExtensionWrapper;
                if (extension != null) {
                    if( extension.ExtensionType.Equals( new Guid( "{65A946F4-D1C5-426b-96DB-7AF4863CE296}" ) ) ) {
                        bool acceptingQuickPollSubmissions = (bool)extension.ExtensionObject;
                        role.AcceptingQuickPollSubmissions = acceptingQuickPollSubmissions;
                    }
                    else {
                        Trace.WriteLine("Unknown Extension id=" + extension.ExtensionType.ToString());
                    }
                }
                role.ForcingStudentNavigationLock = this.ForcingStudentNavigationLock;
            }

            // Update the clock skew from our current value.
            using( Synchronizer.Lock( context.Model.ViewerState.Diagnostic.SyncRoot ) ) {
                context.Model.ViewerState.Diagnostic.AddSkewEntry( this.InstructorClockTicks );
            }

            base.UpdateTarget(context);

            return true;
        }
    }

    [Serializable]
    public class StudentMessage : RoleMessage {
        public StudentMessage(StudentModel role) : base(role) {
            // Currently StudentModel has no published properties.
        }

        protected override bool UpdateTarget(ReceiveContext context) {
            StudentModel role = this.Target as StudentModel;
            if(role == null)
                this.Target = role = new StudentModel(((Guid) this.TargetId));

            base.UpdateTarget(context);

            return true;
        }
    }

    [Serializable]
    public class PublicMessage : RoleMessage {
        public PublicMessage(PublicModel role) : base(role) {
            // Currently PublicModel has no published properties.
        }

        protected override bool UpdateTarget(ReceiveContext context) {
            PublicModel role = this.Target as PublicModel;
            if(role == null)
                this.Target = role = new PublicModel(((Guid) this.TargetId));

            base.UpdateTarget(context);

            return true;
        }
    }

    [Serializable]
    public class InstructorCurrentPresentationChangedMessage : PresentationMessage {
        public InstructorCurrentPresentationChangedMessage(PresentationModel presentation) : base(presentation) {}

        protected override bool UpdateTarget(ReceiveContext context) {
            bool update = base.UpdateTarget(context);

            InstructorModel instructor = this.Parent != null ? this.Parent.Target as InstructorModel : null;
            if(instructor != null) {
                using(Synchronizer.Lock(instructor.SyncRoot)) {
                    instructor.CurrentPresentation = this.Target as PresentationModel;
                }
            }

            return update;
        }
    }

    [Serializable]
    public class InstructorCurrentDeckTraversalChangedMessage : DeckTraversalMessage {
        public InstructorCurrentDeckTraversalChangedMessage(DeckTraversalModel traversal, bool addLocalRef) : base(traversal, addLocalRef) { }
        public InstructorCurrentDeckTraversalChangedMessage(DeckTraversalModel traversal) : this(traversal,true) {}

        protected override bool UpdateTarget(ReceiveContext context) {
            bool update = base.UpdateTarget(context);

            InstructorModel instructor = this.Parent != null ? this.Parent.Target as InstructorModel : null;
            if(instructor != null) {
                using(Synchronizer.Lock(instructor.SyncRoot)) {
                    instructor.CurrentDeckTraversal = this.Target as DeckTraversalModel;
                }
            }

            return update;
        }
    }
}
