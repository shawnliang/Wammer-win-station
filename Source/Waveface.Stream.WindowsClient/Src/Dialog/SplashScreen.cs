using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;


namespace Waveface.Stream.WindowsClient
{
	public partial class SplashScreen : Form
	{
		#region Var
		private Queue<KeyValuePair<string, Action>> processes = new Queue<KeyValuePair<string, Action>>();
		private Thread _processThread;
		#endregion

		#region Private Property
		public Thread m_ProcessThread
		{
			get
			{
				if (_processThread == null)
				{
					_processThread = new Thread(() =>
					{
						var result = System.Windows.Forms.DialogResult.OK;

						try
						{
							while (true)
							{
								if (processes.Count == 0)
									break;

								var process = processes.Dequeue();

								if (this.Visible)
								{
									this.Invoke(new MethodInvoker(() =>
									{
										this.progressText.Text = process.Key;
									}));
								}

								process.Value();
							}
						}
						catch (Exception e)
						{
							MessageBox.Show(e.GetDisplayDescription(), "Ooops....", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							result = System.Windows.Forms.DialogResult.Abort;
						}
						finally
						{
							this.Invoke(new MethodInvoker(() =>
							{
								this.DialogResult = result;
							    this.Dispose();
							}));
						}
					});
					_processThread.IsBackground = true;
				}
				return _processThread;
			}
		}
		#endregion

		#region Constructor
		public SplashScreen()
		{
			InitializeComponent();

			this.ClientSize = BackgroundImage.Size;

			this.Load += SplashScreen_Load;
		}
		#endregion


		#region Public Method
		public SplashScreen AppendProcess(string processName, Action processAction)
		{
			processes.Enqueue(new KeyValuePair<string, Action>(processName, processAction));
			return this;
		}
		#endregion


		#region Event Process
		void SplashScreen_Load(object sender, EventArgs e)
		{
			if (m_ProcessThread.ThreadState != ThreadState.Running)
				m_ProcessThread.Start();
		}
		#endregion
	}
}
