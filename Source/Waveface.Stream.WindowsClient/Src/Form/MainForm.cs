using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;
using mshtml;
using System.Diagnostics;
using WeifenLuo.WinFormsUI.Docking;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Waveface.Stream.Model;
using System.IO;
using System.Drawing;
using System.Reflection;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	public partial class MainForm : Form
	{
        #region Var
        private IBrowserControl _browser;
		private DockPanel _dockPanel;
        #endregion

        #region Static Var
        private static MainForm _instance;
        #endregion

        #region Private Property
        /// <summary>
        /// Gets the m_ browser.
        /// </summary>
        /// <value>
        /// The m_ browser.
        /// </value>
        private IBrowserControl m_Browser
        {
            get
            {
                return _browser ?? (_browser = new IEBrowserControl()
                {
                    Dock = DockStyle.Fill
                });
            }
        }

		/// <summary>
		/// Gets the m_ dock panel.
		/// </summary>
		/// <value>
		/// The m_ dock panel.
		/// </value>
		private DockPanel m_DockPanel 
		{
			get 
			{
				return _dockPanel ?? (_dockPanel = new DockPanel() 
				{
					DocumentStyle = DocumentStyle.DockingSdi,
					Dock = DockStyle.Fill
				});
			}
		}
        #endregion



        #region Public Static Property
        public static MainForm Instance
        { 
            get
            {
				return (_instance == null || _instance.IsDisposed) ? (_instance = new MainForm()) : _instance;
            }
        }
        #endregion


        #region Constructor
        /// <summary>
        /// Prevents a default instance of the <see cref="MainForm" /> class from being created.
        /// </summary>
        private MainForm()
        {
            InitializeComponent();

			try
			{
				SuspendLayout();
#if DEBUG
				this.Controls.Add(m_DockPanel);
				AddDockableContent("Client Web Page", m_Browser as Control);

				AddDockableContent("Log Message", new LogMessageComponent() { Dock = DockStyle.Fill }, DockState.DockBottom);
				AddDockableContent("Mock Data Generator", new DataGenerateComponent() { Dock = DockStyle.Fill }, DockState.DockBottom);
#else
			this.Controls.Add(m_Browser as Control);
#endif

				titlePanel1.SendToBack();
			}
			finally
			{
				ResumeLayout();
			}
		}
        #endregion


        #region Private Method
        /// <summary>
        /// Adds the content of the dockable.
        /// </summary>
        /// <param name="tabText">The tab text.</param>
        /// <param name="control">The control.</param>
        private void AddDockableContent(string tabText, Control control, DockState dockState = DockState.Document)
        {
            var dockContent = new DockContent()
            {
                TabText = tabText
            };

            dockContent.Controls.Add(control);
			dockContent.Show(m_DockPanel, dockState);
        } 

        private void TriggerAutoImport(Boolean alwaysQuery = true)
        {
            var loginedSession = LoginedSessionCollection.Instance.FindOne();

            if (loginedSession == null)
                return;

            var userID = loginedSession.user.user_id;
            var driverCollection = StationDB.GetCollection("drivers");
            BsonDocument driver = driverCollection.FindOne(Query.EQ("_id", userID));
            Boolean isDataImportQueried = driver.Contains("isDataImportQueried") ? driver.GetElement("isDataImportQueried").Value.AsBoolean : false;

            if (alwaysQuery || !isDataImportQueried)
            {
                driverCollection.Update(Query.EQ("_id", userID), MongoDB.Driver.Builders.Update.Set("isDataImportQueried", true));

                AutoImportDialog dialog = new AutoImportDialog()
                {
                    StartPosition = FormStartPosition.CenterParent
                };
                dialog.ShowDialog(this);
            }
        }
        #endregion


        public void Navigate(string url)
        {
            m_Browser.Navigate(url);
        }

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Show();
            TriggerAutoImport(false);
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void accountToolStripMenuItem_Click(object sender, EventArgs e)
        {
			try
			{
				var dialog = AccountInfoForm.Instance;

				dialog.StartPosition = FormStartPosition.CenterParent;
				dialog.Activate();
				dialog.ShowDialog(this);
			}
			catch (Exception)
			{
			}
        }

        private void imageButton1_Click(object sender, EventArgs e)
        {
            var location = PointToScreen(new Point(titlePanel1.Width - contextMenuStrip1.Width, titlePanel1.Bottom));
            contextMenuStrip1.Show(location.X, location.Y);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSettingDialog();
        }

        private DialogResult ShowSettingDialog()
        {
            try
            {
                var dialog = SettingDialog.Instance;

                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.Activate();
                return dialog.ShowDialog(this);
            }
            catch (Exception)
            {
                return DialogResult.None;
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TriggerAutoImport();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
			var fileDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var file = Path.Combine(fileDir, @"Web\index.html");
            Navigate(file);
        }
	}
}
