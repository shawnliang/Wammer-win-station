using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace StationSystemTray
{
	/// <summary>
	/// 
	/// </summary>
	public class CentralLayoutPanel : Control
	{
		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="CentralLayoutPanel"/> class.
		/// </summary>
		public CentralLayoutPanel()
		{
			//this.Controls.Add(m_ControlPool);

			AdjustContentPoolLocation();
			
			this.ControlAdded += new ControlEventHandler(CentralLayoutPanel_ControlAdded);
			this.SizeChanged += new EventHandler(CentralLayoutPanel_SizeChanged);
		}
		#endregion


		#region Private Method
		/// <summary>
		/// Adjusts the content pool location.
		/// </summary>
		private void AdjustContentPoolLocation()
		{
			if (Controls.Count == 0)
				return;

			var centerX = Width / 2;
			var centerY = Height / 2;

			var contentWidth = (from control in Controls.OfType<Control>()
								select control.PreferredSize.Width).Sum();

			var contentHeight = (from control in Controls.OfType<Control>()
								 select control.PreferredSize.Height).Max();

			var startLeft = centerX - contentWidth / 2;
			var startTop = centerY - contentHeight / 2;

			foreach (Control control in Controls)
			{
				control.Dock = DockStyle.None;

				control.Location = new Point(startLeft ,startTop);

				startLeft += control.PreferredSize.Width;
			}
		}
		#endregion



		#region Event Process
		/// <summary>
		/// Handles the ControlAdded event of the CentralLayoutPanel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.ControlEventArgs"/> instance containing the event data.</param>
		void CentralLayoutPanel_ControlAdded(object sender, ControlEventArgs e)
		{
			AdjustContentPoolLocation();
		}

		/// <summary>
		/// Handles the SizeChanged event of the CentralLayoutPanel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void CentralLayoutPanel_SizeChanged(object sender, EventArgs e)
		{
			AdjustContentPoolLocation();
		}
		#endregion
	}
}
