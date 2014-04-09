// $Id: PresentationMessages.cs 1658 2008-07-28 17:49:52Z lining $

using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Runtime.Serialization;

using UW.ClassroomPresenter.Model;
using UW.ClassroomPresenter.Model.Network;
using UW.ClassroomPresenter.Model.Presentation;

namespace UW.ClassroomPresenter.Network.Messages.Presentation {

    [Serializable]
    public abstract class PresentationMessage : Message {
        public readonly string HumanName;

        public PresentationMessage(PresentationModel presentation) : base(presentation.Id) {
            this.AddLocalRef( presentation );
            using(Synchronizer.Lock(presentation.SyncRoot)) {
                this.HumanName = presentation.HumanName;
            }
        }

        protected override bool UpdateTarget(ReceiveContext context) {
            PresentationModel presentation = this.Target as PresentationModel;
            bool isUntitledPresentation = false;
            if (this.HumanName.Equals("Untitled Presentation")) isUntitledPresentation=true;
            if(presentation == null) {
                this.Target = presentation = new PresentationModel(((Guid)this.TargetId), context.Participant, this.HumanName,isUntitledPresentation);
            } else {
                using(Synchronizer.Lock(presentation.SyncRoot)) {
                    presentation.HumanName = this.HumanName;
                    presentation.IsUntitledPresentation = isUntitledPresentation;
                }
            }

            using(Synchronizer.Lock(context.Classroom.SyncRoot)) {
                if(!context.Classroom.Presentations.Contains(presentation))
                    context.Classroom.Presentations.Add(presentation);
            }

            return true;
        }
    }

    [Serializable]
    public sealed class PresentationInformationMessage : PresentationMessage {
        public PresentationInformationMessage(PresentationModel presentation) : base(presentation) {}

        protected override MergeAction MergeInto(Message other) {
            return (other is PresentationInformationMessage || other is PresentationEndedMessage)
                ? MergeAction.DiscardOther : base.MergeInto(other);
        }
    }

    [Serializable]
    public sealed class PresentationEndedMessage : Message {
        public PresentationEndedMessage(PresentationModel presentation) : base(presentation.Id) {}

        protected override bool UpdateTarget(ReceiveContext context) {
            PresentationModel presentation = this.Target as PresentationModel;
            if(presentation != null) {
                using(Synchronizer.Lock(context.Classroom.SyncRoot)) {
                    if(context.Classroom.Presentations.Contains(presentation))
                        context.Classroom.Presentations.Remove(presentation);
                }
            }

            return true;
        }

        protected override MergeAction MergeInto(Message other) {
            return (other is PresentationInformationMessage || other is PresentationEndedMessage)
                ? MergeAction.DiscardOther : base.MergeInto(other);
        }
    }
}
