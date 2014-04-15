// $Id: DeckMessages.cs 1025 2006-07-14 02:41:46Z pediddle $

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using System.Threading;

using UW.ClassroomPresenter.Model.Presentation;

namespace UW.ClassroomPresenter.Network.Messages.Presentation {
    [Serializable]
    public abstract class DeckMessage : Message {
        public readonly string HumanName;
        public readonly DeckDisposition Disposition;
        public readonly Color DeckBackgroundColor;

        public DeckMessage(DeckModel deck) : base(deck.Id) {
            this.AddLocalRef( deck );
            using(Synchronizer.Lock(deck.SyncRoot)) {
                this.Group = deck.Group;
                this.HumanName = deck.HumanName;
                this.Disposition = deck.Disposition;
                this.DeckBackgroundColor = deck.DeckBackgroundColor;
            }
        }

        protected override bool UpdateTarget(ReceiveContext context) {
            DeckModel deck = this.Target as DeckModel;
            if(deck == null) {
                this.Target = deck = new DeckModel(((Guid) this.TargetId), this.Disposition | DeckDisposition.Remote, this.HumanName);
            } else {
                using(Synchronizer.Lock(deck.SyncRoot)) {
                    deck.HumanName = this.HumanName;
                }
            }
            using (Synchronizer.Lock(deck.SyncRoot)) {
                deck.DeckBackgroundColor = this.DeckBackgroundColor;
            }

            return true;
        }
    }

    [Serializable]
    public sealed class DeckInformationMessage : DeckMessage {
        public DeckInformationMessage(DeckModel deck) : base(deck) {}

        protected override MergeAction MergeInto(Message other) {
            return (other is DeckInformationMessage) ? MergeAction.DiscardOther : base.MergeInto(other);
        }
    }
}
