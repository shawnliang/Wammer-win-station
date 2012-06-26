using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Waveface
{

	static class FormExtension
	{
		#region Struct
		struct FullScreenData
		{
			public FormBorderStyle FormBorderStyle { get; set; }
			public FormWindowState WindowState { get; set; }

			public FullScreenData(FormBorderStyle formBorderStyle, FormWindowState windowState)
				: this()
			{
				this.FormBorderStyle = formBorderStyle;
				this.WindowState = windowState;
			}
		}
		#endregion


		#region Var
		private static Dictionary<Form, FullScreenData> _fullScreenDataPool;
		#endregion


		#region Private Property
		private static Dictionary<Form, FullScreenData> m_FullScreenDataPool
		{
			get
			{
				if (_fullScreenDataPool == null)
					_fullScreenDataPool = new Dictionary<Form, FullScreenData>();
				return _fullScreenDataPool;
			}
		}
		#endregion



		#region Public Method
		public static void FullScreen(this Form frm)
		{
			if (m_FullScreenDataPool.ContainsKey(frm))
				return;

			m_FullScreenDataPool.Add(frm, new FullScreenData(frm.FormBorderStyle, frm.WindowState));
			frm.FormBorderStyle = FormBorderStyle.None;
			frm.WindowState = FormWindowState.Normal;
			frm.WindowState = FormWindowState.Maximized;
		}



		public static void UnFullScreen(this Form frm)
		{
			if (!m_FullScreenDataPool.ContainsKey(frm))
				return;

			FullScreenData fd = m_FullScreenDataPool[frm];
			frm.WindowState = fd.WindowState;
			frm.FormBorderStyle = fd.FormBorderStyle;
			m_FullScreenDataPool.Remove(frm);
		}
		#endregion

	}
}
