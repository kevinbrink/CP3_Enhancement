// $Id: ImageSheetMessages.cs 1705 2008-08-12 21:40:25Z lamphare $

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using System.Threading;

using UW.ClassroomPresenter.Model;
using UW.ClassroomPresenter.Model.Presentation;

namespace UW.ClassroomPresenter.Network.Messages.Presentation {
    [Serializable]
    public class ImageSheetMessage : SheetMessage {
        public readonly ByteArray MD5;
        private Image image_;

        public ImageSheetMessage(ImageSheetModel sheet, SheetCollection collection) : base(sheet, collection) {
            if (sheet.Deck != null) {
                // we are in background mode
                // MD5 is not a Published property of ImageSheetModel.
                using (Synchronizer.Lock(sheet.SyncRoot)) {
                    this.MD5 = sheet.MD5;
                }

                // Do NOT try to serialize the ImageContent.  That is done by a separate DeckSlideContentMessage.
            } else {
                // we are in ImageIt mode
                using (Synchronizer.Lock(sheet.SyncRoot)) {
                    this.image_ = sheet.Image;
                }
            }
        }

        protected override bool UpdateTarget(ReceiveContext context) {
            DeckModel deck = (this.Parent != null && this.Parent.Parent != null) ? this.Parent.Parent.Target as DeckModel : null;


            ImageSheetModel sheet = this.Target as ImageSheetModel;
            if(sheet == null) {
                ///create our imagesheet based on what type of image it is
                if (image_ != null) {
                    // ImageIt image
                    this.Target = sheet = new ImageSheetModel((Guid)this.TargetId, this.image_, false, this.Bounds.Location, this.Bounds.Size, this.Height);
                    ///make the image visible
                    sheet.Visible = true;
                } else {
                    // Background image
                    this.Target = sheet = new ImageSheetModel(deck, ((Guid)this.TargetId), this.Disposition | SheetDisposition.Remote, this.Bounds, this.MD5, this.Height);
                }
                
            }
            if (image_ != null) {
                using (Synchronizer.Lock(sheet.SyncRoot)) {
                    sheet.Image = this.image_;
                }
            }
            base.UpdateTarget(context);

            return true;
            
        }
    }
}
