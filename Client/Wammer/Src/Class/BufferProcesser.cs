using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Waveface
{
	/// <summary>
	/// 
	/// </summary>
	public class BufferProcesser
	{
		#region Const
		const int DEFAULT_DELAY_PROCESS_INTERVAL = 500;
		#endregion


		#region Var
		private Timer _processDelayTimer;
		#endregion


		#region Private Property
		/// <summary>
		/// Gets the m_ process delay timer.
		/// </summary>
		/// <value>The m_ process delay timer.</value>
		private Timer m_ProcessDelayTimer
		{
			get
			{
				if (_processDelayTimer == null)
				{
					_processDelayTimer = new Timer();
					_processDelayTimer.Tick += new EventHandler(m_ProcessDelayTimer_Tick);
				}
 				return _processDelayTimer;
			}
		}

		/// <summary>
		/// Gets or sets the m_ process action.
		/// </summary>
		/// <value>The m_ process action.</value>
		private Action m_ProcessAction { get; set; }
		#endregion


		#region Public Property
		/// <summary>
		/// Gets or sets the delay process interval.
		/// </summary>
		/// <value>The delay process interval.</value>
		public int DelayProcessInterval { get; set; }
		#endregion


		#region Constructor & DeConstructor
		/// <summary>
		/// Initializes a new instance of the <see cref="BufferProcesser"/> class.
		/// </summary>
		/// <param name="processAction">The process action.</param>
		/// <param name="delayProcessInterval">The delay process interval.</param>
		public BufferProcesser(Action processAction, int delayProcessInterval = DEFAULT_DELAY_PROCESS_INTERVAL)
		{
			DebugInfo.ShowMethod();

			this.m_ProcessAction = processAction;
			this.DelayProcessInterval = delayProcessInterval;
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="BufferProcesser"/> is reclaimed by garbage collection.
		/// </summary>
		~BufferProcesser()
		{
			DebugInfo.ShowMethod();

			DoProcessIfNecessary();
		}
		#endregion


		#region Private Method
		/// <summary>
		/// Does the process.
		/// </summary>
		private void DoProcessIfNecessary()
		{
			DebugInfo.ShowMethod();

			if (m_ProcessAction == null)
				return;

			if (!m_ProcessDelayTimer.Enabled)
				return;

			m_ProcessDelayTimer.Stop();
			m_ProcessAction();
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Wants the process.
		/// </summary>
		public void WantProcess()
		{
			DebugInfo.ShowMethod();
			m_ProcessDelayTimer.Stop();
			m_ProcessDelayTimer.Interval = DelayProcessInterval;
			m_ProcessDelayTimer.Start();
		}
		#endregion


		#region Event Process
		/// <summary>
		/// Handles the Tick event of the m_ProcessDelayTimer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void m_ProcessDelayTimer_Tick(object sender, EventArgs e)
		{
			DebugInfo.ShowMethod();
			DoProcessIfNecessary();
		}
		#endregion
	}
}
