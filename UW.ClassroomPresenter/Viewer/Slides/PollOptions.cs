﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UW.ClassroomPresenter.Model.Presentation;
using UW.ClassroomPresenter.Model;
using UW.ClassroomPresenter.Model.Network;
using UW.ClassroomPresenter.Viewer.Menus;

namespace UW.ClassroomPresenter.Viewer.Slides
{
    public partial class PollOptions : Form
    {
        private uint seconds = 0;
        private uint minutes = 0;
        private PresenterModel m_Model;
        private RoleModel m_Role;
        private TableOfContentsModel.Entry entry;

        public PollOptions()
        {
            InitializeComponent();
        }

        public PollOptions(PresenterModel modelIn, RoleModel roleIn)
        {
            InitializeComponent();
            m_Model = modelIn;
            m_Role = roleIn;
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
                ///insert the new slide into our deck
                SlideModel slide;
                using (Synchronizer.Lock(deck.SyncRoot))
                {
                    slide = new SlideModel(Guid.NewGuid(), new LocalId(), SlideDisposition.Empty, UW.ClassroomPresenter.Viewer.ViewerForm.DEFAULT_SLIDE_BOUNDS);
                    deck.Dirty = true;                    
                    deck.InsertSlide(slide);
                }

                ///get the index of the current slide that's selected,
                ///so that we can insert our blank slide there
                int current_slide_index;
                using (Synchronizer.Lock(this.m_Model.Workspace.CurrentDeckTraversal.SyncRoot))
                {
                    using (Synchronizer.Lock(this.m_Model.Workspace.CurrentDeckTraversal.Value))
                    {
                        current_slide_index = this.m_Model.Workspace.CurrentDeckTraversal.Value.AbsoluteCurrentSlideIndex;
                    }
                }

                ///don't do anything if no object is selected
                if (current_slide_index < 0)
                {
                    return;
                }

                ///Insert our blank slide after the current index. This is modeled after the powerpoint
                ///UI
                using (Synchronizer.Lock(deck.TableOfContents.SyncRoot))
                {
                    entry = new TableOfContentsModel.Entry(Guid.NewGuid(), deck.TableOfContents, slide);
                    deck.TableOfContents.Entries.Insert(current_slide_index, entry);

                }
            }
            // TODO: Need to add actual polling results to the slide, and potentially advance to it
        }

        private void stopPollButton_Click(object sender, EventArgs e)
        {                                    
            AcceptingQuickPollSubmissionsMenuItem.EndQuickPoll(this.m_Model);

            if (this.m_Role is InstructorModel)
            {                                                        
                using (Synchronizer.Lock(this.m_Role.SyncRoot))
                {
                    ((InstructorModel)this.m_Role).AcceptingQuickPollSubmissions = false;
                }
            }          

            // Close out dialog
            this.Close();
            // TODO: Advance to results slide, and end quick poll 

            // Just ended a quickpoll
            DialogResult displayPoll = MessageBox.Show("Display results from poll?",
                                                        "Display polling results",
                                                        MessageBoxButtons.YesNo);
            if (displayPoll == DialogResult.Yes)
            {   // Yes, we want to display
                // TODO: We need to actually switch to the polling results and figure out how we want to display it all                     

                // lock and switch to new slide
                using (Synchronizer.Lock(this.m_Model.Workspace.CurrentDeckTraversal.SyncRoot))
                {
                    DeckTraversalModel tmpTrav = this.m_Model.Workspace.CurrentDeckTraversal.Value;
                    using (Synchronizer.Lock(tmpTrav.SyncRoot))
                    {
                        tmpTrav.Current = entry;
                    }
                }

                MessageBox.Show("Results Slide here! ");
            }
            
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