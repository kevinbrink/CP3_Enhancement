

using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Windows;
using System.Text;
using NativeWifi;

using UW.ClassroomPresenter.Model;
using UW.ClassroomPresenter.Model.Presentation;
using UW.ClassroomPresenter.Model.Network;
using UW.ClassroomPresenter.Model.Workspace;
using UW.ClassroomPresenter.Model.Viewer;


namespace UW.ClassroomPresenter.Viewer.SecondMonitor
{
    /// <summary>
    /// Represents the deck navigation buttons
    /// </summary>
    /// Class Modified by Eric Dodds to include IP display functionality
    public class FullScreenButtons
    {
        /// <summary>
        /// The model that this UI component modifies
        /// </summary>
        private readonly PresenterModel m_Model;

        NavigationToolBarButton back, forward;
        Button ret;
        Button displayIP;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model">The model that this class modifies</param>
        public FullScreenButtons(PresenterModel model, ControlEventQueue dispatcher)
        {
            this.m_Model = model;


            /// <summary>
            /// Constructs all of the child buttons belonging to this class
            /// </summary>
            /// <param name="parent">The parent ToolStrip to place the buttons in</param>
            /// <param name="dispatcher">The event queue to use for events</param>

            this.ret = new ReturnButton(this.m_Model);
            this.displayIP = new DisplayIPButton(this.m_Model);

            // Create the back button
            this.back = new BackwardNavigationToolBarButton(dispatcher, this.m_Model);
            this.back.Image = UW.ClassroomPresenter.Properties.Resources.left;
            //this.back.AutoSize = true;

            // Create the forward button
            this.forward = new ForwardNavigationToolBarButton(dispatcher, this.m_Model);
            this.forward.Image = UW.ClassroomPresenter.Properties.Resources.right;

        }
        // Add the buttons to the parent 
        public void AddButtons(ViewerPresentationLayout parent)
        {

            this.back.Size = new Size(50, 50);
            this.forward.Size = new Size(50, 50);
            this.ret.Size = new Size(50, 50);
            this.displayIP.Size = new Size(100, 50);
            this.back.Location = new Point(0, 0);//parent.MainSlideView.Height);
            this.forward.Location = new Point(this.back.Right, 0);
            this.ret.Location = new Point(this.forward.Right, 0);
            this.displayIP.Location = new Point(this.ret.Right, 0);

            parent.MainSlideView.Controls.Add(this.back);
            parent.MainSlideView.Controls.Add(this.forward);
            parent.MainSlideView.Controls.Add(this.ret);
            parent.MainSlideView.Controls.Add(this.displayIP);

        }

        public void RemoveButtons(ViewerPresentationLayout parent)
        {
            parent.MainSlideView.Controls.Remove(this.forward);
            parent.MainSlideView.Controls.Remove(this.back);
            parent.MainSlideView.Controls.Remove(this.ret);
            parent.MainSlideView.Controls.Remove(this.displayIP);
        }




        #region NavigationToolBarButton

        /// <summary>
        /// Abstract class representing navigation toolbar buttons
        /// </summary>
        protected abstract class NavigationToolBarButton : Button
        {
            /// <summary>
            /// Event queue to post events to
            /// </summary>
            private readonly ControlEventQueue m_EventQueue;
            /// <summary>
            /// Property dispatcher
            /// </summary>
            private readonly IDisposable m_CurrentDeckTraversalDispatcher;
            /// <summary>
            /// Property Listener
            /// </summary>
            private readonly EventQueue.PropertyEventDispatcher m_TraversalChangedDispatcher;

            /// <summary>
            /// The model that this class modifies
            /// </summary>
            private readonly PresenterModel m_Model;
            /// <summary>
            /// The decktraversal model that this class modifies
            /// </summary>
            private DeckTraversalModel m_CurrentDeckTraversal;
            /// <summary>
            /// Signals if this object has been disposed or not
            /// </summary>
            private bool m_Disposed;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="dispatcher">The event queue</param>
            /// <param name="model">The model</param>
            protected NavigationToolBarButton(ControlEventQueue dispatcher, PresenterModel model)
            {
                this.m_EventQueue = dispatcher;
                this.m_Model = model;

                // Listen to changes in the deck traversal
                this.m_TraversalChangedDispatcher = new EventQueue.PropertyEventDispatcher(this.m_EventQueue, new PropertyEventHandler(this.HandleTraversalChanged));
                // Don't call the event handler immediately (use Listen instead of ListenAndInitialize)
                // because it needs to be delayed until Initialize() below.
                this.m_CurrentDeckTraversalDispatcher =
                    this.m_Model.Workspace.CurrentDeckTraversal.Listen(dispatcher,
                    delegate(Property<DeckTraversalModel>.EventArgs args)
                    {
                        this.CurrentDeckTraversal = args.New;
                    });
            }

