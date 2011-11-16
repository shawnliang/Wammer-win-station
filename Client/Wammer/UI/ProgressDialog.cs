#region

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using UtilityLibrary.WinControls;

#endregion

namespace Waveface
{
    public sealed class ProgressDialog : Form
    {
        #region Delegates

        public delegate object ProgressDialogStart(params object[] Params);

        private delegate bool CancelGetDelegate();
        private delegate int ProgressGetDelegate();

        private delegate void ProgressSetDelegate(int Progress);
        private delegate DialogResult StateGetDelegate();
        private delegate void StateSetDelegate(DialogResult State);
        private delegate void UpdateProgress(object sender, UpdateProgressEventArgs e);

        private event UpdateProgress OnUpdateProgress;

        #endregion

        #region UpdateProgressEventArgs

        private sealed class UpdateProgressEventArgs : EventArgs
        {
            private int m_percent;

            public UpdateProgressEventArgs(int Percent)
            {
                m_percent = Percent;
            }

            public int Percent
            {
                get { return (m_percent); }
            }

            public override string ToString()
            {
                return (m_percent.ToString());
            }
        }

        #endregion

        private ProgressDialogStart m_onstart;
        private object[] m_paramlist;
        private object m_result;
        private ProgressBarEx m_progressBarEx;

        # region Properties

        //Retrieves the result of the method.
        public object Result
        {
            get { return m_result; }
        }

        //Whether or not cancel was requested.
        public bool WasCancelled
        {
            get
            {
                if (InvokeRequired)
                {
                    CancelGetDelegate _temp = delegate { return (WasCancelled); };

                    return ((bool)Invoke(_temp));
                }
                else
                {
                    return ((DialogResult)btnCancel.Tag == DialogResult.Cancel);
                }
            }
        }

        private DialogResult State
        {
            get
            {
                if (InvokeRequired)
                {
                    StateGetDelegate _temp = delegate { return (State); };

                    return ((DialogResult)Invoke(_temp));
                }
                else
                {
                    return (DialogResult);
                }
            }

            set
            {
                if (InvokeRequired)
                {
                    StateSetDelegate _temp = delegate { State = value; };

                    Invoke(_temp, value);
                }
                else
                {
                    DialogResult = value;
                }
            }
        }

        private int Progress
        {
            get
            {
                if (InvokeRequired)
                {
                    ProgressGetDelegate _temp = delegate { return (m_progressBarEx.Value); };

                    return ((int)Invoke(_temp));
                }
                else
                {
                    return (m_progressBarEx.Value);
                }
            }

            set
            {
                if (InvokeRequired)
                {
                    ProgressSetDelegate _temp = delegate { m_progressBarEx.Value = value; };

                    Invoke(_temp, value);
                }
                else
                {
                    m_progressBarEx.Value = Math.Abs(value) % m_progressBarEx.Maximum + 1;
                }
            }
        }

        # endregion

        public ProgressDialog(string text, ProgressDialogStart startMethod, params object[] @params)
        {
            if (startMethod == null)
            {
                throw (new NullReferenceException("A start method must be provided"));
            }

            InitializeComponent();

            m_progressBarEx = new ProgressBarEx(Color.Green, Color.Yellow, Color.Red);
			m_progressBarEx.Location = new Point(12, 12);
			m_progressBarEx.Name = "progressBarEx";
			m_progressBarEx.Size = new Size(320, 24);
			m_progressBarEx.Minimum = 0;
			m_progressBarEx.Maximum = 100;
			m_progressBarEx.Value = 0;
			m_progressBarEx.BorderColor = Color.Black;
            m_progressBarEx.BackgroundColor = Color.FromArgb(255, 255, 192);
            m_progressBarEx.ForegroundColor = Color.Olive;
            m_progressBarEx.Smooth = true;
            m_progressBarEx.Enable3DBorder = true;
            m_progressBarEx.Border3D = Border3DStyle.Sunken;
            m_progressBarEx.ShowProgressText = true;
            Controls.Add(m_progressBarEx);

            Text = text;

            m_onstart = startMethod;

            m_paramlist = @params;

            OnUpdateProgress += UpdateProgressHandler;
        }

        public new DialogResult ShowDialog()
        {
            base.ShowDialog();

            return ((DialogResult)btnCancel.Tag);
        }

        private void ProgressDialog_Load(object sender, EventArgs e)
        {
            btnCancel.Tag = DialogResult.OK;
            btnCancel.Enabled = true;
            btnCancel.Text = "Cancel";

            m_progressBarEx.Value = m_progressBarEx.Minimum;

            (new Thread(delegate()
                            {
                                m_result = m_onstart(m_paramlist);

                                State = DialogResult.OK;
                            })).Start();
        }

        private void ProgressDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (State)
            {
                case DialogResult.None:
                    {
                        e.Cancel = true;
                        break;
                    }

                case DialogResult.Cancel:
                    {
                        if (btnCancel.Visible)
                        {
                            btnCancel.Enabled = false;
                            btnCancel.Text = "Cancelling";
                            btnCancel.Tag = State;
                        }
                        else
                        {
                            MessageBox.Show("This operation can't be cancelled", "Can't close", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }

                        e.Cancel = true;

                        break;
                    }
            }
        }

        public void RaiseUpdateProgress(int Percent)
        {
            OnUpdateProgress(this, new UpdateProgressEventArgs(Percent));
        }

        private void UpdateProgressHandler(object sender, UpdateProgressEventArgs e)
        {
            Progress = e.Percent;
        }

        #region Windows Form Designer generated code

        private Waveface.Component.XPButton btnCancel;
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnCancel = new Waveface.Component.XPButton();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnCancel.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnCancel.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(136, 52);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 28);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // ProgressDialog
            // 
            this.AcceptButton = this.btnCancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(343, 92);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ProgressDialog";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProgressDialog_FormClosing);
            this.Load += new System.EventHandler(this.ProgressDialog_Load);
            this.ResumeLayout(false);

        }

        # endregion
    }
}