using System.Drawing;
using System.Windows.Forms;

namespace Waveface.Stream.WindowsClient
{
	public class SystemIconControl : Control
	{
		#region Enum
		public enum SystemIconType
		{
			Application,
			Asterisk,
			Error,
			Exclamation,
			Hand,
			Information,
			Question,
			Shield,
			Warning,
			WinLogo
		}
		#endregion


		#region Var
		private SystemIconType _iconType;
		private Image _systemIcon;
		#endregion


		#region Private Property
		private Image m_SystemIcon
		{
			get
			{
				return _systemIcon ?? (_systemIcon = GetSystemIcon(IconType).ToBitmap());
			}
			set
			{
				_systemIcon = value;
			}
		}
		#endregion

		#region Public Property
		public SystemIconType IconType
		{
			get
			{
				return _iconType;
			}
			set
			{
				if (_iconType == value)
					return;

				_iconType = value;

				this.m_SystemIcon = null;
			}
		}
		#endregion

		#region Private Method
		private Icon GetSystemIcon(SystemIconType type)
		{
			switch (type)
			{
				case SystemIconType.Application:
					return SystemIcons.Application;
				case SystemIconType.Asterisk:
					return SystemIcons.Asterisk;
				case SystemIconType.Error:
					return SystemIcons.Error;
				case SystemIconType.Exclamation:
					return SystemIcons.Exclamation;
				case SystemIconType.Hand:
					return SystemIcons.Hand;
				case SystemIconType.Information:
					return SystemIcons.Information;
				case SystemIconType.Question:
					return SystemIcons.Question;
				case SystemIconType.Shield:
					return SystemIcons.Shield;
				case SystemIconType.Warning:
					return SystemIcons.Warning;
				case SystemIconType.WinLogo:
					return SystemIcons.WinLogo;
			}
			return null;
		}
		#endregion

		protected override void OnPaint(PaintEventArgs e)
		{
			var g = e.Graphics;
			g.DrawImage(m_SystemIcon, new Rectangle(0, 0, Width, Height));
		}
	}
}
