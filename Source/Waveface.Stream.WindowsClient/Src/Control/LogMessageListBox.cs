using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace Waveface.Stream.WindowsClient
{
	public class LogMessageListBox:ListBox
	{
		/// <summary>
		/// 
		/// </summary>
		private class LogTraceListener : DefaultTraceListener 
		{
			private ListBox m_ListBox { get; set; }

			/// <summary>
			/// Initializes a new instance of the <see cref="LogTraceListener"/> class.
			/// </summary>
			/// <param name="listBox">The list box.</param>
			public LogTraceListener(ListBox listBox)
			{
				m_ListBox = listBox;
			}

			/// <summary>
			/// Writes the output to the OutputDebugString function and to the <see cref="M:System.Diagnostics.Debugger.Log(System.Int32,System.String,System.String)"/> method, followed by a carriage return and line feed (\r\n).
			/// </summary>
			/// <param name="message">The message to write to OutputDebugString and <see cref="M:System.Diagnostics.Debugger.Log(System.Int32,System.String,System.String)"/>.</param>
			/// <PermissionSet>
			/// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
			/// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="ControlEvidence"/>
			/// </PermissionSet>
            public override void WriteLine(string message)
            {
                if (!m_ListBox.Visible)
                    return;

                if (m_ListBox.InvokeRequired)
                {
                    m_ListBox.BeginInvoke(new MethodInvoker(
                           delegate { WriteLine(message); }
                           ));
                    return;
                }

                m_ListBox.Items.Add(string.Format("{0}\t{1}", DateTime.Now, message));
            }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LogMessageListBox"/> class.
		/// </summary>
		public LogMessageListBox()
		{
			this.DrawMode = DrawMode.OwnerDrawVariable;
			Trace.Listeners.Add(new LogTraceListener(this));
		}

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.ListBox.DrawItem" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.DrawItemEventArgs" /> that contains the event data.</param>
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
            if (e.Index >= Items.Count)
            {
                base.OnDrawItem(e);
                return;
            }

            try
            {
                e.DrawBackground();
                e.Graphics.DrawString(this.Items[e.Index].ToString(), this.Font, Brushes.Black, e.Bounds); 
                e.DrawFocusRectangle();
            }
            catch (Exception)
            {
            }
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.ListBox.MeasureItem"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MeasureItemEventArgs"/> that contains the event data.</param>
		protected override void OnMeasureItem(MeasureItemEventArgs e)
		{
            if (e.Index >= Items.Count)
            {
                base.OnMeasureItem(e);
                return;
            }

            try
            {
                var g = e.Graphics;
                var size = g.MeasureString(this.Items[e.Index].ToString(), this.Font, this.Width - 5 - SystemInformation.VerticalScrollBarWidth);
                e.ItemHeight = (int)size.Height + 5;
                e.ItemWidth = (int)size.Width + 5;
            }
            catch (Exception)
            {
            }
		}
	}
}
