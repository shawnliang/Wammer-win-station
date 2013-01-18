using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Waveface.Stream.WindowsClient
{
	public partial class IntroControl : UserControl
	{
		public IntroControl():
			this(null)
		{

		}

		public IntroControl(IEnumerable<Image> images)
		{
			InitializeComponent();

			SetTutorialPhotos(images);
		}

		public void SetTutorialPhotos(IEnumerable<Image> images)
		{
			this.tutorialNavigator1.TutorialPhotos = images;
		}
	}
}
