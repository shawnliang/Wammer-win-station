#region

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Waveface.Component.MultiPage.Design;

#endregion

namespace Waveface.Component.MultiPage
{
	[Designer(typeof(MultiPanelDesigner))]
	public class MultiPanel : Panel
	{
		private MultiPanelPage m_selectedPage;

		public MultiPanelPage SelectedPage
		{
			get
			{
				return m_selectedPage;
			}
			set
			{
				m_selectedPage = value;

				if (m_selectedPage != null)
				{
					foreach (Control _child in Controls)
					{
						if (ReferenceEquals(_child, m_selectedPage))
							_child.Visible = true;
						else
							_child.Visible = false;
					}
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			Graphics g = e.Graphics;

			using (SolidBrush _br = new SolidBrush(BackColor))
				g.FillRectangle(_br, ClientRectangle);
		}

		protected override ControlCollection CreateControlsInstance()
		{
			return new MultiPanelPagesCollection(this);
		}
	}
}