#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

#endregion

namespace Waveface.Component.TimePickerEx
{
    // The TimePickerValueEditor is used to edit the Value property of the TimePicker
    // control during design-mode.  It is mainly used to display a HourMinuteSelectorForm
    // on the screen to let the developer choose the time he wants.
    public class TimePickerValueEditor : UITypeEditor
    {
        private IWindowsFormsEditorService m_oEditorService;

        // The form that will be displayed when the user click on the Value property in design-mode.
        private HourMinuteSelectorForm m_oHourMinuteForm;

        public TimePickerValueEditor()
        {
            m_oEditorService = null;
        }

        // EditValue is inherited from UITypeEditor, it is used to create, place and
        // show the HourMinuteSelectorForm on the screen.
        public override Object EditValue(ITypeDescriptorContext context,
                                         IServiceProvider provider,
                                         Object value)
        {
            if (context != null &&
                context.Instance != null &&
                provider != null)
            {
                m_oEditorService = (IWindowsFormsEditorService) provider.GetService(typeof (IWindowsFormsEditorService));

                if (m_oEditorService != null)
                {
                    m_oHourMinuteForm = new HourMinuteSelectorForm();
                    m_oHourMinuteForm.Location = CalculateLocation(m_oHourMinuteForm.Size);
                    m_oHourMinuteForm.Font = new Font("Microsoft Sans Serif", 10.25F, FontStyle.Regular,
                                                      GraphicsUnit.Point, ((0)));

                    m_oHourMinuteForm.SelectedTime = (TimeSpan) value;

                    m_oEditorService.ShowDialog(m_oHourMinuteForm);

                    value = m_oHourMinuteForm.SelectedTime;
                }
            }

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                return UITypeEditorEditStyle.DropDown;
            }

            return base.GetEditStyle(context);
        }

        // Private method to place a form inside the screen.  This will prevent
        // the form of going outside the limits of the screen.
        private Point CalculateLocation(Size sSizeOfForm)
        {
            Point l_sPoint = Control.MousePosition;

            if (l_sPoint.Y + sSizeOfForm.Height > Screen.PrimaryScreen.WorkingArea.Height)
            {
                // We are near the bottom of the screen.  If the form appears, it won't
                // be entirely shown on the screen.  So place the form above the 
                // Time Picker
                l_sPoint = new Point(l_sPoint.X, l_sPoint.Y - 16 - sSizeOfForm.Height);
            }

            if (l_sPoint.X < 0)
            {
                // We are to much on the left side of the screen.  This causes the 
                // form to be outside the screen.  We have to push back the form 
                // on the edge of the screen.
                l_sPoint = new Point(0, l_sPoint.Y);
            }
            else if (l_sPoint.X + sSizeOfForm.Width > Screen.PrimaryScreen.WorkingArea.Width)
            {
                // This is the opposite of the previous statement.  We are now 
                // on the right side of the screen.  We have to stick the window to
                // the edge of the right border of the screen.
                l_sPoint = new Point(Screen.PrimaryScreen.WorkingArea.Width - sSizeOfForm.Width, l_sPoint.Y);
            }

            return l_sPoint;
        }
    }
}