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

namespace Waveface.Stream.WindowsClient
{
	[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	[ComVisible(true)]
	public partial class MainForm : Form
	{
        #region Var
        private WebBrowser _browser;
        #endregion

        #region Static Var
        private static MainForm _instance;
        #endregion

        #region Private Property
        private WebBrowser m_Browser
        {
            get
            {
                return _browser ?? (_browser = new WebBrowser() 
                {
                    IsWebBrowserContextMenuEnabled = false,
                    WebBrowserShortcutsEnabled = false,
                    AllowWebBrowserDrop=false
                });
            }
        }
        #endregion



        #region Public Static Property
        public static MainForm Instance
        { 
            get
            {
                return _instance ?? (_instance = new MainForm());
            }
        }
        #endregion


        #region Constructor
        private MainForm()
        {
            InitializeComponent();


            var form = new DockContent();
            m_Browser.ObjectForScripting = this;

            m_Browser.DocumentCompleted += (s, e) =>
            {
                var head = m_Browser.Document.GetElementsByTagName("head")[0];
                var script = m_Browser.Document.CreateElement("script");
                var element = (IHTMLScriptElement)script.DomElement;
                element.text = @"if(navigator.userAgent.match(""MSIE 10.0"")){if(XMLHttpRequest !== undefined ) XMLHttpRequest = undefined;} // Hack the ie 10";
                head.AppendChild(script);

                m_Browser.Document.Window.Error += (w, we) =>
                {
                    we.Handled = true;

                    Trace.WriteLine(string.Format(
                           "Error: {0}\nline: {1}\nurl: {2}",
                           we.LineNumber,
                           we.Description,
                           we.Url));
                };
            };


            m_Browser.Dock = DockStyle.Fill;
            form.TabText = "Client Web Page";
            form.Controls.Add(m_Browser);
            form.Show(this.dockPanel1);

            form = new DockContent();
            form.TabText = "Log Message";
            form.Controls.Add(new LogMessageListBox() { Dock = DockStyle.Fill });
            form.Show(this.dockPanel1, DockState.DockBottom);
        } 
        #endregion


        #region Private Method
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
            using (var dialog = new AccountInfoForm())
            {
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ShowDialog(this);
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
            var file = Path.Combine(Directory.GetCurrentDirectory(), @"Web\index.html");
            Navigate(file);
        }
	}
}
