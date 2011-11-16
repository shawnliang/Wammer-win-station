// C# TimePicker Class v2.0
// by Louis-Philippe Carignan - 10 July 2007
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Waveface.Component.TimePickerEx
{
    public partial class TimePicker : System.Windows.Forms.UserControl
    {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimePicker));
            this.m_oBtnShowTimeSelector = new System.Windows.Forms.Button();
            this.m_oPnlBackground = new System.Windows.Forms.Panel();
            this.m_oChkEnable = new System.Windows.Forms.CheckBox();
            this.m_oPnlBackground.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_oBtnShowTimeSelector
            // 
            this.m_oBtnShowTimeSelector.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.m_oBtnShowTimeSelector.Image = ((System.Drawing.Image)(resources.GetObject("m_oBtnShowTimeSelector.Image")));
            this.m_oBtnShowTimeSelector.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_oBtnShowTimeSelector.Location = new System.Drawing.Point(111, 2);
            this.m_oBtnShowTimeSelector.Name = "m_oBtnShowTimeSelector";
            this.m_oBtnShowTimeSelector.Size = new System.Drawing.Size(22, 18);
            this.m_oBtnShowTimeSelector.TabIndex = 4;
            this.m_oBtnShowTimeSelector.Click += new System.EventHandler(this.m_oBtnShowTimeSelector_Click);
            // 
            // m_oPnlBackground
            // 
            this.m_oPnlBackground.BackColor = System.Drawing.Color.White;
            this.m_oPnlBackground.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_oPnlBackground.Controls.Add(this.m_oChkEnable);
            this.m_oPnlBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_oPnlBackground.Location = new System.Drawing.Point(0, 0);
            this.m_oPnlBackground.Name = "m_oPnlBackground";
            this.m_oPnlBackground.Size = new System.Drawing.Size(134, 22);
            this.m_oPnlBackground.TabIndex = 100;
            this.m_oPnlBackground.Click += new System.EventHandler(this.m_oPnlBackground_Click);
            this.m_oPnlBackground.Paint += new System.Windows.Forms.PaintEventHandler(this.m_oPnlBackground_Paint);
            // 
            // m_oChkEnable
            // 
            this.m_oChkEnable.Location = new System.Drawing.Point(5, 0);
            this.m_oChkEnable.Name = "m_oChkEnable";
            this.m_oChkEnable.Size = new System.Drawing.Size(14, 18);
            this.m_oChkEnable.TabIndex = 0;
            this.m_oChkEnable.CheckedChanged += new System.EventHandler(this.m_oChkEnable_CheckedChanged);
            // 
            // TimePicker
            // 
            this.Controls.Add(this.m_oBtnShowTimeSelector);
            this.Controls.Add(this.m_oPnlBackground);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.Name = "TimePicker";
            this.Size = new System.Drawing.Size(134, 22);
            this.FontChanged += new System.EventHandler(this.TimePicker_FontChanged);
            this.m_oPnlBackground.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion Component Designer generated code

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Handlers for the events in the TimePicker
        private void m_oPnlBackground_Click(object sender, EventArgs e)
        {
            // Don't do anything when the control is disabled
            if (!IsEnabled())
                return;

            Point pointClicked = m_oPnlBackground.PointToClient(Control.MousePosition);

            if (m_eTimeSelector == TimePickerFormat.Hours)
            {
                if (pointClicked.X >= m_nHourRange &&
                    pointClicked.X <= m_nSeparatorRange)
                {
                    HoursRegionClicked(sender, e);
                }
            }
            else if (m_eTimeSelector == TimePickerFormat.HoursAndMinutes)
            {
                if (pointClicked.X >= m_nHourRange &&
                    pointClicked.X <= m_nSeparatorRange)
                {
                    HoursRegionClicked(sender, e);
                }
                else if (pointClicked.X >= m_nMinuteRange &&
                    pointClicked.X <= m_nAMPMRange)
                {
                    MinutesRegionClicked(sender, e);
                }
                else if (pointClicked.X >= m_nAMPMRange &&
                    pointClicked.X <= m_nEndRange)
                {
                    AMPMRegionClicked(sender, e);
                }
            }
            else if (m_eTimeSelector == TimePickerFormat.Minutes)
            {
                if (pointClicked.X >= m_nMinuteRange &&
                    pointClicked.X <= m_nAMPMRange)
                {
                    MinutesRegionClicked(sender, e);
                }
            }
        }

        private void TimePicker_FontChanged(object sender, EventArgs e)
        {
            this.Height = this.Font.Height + 7;

            // Resize the DropDown button
            m_oBtnShowTimeSelector.Height = this.Font.Height + 4;
            m_oBtnShowTimeSelector.Location = new Point(this.Width - m_oBtnShowTimeSelector.Width - 3, 2);
        }

        private void m_oPnlBackground_Paint(object sender, PaintEventArgs e)
        {
            // Initial X location of the time to paint
            int xOffset = 5;

            // If the check box is visible, move the X location
            // by adding the withd of the check box
            if (m_oChkEnable.Visible)
            {
                xOffset += m_oChkEnable.Width;
            }

            // If the check box is visible, it will control the
            // enable/disable color of the time.  If it's not
            // visible, the Enable property of the TimePicker
            // takes care of it.
            bool enableColor = IsEnabled();

            SolidBrush brush;
            if (enableColor)
            {
                brush = EnableColor;
            }
            else
            {
                brush = DisableColor;
            }

            if (m_eRightToLeft == RightToLeft.No)
                DrawFromLeftToRight(e.Graphics, brush, xOffset);
            else
                DrawFromRightToLeft(e.Graphics, brush);

        }

        private void DrawFromLeftToRight(Graphics g, Brush brush, int xOffset)
        {
            // Draw the hours
            SizeF area = SizeF.Empty;
            if (m_eTimeSelector == TimePickerFormat.Hours ||
                m_eTimeSelector == TimePickerFormat.HoursAndMinutes)
            {
                m_nHourRange = xOffset;
                g.DrawString(m_szHours, this.Font, brush, xOffset, 1);
                area = g.MeasureString(m_szHours, this.Font);
                xOffset += (int)area.Width;
            }

            // Draw the separator
            if (m_eTimeSelector == TimePickerFormat.HoursAndMinutes)
            {
                m_nSeparatorRange = xOffset;
                g.DrawString(m_szSeparatorAsString, this.Font, brush, xOffset, 1);
                area = g.MeasureString(m_szSeparatorAsString, this.Font);
                xOffset += (int)area.Width;
            }
            else
            {
                m_nSeparatorRange = xOffset;
            }

            // Draw the minutes
            if (m_eTimeSelector == TimePickerFormat.Minutes ||
                m_eTimeSelector == TimePickerFormat.HoursAndMinutes)
            {
                m_nMinuteRange = xOffset;
                g.DrawString(m_szMinutes, this.Font, brush, xOffset, 1);
                area = g.MeasureString(m_szMinutes, this.Font);
                xOffset += (int)area.Width;
            }

            // Draw the AM/PM string
            if (!String.IsNullOrEmpty(m_szAMPM))
            {
                m_nAMPMRange = xOffset;
                g.DrawString(m_szAMPM, this.Font, brush, xOffset, 1);
                area = g.MeasureString(m_szAMPM, this.Font);

                m_nEndRange += (int)area.Width;
            }
            else
            {
                m_nAMPMRange = xOffset;
            }
        }

        private void DrawFromRightToLeft(Graphics g, Brush brush)
        {
            int buttonWitdh = 0;
            if (m_oBtnShowTimeSelector.Visible)
            {
                buttonWitdh = m_oBtnShowTimeSelector.Width;
            }

            SizeF area = SizeF.Empty;
            int xOffset = m_oPnlBackground.Width - buttonWitdh - 5;

            // Draw the AM/PM string
            if (!String.IsNullOrEmpty(m_szAMPM))
            {
                area = g.MeasureString(m_szAMPM, this.Font);

                m_nEndRange = xOffset;
                xOffset -= (int)area.Width;
                m_nAMPMRange = xOffset;

                g.DrawString(m_szAMPM, this.Font, brush, m_nAMPMRange, 1);
            }
            else
            {
                m_nEndRange = m_nAMPMRange = xOffset;
            }

            // Draw the minutes
            if (m_eTimeSelector == TimePickerFormat.Minutes ||
                m_eTimeSelector == TimePickerFormat.HoursAndMinutes)
            {
                area = g.MeasureString(m_szMinutes, this.Font);

                xOffset -= (int)area.Width;
                m_nMinuteRange = xOffset;

                g.DrawString(m_szMinutes, this.Font, brush, xOffset, 1);
            }

            // Draw the separator
            if (m_eTimeSelector == TimePickerFormat.HoursAndMinutes)
            {
                area = g.MeasureString(m_szSeparatorAsString, this.Font);

                xOffset -= (int)area.Width;
                m_nSeparatorRange = xOffset;

                g.DrawString(m_szSeparatorAsString, this.Font, brush, xOffset, 1);
            }
            else
            {
                m_nSeparatorRange = xOffset;
            }

            // Draw the hours
            if (m_eTimeSelector == TimePickerFormat.Hours ||
                m_eTimeSelector == TimePickerFormat.HoursAndMinutes)
            {
                area = g.MeasureString(m_szHours, this.Font);

                xOffset -= (int)area.Width;
                m_nHourRange = xOffset;

                g.DrawString(m_szHours, this.Font, brush, xOffset, 1);
            }
        }

        private void m_oBtnShowTimeSelector_Click(object sender, System.EventArgs e)
        {
            if (m_eTimeSelector == TimePickerFormat.Hours)
            {
                m_oHourForm.SelectedTime = m_sSelectedTime;
                m_oHourForm.Location = CalculateLocation(m_oHourForm.Size);
                m_oHourForm.Show();
                m_oHourForm.BringToFront();
                m_oHourForm.Focus();
            }
            else if (m_eTimeSelector == TimePickerFormat.Minutes)
            {
                m_oMinuteForm.SelectedTime = m_sSelectedTime;
                m_oMinuteForm.Location = CalculateLocation(m_oMinuteForm.Size);
                m_oMinuteForm.Show();
                m_oMinuteForm.BringToFront();
                m_oMinuteForm.Focus();
            }
            else if (m_eTimeSelector == TimePickerFormat.HoursAndMinutes)
            {
                m_oHourMinuteForm.SelectedTime = m_sSelectedTime;
                m_oHourMinuteForm.Location = CalculateLocation(m_oHourMinuteForm.Size);
                m_oHourMinuteForm.Show();
                m_oHourMinuteForm.BringToFront();
                m_oHourMinuteForm.Focus();
            }
        }

        private void m_oHourForm_Deactivate(object sender, System.EventArgs e)
        {
            m_sOldTimeSelected = m_sSelectedTime;
            m_sSelectedTime = m_oHourForm.SelectedTime;
            GenerateFormatedText();
            FireEvents();
        }

        private void m_oMinuteForm_Deactivate(object sender, System.EventArgs e)
        {
            m_sOldTimeSelected = m_sSelectedTime;
            m_sSelectedTime = m_oMinuteForm.SelectedTime;
            GenerateFormatedText();
            FireEvents();
        }

        private void m_oHourMinuteForm_Deactivate(object sender, System.EventArgs e)
        {
            m_sOldTimeSelected = m_sSelectedTime;
            m_sSelectedTime = m_oHourMinuteForm.SelectedTime;
            GenerateFormatedText();
            FireEvents();
        }

        private void m_oChkEnable_CheckedChanged(object sender, System.EventArgs e)
        {
            Checked = m_oChkEnable.Checked;
        }

        private void HoursRegionClicked(object sender, System.EventArgs e)
        {
            m_oHourForm.SelectedTime = m_sSelectedTime;
            m_oHourForm.Location = CalculateLocation(m_oHourForm.Size);
            m_oHourForm.ResetMinute = false;
            m_oHourForm.Show();
            m_oHourForm.BringToFront();
        }

        private void MinutesRegionClicked(object sender, System.EventArgs e)
        {
            m_oMinuteForm.SelectedTime = m_sSelectedTime;
            m_oMinuteForm.Location = CalculateLocation(m_oMinuteForm.Size);
            m_oMinuteForm.ResetMinute = false;
            m_oMinuteForm.Show();
            m_oMinuteForm.BringToFront();
        }

        private void AMPMRegionClicked(object sender, System.EventArgs e)
        {
            if (String.Compare(m_szAMPM, AM, false) == 0)
            {
                m_szAMPM = PM;
                m_sSelectedTime = m_sSelectedTime.Add(TWELVE_HOURS);
                GenerateFormatedText();
            }
            else
            {
                m_szAMPM = AM;
                m_sSelectedTime = m_sSelectedTime.Subtract(TWELVE_HOURS);
                GenerateFormatedText();
            }
        }
        #endregion Handlers for the events in the TimePicker

        private System.Windows.Forms.Panel m_oPnlBackground;
        private System.Windows.Forms.CheckBox m_oChkEnable;
        private System.Windows.Forms.Button m_oBtnShowTimeSelector;
    }
}