            protected virtual void Initialize()
            {
                using (Synchronizer.Lock(this.m_Model.Workspace.CurrentDeckTraversal.SyncRoot))
                {
                    this.CurrentDeckTraversal = this.m_Model.Workspace.CurrentDeckTraversal.Value;
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (this.m_Disposed) return;
                try
                {
                    if (disposing)
                    {
                        this.m_CurrentDeckTraversalDispatcher.Dispose();
                        // Unregister event listeners via the CurrentDeckTraversal setter
                        this.CurrentDeckTraversal = null;
                    }
                }
                finally
                {
                    base.Dispose(disposing);
                }
                this.m_Disposed = true;
            }

            protected abstract string TraversalProperty { get; }

            protected virtual DeckTraversalModel CurrentDeckTraversal
            {
                get { return this.m_CurrentDeckTraversal; }
                set
                {
                    if (this.m_CurrentDeckTraversal != null)
                    {
                        this.m_CurrentDeckTraversal.Changed[this.TraversalProperty].Remove(this.m_TraversalChangedDispatcher.Dispatcher);
                    }

                    this.m_CurrentDeckTraversal = value;

                    if (this.m_CurrentDeckTraversal != null)
                    {
                        this.m_CurrentDeckTraversal.Changed[this.TraversalProperty].Add(this.m_TraversalChangedDispatcher.Dispatcher);
                    }

                    this.m_TraversalChangedDispatcher.Dispatcher(this, null);
                }
            }

            protected abstract void HandleTraversalChanged(object sender, PropertyEventArgs args);
        }

        #endregion

        #region ForwardNavigationToolBarButton

        /// <summary>
        /// Button handling forward navigation in decks
        /// </summary>
        protected class ForwardNavigationToolBarButton : NavigationToolBarButton
        {
            /// <summary>
            /// The presenter model
            /// NOTE: Is this replicated with the base class???
            /// </summary>
            private readonly PresenterModel m_Model;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="dispatcher">The event queue</param>
            /// <param name="model">The model</param>
            public ForwardNavigationToolBarButton(ControlEventQueue dispatcher, PresenterModel model)
                : base(dispatcher, model)
            {
                this.m_Model = model;
                //this.ToolTipText = "Go to the next slide.";

                this.Initialize();
            }

            /// <summary>
            /// This class listens to changes in the "Next" property of the deck traversal
            /// </summary>
            protected override string TraversalProperty
            {
                get { return "Next"; }
            }

            /// <summary>
            /// Handles changes to the DeckTraversal and the "Next" property
            /// </summary>
            /// <param name="sender">The event sender</param>
            /// <param name="args">Arguments</param>
            protected override void HandleTraversalChanged(object sender, PropertyEventArgs args)
            {
                if (this.CurrentDeckTraversal == null)
                {
                    this.Enabled = false;
                }
                else
                {
                    using (Synchronizer.Lock(this.CurrentDeckTraversal.SyncRoot))
                    {
                        using (Synchronizer.Lock(this.CurrentDeckTraversal.Deck.SyncRoot))
                        {
                            // Update the button state.  Enable the button if there is a "next" slide,
                            // OR if this is a non-remote whiteboard deck (to which we can add additional slides).
                            if (((this.CurrentDeckTraversal.Deck.Disposition & DeckDisposition.Whiteboard) != 0) && ((this.CurrentDeckTraversal.Deck.Disposition & DeckDisposition.Remote) == 0))
                                this.Enabled = true;
                            else
                                this.Enabled = (this.CurrentDeckTraversal.Next != null);


                        }
                    }
                }
            }

