using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StationSystemTray
{
	public partial class TutorialNavigator : UserControl
	{
		#region Public Property
		/// <summary>
		/// Gets or sets the tutorial photos.
		/// </summary>
		/// <value>The tutorial photos.</value>
		public IEnumerable<Image> TutorialPhotos
		{
			get
			{
				return photoGalleryViewer1.Images;
			}
			set
			{
				photoGalleryViewer1.Images = value;
			}
		}

		/// <summary>
		/// Gets the step count.
		/// </summary>
		/// <value>The step count.</value>
		public int StepCount
		{
			get
			{
				return stepNavigator1.StepCount;
			}
		}

		/// <summary>
		/// Gets the index of the step.
		/// </summary>
		/// <value>The index of the step.</value>
		public int StepIndex
		{
			get
			{
				return stepNavigator1.StepIndex;
			}
		}


		/// <summary>
		/// Gets or sets a value indicating whether [enable manual navigate].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable manual navigate]; otherwise, <c>false</c>.
		/// </value>
		public bool EnableManualNavigate
		{
			get
			{
				return stepNavigator1.EnableManualNavigate;
			}
			set
			{
				stepNavigator1.EnableManualNavigate = value;
			}
		}
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="TutorialNavigator"/> class.
		/// </summary>
		public TutorialNavigator()
		{
			InitializeComponent();

			photoGalleryViewer1.PhotoIndexChanged += new EventHandler(photoGalleryViewer1_PhotoIndexChanged);
		}

		public TutorialNavigator(Image[] images)
			: this()
		{
			photoGalleryViewer1.Images = images;
		}
		#endregion


		#region Event Process
		/// <summary>
		/// Handles the PhotosChanged event of the photoGalleryViewer1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void photoGalleryViewer1_PhotosChanged(object sender, EventArgs e)
		{
			stepNavigator1.StepCount = photoGalleryViewer1.PhotoCount;
		}

		/// <summary>
		/// Handles the StepIndexChanged event of the stepNavigator1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void stepNavigator1_StepIndexChanged(object sender, EventArgs e)
		{
			photoGalleryViewer1.PhotoIndex = stepNavigator1.StepIndex;
		}

		void photoGalleryViewer1_PhotoIndexChanged(object sender, EventArgs e)
		{
			stepNavigator1.StepIndex = photoGalleryViewer1.PhotoIndex;
		}
		#endregion

		#region Public Methods
		public void Next()
		{
			photoGalleryViewer1.NextPhoto();
		}

		public void Prev()
		{
			photoGalleryViewer1.PreviousPhoto();
		}
		#endregion
	}
}
