using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Waveface
{
	public class PhotoNavigator : UserControl
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
		private Object _prepareThumbnailsIndexLocker;
		private List<string> _thumbnailFiles;
		private List<Lazy<Image>> _thumbnails;
		private Queue<int> _prepareThumbnailsIndex;
		private Image _defaultThumbnail;
		private ThumbnailInfo[] _DisplayedThumbnailInfos;
		private Rectangle _selectedRectangle;

		private int _selectedIndex;
		private int _thumbnailWidth = DEFAULT_THUMBNAIL_WIDTH;

		private Thread _prepareThumbnailLoadingThread;
		#endregion


		#region Private Property
		private Object m_PrepareThumbnailsIndexLocker
		{
			get
			{
				return _prepareThumbnailsIndexLocker ?? (_prepareThumbnailsIndexLocker = new Object());
			}
		}

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

		private Queue<int> m_PrepareThumbnailsIndex
		{
			get
			{
				lock (m_PrepareThumbnailsIndexLocker)
				{
					return _prepareThumbnailsIndex ?? (_prepareThumbnailsIndex = new Queue<int>());
				}
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

		private Rectangle m_SelectedRectangle
		{
			get
			{
				if(_selectedRectangle.IsEmpty)
					_selectedRectangle = new Rectangle((this.Width - ThumbnailWidth) / 2, ThumbnailPadding.Top, ThumbnailWidth, ThumbnailWidth);
				return _selectedRectangle;
			}
		}

		private Thread m_PrepareThumbnailLoadingThread
		{
			get
			{
				return _prepareThumbnailLoadingThread ?? (_prepareThumbnailLoadingThread = new Thread(() => 
				{
					try
					{
						Array.ForEach(m_PrepareThumbnailsIndex.ToArray(), (index) => 
							{
								m_Thumbnails[index].BeginInit();
							});
					}
					catch (Exception)
					{
					}
					_prepareThumbnailLoadingThread = null;
				}));
			}
			set
			{
				if (_prepareThumbnailLoadingThread != null)
				{
					try
					{
						_prepareThumbnailLoadingThread.Abort();
					}
					catch { }
					
					try
					{
						Array.ForEach(m_PrepareThumbnailsIndex.ToArray(), (index) => m_Thumbnails[index].CancelInit());
					}
					catch (Exception)
					{
					}
				}
				_prepareThumbnailLoadingThread = value;
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
		public PhotoNavigator()
		{
			this.Click += new EventHandler(ThumbnailNavigator_Click);
			this.MouseWheel += new MouseEventHandler(PhotoNavigator_MouseWheel);
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


				var isHorizontalPhoto = image.Width > image.Height;

				float ratio1 = size / (float)image.Width;
				float ratio2 = size / (float)image.Height;
				float ratio = (image.Width < image.Height) ? ratio1 : ratio2;

				var width = (int)(image.Width * ratio);
				var height = (int)(image.Height * ratio);

				Image thumbnail = (size <= 300) ? image.GetThumbnailImage(width, height, null, IntPtr.Zero) :
					new Bitmap(image, new Size(width, height));

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

	
		private void PrepareDisplayedThumbnailInfos()
		{
			var itemWidth = ThumbnailWidth + ThumbnailPadding.Left + ThumbnailPadding.Right;
			var displayCount = this.Width / itemWidth;
			var halfCount = displayCount / 2;

			var leftThumbnailCount = (SelectedIndex > halfCount) ? halfCount : SelectedIndex;

			var leftThumbnailIndex = SelectedIndex - leftThumbnailCount;

			var thumbnailInfos = new List<ThumbnailInfo>();
			for (int index = 1; index <= leftThumbnailCount; ++index)
			{
				var rectangle = new Rectangle(m_SelectedRectangle.Left - index * itemWidth, ThumbnailPadding.Top, ThumbnailWidth, ThumbnailWidth);

				thumbnailInfos.Add(new ThumbnailInfo()
				{
					Index = SelectedIndex - index,
					Bounds = rectangle
				});
			}

			thumbnailInfos.Add(new ThumbnailInfo()
			{
				Index = SelectedIndex,
				Bounds = m_SelectedRectangle
			});

			var rightThumbnailCount = (m_Thumbnails.Count >= SelectedIndex) ? m_Thumbnails.Count - SelectedIndex - 1 : 0;

			if (rightThumbnailCount > halfCount)
				rightThumbnailCount = halfCount;

			var rightThumbnailIndex = SelectedIndex + rightThumbnailCount;

			for (int index = 1; index <= rightThumbnailCount; ++index)
			{
				var rectangle = new Rectangle(m_SelectedRectangle.Left + index * itemWidth, ThumbnailPadding.Top, ThumbnailWidth, ThumbnailWidth);

				thumbnailInfos.Add(new ThumbnailInfo()
				{
					Index = SelectedIndex + index,
					Bounds = rectangle
				});
			}
			m_DisplayedThumbnailInfos = thumbnailInfos.ToArray();

			m_PrepareThumbnailLoadingThread = null;
			m_PrepareThumbnailsIndex.Clear();
			if (rightThumbnailIndex < m_Thumbnails.Count)
			{
				var prepareRightThumbnailIndex = rightThumbnailIndex + displayCount;

				if (prepareRightThumbnailIndex >= m_Thumbnails.Count - 1)
					prepareRightThumbnailIndex = m_Thumbnails.Count - 1;

				var linq = from index in Enumerable.Range(rightThumbnailIndex, prepareRightThumbnailIndex - rightThumbnailIndex)
						   where !m_Thumbnails[index].IsValueCreated
						   select index;

				Array.ForEach(linq.ToArray(),
					(item) => m_PrepareThumbnailsIndex.Enqueue(item));
			}

			if (leftThumbnailIndex > 0)
			{
				var prepareLeftThumbnailIndex = leftThumbnailIndex - displayCount;

				if (prepareLeftThumbnailIndex < 0)
					prepareLeftThumbnailIndex = 0;

				var linq = from index in Enumerable.Range(prepareLeftThumbnailIndex, leftThumbnailIndex - prepareLeftThumbnailIndex)
						   where !m_Thumbnails[index].IsValueCreated
						   select index;
				
				Array.ForEach(linq.ToArray(), 
					(item) => m_PrepareThumbnailsIndex.Enqueue(item));
			}

			m_PrepareThumbnailLoadingThread.Start();
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
			if (m_Thumbnails.Count == 0)
				return;

			if (SelectedIndex == DEFAULT_INDEX)
				return;

			PrepareDisplayedThumbnailInfos();

			var g = e.Graphics;
			foreach (var info in m_DisplayedThumbnailInfos)
			{
				var bounds = info.Bounds;

				if (Rectangle.Intersect(e.ClipRectangle, bounds).IsEmpty)
					continue;

				g.DrawImage(m_Thumbnails[info.Index].Value, bounds);
			}

			if (!Rectangle.Intersect(e.ClipRectangle, m_SelectedRectangle).IsEmpty)
				g.DrawRectangle(Pens.White, m_SelectedRectangle);
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
				case Keys.PageUp:
					PreviousPage();
					break;
				case Keys.PageDown:
					NextPage();
					break;
				case Keys.Home:
					FirstThumbnail();
					break;
				case Keys.End:
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

		/// <summary>
		/// Handles the MouseWheel event of the PhotoNavigator control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		void PhotoNavigator_MouseWheel(object sender, MouseEventArgs e)
		{
			if (e.Delta >= 0)
				PreviousThumbnail();
			else
				NextThumbnail();
		}
		#endregion
	}
}
