using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace StationSystemTray
{
	public class PhotoGalleryViewer:Control
	{
		#region Var
		private PictureBox _photoViewer;
		private int _photoIndex;
		private Label _prev;
		private Label _next;
		private IEnumerable<Image> _images = new List<Image>();
		#endregion


		#region Private Property
		/// <summary>
		/// Gets the m_ photo viewer.
		/// </summary>
		/// <value>The m_ photo viewer.</value>
		private PictureBox m_PhotoViewer
		{
			get
			{
				return _photoViewer ?? (_photoViewer = new PictureBox() 
				{
					SizeMode = PictureBoxSizeMode.Zoom,
					Dock = DockStyle.Fill
				});
			}
		}

		private Label m_Prev
		{
			get
			{
				return _prev ?? (_prev = new Label { 
					Dock = DockStyle.Left,
					Text = "<",
					TextAlign = ContentAlignment.MiddleCenter,
				});
			}
		}

		private Label m_Next
		{
			get
			{
				return _next ?? (_next = new Label
				{
					Dock = DockStyle.Right,
					Text = ">",
					TextAlign = ContentAlignment.MiddleCenter,
				});
			}
		}
		#endregion


		#region Public Property
		/// <summary>
		/// Gets or sets the photos.
		/// </summary>
		/// <value>The photos.</value>
		public IEnumerable<Image> Images
		{
			get
			{
				return _images; 
			}
			set 
			{
				if (_images == value)
					return;

				OnPhotosChanging(EventArgs.Empty);
				_images = value;
				OnPhotosChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets the photo count.
		/// </summary>
		/// <value>The photo count.</value>
		public int PhotoCount 
		{
			get
			{
				return Images.Count();
			}
		}

		/// <summary>
		/// Gets or sets the index of the photo.
		/// </summary>
		/// <value>The index of the photo.</value>
		public int PhotoIndex
		{
			get 
			{
				return _photoIndex;
			}
			set
			{
				if (_photoIndex == value)
					return;
				
				if (_images == null)
					return;

				OnPhotoIndexChanging(EventArgs.Empty);
				_photoIndex = value;
				OnPhotoIndexChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the current photo.
		/// </summary>
		/// <value>The current photo.</value>
		public string CurrentPhoto { get; private set; }
		public Image CurrentImage { get; private set; }
		#endregion


		#region Event
		public event EventHandler PhotosChanging;
		public event EventHandler PhotosChanged;
		public event EventHandler PhotoIndexChanging;
		public event EventHandler PhotoIndexChanged;
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="PhotoGalleryViewer"/> class.
		/// </summary>
		public PhotoGalleryViewer()
		{
			this.Controls.Add(m_PhotoViewer);
			this.Controls.Add(m_Prev);
			this.Controls.Add(m_Next);

			m_Prev.Click += (s, e) => { PreviousPhoto(); };
			m_Next.Click += (s, e) => { NextPhoto(); };
			this.PhotosChanged += new EventHandler(PhotoGalleryViewer_PhotosChanged);
			this.PhotoIndexChanged += new EventHandler(PhotoGalleryViewer_PhotoIndexChanged);
		}


		public PhotoGalleryViewer(IEnumerable<Image> images)
			: this()
		{
			this.Images = images;
		}
		#endregion


		#region Private Method

		#endregion


		#region Protected Method
		/// <summary>
		/// Raises the <see cref="E:PhotosChanging"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void OnPhotosChanging(EventArgs e)
		{
			if (PhotosChanging == null)
				return;

			PhotosChanging(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:PhotosChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void OnPhotosChanged(EventArgs e)
		{
			if (PhotosChanged == null)
				return;

			PhotosChanged(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:PhotoIndexChanging"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void OnPhotoIndexChanging(EventArgs e)
		{
			if (PhotoIndexChanging == null)
				return;

			PhotoIndexChanging(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:PhotoIndexChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void OnPhotoIndexChanged(EventArgs e)
		{
			if (PhotoIndexChanged == null)
				return;

			PhotoIndexChanged(this, e);
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Firsts the photo.
		/// </summary>
		public void FirstPhoto()
		{
			this.PhotoIndex = 1;
		}

		/// <summary>
		/// Lasts the photo.
		/// </summary>
		public void LastPhoto()
		{
			this.PhotoIndex = PhotoCount;
		}

		/// <summary>
		/// Previouses the photo.
		/// </summary>
		public void PreviousPhoto()
		{
			var photoIndex = this.PhotoIndex;
			if (photoIndex <= 1)
				return;

			this.PhotoIndex = photoIndex - 1;
		}

		/// <summary>
		/// Nexts the photo.
		/// </summary>
		public void NextPhoto()
		{
			var photoIndex = this.PhotoIndex;
			if (photoIndex >= PhotoCount)
				return;

			this.PhotoIndex = photoIndex + 1;
		}
		#endregion


		#region Event Process
		/// <summary>
		/// Handles the PhotosChanged event of the PhotoGalleryViewer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void PhotoGalleryViewer_PhotosChanged(object sender, EventArgs e)
		{
			FirstPhoto();
		}

		/// <summary>
		/// Handles the PhotoIndexChanged event of the PhotoGalleryViewer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void PhotoGalleryViewer_PhotoIndexChanged(object sender, EventArgs e)
		{
			if (Images == null)
				return;

			CurrentImage = Images.ElementAt(PhotoIndex - 1);
			m_PhotoViewer.Image = CurrentImage;
		}
		#endregion
	}
}
