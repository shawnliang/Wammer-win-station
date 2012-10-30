using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StationSystemTray.Src.Control;
using System.Threading;
using System.IO;
using Wammer.Station;
using Microsoft.WindowsAPICodePack.Shell;
using System.Runtime.InteropServices;
using System.Reflection;
using StationSystemTray.Src.Class;
using System.Windows.Media.Imaging;
using Wammer.Model;
using MongoDB.Driver.Builders;

namespace StationSystemTray
{
	public partial class StepByStepWizardDialog : Form
	{

		#region Private Property
		/// <summary>
		/// Gets or sets the m_ original title.
		/// </summary>
		/// <value>The m_ original title.</value>
		private string m_OriginalTitle { get; set; }
		#endregion

		#region Public Property
		public WizardParameters Parameters
		{
			get { return wizardControl.Parameters; }
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
		#endregion

		#region Private Method
		/// <summary>
		/// Updates the title.
		/// </summary>
		private void UpdateTitle()
		{
			this.Text = string.Format("{0} ({1} of {2})", m_OriginalTitle, wizardControl.PageIndex, wizardControl.PageCount);
		}

		/// <summary>
		/// Updates the button.
		/// </summary>
		private void UpdateButton()
		{
			var pageIndex = wizardControl.PageIndex;

			var prevPageIndex = pageIndex - 1;

			prevButton.Enabled = (pageIndex > 1) && 
				!wizardControl.GetPage(prevPageIndex - 1).RunOnce; 
				// Question: why (prevPageIndex - 1) ??
				// Answer: pageIndex and prevPageIndex are 1-based index but 
				//         GetPage(index) uses 0-based.

			prevButton.Visible = nextButton.Visible = wizardControl.CurrentPage.HasPrevAndBack;

			nextButton.Text = (pageIndex < wizardControl.PageCount) ? "Next" : "Done";
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
