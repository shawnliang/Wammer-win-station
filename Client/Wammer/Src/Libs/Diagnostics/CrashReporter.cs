#region

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

#endregion

namespace Waveface.Diagnostics
{
    public partial class CrashReporter : Form
    {
        /**** CHANGE THESE ****/
        private readonly string BugReportURI = "http://www.eaveface.com/";
        private readonly string ErrorDirectory = string.Format(@"{0}", Application.StartupPath + "\\Log\\");

        private readonly string m_strErrorLogPath = "";

        public CrashReporter(Exception e)
        {
            InitializeComponent();

            // Set application product name
            labelInfo.Text = string.Format(labelInfo.Text, Application.ProductName);

            // Set icons to error
            Icon = SystemIcons.Error;

            pictureBoxErr.Image = SystemIcons.Error.ToBitmap();

            string _strBuildTime =
                new DateTime(2000, 1, 1).AddDays(Assembly.GetExecutingAssembly().GetName().Version.Build).
                    ToShortDateString();

            // Gets program uptime
            TimeSpan _timeSpanProcTime = Process.GetCurrentProcess().TotalProcessorTime;

            listView.Items.Add(new ListViewItem(new[] { "Current Date/Time", DateTime.Now.ToString() }));
            listView.Items.Add(new ListViewItem(new[] { "Exec. Date/Time", Process.GetCurrentProcess().StartTime.ToString() }));
            listView.Items.Add(new ListViewItem(new[] { "Build Date", _strBuildTime }));
            listView.Items.Add(new ListViewItem(new[] { "OS", Environment.OSVersion.VersionString }));
            listView.Items.Add(new ListViewItem(new[] { "Language", Application.CurrentInputLanguage.LayoutName }));
            listView.Items.Add(
                new ListViewItem(new[]
                                     {
                                         "System Uptime",
                                         string.Format("{0} Days {1} Hours {2} Mins {3} Secs",
                                                       Math.Round((decimal) GetTickCount()/86400000),
                                                       Math.Round((decimal) GetTickCount()/3600000%24),
                                                       Math.Round((decimal) GetTickCount()/120000%60),
                                                       Math.Round((decimal) GetTickCount()/1000%60))
                                     }));

            listView.Items.Add(
                new ListViewItem(new[]
                                     {
                                         "Program Uptime",
                                         string.Format("{0} hours {1} mins {2} secs",
                                                       _timeSpanProcTime.TotalHours.ToString("0"),
                                                       _timeSpanProcTime.TotalMinutes.ToString("0"),
                                                       _timeSpanProcTime.TotalSeconds.ToString("0"))
                                     }));

            listView.Items.Add(new ListViewItem(new[] { "PID", Process.GetCurrentProcess().Id.ToString() }));
            listView.Items.Add(new ListViewItem(new[] { "Thread Count", Process.GetCurrentProcess().Threads.Count.ToString() }));
            listView.Items.Add(new ListViewItem(new[] { "Thread Id", Thread.CurrentThread.ManagedThreadId.ToString() }));
            listView.Items.Add(new ListViewItem(new[] { "Executable", Application.ExecutablePath }));
            listView.Items.Add(new ListViewItem(new[] { "Process Name", Process.GetCurrentProcess().ProcessName }));
            listView.Items.Add(new ListViewItem(new[] { "Version", Application.ProductVersion }));
            listView.Items.Add(new ListViewItem(new[] { "CLR Version", Environment.Version.ToString() }));

            Exception _ex = e;

            for (int i = 0; _ex != null; _ex = _ex.InnerException, i++)
            {
                listView2.Items.Add(new ListViewItem(new[] { "Type #" + i.ToString(), _ex.GetType().ToString() }));

                if (!string.IsNullOrEmpty(_ex.Message))
                    listView2.Items.Add(new ListViewItem(new[] { "Message #" + i.ToString(), _ex.Message }));

                if (!string.IsNullOrEmpty(_ex.Source))
                    listView2.Items.Add(new ListViewItem(new[] { "Source #" + i.ToString(), _ex.Source }));

                if (!string.IsNullOrEmpty(_ex.HelpLink))
                    listView2.Items.Add(new ListViewItem(new[] { "Help Link #" + i.ToString(), _ex.HelpLink }));

                if (_ex.TargetSite != null)
                    listView2.Items.Add(new ListViewItem(new[] { "Target Site #" + i.ToString(), _ex.TargetSite.ToString() }));

                if (_ex.Data != null)
                {
                    foreach (DictionaryEntry _de in _ex.Data)
                    {
                        listView2.Items.Add(
                            new ListViewItem(new[]
                                                 {
                                                     "Dictionary Entry #" + i.ToString(),
                                                     string.Format("Key: {0} Value: {1}", _de.Key, _de.Value)
                                                 }));
                    }
                }
            }

            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            textBoxStackTrace.Text = e.StackTrace;

            m_strErrorLogPath = string.Format("{0}\\{1:yyyy}_{1:MM}_{1:dd}_{1:HH}{1:mm}{1:ss}.txt", ErrorDirectory,
                                        DateTime.Now);
        }

        [DllImport("kernel32.dll")]
        internal static extern uint GetTickCount();

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (CreateErrorLog())
            {
                bool _reportSent = SendBugReport();

                if (!_reportSent)
                    MessageBox.Show(this, "The bug report could not be sent", Application.ProductName,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show(this, "Sent bug report successfully", Application.ProductName, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(this, "The error report could not be created", Application.ProductName,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Close();
        }

        private bool SendBugReport()
        {
            return true;
        }

        private bool CreateErrorLog()
        {
            // Create directory if it doesnt exist
            if (!Directory.Exists(ErrorDirectory))
                Directory.CreateDirectory(ErrorDirectory);

            try
            {
                StreamWriter _streamErrorLog = System.IO.File.CreateText(m_strErrorLogPath);

                int i;

                for (i = 0; i < listView.Items.Count; i++)
                {
                    string _strDesc = listView.Items[i].SubItems[0].Text;
                    string _strValue = listView.Items[i].SubItems[1].Text;

                    _streamErrorLog.WriteLine(string.Format("{0}: {1}", _strDesc, _strValue));
                }

                _streamErrorLog.WriteLine();

                for (i = 0; i < listView2.Items.Count; i++)
                {
                    string _strDesc = listView2.Items[i].SubItems[0].Text;
                    string _strValue = listView2.Items[i].SubItems[1].Text;

                    _streamErrorLog.WriteLine(string.Format("{0}: {1}", _strDesc, _strValue));
                }

                _streamErrorLog.WriteLine("Stack Trace:");
                _streamErrorLog.WriteLine(textBoxStackTrace.Text);

                _streamErrorLog.Close();
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void ErrorDlg_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (checkBoxRestart.Checked)
            {
                Application.Restart();
                Process.GetCurrentProcess().Kill();
            }
        }

        private void buttonDontSend_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}