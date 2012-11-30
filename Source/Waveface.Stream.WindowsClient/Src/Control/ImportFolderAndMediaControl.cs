using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Waveface.Stream.WindowsClient
{
	public partial class ImportFolderAndMediaControl : StepPageControl
	{
		private IPhotoSearch photoSearch;
		private WizardParameters parameters;

		public ImportFolderAndMediaControl(IPhotoSearch photoSearch)
		{
			InitializeComponent();
			this.photoSearch = photoSearch;
		}

		public override void OnEnteringStep(WizardParameters parameters)
		{
			this.parameters = parameters;
		}

		private void importFromDiskButton_Click(object sender, EventArgs e)
		{
			var fileImport = new FileImportControl(photoSearch, SynchronizationContext.Current)
			{
				CustomLabelForNextStep = "Import"
			};

			var dialog = new StepByStepWizardDialog(fileImport)
			{
				Size = new Size(562, 481),
				Parameters = parameters
			};

			dialog.ShowDialog(this);
		}

		private void importFromMediaButton_Click(object sender, EventArgs e)
		{
			var mediaImport = new ImportFromPotableMediaControl(new PortableMediaService())
			{
				CustomLabelForNextStep = "Done"
			};

			var dialog = new StepByStepWizardDialog(mediaImport)
			{
				Size = new Size(562, 481),
				Parameters = parameters
			};

			dialog.ShowDialog(this);
		}
	}
}
