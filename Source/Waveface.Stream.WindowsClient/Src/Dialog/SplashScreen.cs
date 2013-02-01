using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;


namespace Waveface.Stream.WindowsClient
{
	public partial class SplashScreen : Form
	{
		static SplashScreen instance;
		static Thread thread;

		public SplashScreen()
		{
			InitializeComponent();
			this.ClientSize = BackgroundImage.Size;
		}

		private static void ShowForm()
		{
			instance = new SplashScreen();
			instance.ShowDialog();
		}

		public static void CloseForm()
		{
			if (instance != null && !instance.IsDisposed)
			{
				instance.Invoke(new MethodInvoker(() => instance.Close()));
				instance = null;
				thread = null;
			}
		}

		public static void ShowSplashScreen()
		{
			if (instance != null)
				return;

			thread = new Thread(SplashScreen.ShowForm);
			thread.IsBackground = true;
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();

			while (instance == null)
				Application.DoEvents();
		}

		public static void SetProgressText(string text)
		{
			if (instance == null || instance.IsDisposed)
				throw new InvalidOperationException();

			if (instance.InvokeRequired)
			{
				instance.Invoke( new MethodInvoker(() => {
					instance.progressText.Text = text;
				}));
				return;
			}


			instance.progressText.Text = text;
		}
	}
}
