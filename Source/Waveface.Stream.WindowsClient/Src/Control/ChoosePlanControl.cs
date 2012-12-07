using System.Drawing;

namespace Waveface.Stream.WindowsClient
{

	public partial class ChoosePlanControl : StepPageControl
	{
		public ChoosePlanControl()
		{
			InitializeComponent();
			PageTitle = "Upgrade";
			CustomSize = new Size(710, 437);
		}

		public override bool RunOnce
		{
			get
			{
				return true;
			}
		}
	}
}
