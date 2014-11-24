// $Id: HelpMenu.cs 1709 2008-08-13 20:52:07Z fred $

using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Forms;
using NativeWifi;
using System.Collections.ObjectModel;
using System.Text;
using System.Net.NetworkInformation;

namespace UW.ClassroomPresenter.Viewer.Menus
{
    public class HelpMenu : MenuItem
    {

        public HelpMenu()
        {
            this.Text = Strings.Help;
            this.MenuItems.Add(new OnlineHelpMenuItem());
            this.MenuItems.Add("-");
            this.MenuItems.Add(new GettingStartedMenuItem());
            this.MenuItems.Add("-");
            this.MenuItems.Add(new VersionInfoMenuItem());
            this.MenuItems.Add(new IPAddressMenuItem());
            this.MenuItems.Add(new LicenseMenuItem());
            this.MenuItems.Add("-");
            this.MenuItems.Add(new AboutMenuItem());
        }

        public class OnlineHelpMenuItem : MenuItem
        {
            public OnlineHelpMenuItem()
            {
                this.Text = Strings.OnlineHelp;
                this.Shortcut = Shortcut.F1;
                this.ShowShortcut = true;
            }

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                Help.ShowHelp(this.Parent.GetMainMenu().GetForm(), "http://www.cs.washington.edu/education/dl/presenter/");
            }
        }
        public class GettingStartedMenuItem : MenuItem
        {
            public GettingStartedMenuItem()
            {
                this.Text = Strings.LaunchGettingStartedGuid;
            }
            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                string s = System.Reflection.Assembly.GetExecutingAssembly().Location;
                while (s[s.Length - 1] != '\\')
                {
                    s = s.Substring(0, s.Length - 1);
                } try
                {
                    Help.ShowHelp(((Control)(this.Parent.Container)), s + "Help\\startguide3.html");
                }
                catch { }
            }
        }

        public class IPAddressMenuItem : MenuItem
        {
            //Set IP button text
            public IPAddressMenuItem()
            {
                this.Text = Strings.IPAddress;
            }

            //Display IP chooser dialog on button click
            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                IPAddressMessageBox mb = new IPAddressMessageBox();
                DialogResult dr = mb.ShowDialog();
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
                Collection<String> connectedSsids = new Collection<string>();

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

        public class LicenseMenuItem : MenuItem
        {
            public LicenseMenuItem()
            {
                this.Text = Strings.License;
            }

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                UW.ClassroomPresenter.Misc.LicenseForm lf = new UW.ClassroomPresenter.Misc.LicenseForm();
                lf.ShowDialog();
            }
        }

        public class AboutMenuItem : MenuItem
        {
            public AboutMenuItem()
            {
                this.Text = Strings.AboutClassroomPresenter;
            }

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                UW.ClassroomPresenter.Misc.AboutForm af = new UW.ClassroomPresenter.Misc.AboutForm();
                af.ShowDialog();
            }
        }

        public class VersionInfoMenuItem : MenuItem
        {
            public VersionInfoMenuItem()
            {
                this.Text = Strings.VersionInfo;
            }

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                UW.ClassroomPresenter.Misc.VersionCompatibilityInfoForm vf = new UW.ClassroomPresenter.Misc.VersionCompatibilityInfoForm();
                vf.ShowDialog();
            }
        }
    }
}
