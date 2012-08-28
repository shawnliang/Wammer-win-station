using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Waveface
{
	/// <summary>
	/// 
	/// </summary>
	public partial class AutoImportDialog : Form
	{
		#region Var
		private SynchronizationContext _syncContext = SynchronizationContext.Current;
		#endregion

		#region Private Property
		/// <summary>
		/// Gets the m_ sync context.
		/// </summary>
		/// <value>The m_ sync context.</value>
		private SynchronizationContext m_SyncContext
		{
			get
			{
				return _syncContext;
			}
		}
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="AutoImportDialog"/> class.
		/// </summary>
		public AutoImportDialog()
		{
			InitializeComponent();
		} 
		#endregion


		#region Private Method
		/// <summary>
		/// Sends the sync context.
		/// </summary>
		/// <param name="target">The target.</param>
		private void SendSyncContext(Action target)
		{
			m_SyncContext.Send((obj) => target(), null);
		}

		/// <summary>
		/// Sends the sync context.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target">The target.</param>
		/// <param name="o">The o.</param>
		private void SendSyncContext<T>(Action<T> target, Object o)
		{
			m_SyncContext.Send((obj) => target((T)obj), o);
		}
		#endregion


		#region Event Process
		/// <summary>
		/// Handles the Click event of the btnImport control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnImport_Click(object sender, EventArgs e)
		{
			MethodInvoker mi = new MethodInvoker(() =>
			{
				var importer = new AutoImporter();
				importer.Import();
			});

			btnImport.Enabled = false;
			progressBar1.Visible = true;
			mi.BeginInvoke((result) =>
			{
				SendSyncContext(() =>
				{
					this.DialogResult = DialogResult.OK;
					progressBar1.Visible = false;
				});
			}, null);
		} 
		#endregion
	}
}
