using System.Windows.Forms;
using SharpSetup.Base;
using SharpSetup.UI.Controls;
using SharpSetup.UI.Forms.Modern;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Gui
{
	public partial class SetupWizard : ModernWizard
	{
		public SetupWizard()
		{
			InitializeComponent();
		}

		InstallationModeCollection GetInstallationModes(MsiInstallationModes mode)
		{
			var modes = SetupHelper.GetStandardInstallationModes(mode);
			//uncomment this line if you want to support modification mode iff reinstallation is possible
			//modes.InsertBefore(InstallationMode.Reinstall, InstallationMode.Modify);
			//uncomment this line if you don't want to support reinstallation
			//modes.Remove(InstallationMode.Reinstall);
			if (modes.Contains(SetupHelper.InstallationModeFromCommandLine))
				return new InstallationModeCollection(SetupHelper.InstallationModeFromCommandLine);
			else
				return modes;
		}

		public override void LifecycleAction(string type, object argument)
		{
			if (type == LifecycleActionType.Initialization)
			{
				AddStep(new InitializationStep());
			}
			else if (type == LifecycleActionType.ConnectionOpened)
			{
				//uncomment this line if you want to support MSI property passthrough
				//SetupHelper.ApplyMsiProperties();
				var modes = GetInstallationModes(MsiConnection.Instance.Mode);
				if (modes.Contains(InstallationMode.Downgrade))
					AddStep(new FatalErrorStep(Gui.Properties.Resources.DowngradeNotSupported));
				else if (modes.Count > 1)
					AddStep(new InstallationModeStep(modes));
				else if (modes.Count == 1)
					AddStep(new WelcomeStep(modes[0]));
			}
			else if (type == LifecycleActionType.ModeSelected)
			{
				FeatureSelectionStep featureStep = new FeatureSelectionStep();

				switch ((InstallationMode)argument)
				{
					case InstallationMode.Install:
						AddStep(new LicenseStep());
						//AddStep(new PrerequisiteCheckStep());
						//AddStep(new UserRegistrationStep());
						//AddStep(new InstallationTypeStep());
						//AddStep(new InstallationLocationStep());
						
						AddStep(featureStep);

						//AddStep(new Step1());
						//AddStep(new ReadyStep());
						AddStep(new InstallationStep(InstallationMode.Install, featureStep));
						AddStep(new FinishStep(InstallationMode.Install, featureStep));
						break;
					case InstallationMode.Uninstall:
						AddStep(new InstallationStep(InstallationMode.Uninstall));
						AddStep(new FinishStep(InstallationMode.Uninstall));
						break;
					case InstallationMode.Upgrade:
						AddStep(new LicenseStep());

						// Cannot change feature in upgade mode. So use existing feature set.
						featureStep.SelectedFeature = Migration.HasFeaure("MainFeature") ?
							FeatureSet.StationAndClient : FeatureSet.ClientOnly;

						AddStep(new BackupAndUninstallStep());
						
						/*
						AddStep(new InstallationTypeStep());
						AddStep(new InstallationLocationStep());
						*/

						AddStep(new InstallationStep(InstallationMode.Install, featureStep));
						AddStep(new FinishStep(InstallationMode.Upgrade, featureStep));
						break;
					case InstallationMode.Reinstall:
						AddStep(new InstallationStep(InstallationMode.Install));
						AddStep(new FinishStep(InstallationMode.Reinstall));
						break;
					default:
						MessageBox.Show("Mode not supported: " + (InstallationMode)argument);
						break;
				}
			}
			else
				MessageBox.Show("Unsupported lifecycle action");
		}
	}
}
