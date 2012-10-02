using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Waveface
{
	public partial class PropertyDialog : Form
	{
		#region Var
		private IEnumerable<KeyValuePair<string, string>> _initExifItems;
		#endregion

		#region Private Property
		private IEnumerable<KeyValuePair<string, string>> m_InitExifItems
		{
			get
			{
				return _initExifItems ?? (_initExifItems = new KeyValuePair<string, string>[] { });
			}
			set
			{
				_initExifItems = value;
			}
		}
		#endregion

		#region Constructor
		public PropertyDialog()
		{
			InitializeComponent();
		}

		public PropertyDialog(IEnumerable<KeyValuePair<string,string>> exifItems):this()
		{
			m_InitExifItems = exifItems;
		} 
		#endregion


		#region Public Method
		/// <summary>
		/// Clears the exif items.
		/// </summary>
		public void ClearExifItems()
		{
			lvExif.Items.Clear();
		}

		/// <summary>
		/// Adds the exif item.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public void AddExifItem(string name, string value)
		{
			lvExif.Items.Add(name).SubItems.Add(value);
		}

		/// <summary>
		/// Adds the exif items.
		/// </summary>
		/// <param name="exifItems">The exif items.</param>
		public void AddExifItems(IEnumerable<KeyValuePair<string,string>> exifItems)
		{
			try
			{
				lvExif.SuspendLayout();
				lvExif.BeginUpdate();
				foreach (var exifItem in exifItems)
				{
					AddExifItem(exifItem.Key, exifItem.Value);
				}
			}
			finally
			{
				lvExif.EndUpdate();
				lvExif.ResumeLayout();
			}
		}
		#endregion

		#region Event Process
		/// <summary>
		/// Handles the Load event of the PropertyDialog control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void PropertyDialog_Load(object sender, EventArgs e)
		{
			AddExifItems(m_InitExifItems);
		} 
		#endregion
	}
}
