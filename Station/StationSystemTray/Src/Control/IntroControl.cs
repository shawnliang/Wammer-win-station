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
