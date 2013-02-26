using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Waveface.Stream.WindowsClient
{
	public class PanelEx : Panel
	{
		#region Property
		public Boolean EnableLinearGradientBackground { get; set; }
		public Color LinearGradientStartColor { get; set; }
		public Color LinearGradientEndColor { get; set; }
		#endregion

		#region Protected Method
		protected override void OnPaint(PaintEventArgs e)
		{
			if (!EnableLinearGradientBackground)
			{
				base.OnPaint(e);
				return;
			}

			var g = e.Graphics;
			var brush = new LinearGradientBrush(this.ClientRectangle, LinearGradientStartColor, LinearGradientEndColor, 90.0f);
			g.FillRectangle(brush, this.ClientRectangle);
		}
		#endregion
	}
}
