using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace StationSystemTray
{
    public class CueComboBox : ComboBox
    {
		[DllImport("user32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback,ref IntPtr i);

		private delegate bool EnumWindowProc(IntPtr hWnd,ref IntPtr parameter);

    	private IntPtr _editHandle;
        private string _cueText;
    	private Graphics _g;

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

    	public CueComboBox()
    	{
    		RetreiveEditControl();
    	}

		const int WM_PAINT = 0xF; //繪製的訊息

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			if (m.Msg == WM_PAINT && !string.IsNullOrEmpty(CueText) && Text.Length == 0 && Enabled && !Focused) //判斷TextBox的狀態決定要不要顯示提示訊息
			{
					TextRenderer.DrawText(_g, CueText, this.Font, ClientRectangle,
					                      ColorTranslator.FromHtml("#c2c2c2"), BackColor, TextFormatFlags.Left); //畫出提示訊息
					TextRenderer.DrawText(Graphics.FromHwnd(this.Handle), CueText, this.Font, ClientRectangle,
											  ColorTranslator.FromHtml("#c2c2c2"), BackColor, TextFormatFlags.Left); //畫出提示訊息
			}
		}

		/// <summary>
		/// 获取内部EDIT的句柄。
		/// </summary>
		private void RetreiveEditControl()
		{
			IntPtr handle = new IntPtr();

			EnumChildWindows(this.Handle, GetChildCallback, ref handle);

			//this._editHandle = handle;
			_editHandle = GetComboBoxInfo(this).hwndItem;
			_g = Graphics.FromHwnd(_editHandle);
		}

		[DllImport("user32.dll")]
		private static extern bool GetComboBoxInfo(IntPtr hwnd, ref COMBOBOXINFO pcbi);

		[StructLayout(LayoutKind.Sequential)]
		private struct COMBOBOXINFO
		{
			public int cbSize;
			public RECT rcItem;
			public RECT rcButton;
			public IntPtr stateButton;
			public IntPtr hwndCombo;
			public IntPtr hwndItem;
			public IntPtr hwndList;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		}

		/// <summary>
		/// EnumChildWindows的回调函数。
		/// </summary>
		private bool GetChildCallback(IntPtr hWnd, ref IntPtr lParam)
		{
			// 因为原生COMBOBOX只有一个子控件，因此不用作任何判断直接返回。
			lParam = hWnd;
			return false;
		}

		private static COMBOBOXINFO GetComboBoxInfo(System.Windows.Forms.Control control)
		{
			COMBOBOXINFO info = new COMBOBOXINFO();
			//a combobox is made up of three controls, a button, a list and textbox;
			//we want the textbox
			info.cbSize = Marshal.SizeOf(info);
			GetComboBoxInfo(control.Handle, ref info);
			return info;
		}
    }
}
