using System;
using System.Drawing;
using System.Windows.Forms;

namespace Waveface.Stream.WindowsClient.Src.Control
{
	public partial class UsageBar : UserControl
	{
		#region Public Property
		public int Minimum
		{
			get
			{
				return progressBar1.Minimum;
			}
			set
			{
				progressBar1.Minimum = value;
			}
		}

		public int Maximum
		{
			get
			{
				return progressBar1.Maximum;
			}
			set
			{
				progressBar1.Maximum = value;
			}
		}

		public int Value
		{
			get
			{
				return progressBar1.Value;
			}
			set
			{
				progressBar1.Value = value;
			}
		}

		public string Unit { get; set; }
		#endregion


		#region Constructor
		public UsageBar()
		{
			InitializeComponent();
		}
		#endregion


		#region Event Process
		private void panel1_Paint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;
			var increase = Maximum / 5;
			var left = 0;

			var value = String.Format("{0} {1}", "0", Unit);
			var firstValueWidth = (int)Math.Floor(g.MeasureString(value, this.Font).Width);
			g.DrawString(value, this.Font, Brushes.Black, 0, 0);


			value = String.Format("{0} {1}", Maximum.ToString(), Unit);
			var lastValueWidth = (int)Math.Floor(g.MeasureString(value, this.Font).Width);
			g.DrawString(value, this.Font, Brushes.Black, this.Width - lastValueWidth, 0);

			var remainedWidth = this.Width - firstValueWidth;
			var increaseWidth = remainedWidth / 5;

			for (int size = increase; size < Maximum; size += increase)
			{
				left += increaseWidth;
				g.DrawString(String.Format("{0} {1}", size.ToString(), Unit), this.Font, Brushes.Black, left, 0);
			}
		}
		#endregion
	}
}
