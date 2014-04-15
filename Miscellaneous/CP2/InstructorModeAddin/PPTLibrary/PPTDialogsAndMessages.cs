using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PPTLibrary
{
	/// <summary>
	/// Wrapper for dialogs and message boxes that makes them come to the front in PPT.
	/// </summary>
	/// <remarks>Note that the overwrite dialog in SaveFileDialogs does not work under PPT. This is
	/// probably true of various other subdialogs as well. Users of this class should use a MessageBox
	/// to reimplement the functionality of these broken dialogs.</remarks>
	public class DialogMessageWrapper : System.Windows.Forms.Form
	{
		public static readonly DialogMessageWrapper Wrapper = new DialogMessageWrapper();

		/// <summary>
		/// Shows the given dialog on top of all other windows, even from within PPT.
		/// </summary>
		/// <param name="dialog">a non-null dialog to display</param>
		/// <returns>the returned result of the dialog itself</returns>
		public static DialogResult ShowDialog(CommonDialog dialog)
		{
			return Wrapper.InternalShowDialog(dialog);
		}

		/// <summary>
		/// Shows the given dialog on top of all other windows, even from within PPT.
		/// </summary>
		/// <param name="dialog">a non-null dialog to display</param>
		/// <returns>the returned result of the dialog itself</returns>
		public static DialogResult ShowDialog(Form dialog)
		{
			return Wrapper.InternalShowDialog(dialog);
		}

		public static DialogResult ShowMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
		{
			return Wrapper.InternalShowMessageBox(text, caption, buttons, icon, defaultButton, options);
		}

		public static DialogResult ShowMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		{
			return Wrapper.InternalShowMessageBox(text, caption, buttons, icon, defaultButton);
		}

		public static DialogResult ShowMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			return Wrapper.InternalShowMessageBox(text, caption, buttons, icon);
		}

		public static DialogResult ShowMessageBox(string text, string caption, MessageBoxButtons buttons)
		{
			return Wrapper.InternalShowMessageBox(text, caption, buttons);
		}

		public static DialogResult ShowMessageBox(string text, string caption)
		{
			return Wrapper.InternalShowMessageBox(text, caption);
		}

		public static DialogResult ShowMessageBox(string text)
		{
			return Wrapper.InternalShowMessageBox(text);
		}

		
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

		public DialogMessageWrapper()
		{
			this.Visible = true;

			this.SetStyle(ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.BackColor = System.Drawing.Color.Transparent;
		}

		private void PrepareForDialog()
		{
			this.Visible = true;
			this.Activate();
		}

		private void DecomissionFromDialog()
		{
			// Not doing anything as long as visible can remain true.
		}

		private DialogResult InternalShowDialog(CommonDialog dialog)
		{
			if (dialog == null)
				throw new ArgumentNullException("dialog");

			this.PrepareForDialog();
			try
			{
				return dialog.ShowDialog(this);
			}
			finally
			{
				this.DecomissionFromDialog();
			}
		}

		private DialogResult InternalShowDialog(Form dialog)
		{
			if (dialog == null)
				throw new ArgumentNullException("dialog");

			this.PrepareForDialog();
			try
			{
				return dialog.ShowDialog(this);
			}
			finally
			{
				this.DecomissionFromDialog();
			}
		}

		private DialogResult InternalShowMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
		{
			this.PrepareForDialog();
			try
			{
				return MessageBox.Show(this, text, caption, buttons, icon, defaultButton, options);
			}
			finally
			{
				this.DecomissionFromDialog();
			}
		}

		private DialogResult InternalShowMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		{
			this.PrepareForDialog();
			try
			{
				return MessageBox.Show(this, text, caption, buttons, icon, defaultButton);
			}
			finally
			{
				this.DecomissionFromDialog();
			}
		}

		private DialogResult InternalShowMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			this.PrepareForDialog();
			try
			{
				return MessageBox.Show(this, text, caption, buttons, icon);
			}
			finally
			{
				this.DecomissionFromDialog();
			}
		}

		private DialogResult InternalShowMessageBox(string text, string caption, MessageBoxButtons buttons)
		{
			this.PrepareForDialog();
			try
			{
				return MessageBox.Show(this, text, caption, buttons);
			}
			finally
			{
				this.DecomissionFromDialog();
			}
		}

		private DialogResult InternalShowMessageBox(string text, string caption)
		{
			this.PrepareForDialog();
			try
			{
				return MessageBox.Show(this, text, caption);
			}
			finally
			{
				this.DecomissionFromDialog();
			}
		}

		private DialogResult InternalShowMessageBox(string text)
		{
			this.PrepareForDialog();
			try
			{
				return MessageBox.Show(this, text);
			}
			finally
			{
				this.DecomissionFromDialog();
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// DialogMessageWrapper
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(0, 0);
			this.ControlBox = false;
			this.Enabled = false;
			this.ForeColor = System.Drawing.SystemColors.Control;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DialogMessageWrapper";
			this.Opacity = 0;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.TopMost = true;
			this.TransparencyKey = System.Drawing.SystemColors.Control;

		}
		#endregion
	}
}
