using System;
using System.Windows.Forms;

namespace StationSystemTray
{
	public class SimpleEventArgs : EventArgs
	{
		public object param;

		public SimpleEventArgs(object param)
		{
			this.param = param;
		}
	}

	public abstract class SimpleUIController
	{
		protected Form _form;
		protected object _parameter;

		protected SimpleUIController(Form form)
		{
			_form = form;
		}

		public event EventHandler<SimpleEventArgs> UICallback;

		public event EventHandler<SimpleEventArgs> UIError;

		public void PerformAction()
		{
			PerformAction(null);
		}

		public void PerformAction(object obj)
		{
			_parameter = obj;

			PerformActionDelegate performActionDelegate = Action;
			performActionDelegate.BeginInvoke(obj, PerformActionCallback, performActionDelegate);
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
			try
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
			catch (InvalidOperationException)
			{
				// On some condition, the UI forms has been closed before this UI controller is processing
				// and InvalidOperationException is then throw on _form.Invoke().
				//
				// just ignore the exception and abort the original UI update.
			}
		}

		private void BeginUpdateUIInError(Exception ex)
		{
			try
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
			catch (InvalidOperationException)
			{
				// On some condition, the UI forms has been closed before this UI controller is processing
				// and InvalidOperationException is then throw on _form.Invoke().
				//
				// just ignore the exception and abort the original UI update.
			}
		}

		protected abstract object Action(object obj);

		protected abstract void ActionCallback(object obj);

		protected abstract void ActionError(Exception ex);

		#region Nested type: PerformActionDelegate

		private delegate object PerformActionDelegate(object obj);

		#endregion Nested type: PerformActionDelegate
	}
}