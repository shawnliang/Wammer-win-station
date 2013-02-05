using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Waveface.Stream.Core;


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
						while (true)
						{
							if (processes.Count == 0)
								break;

							var process = processes.Dequeue();

							this.Invoke(new MethodInvoker(() =>
							{
								this.progressText.Text = process.Key;
							}));
							process.Value();
						}

						this.Invoke(new MethodInvoker(() =>
						{
							this.Dispose();
						}));
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
		} 
		#endregion


		#region Public Method
		public SplashScreen AppendProcess(string processName, Action processAction)
		{
			processes.Enqueue(new KeyValuePair<string, Action>(processName, processAction));

			if (m_ProcessThread.ThreadState != ThreadState.Running && processes.Count == 1)
			{
				m_ProcessThread.Start();
			}

			return this;
		} 
		#endregion
	}
}
