using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UW.ClassroomPresenter.Model.Network;
using UW.ClassroomPresenter.Model;
using UW.ClassroomPresenter.Network.Messages.Network;

namespace UW.ClassroomPresenter.Misc {
    public partial class VersionCompatibilityInfoForm : Form {
        public VersionCompatibilityInfoForm() {
            InitializeComponent();
        }

        /// <summary>
        /// Load the remote node compatibility information.
        /// </summary>
        private void LoadDataGridView() {
            //This seems to be a simple approach:  Put the data into
            //a DataTable, then bind the DataTable to the DataGridView.
            DataTable dt = new DataTable();
            DataColumn dc;
            DataRow dr;

            dc = new DataColumn();
            dc.DataType = Type.GetType("System.String");
            dc.ColumnName = "Remote Node";
            dt.Columns.Add(dc);

            dc = new DataColumn();
            dc.DataType = Type.GetType("System.String");
            dc.ColumnName = "Version";
            dt.Columns.Add(dc);

            dc = new DataColumn();
            dc.DataType = Type.GetType("System.String");
            dc.ColumnName = "Compatibility";
            dt.Columns.Add(dc);

            dc = new DataColumn();
            dc.DataType = Type.GetType("System.String");
            dc.ColumnName = "Notes";
            dt.Columns.Add(dc);

            using (Synchronizer.Lock(PresenterModel.TheInstance.VersionExchange.SyncRoot)) {
                foreach (VersionExchange ve in PresenterModel.TheInstance.VersionExchange.VersionExchanges) {
                    if (!ve.DisplayVersionInfo) {
                        continue;
                    }

                    dr = dt.NewRow();
                    if ((ve.RemoteHumanName == null) || (ve.RemoteHumanName == "")) {
                        dr["Remote Node"] = "Unknown Name";
                        
                    }
                    else { 
                        dr["Remote Node"] = ve.RemoteHumanName;                  
                    }
                    if (ve.RemoteVersion == null) {
                        dr["Version"] = "Unknown Version";
                        dr["Notes"] = "The node has not responded to a version request.  It is probably a pre-release version.";
                    }
                    else {
                        dr["Version"] = ve.RemoteVersion.ToString();
                    }

                    dr["Compatibility"] = Enum.GetName(typeof(VersionCompatibility),ve.RemoteCompatibility);

                    if ((ve.WarningMessage != null ) && (ve.WarningMessage != "")) { 
                        dr["Notes"] = ve.WarningMessage;                       
                    }

                    dt.Rows.Add(dr);
                }
            }

            this.dataGridView1.DataSource = dt;
            this.dataGridView1.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void button1_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void VersionCompatibilityInfoForm_Load(object sender, EventArgs e) {
            this.linkLabelURL.Text = VersionExchangeModel.CompatiblityInfoURL;
            this.linkLabelURL.Links.Add(0, VersionExchangeModel.CompatiblityInfoURL.Length,VersionExchangeModel.CompatiblityInfoURL);
            this.labelThisCPVersion.Text = "This Classroom Presenter Version: " + VersionExchangeModel.LocalVersion.ToString();
            LoadDataGridView();
        }

        private void linkLabelURL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
    }
}