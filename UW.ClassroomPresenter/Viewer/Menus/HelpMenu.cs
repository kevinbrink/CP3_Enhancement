// $Id: HelpMenu.cs 1709 2008-08-13 20:52:07Z fred $

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Net;
using System.Diagnostics;
using System.Net.Sockets;

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

                IPAddressMessageBox mb = new IPAddressMessageBox(detectIPInformation());
                DialogResult dr = mb.ShowDialog();
                //string chosenIP = mb.chosenIP;

            }

            /// <summary>
            /// Return IP address for the IP Address menu command.  The goal is to return one address
            /// and have it be useful most of the time.  There is no guarantee that the address we return
            /// will be the one the user wants in all cases.
            /// -Since it will be less practical for users to manually enter IPv6 addresses, we return them
            /// only if there are no IPv4 addresses.  IPv6 users will probably prefer to use DNS names for manual
            /// connections.
            /// -Othewise we prefer routable over non-routable addresses..
            /// </summary>
            /// <returns></returns>
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
            /*
            string currentItem;

             public string chosenIP
            {
                get
                {
                    return currentItem;
                }
            }
            */
            public IPAddressMessageBox(string[] ipAddressString)
            {

                this.Font = Model.Viewer.ViewerStateModel.FormFont;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.ShowInTaskbar = false;

                Label label = new Label();
                label.FlatStyle = FlatStyle.System;
                label.Location = new Point(10, 15);

                label.Font = new Font("Arial", 20);
                label.Text = "The IP is one of:";

                ListBox viewer = new ListBox();
                viewer.Size = new System.Drawing.Size(200, 130);
                viewer.Location = new System.Drawing.Point(20, 45);
                viewer.MultiColumn = true;
                this.Controls.Add(viewer);

                // Add the ListBox to the form. 
                viewer.BeginUpdate();
                foreach (String ip in ipAddressString)
                {
                    viewer.Items.Add(ip);
                }
                viewer.EndUpdate();

               // currentItem = viewer.SelectedItem.ToString();
                

                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Parent = this;
                label.Size = label.PreferredSize;

                this.Width = 265;
                this.Height = 270;


                Button button = new Button();
                button.FlatStyle = FlatStyle.System;
                button.Font = Model.Viewer.ViewerStateModel.StringFont1;
                button.Parent = this;
                button.Text = Strings.OK;
                button.Location = new Point(this.Width / 2 - 115, 180);
                button.Size = new Size(60, 40);
                button.DialogResult = DialogResult.OK;
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
