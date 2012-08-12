using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Waveface.Component
{
	internal sealed class ControlPaint
	{
		private ControlPaint()
		{
		}

		public static Color BorderColor
		{
			get { return Color.FromArgb(127, 157, 185); }
		}

		public static Color DisabledBorderColor
		{
			get { return Color.FromArgb(201, 199, 186); }
		}

		public static Color ButtonBorderColor
		{
			get { return Color.FromArgb(28, 81, 128); }
		}

		public static Color DisabledButtonBorderColor
		{
			get { return Color.FromArgb(202, 200, 187); }
		}

		public static Color DisabledBackColor
		{
			get { return Color.FromArgb(236, 233, 216); }
		}

		public static Color DisabledForeColor
		{
			get { return Color.FromArgb(161, 161, 146); }
		}

		public static StringFormat GetStringFormat(ContentAlignment contentAlignment)
		{
			if (!Enum.IsDefined(typeof (ContentAlignment), (int) contentAlignment))
				throw new InvalidEnumArgumentException(
					"contentAlignment", (int) contentAlignment, typeof (ContentAlignment));

			StringFormat stringFormat = new StringFormat();

			switch (contentAlignment)
			{
				case ContentAlignment.MiddleCenter:
					stringFormat.LineAlignment = StringAlignment.Center;
					stringFormat.Alignment = StringAlignment.Center;
					break;

				case ContentAlignment.MiddleLeft:
					stringFormat.LineAlignment = StringAlignment.Center;
					stringFormat.Alignment = StringAlignment.Near;
					break;

				case ContentAlignment.MiddleRight:
					stringFormat.LineAlignment = StringAlignment.Center;
					stringFormat.Alignment = StringAlignment.Far;
					break;

				case ContentAlignment.TopCenter:
					stringFormat.LineAlignment = StringAlignment.Near;
					stringFormat.Alignment = StringAlignment.Center;
					break;

				case ContentAlignment.TopLeft:
					stringFormat.LineAlignment = StringAlignment.Near;
					stringFormat.Alignment = StringAlignment.Near;
					break;

				case ContentAlignment.TopRight:
					stringFormat.LineAlignment = StringAlignment.Near;
					stringFormat.Alignment = StringAlignment.Far;
					break;

				case ContentAlignment.BottomCenter:
					stringFormat.LineAlignment = StringAlignment.Far;
					stringFormat.Alignment = StringAlignment.Center;
					break;

				case ContentAlignment.BottomLeft:
					stringFormat.LineAlignment = StringAlignment.Far;
					stringFormat.Alignment = StringAlignment.Near;
					break;

				case ContentAlignment.BottomRight:
					stringFormat.LineAlignment = StringAlignment.Far;
					stringFormat.Alignment = StringAlignment.Far;
					break;
			}

			return stringFormat;
		}

		public static void DrawRoundedRectangle(Graphics g, Pen p, Rectangle rc, Size size)
		{
			// 1 pixel indent in all sides = Size(4, 4)
			// To make pixel indentation larger, change by a factor of 4,
			// i. e., 2 pixels indent = Size(8, 8);

			SmoothingMode oldSmoothingMode = g.SmoothingMode;
			g.SmoothingMode = SmoothingMode.AntiAlias;

			g.DrawLine(p, rc.Left + size.Width/2, rc.Top,
			           rc.Right - size.Width/2, rc.Top);
			g.DrawArc(p, rc.Right - size.Width, rc.Top,
			          size.Width, size.Height, 270, 90);

			g.DrawLine(p, rc.Right, rc.Top + size.Height/2,
			           rc.Right, rc.Bottom - size.Height/2);
			g.DrawArc(p, rc.Right - size.Width, rc.Bottom - size.Height,
			          size.Width, size.Height, 0, 90);

			g.DrawLine(p, rc.Right - size.Width/2, rc.Bottom,
			           rc.Left + size.Width/2, rc.Bottom);
			g.DrawArc(p, rc.Left, rc.Bottom - size.Height,
			          size.Width, size.Height, 90, 90);

			g.DrawLine(p, rc.Left, rc.Bottom - size.Height/2,
			           rc.Left, rc.Top + size.Height/2);
			g.DrawArc(p, rc.Left, rc.Top,
			          size.Width, size.Height, 180, 90);

			g.SmoothingMode = oldSmoothingMode;
		}

		public static void DrawBorder(Graphics g, int x, int y, int width, int height)
		{
			g.DrawRectangle(new Pen(BorderColor, 0), x, y,
			                width, height);
		}

		public static void EraseExcessOldDropDown(Graphics g, Rectangle newButton)
		{
			g.FillRectangle(new SolidBrush(SystemColors.Window), newButton.X - 2, newButton.Y,
			                2, newButton.Height + 1);
		}

	}

}