            /// <summary>
            /// Handle the button being clicked
            /// </summary>
            /// <param name="args">The event arguments</param>
            protected override void OnClick(EventArgs args)
            {
                using (Synchronizer.Lock(this))
                {
                    if (this.CurrentDeckTraversal == null) return;
                    using (Synchronizer.Lock(this.CurrentDeckTraversal.SyncRoot))
                    {
                        if (this.CurrentDeckTraversal.Next != null)
                            this.CurrentDeckTraversal.Current = this.CurrentDeckTraversal.Next;
                        else
                        {
                            // Add a whiteboard slide if we are at the end of the deck
                            using (Synchronizer.Lock(this.CurrentDeckTraversal.Deck.SyncRoot))
                            {
                                if (((this.CurrentDeckTraversal.Deck.Disposition & DeckDisposition.Whiteboard) != 0) && ((this.CurrentDeckTraversal.Deck.Disposition & DeckDisposition.Remote) == 0))
                                {
                                    // Add the new slide
                                    SlideModel slide = new SlideModel(Guid.NewGuid(), new LocalId(), SlideDisposition.Empty, UW.ClassroomPresenter.Viewer.ViewerForm.DEFAULT_SLIDE_BOUNDS);
                                    this.CurrentDeckTraversal.Deck.InsertSlide(slide);
                                    using (Synchronizer.Lock(this.CurrentDeckTraversal.Deck.TableOfContents.SyncRoot))
                                    {
                                        // Add the new table of contents entry
                                        TableOfContentsModel.Entry entry = new TableOfContentsModel.Entry(Guid.NewGuid(), this.CurrentDeckTraversal.Deck.TableOfContents, slide);
                                        this.CurrentDeckTraversal.Deck.TableOfContents.Entries.Add(entry);

                                        // Move to the slide
                                        this.CurrentDeckTraversal.Current = entry;
                                    }
                                }
                            }
                        }
                    }
                }
                base.OnClick(args);
            }
        }

        #endregion

        #region BackwardNavigationToolBarButton

        /// <summary>
        /// Button handling backward navigation in decks
        /// </summary>
        protected class BackwardNavigationToolBarButton : NavigationToolBarButton
        {
            /// <summary>
            /// The presenter model
            /// NOTE: Is this replicated with the base class???
            /// </summary>
            private readonly PresenterModel m_Model;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="dispatcher">The event queue</param>
            /// <param name="model">The model</param>
            public BackwardNavigationToolBarButton(ControlEventQueue dispatcher, PresenterModel model)
                : base(dispatcher, model)
            {
                this.m_Model = model;

                this.Initialize();
            }

            /// <summary>
            /// This class listens to changes in the "Previous" property of the deck traversal
            /// </summary>
            protected override string TraversalProperty
            {
                get { return "Previous"; }
            }

            /// <summary>
            /// Handles changes to the DeckTraversal and the "Previous" property
            /// </summary>
            /// <param name="sender">The event sender</param>
            /// <param name="args">Arguments</param>
            protected override void HandleTraversalChanged(object sender, PropertyEventArgs args)
            {
                if (this.CurrentDeckTraversal == null)
                {
                    this.Enabled = false;
                }
                else
                {
                    using (Synchronizer.Lock(this.CurrentDeckTraversal.SyncRoot))
                    {
                        this.Enabled = (this.CurrentDeckTraversal.Previous != null);
                    }
                }
            }

            /// <summary>
            /// Handle the button being clicked
            /// </summary>
            /// <param name="args">The event arguments</param>
            protected override void OnClick(EventArgs args)
            {
                if (this.CurrentDeckTraversal == null) return;
                using (Synchronizer.Lock(this.CurrentDeckTraversal.SyncRoot))
                {
                    this.CurrentDeckTraversal.Current = this.CurrentDeckTraversal.Previous;
                }
                base.OnClick(args);
            }
        }

        #endregion

        protected class ReturnButton : Button
        {
            PresenterModel m_Model;
            public ReturnButton(PresenterModel model)
            {
                this.m_Model = model;
                this.Text = "Exit";
            }
            protected override void OnClick(EventArgs e)
            {
                using (Synchronizer.Lock(this.m_Model.ViewerState.SyncRoot))
                {
                    this.m_Model.ViewerState.PrimaryMonitorFullScreen = false;
                }
                base.OnClick(e);
            }
        }
        
        protected class DisplayIPButton : Button
        {
            PresenterModel m_Model;
            public DisplayIPButton(PresenterModel model)
            {
                this.m_Model = model;
                this.Text = "Display IP";
            }

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);

                IPAddressMessageBox mb = new IPAddressMessageBox();
                DialogResult dr = mb.ShowDialog();
            }

            private String[] detectIPInformation()
            {
                int counter = 0;
                String computerHostName = Dns.GetHostName();
                IPAddress[] IPlist = Dns.GetHostAddresses(computerHostName);

                foreach (IPAddress ip in IPlist)
                {
                    int ipDigit = ip.ToString().Length;
                    if (7 <= ipDigit && ipDigit <= 15)
                    {
                        counter++;
                    }
                }

                String[] IPAddressList = new String[counter];
                counter = 0;
                foreach (IPAddress ip in IPlist)
                {
                    int ipDigit = ip.ToString().Length;
                    if (7 <= ipDigit && ipDigit <= 15)
                    {
                        IPAddressList[counter] = ip.ToString();
                        counter++;
                    }
                }
                return IPAddressList;
            }

        }

