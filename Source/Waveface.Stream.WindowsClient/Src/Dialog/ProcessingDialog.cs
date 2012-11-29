using System;
using System.Windows.Forms;

namespace Waveface.Stream.WindowsClient
{
	public partial class ProcessingDialog : Form
	{
		#region Private Property
		private int m_ProgressBarOriginalTop { get; set; }
		private int m_OriginalFormHeight { get; set; }
		#endregion

		#region Public Property
		/// <summary>
		/// Gets or sets a value indicating whether [show process message].
		/// </summary>
		/// <value><c>true</c> if [show process message]; otherwise, <c>false</c>.</value>
		public Boolean ShowProcessMessage 
		{
			get
			{
				return label1.Visible;
			}
			set
			{
				label1.Visible = value;
			}
		}

		/// <summary>
		/// Gets or sets the process message.
		/// </summary>
		/// <value>The process message.</value>
		public String ProcessMessage 
		{
			get
			{
				return label1.Text;
			}
			set
			{
				label1.Text = value;
			}
		}

		/// <summary>
		/// Gets or sets the progress value.
		/// </summary>
		/// <value>The progress value.</value>
		public int ProgressValue 
		{
			get
			{
				return progressBar1.Value;
			}
			set
			{
				progressBar1.Value = value;
			}
		}

		/// <summary>
		/// Gets or sets the progress min value.
		/// </summary>
		/// <value>The progress min value.</value>
		public int ProgressMinValue
		{
			get
			{
				return progressBar1.Minimum;
			}
			set
			{
				progressBar1.Minimum = value;
			}
		}

		/// <summary>
		/// Gets or sets the progress max value.
		/// </summary>
		/// <value>The progress max value.</value>
		public int ProgressMaxValue 
		{
			get
			{
				return progressBar1.Maximum;
			}
			set
			{
				progressBar1.Maximum = value;
			}
		}

		/// <summary>
		/// Gets or sets the progress style.
		/// </summary>
		/// <value>The progress style.</value>
		public ProgressBarStyle ProgressStyle 
		{
			get
			{
				return progressBar1.Style;
			}
			set
			{
				progressBar1.Style = value;
			}
		}
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessingDialog"/> class.
		/// </summary>
		public ProcessingDialog()
		{
			InitializeComponent();

			label1.Text = string.Empty;
			m_ProgressBarOriginalTop = progressBar1.Top;
			m_OriginalFormHeight = this.Height;
		} 
		#endregion


		#region Event Process
		/// <summary>
		/// Handles the VisibleChanged event of the label1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void label1_VisibleChanged(object sender, EventArgs e)
		{
			if (!this.Visible)
				return;

			progressBar1.Top = (ShowProcessMessage) ? m_ProgressBarOriginalTop : label1.Top;
			this.Height = (ShowProcessMessage) ? m_OriginalFormHeight : m_OriginalFormHeight - label1.Height;
		} 
		#endregion
	}
}
