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
	public partial class FirstUseWizardDialog : Form
	{
		#region Var
		private InstallAppMonitor m_installAppMonitor;
		#endregion


		#region Private Property
		/// <summary>
		/// Gets or sets the m_ original title.
		/// </summary>
		/// <value>The m_ original title.</value>
		private string m_OriginalTitle { get; set; } 

		private string m_SessionToken { get; set; }
		#endregion


		#region Constructor
		public FirstUseWizardDialog(string user_id, string sessionToken)
		{
			InitializeComponent();

			m_SessionToken = sessionToken;
			m_OriginalTitle = this.Text;
			m_installAppMonitor = new InstallAppMonitor(user_id);

			wizardControl1.PageChanged += new EventHandler(wizardControl1_PageChanged);
			wizardControl1.WizardPagesChanged += new EventHandler(wizardControl1_WizardPagesChanged);

			var buildPersonalCloud = new BuildPersonalCloudUserControl();
			buildPersonalCloud.OnAppInstall += m_installAppMonitor.OnAppInstall;
			buildPersonalCloud.OnAppInstallCanceled += m_installAppMonitor.OnAppInstallCanceled;

			var photoSearch = new PhotoSearch(m_SessionToken);

			wizardControl1.SetWizardPages(new AbstractStepPageControl[]
			{
				buildPersonalCloud,
				new FileImportControl(photoSearch, SynchronizationContext.Current),
				new ServiceImportControl(new FacebookConnectableService(user_id,sessionToken,StationAPI.API_KEY)),
				new CongratulationControl()
			});

			photoSearch.StartSearchAsync();
		} 
		#endregion


		#region Private Method
		/// <summary>
		/// Updates the title.
		/// </summary>
		private void UpdateTitle()
		{
			this.Text = string.Format("{0} ({1} of {2})", m_OriginalTitle, wizardControl1.PageIndex, wizardControl1.PageCount);
		}

		/// <summary>
		/// Updates the button.
		/// </summary>
		private void UpdateButton()
		{
			var pageIndex = wizardControl1.PageIndex;
			button2.Enabled = (pageIndex > 1);
			button1.Text = (pageIndex < wizardControl1.PageCount) ? "Next" : "Done";
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
		private void button1_Click(object sender, EventArgs e)
		{
			if (wizardControl1.PageIndex == wizardControl1.PageCount)
			{
				Close();
				return;
			}

			wizardControl1.NextPage();
		}

		/// <summary>
		/// Handles the Click event of the button2 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void button2_Click(object sender, EventArgs e)
		{
			wizardControl1.PreviousPage();
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

			//if (wizardControl1.CurrentPage is FileImportControl)
			//{
			//    ProcessFileImportStep();
			//}
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
