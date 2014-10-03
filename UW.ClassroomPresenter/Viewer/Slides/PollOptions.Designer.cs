namespace UW.ClassroomPresenter.Viewer.Slides
{
    partial class PollOptions
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.startPollButton = new System.Windows.Forms.Button();
            this.stopPollButton = new System.Windows.Forms.Button();
            this.whenDoneRadioButton = new System.Windows.Forms.RadioButton();
            this.liveRadioButton = new System.Windows.Forms.RadioButton();
            this.neverRadioButton = new System.Windows.Forms.RadioButton();
            this.displayPollResultsLabel = new System.Windows.Forms.Label();
            this.pollTimer = new System.Windows.Forms.Timer(this.components);
            this.elapsedTimeLabel = new System.Windows.Forms.Label();
            this.updateTimeLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // startPollButton
            // 
            this.startPollButton.Location = new System.Drawing.Point(179, 12);
            this.startPollButton.Name = "startPollButton";
            this.startPollButton.Size = new System.Drawing.Size(101, 61);
            this.startPollButton.TabIndex = 0;
            this.startPollButton.Text = "Start Poll";
            this.startPollButton.UseVisualStyleBackColor = true;
            this.startPollButton.Click += new System.EventHandler(this.startPollButton_Click);
            // 
            // stopPollButton
            // 
            this.stopPollButton.Enabled = false;
            this.stopPollButton.Location = new System.Drawing.Point(179, 87);
            this.stopPollButton.Name = "stopPollButton";
            this.stopPollButton.Size = new System.Drawing.Size(101, 61);
            this.stopPollButton.TabIndex = 1;
            this.stopPollButton.Text = "Stop Poll";
            this.stopPollButton.UseVisualStyleBackColor = true;
            this.stopPollButton.Click += new System.EventHandler(this.stopPollButton_Click);
            // 
            // whenDoneRadioButton
            // 
            this.whenDoneRadioButton.AutoSize = true;
            this.whenDoneRadioButton.Checked = true;
            this.whenDoneRadioButton.Location = new System.Drawing.Point(12, 34);
            this.whenDoneRadioButton.Name = "whenDoneRadioButton";
            this.whenDoneRadioButton.Size = new System.Drawing.Size(158, 17);
            this.whenDoneRadioButton.TabIndex = 2;
            this.whenDoneRadioButton.TabStop = true;
            this.whenDoneRadioButton.Text = "Display Results When Done";
            this.whenDoneRadioButton.UseVisualStyleBackColor = true;
            // 
            // liveRadioButton
            // 
            this.liveRadioButton.AutoSize = true;
            this.liveRadioButton.Location = new System.Drawing.Point(12, 67);
            this.liveRadioButton.Name = "liveRadioButton";
            this.liveRadioButton.Size = new System.Drawing.Size(120, 17);
            this.liveRadioButton.TabIndex = 3;
            this.liveRadioButton.Text = "Display Results Live";
            this.liveRadioButton.UseVisualStyleBackColor = true;
            // 
            // neverRadioButton
            // 
            this.neverRadioButton.AutoSize = true;
            this.neverRadioButton.Location = new System.Drawing.Point(12, 101);
            this.neverRadioButton.Name = "neverRadioButton";
            this.neverRadioButton.Size = new System.Drawing.Size(125, 17);
            this.neverRadioButton.TabIndex = 4;
            this.neverRadioButton.Text = "Don\'t Display Results";
            this.neverRadioButton.UseVisualStyleBackColor = true;
            // 
            // displayPollResultsLabel
            // 
            this.displayPollResultsLabel.AutoSize = true;
            this.displayPollResultsLabel.Location = new System.Drawing.Point(12, 12);
            this.displayPollResultsLabel.Name = "displayPollResultsLabel";
            this.displayPollResultsLabel.Size = new System.Drawing.Size(116, 13);
            this.displayPollResultsLabel.TabIndex = 5;
            this.displayPollResultsLabel.Text = "Display Polling Results:";
            // 
            // pollTimer
            // 
            this.pollTimer.Interval = 1000;
            this.pollTimer.Tick += new System.EventHandler(this.pollTimer_Tick);
            // 
            // elapsedTimeLabel
            // 
            this.elapsedTimeLabel.AutoSize = true;
            this.elapsedTimeLabel.Location = new System.Drawing.Point(12, 135);
            this.elapsedTimeLabel.Name = "elapsedTimeLabel";
            this.elapsedTimeLabel.Size = new System.Drawing.Size(77, 13);
            this.elapsedTimeLabel.TabIndex = 6;
            this.elapsedTimeLabel.Text = "Elapsed Time: ";
            // 
            // updateTimeLabel
            // 
            this.updateTimeLabel.AutoSize = true;
            this.updateTimeLabel.Location = new System.Drawing.Point(96, 135);
            this.updateTimeLabel.Name = "updateTimeLabel";
            this.updateTimeLabel.Size = new System.Drawing.Size(0, 13);
            this.updateTimeLabel.TabIndex = 7;
            // 
            // PollOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 162);
            this.Controls.Add(this.updateTimeLabel);
            this.Controls.Add(this.elapsedTimeLabel);
            this.Controls.Add(this.displayPollResultsLabel);
            this.Controls.Add(this.neverRadioButton);
            this.Controls.Add(this.liveRadioButton);
            this.Controls.Add(this.whenDoneRadioButton);
            this.Controls.Add(this.stopPollButton);
            this.Controls.Add(this.startPollButton);
            this.Name = "PollOptions";
            this.Text = "PollOptions";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startPollButton;
        private System.Windows.Forms.Button stopPollButton;
        private System.Windows.Forms.RadioButton whenDoneRadioButton;
        private System.Windows.Forms.RadioButton liveRadioButton;
        private System.Windows.Forms.RadioButton neverRadioButton;
        private System.Windows.Forms.Label displayPollResultsLabel;
        private System.Windows.Forms.Timer pollTimer;
        private System.Windows.Forms.Label elapsedTimeLabel;
        private System.Windows.Forms.Label updateTimeLabel;
    }
}