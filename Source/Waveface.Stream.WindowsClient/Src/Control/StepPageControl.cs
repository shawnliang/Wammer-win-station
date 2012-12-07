using System.Drawing;
using System.Windows.Forms;


namespace Waveface.Stream.WindowsClient
{
	public class StepPageControl : UserControl
	{
		private string customLabelForNextStep;

		public WizardControl WizardControl { get; set; }
		public Size CustomSize { get; set; }

		public virtual void OnEnteringStep(WizardParameters parameters)
		{
		}

		public virtual void OnLeavingStep(WizardParameters parameters)
		{
		}

		public virtual void OnStepEntered(WizardParameters parameters)
		{

		}

		/// <summary>
		/// If true, wizard will provice "next" and "prev" buttons on this page
		/// </summary>
		public virtual bool HasPrevAndBack
		{
			get { return true; }
		}

		/// <summary>
		/// If true, user cannot go back to this step. Default is false.
		/// </summary>
		public virtual bool RunOnce
		{
			get { return false; }
		}

		public string CustomLabelForNextStep
		{
			get { return customLabelForNextStep; }
			set { customLabelForNextStep = value; }
		}

		public string PageTitle { get; set; }
	}
}
