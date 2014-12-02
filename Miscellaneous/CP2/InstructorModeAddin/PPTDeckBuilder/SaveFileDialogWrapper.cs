using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PPTDeckBuilder
{
	/// <summary>
	/// Wrapper for dialogs that brings them to the front when they pop up even if running from PPT.
	/// </summary>
	public class DialogWrapper : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		public DialogWrapper()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		private CommonDialog myDialog;

		public CommonDialog Dialog
		{
			get { return this.myDialog; }
			set { this.myDialog = value; }
		}

		public DialogResult ShowInternalDialog()
		{
			if (this.myDialog == null)
				return DialogResult.Cancel;
			else
			{
				DialogResult result;
				this.Visible = true;
				this.Activate();
				this.BringToFront();
				result = this.myDialog.ShowDialog(this);
				this.Visible = false;
				return result;
			}
		}

		public DialogResult ShowMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
		{
			return MessageBox.Show(this, text, caption, buttons, icon, defaultButton, options);
		}

		public DialogResult ShowMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		{
			return MessageBox.Show(this, text, caption, buttons, icon, defaultButton);
		}

		public DialogResult ShowMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			return MessageBox.Show(this, text, caption, buttons, icon);
		}

		public DialogResult ShowMessageBox(string text, string caption, MessageBoxButtons buttons)
		{
			return MessageBox.Show(this, text, caption, buttons);
		}

		public DialogResult ShowMessageBox(string text, string caption)
		{
			return MessageBox.Show(this, text, caption);
		}

		public DialogResult ShowMessageBox(string text)
		{
			return MessageBox.Show(this, text);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// SaveFileDialogWrapper
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(115, 0);
			this.ControlBox = false;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SaveFileDialogWrapper";
			this.Opacity = 0;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "SaveFileDialogWrapper";
			this.TopMost = true;

		}
		#endregion
	}
}