        public class IPAddressMessageBox : Form
        {
            //Create all components for dialog box
            Label label = new Label();
            Label selectedLabel = new Label();
            ListBox viewer = new ListBox();
            Button OKButton = new Button();
            String ip;

            public IPAddressMessageBox()
            {
                WlanClient wlan = new WlanClient();
                System.Collections.ObjectModel.Collection<String> connectedSsids = new System.Collections.ObjectModel.Collection<string>();

                //Cycle through each WLAN interface to see which are connected
                foreach (WlanClient.WlanInterface wlanInterface in wlan.Interfaces)
                {
                    if (wlanInterface.InterfaceState == Wlan.WlanInterfaceState.Disconnected) continue;
                    Wlan.Dot11Ssid ssid = wlanInterface.CurrentConnection.wlanAssociationAttributes.dot11Ssid;
                    connectedSsids.Add(new String(Encoding.ASCII.GetChars(ssid.SSID, 0, (int)ssid.SSIDLength)));
                }

                //Set items for dialog box
                this.Font = Model.Viewer.ViewerStateModel.FormFont;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.ShowInTaskbar = false;

                label.FlatStyle = FlatStyle.System;
                label.Location = new Point(10, 10);

                label.Font = new Font("Arial", 12);
                label.Text = "IP addresses available";

                viewer.Size = new System.Drawing.Size(300, 130);
                viewer.Location = new System.Drawing.Point(20, 55);
                viewer.MultiColumn = true;
                this.Controls.Add(viewer);

                viewer.BeginUpdate();
                //Cycle through all available interfaces
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    //Check that network is operational
                    ni.OperationalStatus.Equals(OperationalStatus.Up);
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        //Get the IP for each interface
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                //Check to make sure it is an ethernet or wifi interface
                                if ((ni.Name.Equals("Ethernet") || ni.Name.Equals("Wi-Fi")) && (ni.OperationalStatus == OperationalStatus.Up))
                                {
                                    if (ni.Name.Equals("Wi-Fi"))
                                        foreach (String ssid in connectedSsids)
                                        {
                                            if (!viewer.Items.Contains(ip.Address.ToString() + " on " + ssid + " using " + ni.Name + "\n"))
                                                viewer.Items.Add(ip.Address.ToString() + " on " + ssid + " using " + ni.Name + "\n");

                                        }
                                    if (ni.Name.Equals("Ethernet"))
                                        viewer.Items.Add(ip.Address.ToString() + " on " + ni.Name + "\n");
                                }
                            }
                        }
                    }
                }
                viewer.EndUpdate();

                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Parent = this;
                label.Size = label.PreferredSize;

                this.Width = 360;
                this.Height = 270;

                OKButton.FlatStyle = FlatStyle.System;
                OKButton.Font = Model.Viewer.ViewerStateModel.StringFont1;
                OKButton.Parent = this;
                OKButton.Text = Strings.OK;
                OKButton.Location = new Point(this.Width / 2 - 115, 190);
                OKButton.Size = new Size(60, 40);
                OKButton.Click += new EventHandler(button_Click);
            }
            //Set up the dialog box which will contain the IP to dispay
            void button_Click(object sender, EventArgs e)
            {
                //Retrieve IP from getSelected function
                ip = getSelected();
                this.Controls.Remove(viewer);

                label.FlatStyle = FlatStyle.System;
                label.Location = new Point(10, 15);

                label.Font = new Font("Arial", 12);
                label.Text = "IP address is : ";

                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Parent = this;
                label.Size = label.PreferredSize;

                selectedLabel.Text = ip;
                selectedLabel.Font = new Font("Arial", 16);
                selectedLabel.Size = selectedLabel.PreferredSize;
                selectedLabel.Location = new System.Drawing.Point(20, 45);
                selectedLabel.TextAlign = ContentAlignment.MiddleCenter;
                selectedLabel.Parent = this;

                OKButton.Font = Model.Viewer.ViewerStateModel.StringFont1;
                OKButton.Parent = this;
                OKButton.Text = Strings.OK;
                OKButton.DialogResult = DialogResult.OK;
                OKButton.Location = new Point(this.Width / 2 - 50, 80);
                OKButton.Size = new Size(50, 30);

                //Resize window and update with the new IP text
                this.Height = 160;
                this.Width = 500;
                this.Update();
            }

            //Get the selected IP from the dialog box and return it for display
            private string getSelected()
            {
                //Cycle through list to find selected item
                for (int x = 0; x < viewer.Items.Count; x++)
                {
                    if (viewer.GetSelected(x) == true)
                    {
                        return viewer.SelectedItem.ToString();
                    }
                }
                //If no IP was selected instruct the user so
                return "No IP Selected";
            }
        }
    }
}