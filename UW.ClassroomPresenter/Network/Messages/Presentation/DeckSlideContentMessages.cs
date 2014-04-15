// $Id: DeckSlideContentMessages.cs 1025 2006-07-14 02:41:46Z pediddle $

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using System.Threading;

using UW.ClassroomPresenter.Model.Presentation;

namespace UW.ClassroomPresenter.Network.Messages.Presentation {

    [Serializable]
    public class DeckSlideContentMessage : Message {
        /// <summary>
        /// The <see cref="ImageHashtable.ImageHashTableItem"/> serves as a <em>serializable</em> wrapper around an <see cref="Image"/>.
        /// </summary>
        public readonly ImageHashtable.ImageHashTableItem Content;

        public DeckSlideContentMessage(DeckModel deck, ByteArray hash) : base(hash) {
            using(Synchronizer.Lock(deck.SyncRoot)) {
                Image image = deck.GetSlideContent(hash);

                if(image == null)
                    throw new ArgumentException("The specified ByteArray does not map to slide content in the specified deck.", "hash");

                this.Target = this.Content = new ImageHashtable.ImageHashTableItem(hash, image);
                this.AddLocalRef( this.Target );
            }
        }

        protected override bool UpdateTarget(ReceiveContext context) {
            Debug.Assert(this.Content != null);
            if(this.Content.image == null)
                throw new ArgumentException("The deserialized content image is null.", "Content");

            this.Target = this.Content;

            DeckModel deck = this.Parent != null ? this.Parent.Target as DeckModel : null;
            if(deck != null) {
                using(Synchronizer.Lock(deck.SyncRoot)) {
                    // Note: If the deck already has content with this hash, nothing will happen.
                    deck.AddSlideContent(this.Content.key, this.Content.image);
                }
            }

            return true;
        }
    }
}
