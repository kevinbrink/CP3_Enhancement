using System;
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
        private List<String> instructorQA;

        public PollOptions()
        {
            InitializeComponent();
        }

        public PollOptions(PresenterModel modelIn)
        {
            InitializeComponent();
            m_Model = modelIn;
            using (Synchronizer.Lock(modelIn.Participant.SyncRoot))
            {
                m_Role = modelIn.Participant.Role;
            }
        }

        public List<String> InstructorQA
        { 
            get { return instructorQA; } 
            set { instructorQA = value; }
        }

        public PresenterModel Model
        {
            get { return m_Model; }
            set { m_Model = value; }
        }

        /*protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            e.Cancel = true;
            this.Hide();
        }*/


        private void startPollButton_Click(object sender, EventArgs e)
        {
            DeckModel deck; // current deck
            LocalId newslide = null; // LocalID for new Slide

            // Disable everything but the stop poll button
            whenDoneRadioButton.Enabled = false;
            liveRadioButton.Enabled = false;
            neverRadioButton.Enabled = false;
            startPollButton.Enabled = false;
            // Enable stop button
            stopPollButton.Enabled = true;
            // Start timer
            seconds = 0;
            minutes = 0;
            pollTimer.Start();            

            // Get the deck
            using (Synchronizer.Lock(this.m_Model.Workspace.CurrentDeckTraversal.SyncRoot))
            {
                using (Synchronizer.Lock(this.m_Model.Workspace.CurrentDeckTraversal.Value))
                {
                    deck = this.m_Model.Workspace.CurrentDeckTraversal.Value.Deck;
                }
            }
           
            // create results slide if whenDone or live is checked
            if (whenDoneRadioButton.Checked || liveRadioButton.Checked)
            {

                // Create a lock on the deck 
                using (Synchronizer.Lock(deck.SyncRoot))
                {
                    ///insert the new slide with poll data into our deck
                    SlideModel slide;
                    using (Synchronizer.Lock(this.m_Model.Workspace.CurrentDeckTraversal.SyncRoot))
                    {
                        using (Synchronizer.Lock(this.m_Model.Workspace.CurrentDeckTraversal.Value))
                        {
                            using (Synchronizer.Lock(this.m_Model.Workspace.CurrentDeckTraversal.Value.Current.Slide))
                            {
                                slide = new SlideModel(Guid.NewGuid(), new LocalId(), SlideDisposition.Empty, UW.ClassroomPresenter.Viewer.ViewerForm.DEFAULT_SLIDE_BOUNDS);                                
                            }

                            // get the LocaID for the new slide
                            using (Synchronizer.Lock(slide.SyncRoot))
                            {
                                newslide = slide.LocalId;
                            }
                        }
                    }

                    using (Synchronizer.Lock(deck.SyncRoot))
                    {
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
            }
            
            // start polling
            AcceptingQuickPollSubmissionsMenuItem.CreateNewQuickPoll(this.m_Model, this.m_Role, instructorQA, newslide);

            if (this.m_Role is InstructorModel)
            {   
                using (Synchronizer.Lock(this.m_Role.SyncRoot))
                {
                    ((InstructorModel)this.m_Role).AcceptingQuickPollSubmissions = true;
                }
            }

            // switch to slide if live is checked
            if (liveRadioButton.Checked)
            {   // Yes, we want to display                   

                // lock and switch to new slide
                using (Synchronizer.Lock(this.m_Model.Workspace.CurrentDeckTraversal.SyncRoot))
                {
                    DeckTraversalModel tmpTrav = this.m_Model.Workspace.CurrentDeckTraversal.Value;
                    using (Synchronizer.Lock(tmpTrav.SyncRoot))
                    {
                        tmpTrav.Current = entry;
                    }
                }
            }
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
            //this.Close();
            // TODO: Advance to results slide, and end quick poll 

            pollTimer.Stop();

            whenDoneRadioButton.Enabled = true;
            liveRadioButton.Enabled = true;
            neverRadioButton.Enabled = true;
            startPollButton.Enabled = true;
            stopPollButton.Enabled = false;

            //seconds = 0;
            //minutes = 0;


            if (whenDoneRadioButton.Checked)
            {   // Yes, we want to display                   

                // lock and switch to new slide
                using (Synchronizer.Lock(this.m_Model.Workspace.CurrentDeckTraversal.SyncRoot))
                {
                    DeckTraversalModel tmpTrav = this.m_Model.Workspace.CurrentDeckTraversal.Value;
                    using (Synchronizer.Lock(tmpTrav.SyncRoot))
                    {
                        tmpTrav.Current = entry;
                    }
                }
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
