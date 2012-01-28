using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StationSystemTray
{
	public class SimpleEventArgs : EventArgs
	{
		public object param;

		public SimpleEventArgs(object param)
			: base()
		{
			this.param = param;
		}
	}

	public abstract class SimpleUIController
	{
		private delegate object PerformActionDelegate(object obj);
		private delegate void UpdateUIInCallbackDelegate(object obj);
		private delegate void UpdateUIInErrorDelegate(Exception ex);

		public event EventHandler<SimpleEventArgs> UICallback;
		public event EventHandler<SimpleEventArgs> UIError;

		protected Form _form;
		protected object _parameter;

		protected SimpleUIController(Form form)
		{
			this._form = form;
		}

		public void PerformAction()
		{
			PerformAction(null);
		}

		public void PerformAction(object obj)
		{
			this._parameter = obj;

			PerformActionDelegate performActionDelegate = new PerformActionDelegate(Action);
			performActionDelegate.BeginInvoke(obj, new AsyncCallback(PerformActionCallback), performActionDelegate);
		}

		private void PerformActionCallback(IAsyncResult result)
		{
			try
			{
				object ret = ((PerformActionDelegate)result.AsyncState).EndInvoke(result);
				ActionCallback(ret);
				BeginUpdateUIInCallback(ret);
			}
			catch (Exception ex)
			{
				ActionError(ex);
				BeginUpdateUIInError(ex);
			}
		}

		private void BeginUpdateUIInCallback(object obj)
		{
			if (_form.InvokeRequired)
			{
				if (!_form.IsDisposed)
				{
					_form.Invoke(new EventHandler<SimpleEventArgs>(UICallback), this, new SimpleEventArgs(obj));
				}
			}
			else
			{
				UICallback(this, new SimpleEventArgs(obj));
			}
		}

		private void BeginUpdateUIInError(Exception ex)
		{
			if (_form.InvokeRequired)
			{
				if (!_form.IsDisposed)
				{
					_form.Invoke(new EventHandler<SimpleEventArgs>(UIError), this, new SimpleEventArgs(ex));
				}
			}
			else
			{
				UIError(this, new SimpleEventArgs(ex));
			}
		}

		protected abstract object Action(object obj);

		protected abstract void ActionCallback(object obj);

		protected abstract void ActionError(Exception ex);
	}
}
