using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StationSystemTray
{
	public abstract class SimpleUIController
	{
		private delegate object PerformActionDelegate(object obj);
		private delegate void UpdateUIDelegate(object obj);
		private delegate void UpdateUIInCallbackDelegate(object obj);
		private delegate void UpdateUIInErrorDelegate(Exception ex);
		private delegate void SetFormControlsDelegate(object obj);
		private delegate void SetFormControlsInCallbackDelegate(object obj);
		private delegate void SetFormControlsInErrorDelegate(Exception ex);

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

			SetFormControls(obj);
			BeginUpdateUI(obj);
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
				if (!_form.IsDisposed)
					_form.BeginInvoke(new SetFormControlsInCallbackDelegate(SetFormControlsInCallback), ret);
			}
			catch (Exception ex)
			{
				ActionError(ex);
				BeginUpdateUIInError(ex);
				if (!_form.IsDisposed)
					_form.BeginInvoke(new SetFormControlsInErrorDelegate(SetFormControlsInError), ex);
			}
		}

		private void BeginUpdateUI(object obj)
		{
			if (_form.InvokeRequired)
			{
				if (!_form.IsDisposed)
					_form.Invoke(new UpdateUIDelegate(UpdateUI), obj);
			}
			else
				UpdateUI(obj);
		}

		private void BeginUpdateUIInCallback(object obj)
		{
			if (_form.InvokeRequired)
			{
				if (!_form.IsDisposed)
					_form.Invoke(new UpdateUIInCallbackDelegate(UpdateUIInCallback), obj);
			}
			else
				UpdateUIInCallback(obj);
		}

		private void BeginUpdateUIInError(Exception ex)
		{
			if (_form.InvokeRequired)
			{
				if (!_form.IsDisposed)
					_form.Invoke(new UpdateUIInErrorDelegate(UpdateUIInError), ex);
			}
			else
				UpdateUIInError(ex);
		}

		protected abstract void SetFormControls(object obj);

		protected abstract void SetFormControlsInCallback(object obj);

		protected abstract void SetFormControlsInError(Exception ex);

		protected abstract void UpdateUI(object obj);

		protected abstract void UpdateUIInCallback(object obj);

		protected abstract void UpdateUIInError(Exception ex);

		protected abstract object Action(object obj);

		protected abstract void ActionCallback(object obj);

		protected abstract void ActionError(Exception ex);
	}
}
