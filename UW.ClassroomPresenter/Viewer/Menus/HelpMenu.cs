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
            public IPAddressMenuItem()
            {
                this.Text = Strings.IPAddress;
            }

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);

                IPAddressMessageBox mb = new IPAddressMessageBox();
                DialogResult dr = mb.ShowDialog();
                //string chosenIP = mb.chosenIP;

            }

        }


        public class IPAddressMessageBox : Form
        {
            Label label = new Label();
            Label selectedLabel = new Label();
            ListBox viewer = new ListBox();
            Button button = new Button();
            String ip;


            public IPAddressMessageBox()
            {
                WlanClient wlan = new WlanClient();
                Collection<String> connectedSsids = new Collection<string>();                

                foreach (WlanClient.WlanInterface wlanInterface in wlan.Interfaces)
                {
                    if (wlanInterface.InterfaceState == Wlan.WlanInterfaceState.Disconnected) continue;
                    Wlan.Dot11Ssid ssid = wlanInterface.CurrentConnection.wlanAssociationAttributes.dot11Ssid;
                    connectedSsids.Add(new String(Encoding.ASCII.GetChars(ssid.SSID, 0, (int)ssid.SSIDLength)));
                }

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
                
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    
                    ni.OperationalStatus.Equals(OperationalStatus.Up);
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {                        
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {

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

                button.FlatStyle = FlatStyle.System;
                button.Font = Model.Viewer.ViewerStateModel.StringFont1;
                button.Parent = this;
                button.Text = Strings.OK;
                button.Location = new Point(this.Width / 2 - 115, 190);
                button.Size = new Size(60, 40);
                button.Click += new EventHandler(button_Click);
            }

            void button_Click(object sender, EventArgs e)
            {
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

                button.Font = Model.Viewer.ViewerStateModel.StringFont1;
                button.Parent = this;
                button.Text = Strings.OK;
                button.DialogResult = DialogResult.OK;
                button.Location = new Point(this.Width / 2 - 50, 80);
                button.Size = new Size(50, 30);

                this.Height = 160;
                this.Width = 500;
                this.Update();
            }

            private string getSelected()
            {
                for (int x = 0; x < viewer.Items.Count; x++)
                {
                    if (viewer.GetSelected(x) == true)
                    {
                        return viewer.SelectedItem.ToString();
                    }
                }
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
