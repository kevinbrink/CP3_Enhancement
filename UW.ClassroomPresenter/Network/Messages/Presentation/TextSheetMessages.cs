// $Id: TextSheetMessages.cs 1384 2007-06-22 22:01:36Z fred $

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
    public class TextSheetMessage : SheetMessage {
        public readonly string Text;
        public readonly Font font_;
        public readonly Color color_;
        public readonly bool is_public_;

        public TextSheetMessage(TextSheetModel sheet, SheetCollection collection) : base(sheet, collection) {
            using(Synchronizer.Lock(sheet.SyncRoot)) {
                this.Text = sheet.Text;
                this.font_ = sheet.Font;
                this.color_ = sheet.Color;
                this.is_public_ = sheet.IsPublic;

            }
        }

        protected override bool UpdateTarget(ReceiveContext context) {
            
            TextSheetModel sheet = this.Target as TextSheetModel;
            if (sheet == null) {
                ///if we sent it, this means that by definition the sheet is public
                ///edit: made is_editable true so that instructors can copy and paste student code
                this.Target = sheet = new TextSheetModel(((Guid)this.TargetId), this.Text, true, true, this.Bounds, this.color_, this.font_);
            }

            using (Synchronizer.Lock(sheet.SyncRoot)) {
                sheet.Text = this.Text;
                sheet.Font = this.font_;
                sheet.IsEditable = false;
                ///if we sent it, this means that by definition the sheet is public
                sheet.IsPublic = true;
                sheet.Color = this.color_;
            }

            base.UpdateTarget(context);

            return true;
        }
    }
}
