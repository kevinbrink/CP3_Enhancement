﻿/* 
 * Author:  Gabriel Martin
 * Date:    Sep 24, 2014
 * Purpose: A custom poll class used for holding new poll data that is read by the CreateSlide method of the PPTDeckIO class.
 */

using Microsoft.Office.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace UW.ClassroomPresenter.Model.Presentation
{
    [Serializable]
    public class CustomPoll
    {
        #region Private Members

        /* All of the necessary poll properties */
        private string question;
        private string pollType;
        private int slide;
        private string correctAnswr;
        private ArrayList answrs;

        #endregion

        #region Constructors

        public CustomPoll(string question, string pollType, int slide, string correctAnswr, ArrayList answrs)
        {
            this.question = question;
            this.pollType = pollType;
            this.slide = slide;
            this.correctAnswr = correctAnswr;
            this.answrs = answrs;
        }

        #endregion

        #region Methods

        /* Appropriate getter methods for retrieving the desired data */
        public string GetQuestion()
        {
            return question;
        }
        public string GetPollType()
        {
            return pollType;
        }
        public int GetSlide()
        {
            return slide;
        }
        public string GetCorrectAnswr()
        {
            return correctAnswr;
        }
        public ArrayList GetAnswrs()
        {
            return answrs;
        }

        #endregion
    }
}
