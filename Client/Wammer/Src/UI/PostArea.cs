﻿#region

using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Waveface
{
    public partial class PostArea : UserControl
    {
		#region Var
		private string _dateText;
		private Font _font;
		private Brush _timeBarBackgroundBrush;
		#endregion


		#region Private Property
		/// <summary>
		/// Gets or sets the m_ date text.
		/// </summary>
		/// <value>The m_ date text.</value>
		private string m_DateText
		{
			get
			{
				return _dateText ?? string.Empty;
			}
			set
			{
				if (_dateText == value)
					return;

				_dateText = value;
				panelTimeBar.Refresh();
			}
		}

		/// <summary>
		/// Gets or sets the m_ time bar background brush.
		/// </summary>
		/// <value>The m_ time bar background brush.</value>
		private Brush m_TimeBarBackgroundBrush
		{
			get
			{
				return _timeBarBackgroundBrush ?? (_timeBarBackgroundBrush = new SolidBrush(Color.FromArgb(57, 80, 85)));
			}
			set
			{
				if (_timeBarBackgroundBrush == value)
					return;

				if (_timeBarBackgroundBrush != null)
				{
					_timeBarBackgroundBrush.Dispose();
					_timeBarBackgroundBrush = null;
				}

				_timeBarBackgroundBrush = value;
			}
		}
		#endregion


		#region Public Property
		/// <summary>
		/// Gets the posts list.
		/// </summary>
		/// <value>The posts list.</value>
		public PostsList PostsList
		{
			get
			{
				postList.MyParent = this;

				return postList;
			}
		} 
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="PostArea"/> class.
		/// </summary>
		public PostArea()
		{
			InitializeComponent();

			_font = new Font("Tahoma", 9);
		}
		#endregion



		#region Public Method
		/// <summary>
		/// Refreshes the timeline UI.
		/// </summary>
		public void RefreshTimelineUI()
		{
			postList.RefreshTimelineUI();
		}

		/// <summary>
		/// Removes the post.
		/// </summary>
		public void RemovePost()
		{
			postList.RemovePost();
		}

		/// <summary>
		/// Sets the date text.
		/// </summary>
		/// <param name="text">The text.</param>
		public void SetDateText(string text)
		{
			m_DateText = text;
		}

		/// <summary>
		/// Sets the date text font.
		/// </summary>
		/// <param name="font">The font.</param>
		public void SetDateTextFont(Font font)
		{
			if (_font != font)
				_font = font;
		} 
		#endregion



		#region Event Process
		/// <summary>
		/// Handles the Paint event of the panelTimeBar control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
		private void panelTimeBar_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawImage(Properties.Resources.timebar, 0, 0);

			if (_font != null)
			{
				e.Graphics.DrawString(m_DateText, _font, m_TimeBarBackgroundBrush, 4, 2);
			}
		} 
		#endregion
    }
}