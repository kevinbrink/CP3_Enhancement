/* 
 * Author: Gabriel Martin
 * SN:     040689552
 * Date:   Oct 30, 2014
 * Purpose: A simple question class used for holding question data that the student submits to the instructor.
 */

using System;

using UW.ClassroomPresenter.Model.Network;

namespace UW.ClassroomPresenter.Model
{
    /// <summary>
    /// Represents all the questions that are asked by students.
    /// </summary>
    [Serializable]
    public class QuestionModel : PropertyPublisher
    {

        #region Private Members

        private readonly Guid m_Id;

        /// <summary>
        /// The text of the question
        /// </summary>
        private string m_Question;

        #endregion

        #region Constructors

        public QuestionModel(Guid Id, string question)
        {
            this.m_Id = Id;
            this.m_Question = question;
        }

        #endregion

        #region Id

        /// <summary>
        /// Guid to uniquely represent this PresentationModel
        /// </summary>
        public Guid Id
        {
            get { return this.m_Id; }
        }

        #endregion

        #region Question

        /// <summary>
        /// Public property publisher for the Human Name
        /// </summary>
        [Published]
        public string Question
        {
            get { return this.GetPublishedProperty("Question", ref this.m_Question); }
            set { this.SetPublishedProperty("Question", ref this.m_Question, value); }
        }

        #endregion
    }
}
