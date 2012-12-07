using System;
using System.Drawing;
using System.Windows.Forms;

namespace Waveface.Stream.WindowsClient
{
	public partial class StepByStepWizardDialog : Form
	{

		#region Private Property
		/// <summary>
		/// Gets or sets the m_ original title.
		/// </summary>
		/// <value>The m_ original title.</value>
		private string m_OriginalTitle { get; set; }
		private Size m_OriginalSize { get; set; }
		#endregion

		#region Public Property
		public WizardParameters Parameters
		{
			get { return wizardControl.Parameters; }
			set { wizardControl.Parameters = value; }
		}

		#endregion

		#region Constructor
		public StepByStepWizardDialog()
		{
			InitializeComponent();
			m_OriginalTitle = this.Text;

			wizardControl.PageChanged += new EventHandler(wizardControl1_PageChanged);
			wizardControl.WizardPagesChanged += new EventHandler(wizardControl1_WizardPagesChanged);
		}

		public StepByStepWizardDialog(params StepPageControl[] controls)
			: this()
		{
			wizardControl.SetWizardPages(controls);
		}
		#endregion

		#region Private Method
		/// <summary>
		/// Updates the title.
		/// </summary>
		private void UpdateTitle()
		{
			this.Text = "Stream";

			if (wizardControl.CurrentPage != null && !string.IsNullOrEmpty(wizardControl.CurrentPage.PageTitle))
				this.Text = wizardControl.CurrentPage.PageTitle;
		}

		/// <summary>
		/// Updates the button.
		/// </summary>
		private void UpdateButton()
		{
			var pageIndex = wizardControl.PageIndex;

			var prevPageIndex = pageIndex - 1;

			var canGoBack = (pageIndex > 1) &&
				!wizardControl.GetPage(prevPageIndex - 1).RunOnce;
			// Question: why (prevPageIndex - 1) ??
			// Answer: pageIndex and prevPageIndex are 1-based index but 
			//         GetPage(index) uses 0-based.

			nextButton.Visible = wizardControl.CurrentPage.HasPrevAndBack;
			prevButton.Visible = canGoBack && wizardControl.CurrentPage.HasPrevAndBack;

			if (string.IsNullOrEmpty(wizardControl.CurrentPage.CustomLabelForNextStep))
			{
				nextButton.Text = (pageIndex < wizardControl.PageCount) ? "Next" : "Done";
			}
			else
			{
				nextButton.Text = wizardControl.CurrentPage.CustomLabelForNextStep;
			}
		}

		#endregion

		#region Event Process
		/// <summary>
		/// Handles the Load event of the FirstUseWizardDialog control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void FirstUseWizardDialog_Load(object sender, EventArgs e)
		{
			m_OriginalSize = this.Size;
			UpdateTitle();
		}

		/// <summary>
		/// Handles the Click event of the button1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void nextButton_Click(object sender, EventArgs e)
		{
			if (wizardControl.PageIndex == wizardControl.PageCount)
			{
				wizardControl.CurrentPage.OnLeavingStep(wizardControl.Parameters);
				Close();
				return;
			}

			wizardControl.NextPage();
		}

		/// <summary>
		/// Handles the Click event of the button2 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void prevButton_Click(object sender, EventArgs e)
		{
			wizardControl.PreviousPage();
		}

		/// <summary>
		/// Handles the PageChanged event of the wizardControl1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void wizardControl1_PageChanged(object sender, EventArgs e)
		{
			UpdateTitle();
			UpdateButton();

			var customSize = wizardControl.CurrentPage.CustomSize;

			if (customSize.IsEmpty)
				this.Size = m_OriginalSize;
			else
				this.Size = customSize;
		}

		/// <summary>
		/// Handles the WizardPagesChanged event of the wizardControl1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void wizardControl1_WizardPagesChanged(object sender, EventArgs e)
		{
			UpdateButton();
		}


		#endregion
	}
}
