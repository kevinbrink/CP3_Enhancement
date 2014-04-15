// $Id: ImageSheetModel.cs 1631 2008-07-07 22:15:35Z lamphare $

using System;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Threading;
using UW.ClassroomPresenter.Viewer.TextAndImageEditing;


namespace UW.ClassroomPresenter.Model.Presentation {
    [Serializable]
    ///represents data for images loaded by student or instructor
    public class ImageSheetModel : EditableSheetModel {

        private readonly DeckModel m_Deck;
        public DeckModel Deck {
            [DebuggerStepThrough]
            get { return this.m_Deck; }
        }

        private readonly ByteArray m_MD5;
        public ByteArray MD5 {
            [DebuggerStepThrough]
            get { return this.m_MD5; }
        }

        private bool visible_;
        public bool Visible {
            get { return visible_; }
            set { visible_ = value; }
        }

        public ImageSheetModel(DeckModel deck, Guid id, SheetDisposition disp, Rectangle bounds, ByteArray md5, int height) : base(id, disp, bounds, false, height) {
            this.m_MD5 = md5;
            this.m_Deck = deck;
            this.visible_ = true;
        }

        #region ImageIt Images (Julia)
        /// <summary>
        /// the image we have.
        /// </summary>
        private Image image_;

        public ImageSheetModel (Guid id, Image image, bool is_editable, Point p, int height): base(id, SheetDisposition.All, new Rectangle(p, image.Size + new Size(ImageIt.image_padding, ImageIt.image_padding)), is_editable, height){
            image_ = image;
        }

        public ImageSheetModel (Guid id, Image image, bool is_editable, Point p, Size s, int height): base(id, SheetDisposition.All, new Rectangle(p, s + new Size(ImageIt.image_padding, ImageIt.image_padding)), is_editable, height){
            image_ = image;
        }

        /// <summary>
        /// FIXME: image should be published, or
        /// Image should only be set once
        /// </summary>
        [Published]
        public Image Image {
            get { return image_; }
            set { image_ = value; }
        }

        public ImageSheetModel Clone() {
            Rectangle bounds;
            using (Synchronizer.Lock(this.SyncRoot)) {
                bounds = this.Bounds;
            }
            return new ImageSheetModel(Guid.NewGuid(), this.image_, is_editable_, bounds.Location, bounds.Size, this.Height);
        }

        #endregion

        
    }
}
