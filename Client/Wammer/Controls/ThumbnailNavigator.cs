using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace Waveface
{
	public class ThumbnailNavigator : Control
	{
		#region Structure
		struct ThumbnailInfo
		{
			public int Index { get; set; }
			public Rectangle Bounds { get; set; }
		}
		#endregion


		#region Const
		private const int DEFAULT_THUMBNAIL_WIDTH = 64;
		private const int DEFAULT_INDEX = -1;
		#endregion


		#region Var
		private List<string> _thumbnailFiles;
		private List<Lazy<Image>> _thumbnails;
		private Image _defaultThumbnail;
		private ThumbnailInfo[] _DisplayedThumbnailInfos;

		private int _selectedIndex;
		private int _thumbnailWidth = DEFAULT_THUMBNAIL_WIDTH;
		#endregion


		#region Private Property
		/// <summary>
		/// Gets the m_ thumbnail files.
		/// </summary>
		/// <value>The m_ thumbnail files.</value>
		private List<string> m_ThumbnailFiles
		{
			get
			{
				return _thumbnailFiles ?? (_thumbnailFiles = new List<string>());
			}
		}

		/// <summary>
		/// Gets the m_ thumbnails.
		/// </summary>
		/// <value>The m_ thumbnails.</value>
		private List<Lazy<Image>> m_Thumbnails
		{
			get
			{
				return _thumbnails ?? (_thumbnails = new List<Lazy<Image>>());
			}
		}

		/// <summary>
		/// Gets or sets the m_ displayed thumbnail infos.
		/// </summary>
		/// <value>The m_ displayed thumbnail infos.</value>
		private ThumbnailInfo[] m_DisplayedThumbnailInfos
		{
			get
			{
				return _DisplayedThumbnailInfos ?? (_DisplayedThumbnailInfos = new ThumbnailInfo[0]);
			}
			set
			{
				_DisplayedThumbnailInfos = value;
			}
		}
		#endregion


		#region Public Property
		public Image DefaultThumbnail
		{
			get
			{
				if (_defaultThumbnail == null)
				{
					_defaultThumbnail = new Bitmap(ThumbnailWidth, ThumbnailWidth);
					using (var g = Graphics.FromImage(_defaultThumbnail))
					{
						g.Clear(BackColor);
					}
				}

				return _defaultThumbnail;
			}
			set
			{
				_defaultThumbnail = value;
			}
		}

		public Padding ThumbnailPadding { get; set; }

		/// <summary>
		/// Gets or sets the width of the thumbnail.
		/// </summary>
		/// <value>The width of the thumbnail.</value>
		public int ThumbnailWidth
		{
			get
			{
				return _thumbnailWidth;
			}
			set
			{
				_thumbnailWidth = value;
			}
		}

		/// <summary>
		/// Gets or sets the index of the selected.
		/// </summary>
		/// <value>The index of the selected.</value>
		public int SelectedIndex
		{
			get
			{
				if (m_Thumbnails.Count <= _selectedIndex)
					SelectedIndex = DEFAULT_INDEX;
				return _selectedIndex;
			}
			set
			{
				if (value >= m_Thumbnails.Count)
					throw new ArgumentOutOfRangeException("SelectedIndex");

				if (value == _selectedIndex)
					return;

				OnSelectedIndexChanging(EventArgs.Empty);
				_selectedIndex = value;
				OnSelectedIndexChanged(EventArgs.Empty);

				Invalidate();
			}
		}
		#endregion


		#region Event
		public event EventHandler SelectedIndexChanging;
		public event EventHandler SelectedIndexChanged;
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ThumbnailGallery"/> class.
		/// </summary>
		public ThumbnailNavigator()
		{
			this.Click += new EventHandler(ThumbnailNavigator_Click);
		}
		#endregion


		#region Private Method
		private Image GenerateSquareImage(Image image, int size)
		{
			if (image == null)
				throw new ArgumentNullException("image");

			try
			{
				if ((image.Width < size) && (image.Height < size)) //圖片比指定大小還小，不需切割
					return image;

				if (image.Width <= 300 && image.Height <= 300)
					return image.GetThumbnailImage(size, size, null, IntPtr.Zero);

				var isHorizontalPhoto = image.Width > image.Height;

				float ratio1 = size / (float)image.Width;
				float ratio2 = size / (float)image.Height;
				float ratio = (image.Width < image.Height) ? ratio1 : ratio2;

				Image thumbnail = new Bitmap(image, new Size((int)(image.Width * ratio), (int)(image.Height * ratio)));

				if (thumbnail.Width == thumbnail.Height)
					return thumbnail;

				using (thumbnail)
				{
					//切方圖
					Image _crapped = new Bitmap(size, size);
					Graphics _g = Graphics.FromImage(_crapped);

					if (isHorizontalPhoto)
					{
						var x = (thumbnail.Width - thumbnail.Height) / 2;

						if (thumbnail.Width - x < size)
							x = thumbnail.Width - size;

						_g.DrawImage(thumbnail,
									 new Rectangle(0, 0, size, size),
									 new Rectangle(x, 0, size, size),
									 GraphicsUnit.Pixel
							);
					}
					else
					{
						var y = (int)(thumbnail.Height * 0.08);

						if (thumbnail.Height - y < size)
							y = thumbnail.Height - size;

						_g.DrawImage(thumbnail,
									 new Rectangle(0, 0, size, size),
									 new Rectangle(0, y, size, size),
									 GraphicsUnit.Pixel
							);
					}

					return _crapped;
				}
			}
			catch
			{
				return image;
			}
		}

		private void PrepareDisplayedThumbnailInfos(Rectangle selectedRectangle)
		{
			var itemWidth = ThumbnailWidth + ThumbnailPadding.Left + ThumbnailPadding.Right;
			var displayCount = this.Width / itemWidth;
			var halfCount = displayCount / 2;

			var leftThumbnailCount = (SelectedIndex > halfCount) ? halfCount : SelectedIndex;

			var thumbnailInfos = new List<ThumbnailInfo>();
			for (int index = 1; index <= leftThumbnailCount; ++index)
			{
				var rectangle = new Rectangle(selectedRectangle.Left - index * itemWidth, ThumbnailPadding.Top, ThumbnailWidth, ThumbnailWidth);

				thumbnailInfos.Add(new ThumbnailInfo()
				{
					Index = SelectedIndex - index,
					Bounds = rectangle
				});
			}

			thumbnailInfos.Add(new ThumbnailInfo()
			{
				Index = SelectedIndex,
				Bounds = selectedRectangle
			});

			var rightThumbnailCount = (m_Thumbnails.Count >= SelectedIndex) ? m_Thumbnails.Count - SelectedIndex - 1 : 0;

			if (rightThumbnailCount > halfCount)
				rightThumbnailCount = halfCount;

			for (int index = 1; index <= rightThumbnailCount; ++index)
			{
				var rectangle = new Rectangle(selectedRectangle.Left + index * itemWidth, ThumbnailPadding.Top, ThumbnailWidth, ThumbnailWidth);

				thumbnailInfos.Add(new ThumbnailInfo()
				{
					Index = SelectedIndex + index,
					Bounds = rectangle
				});
			}
			m_DisplayedThumbnailInfos = thumbnailInfos.ToArray();
		}

		[Conditional("DEBUG")]
		private void ShowMessageBoxWhenDebug(string message)
		{
			MessageBox.Show(message);
		}
		#endregion


		#region Protected Method
		/// <summary>
		/// Raises the <see cref="E:SelectedIndexChanging"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void OnSelectedIndexChanging(EventArgs e)
		{
			if (SelectedIndexChanging == null)
				return;
			SelectedIndexChanging(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:SelectedIndexChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void OnSelectedIndexChanged(EventArgs e)
		{
			if (SelectedIndexChanged == null)
				return;
			SelectedIndexChanged(this, e);
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Loads the thumbnails.
		/// </summary>
		/// <param name="thumbnailFiles">The thumbnail files.</param>
		public void LoadThumbnails(IEnumerable<string> thumbnailFiles, int selectedIndex = DEFAULT_INDEX)
		{
			m_ThumbnailFiles.Clear();
			m_ThumbnailFiles.AddRange(thumbnailFiles);
			var linq = from item in thumbnailFiles
					   select new Lazy<Image>(() => GenerateSquareImage(Image.FromFile(item), ThumbnailWidth)) 
					   {   
						   Tag = item,
						   InitialValue = DefaultThumbnail
					   };

			m_Thumbnails.Clear();
			m_Thumbnails.AddRange(linq);

			m_Thumbnails.ForEach((item) => item.ValueInited += new EventHandler(thumbnail_ValueInited));

			if (m_Thumbnails.Count > selectedIndex)
				this.SelectedIndex = selectedIndex;

			Invalidate();
		}

		/// <summary>
		/// Previouses the thumbnail.
		/// </summary>
		public void PreviousThumbnail()
		{
			if (SelectedIndex - 1 < 0)
			{
				SelectedIndex = m_Thumbnails.Count - 1;
				return;
			}
			SelectedIndex -= 1;
		}

		/// <summary>
		/// Nexts the thumbnail.
		/// </summary>
		public void NextThumbnail()
		{
			SelectedIndex = (SelectedIndex + 1) % m_Thumbnails.Count;
		}

		/// <summary>
		/// Previouses the page.
		/// </summary>
		public void PreviousPage()
		{
			int count = m_DisplayedThumbnailInfos.Length;

			if (count > SelectedIndex)
				count = SelectedIndex;

			SelectedIndex -= count;
		}

		/// <summary>
		/// Nexts the page.
		/// </summary>
		public void NextPage()
		{
			int count = m_DisplayedThumbnailInfos.Length;

			if (SelectedIndex + count > m_Thumbnails.Count)
				count = m_Thumbnails.Count - SelectedIndex;
			SelectedIndex += count;
		}

		/// <summary>
		/// Firsts the thumbnail.
		/// </summary>
		public void FirstThumbnail()
		{
			SelectedIndex = 0;
		}

		/// <summary>
		/// Lasts the thumbnail.
		/// </summary>
		public void LastThumbnail()
		{
			SelectedIndex = m_Thumbnails.Count - 1;
		}
		#endregion


		#region Event Process
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.Paint"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
		protected override void OnPaint(PaintEventArgs e)
		{
			try
			{
				if (m_Thumbnails.Count == 0)
					return;

				if (SelectedIndex == DEFAULT_INDEX)
					return;

				var selectedRectangle = new Rectangle((this.Width - ThumbnailWidth) / 2, ThumbnailPadding.Top, ThumbnailWidth, ThumbnailWidth);
				PrepareDisplayedThumbnailInfos(selectedRectangle);

				var g = e.Graphics;
				foreach (var info in m_DisplayedThumbnailInfos)
				{
					var bounds = info.Bounds;

					if (Rectangle.Intersect(e.ClipRectangle, bounds).IsEmpty)
						continue;

					g.DrawImage(m_Thumbnails[info.Index].Value, bounds);
				}

				if (!Rectangle.Intersect(e.ClipRectangle, selectedRectangle).IsEmpty)
					g.DrawRectangle(Pens.White, selectedRectangle);
			}
			catch (Exception ex)
			{
				ShowMessageBoxWhenDebug(ex.ToString());
			}
		}


		/// <summary>
		/// Handles the Click event of the ThumbnailNavigator control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void ThumbnailNavigator_Click(object sender, EventArgs e)
		{
			var mousePosition = PointToClient(MousePosition);
			var linq = from item in m_DisplayedThumbnailInfos
					   where item.Bounds.Contains(mousePosition)
					   select item.Index;

			if (!linq.Any())
				return;

			this.SelectedIndex = linq.FirstOrDefault();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.PreviewKeyDown"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.PreviewKeyDownEventArgs"/> that contains the event data.</param>
		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Left:
					PreviousThumbnail();
					break;
				case Keys.Right:
					NextThumbnail();
					break;
			}
		}

		/// <summary>
		/// Handles the ValueInited event of the thumbnail control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void thumbnail_ValueInited(object sender, EventArgs e)
		{
			var file = (sender as Lazy<Image>).Tag.ToString();
			var linq = from item in m_DisplayedThumbnailInfos
					   where m_ThumbnailFiles[item.Index] == file
					   select item.Bounds;

			if (!linq.Any())
				return;

			Invalidate(linq.Single());
		}
		#endregion
	}
}
