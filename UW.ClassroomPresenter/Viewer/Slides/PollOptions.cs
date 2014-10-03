using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UW.ClassroomPresenter.Viewer.Slides
{
    public partial class PollOptions : Form
    {
        private uint seconds = 0;
        private uint minutes = 0;
        public PollOptions()
        {
            InitializeComponent();
        }

        private void startPollButton_Click(object sender, EventArgs e)
        {
            // Disable everything but the stop poll button
            whenDoneRadioButton.Enabled = false;
            liveRadioButton.Enabled = false;
            neverRadioButton.Enabled = false;
            startPollButton.Enabled = false;
            // Enable stop button
            stopPollButton.Enabled = true;
            // Start timer
            pollTimer.Start();
        }

        private void stopPollButton_Click(object sender, EventArgs e)
        {
            // Close out dialog
            this.Close();
            // TODO: Advance to results slide
            // TODO: Create a new slide in the presentation to display the polling results
        }

        // An event handler for each tick (in this case, 1 second) of the timer
        private void pollTimer_Tick(object sender, EventArgs e)
        {
            // If we're at 59 seconds
            if (seconds == 59)
            {
                // Reset
                seconds = 0;
                // Increment minutes
                ++minutes;
            }
            else
            {
                // Just increment the seconds
                ++seconds;
            }
            // Update the label
            updateTimeLabel.Text = String.Format("{0}:{1:00}", minutes, seconds);
        }
    }
}
