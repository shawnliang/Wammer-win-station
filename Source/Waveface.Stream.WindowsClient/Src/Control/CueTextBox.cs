using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Waveface.Stream.WindowsClient
{
	public class CueTextBox : TextBox
	{
		private string _cueText;

		[Localizable(true)]
		public string CueText
		{
			get
			{
				return _cueText ?? string.Empty;
			}
			set
			{
				_cueText = value;
			}
		}

		const int WM_PAINT = 0xF; //繪製的訊息

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			if (m.Msg == WM_PAINT && !string.IsNullOrEmpty(CueText) && Text.Length == 0 && Enabled && !ReadOnly && !Focused) //判斷TextBox的狀態決定要不要顯示提示訊息
			{
				TextFormatFlags formatFlags = TextFormatFlags.Default; //使用原始設定的對齊方式來顯示提示訊息

				switch (TextAlign)
				{
					case HorizontalAlignment.Center:
						formatFlags = TextFormatFlags.HorizontalCenter;
						break;
					case HorizontalAlignment.Left:
						formatFlags = TextFormatFlags.Left;
						break;
					case HorizontalAlignment.Right:
						formatFlags = TextFormatFlags.Right;
						break;
				}

				TextRenderer.DrawText(Graphics.FromHwnd(Handle), CueText, this.Font, ClientRectangle, ColorTranslator.FromHtml("#c2c2c2"), BackColor, formatFlags); //畫出提示訊息
			}
		}
	}
}
