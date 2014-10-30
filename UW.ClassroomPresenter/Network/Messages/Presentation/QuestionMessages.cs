using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Runtime.Serialization;

using UW.ClassroomPresenter.Model;
using UW.ClassroomPresenter.Model.Network;
using UW.ClassroomPresenter.Model.Presentation;
using System.Windows.Forms;

namespace UW.ClassroomPresenter.Network.Messages.Presentation
{

    [Serializable]
    public abstract class QuestionMessage : Message
    {
        public readonly string Question;

        public QuestionMessage(QuestionModel question)
            : base(question.Id)
        {
            this.AddLocalRef(question);
            using (Synchronizer.Lock(question.SyncRoot))
            {
                this.Question = question.Question;
            }
        }

        #region Receiving

        protected override bool UpdateTarget(ReceiveContext context)
        {
            /* Test dialog with question */
            string messageBoxText = this.Question;
            string caption = "Student's Question";
            MessageBox.Show(messageBoxText, caption);

            return true;
        }

        #endregion
    }

    [Serializable]
    public sealed class QuestionInformationMessage : QuestionMessage
    {
        public QuestionInformationMessage(QuestionModel question) : base(question) { }

        protected override MergeAction MergeInto(Message other)
        {
            return (other is QuestionInformationMessage) ? MergeAction.KeepBothInOrder : base.MergeInto(other);
        }
    }
}
