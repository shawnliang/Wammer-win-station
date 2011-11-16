// -- FILE ------------------------------------------------------------------
// name       : ColorDialog.cs
// created    : Jani Giannoudis - 2008.05.10
// language   : c#
// comment    : wpf does not provide a color picker
//              use the windows forms color dialog for demonstration purposes
// environment: .NET 3.0
// --------------------------------------------------------------------------
using System.Windows.Media;
using WinForms = System.Windows.Forms;

namespace Waveface.Solutions.Community.ConfigurationWindowsDemo
{

	// ------------------------------------------------------------------------
	public class ColorDialog : WinForms.ColorDialog
	{

		// ----------------------------------------------------------------------
		public new Color Color
		{
			get 
			{
				System.Drawing.Color color = base.Color;
				return Color.FromArgb( color.A, color.R, color.G, color.B );
			}
			set 
			{ 
				base.Color = System.Drawing.Color.FromArgb( value.A, value.R, value.G, value.B ); 
			}
		} // Color

		// ----------------------------------------------------------------------
		public new bool ShowDialog()
		{
			return base.ShowDialog() == WinForms.DialogResult.OK;
		} // ShowDialog

	} // class ColorDialog

} // namespace Itenso.Solutions.Community.ConfigurationWindowsDemo
// -- EOF -------------------------------------------------------------------
