
#region

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Waveface.Configuration;

#endregion

namespace Waveface.Solutions.Community.ConfigurationWindowsFormsDemo
{
    internal enum CustomAlignment
    {
        Left,
        Center,
        Right,
    }

    public partial class SettingsForm : Form
    {
        private readonly FormSettings m_formSettings;
        private Color formColor = Color.FromArgb(200, 255, 200); //不可更改 Field Name
        private Font formFont = new Font(FontFamily.GenericMonospace, 8); //不可更改 Field Name

        #region Properties

        public string HostName
        {
            get { return hostEdit.Text; }
            set { hostEdit.Text = value; }
        }

        internal CustomAlignment Alignment
        {
            get
            {
                if (alignmentLeftOption.Checked)
                {
                    return CustomAlignment.Left;
                }

                if (alignmentCenterOption.Checked)
                {
                    return CustomAlignment.Center;
                }

                return alignmentRightOption.Checked ? CustomAlignment.Right : CustomAlignment.Left;
            }
            set
            {
                switch (value)
                {
                    case CustomAlignment.Left:
                        alignmentLeftOption.Checked = true;
                        break;
                    case CustomAlignment.Center:
                        alignmentCenterOption.Checked = true;
                        break;
                    case CustomAlignment.Right:
                        alignmentRightOption.Checked = true;
                        break;
                }
            }
        }

        #endregion

        public SettingsForm()
        {
            InitializeComponent();

            SetupListItems(50);

            // create settings group
            m_formSettings = new FormSettings(this);
            m_formSettings.SaveOnClose = false; // disable auto-save

            // add settings to group
            m_formSettings.Settings.Add(
                new PropertySetting("Panel.SplitterPosition", panelSplitter, "SplitPosition", 100));
            m_formSettings.Settings.Add(
                new PropertySetting("CustomAlignment", this, "Alignment", CustomAlignment.Left));
            m_formSettings.Settings.Add(
                new FieldSetting("FormColor", this, "formColor", formColor));
            m_formSettings.Settings.Add(
                new FieldSetting("FormFont", this, "formFont", formFont));

            InitControls();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            UpdateControls();
        }

        private void SetupListItems(int count)
        {
            itemsList.BeginUpdate();

            for (int i = 0; i < count; i++)
            {
                itemsList.Items.Add("Item " + (i + 1));
            }

            itemsList.SelectedIndex = 0;
            itemsList.EndUpdate();
        }

        private void InitControls()
        {
            userConfigNameLabel.Text = ApplicationSettings.UserConfigurationFilePath;
            formToolTip.SetToolTip(userConfigNameLabel, ApplicationSettings.UserConfigurationFilePath);
            UpdateControls();
        }

        private void UpdateControls()
        {
            windowLocationLabel.Text = string.Concat(Left.ToString(), ", ", Top.ToString());
            windowSizeLabel.Text = string.Concat(Width.ToString(), " x ", Height.ToString());
            windowStateLabel.Text = string.Concat(WindowState.ToString());

            selectedItemLabel.Text = itemsList.SelectedItem != null ? itemsList.SelectedItem.ToString() : "-";
            spltterPositionText.Text = panelSplitter.SplitPosition.ToString();

            colorText.Text = formColor.ToString();
            colorText.BackColor = formColor;
            formToolTip.SetToolTip(colorText, formColor.ToString());

            fontText.Text = formFont.ToString();
            fontText.Font = formFont;
            formToolTip.SetToolTip(fontText, formFont.ToString());
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);

            UpdateControls();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            UpdateControls();
        }

        private void ItemChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void SplitterMoving(object sender, SplitterEventArgs e)
        {
            UpdateControls();
        }

        private void ChangeColor(object sender, EventArgs e)
        {
            colorSetupDialog.Color = formColor;
          
            if (colorSetupDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            formColor = colorSetupDialog.Color;
            UpdateControls();
        }

        private void ChangeFont(object sender, EventArgs e)
        {
            fontSetupDialog.Font = formFont;
         
            if (fontSetupDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            formFont = fontSetupDialog.Font;
            UpdateControls();
        }
        private void ResetSettings(object sender, EventArgs e)
        {
            m_formSettings.Reset();
            UpdateControls();
        }

        private void ReloadSettings(object sender, EventArgs e)
        {
            m_formSettings.Reload();
            UpdateControls();
        }

        private void SaveSettings(object sender, EventArgs e)
        {
            m_formSettings.Save();
            UpdateControls();
        }

        private void OpenUserConfig(object sender, EventArgs e)
        {
            string _fileName = userConfigNameLabel.Text;
          
            if (!File.Exists(_fileName))
            {
                MessageBox.Show("File not available:\n\n" + _fileName, Text);
                return;
            }

            Process.Start(_fileName);
        }

        private void CollectedSettings(object sender, EventArgs e)
        {
            new CollectedSettingsForm().ShowDialog();
        }

        private void GridViewSettings(object sender, EventArgs e)
        {
            new DataGridViewSettingsForm().ShowDialog();
        }

        private void Close(object sender, EventArgs e)
        {
            if (saveOnCloseOption.Checked)
            {
                m_formSettings.Save();
            }

            Close();
        }
    }
}