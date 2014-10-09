using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UW.ClassroomPresenter.Model.Presentation;

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
            // Get the deck
            var deck = PresentationModel.CurrentPresentation.DeckTraversals[0].Deck;
            // Create a lock on the deck
            using (Synchronizer.Lock(deck.SyncRoot))
            {
                // Create a new, blank slide
                SlideModel slide = new SlideModel(Guid.NewGuid(), new LocalId(), SlideDisposition.Empty, UW.ClassroomPresenter.Viewer.ViewerForm.DEFAULT_SLIDE_BOUNDS);
                // Insert it
                deck.InsertSlide(slide);
                // Add to the table of contents
                using (Synchronizer.Lock(deck.TableOfContents.SyncRoot))
                {
                    TableOfContentsModel.Entry entry = new TableOfContentsModel.Entry(Guid.NewGuid(), deck.TableOfContents, slide);
                    deck.TableOfContents.Entries.Add(entry);
                }
            }
            // TODO: Need to add actual polling results to the slide, and potentially advance to it
        }

        private void stopPollButton_Click(object sender, EventArgs e)
        {
            // Close out dialog
            this.Close();
            // TODO: Advance to results slide, and end quick poll 
            
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
