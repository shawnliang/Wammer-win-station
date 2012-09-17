﻿#region

using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

#endregion

namespace Waveface
{
    public partial class PostArea : UserControl
    {
		#region Var
		private string _dateText;
		private Font _font;
		private Brush _timeBarBackgroundBrush;
		private Image _topTimerBar;
		#endregion


		#region Private Property
		private string m_WebHistoryItem { get; set; }

		private Image m_TopTimerBar 
		{
			get
			{
				if (_topTimerBar == null)
				{
					_topTimerBar = new Bitmap(panelTimeBar.Width, panelTimeBar.Height);
					using (var g = Graphics.FromImage(_topTimerBar))
					{
						using (var brush = new LinearGradientBrush(panelTimeBar.Bounds, ColorTranslator.FromHtml("#F4f4f4"), ColorTranslator.FromHtml("#CDCDCD"), LinearGradientMode.Vertical))
						{
							g.FillRectangle(brush, panelTimeBar.Bounds);
						}
					}
				}
				return _topTimerBar;
			}
		}

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

			m_WebHistoryItem = comboBox1.Items[4].ToString();

			comboBox1.Items.RemoveAt(4);
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

		public void EnableWebHistoryFilter()
		{
			if (comboBox1.Items.Contains(m_WebHistoryItem))
				return;
			comboBox1.Items.Add(m_WebHistoryItem);
		}

		public void DisableWebHistoryFilter()
		{
			if (!comboBox1.Items.Contains(m_WebHistoryItem))
				return;
			comboBox1.Items.Remove(m_WebHistoryItem);
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
			var g = e.Graphics;
			g.DrawImage(m_TopTimerBar, 0, 0);

			if (_font != null)
			{
				var textSize = g.MeasureString(m_DateText, _font);
				g.DrawString(m_DateText, _font, m_TimeBarBackgroundBrush, 4, (panelTimeBar.Height - textSize.Height) / 2);
			}
		} 
		#endregion

		private void PostArea_Load(object sender, System.EventArgs e)
		{
			comboBox1.SelectedIndex = 0;

			comboBox1.SelectedIndexChanged += new System.EventHandler(comboBox1_SelectedIndexChanged);
		}

		private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			var index = comboBox1.SelectedIndex;

			string type = string.Empty;

			switch (index)
			{
 				case 1:
					type = "text";
					break;
				case 2:
					type = "image";
					break;
				case 3:
					type = "link";
					break;
				case 4:
					type = "web history";
					break;
			}

			Main.Current.DisplayFilter(type);
		}
    }
}