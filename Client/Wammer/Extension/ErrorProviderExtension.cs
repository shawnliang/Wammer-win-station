using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Waveface
{
	public static class ErrorProviderExtension
	{
		#region Private Method
		private static IEnumerable<Control> GetControls(Control control)
		{
			if (control == null)
				return new Control[0];

			var controls = new List<Control>();
			foreach (Control childControl in control.Controls)
			{
				controls.Add(childControl);
				controls.AddRange(GetControls(childControl));
			}
			return controls;
		}
		#endregion

		public static IEnumerable<Control> GetErrorControls(this ErrorProvider ep)
		{
			if (ep.ContainerControl == null)
			{
				return new Control[0];
			}
			var linq = from c in GetControls(ep.ContainerControl)
					   let msg = ep.GetError(c)
					   where msg.Length > 0
					   select c;
			return linq;
		}

		public static IEnumerable<String> GetErrorMsgs(this ErrorProvider ep)
		{
			if (ep.ContainerControl == null) {
				return new string[0];
			}
			var linq = from c in GetControls(ep.ContainerControl)
					   let msg = ep.GetError(c)
					   where msg.Length > 0
					   select msg;
			return linq;
		}

		public static bool HasError(this ErrorProvider ep)
		{
			return ep.GetErrorMsgs().Any();
		}

	}
}
