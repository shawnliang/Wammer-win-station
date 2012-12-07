using System.Collections.Generic;
using System.Drawing;

namespace Waveface.Stream.WindowsClient
{
	public partial class IntroControl : StepPageControl
	{
		public IntroControl(IEnumerable<Image> images)
		{
			InitializeComponent();
			this.tutorialNavigator1.TutorialPhotos = images;
			this.PageTitle = "Introduction to Stream";
		}
	}
}
