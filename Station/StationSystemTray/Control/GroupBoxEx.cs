using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms.VisualStyles;

namespace Waveface
{
	public class GroupBoxEx : GroupBox
	{
		public Color BorderColor { get; set; }

		#region Constructor
		public GroupBoxEx()
		{
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
		}
		#endregion

		void DrawGroupBox(Graphics g)
		{
			g.FillRectangle(new SolidBrush(this.Parent.BackColor), 0, 0, ClientRectangle.Width, ClientRectangle.Top + 5);
			DrawRoundRect(g, BorderColor, ClientRectangle.Left, ClientRectangle.Top + 5, ClientRectangle.Right - 2, ClientRectangle.Bottom - 2, 2);
			var messageSize = g.MeasureString(this.Text, this.Font);

			var messageTop = ClientRectangle.Top + 5 - (messageSize.Height / 2);
			if (messageTop < 0)
				messageTop = 0;

			g.DrawLine(new Pen(this.Parent.BackColor), 5, ClientRectangle.Top + 5, messageSize.Width + 5, ClientRectangle.Top + 5);
			g.DrawString(this.Text, this.Font, new SolidBrush(Color.White), 5 + 1, messageTop + 1);
			g.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), 5, messageTop);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var g = e.Graphics;
			DrawGroupBox(g);
		}

		public static void DrawRoundRect(System.Drawing.Graphics currentGraphicObject, Color lineColor, int nLeft, int nTop, int nRight, int nBottom, int round)
		{
			const int MaxRoundRadius = 3;
			const int MinBorderLength = 20;
			if (round > MaxRoundRadius)
			{
				round = MaxRoundRadius;
			}
			else if (round < 0)
			{
				round = 0;
			}
			if (Math.Abs(nRight - nLeft) < MinBorderLength && Math.Abs(nBottom - nTop) < MinBorderLength)
			{
				round = 1;
			}

			Point Polygon1 = new Point(nLeft + round, nTop);
			Point Polygon2 = new Point(nRight - round + 1, nTop);

			Point Polygon3 = new Point(nLeft, nTop + round);
			Point Polygon4 = new Point(nRight + 1, nTop + round);

			Point Polygon5 = new Point(nLeft, nBottom - round);
			Point Polygon6 = new Point(nRight + 1, nBottom - round);

			Point Polygon7 = new Point(nLeft + round, nBottom + 1);
			Point Polygon8 = new Point(nRight - round, nBottom + 1);

			//四条主线(上下左右)
			currentGraphicObject.DrawLine(new System.Drawing.Pen(lineColor), Polygon1.X, Polygon1.Y, Polygon2.X, Polygon2.Y);
			currentGraphicObject.DrawLine(new System.Drawing.Pen(lineColor), Polygon7.X, Polygon7.Y, Polygon8.X, Polygon8.Y);
			currentGraphicObject.DrawLine(new System.Drawing.Pen(lineColor), Polygon3.X, Polygon3.Y, Polygon5.X, Polygon5.Y);
			currentGraphicObject.DrawLine(new System.Drawing.Pen(lineColor), Polygon4.X, Polygon4.Y, Polygon6.X, Polygon6.Y);

			//四个边角
			currentGraphicObject.DrawLine(new System.Drawing.Pen(lineColor), Polygon1.X, Polygon1.Y, Polygon3.X, Polygon3.Y);
			currentGraphicObject.DrawLine(new System.Drawing.Pen(lineColor), Polygon2.X, Polygon2.Y, Polygon4.X, Polygon4.Y);
			currentGraphicObject.DrawLine(new System.Drawing.Pen(lineColor), Polygon5.X, Polygon5.Y, Polygon7.X, Polygon7.Y);
			currentGraphicObject.DrawLine(new System.Drawing.Pen(lineColor), Polygon6.X, Polygon6.Y, Polygon8.X, Polygon8.Y);
		}
	}
}
