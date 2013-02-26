using System;
using System.Windows.Forms;

namespace Waveface.Stream.WindowsClient
{
	public partial class FileImportDialog : Form
	{
		private static FileImportDialog _instance;

		public static FileImportDialog Instance
		{
			get
			{
				return (_instance == null || _instance.IsDisposed) ? (_instance = new FileImportDialog()) : _instance;
			}
		}

		private FileImportDialog()
		{
			InitializeComponent();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			fileImportControl1.ImportSelectedPaths();
		}
	}
}
