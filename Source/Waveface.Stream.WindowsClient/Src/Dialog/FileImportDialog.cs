using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Waveface.Stream.ClientFramework;

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
			fileImportControl1.ChangeLocation();
			fileImportControl1.ImportSelectedPaths();
		}
	}
}
