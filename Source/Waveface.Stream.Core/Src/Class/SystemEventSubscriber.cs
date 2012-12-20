using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.Core
{
	public class SystemEventSubscriber
	{
		#region Static Var
        private static SystemEventSubscriber _instance;
        #endregion


		#region Public Static Property
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
		public static SystemEventSubscriber Instance
        { 
            get
            {
                return _instance ?? (_instance = new SystemEventSubscriber());
            }
        }
        #endregion


		#region Event
		public event EventHandler<SystemEventSubscribeEventArgs> EventSubscribed;
		public event EventHandler<SystemEventSubscribeEventArgs> EventUnSubscribed;


		/// <summary>
		/// Occurs when [post added].
		/// </summary>
		public event EventHandler<SystemEventEventArgs> PostAdded;

		/// <summary>
		/// Occurs when [post updated].
		/// </summary>
		public event EventHandler<SystemEventEventArgs> PostUpdated;

		/// <summary>
		/// Occurs when [attachment added].
		/// </summary>
		public event EventHandler<SystemEventEventArgs> AttachmentAdded;

		/// <summary>
		/// Occurs when [attachment updated].
		/// </summary>
		public event EventHandler<SystemEventEventArgs> AttachmentUpdated;

		/// <summary>
		/// Occurs when [attachment arrived].
		/// </summary>
		public event EventHandler<SystemEventEventArgs> AttachmentArrived;

		/// <summary>
		/// Occurs when [collection added].
		/// </summary>
		public event EventHandler<SystemEventEventArgs> CollectionAdded;

		/// <summary>
		/// Occurs when [collection updated].
		/// </summary>
		public event EventHandler<SystemEventEventArgs> CollectionUpdated;
		#endregion



		#region Constructor
		private SystemEventSubscriber()
        {

        }
        #endregion


		#region Protected Method
		/// <summary>
		/// Raises the <see cref="E:EventSubscribed" /> event.
		/// </summary>
		/// <param name="e">The <see cref="SystemEventSubscribeEventArgs" /> instance containing the event data.</param>
		protected void OnEventSubscribed(SystemEventSubscribeEventArgs e)
		{
			if (EventSubscribed == null)
				return;

			EventSubscribed(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:EventUnSubscribed" /> event.
		/// </summary>
		/// <param name="e">The <see cref="SystemEventSubscribeEventArgs" /> instance containing the event data.</param>
		protected void OnEventUnSubscribed(SystemEventSubscribeEventArgs e)
		{
			if (EventUnSubscribed == null)
				return;

			EventUnSubscribed(this, e);
		}


		/// <summary>
		/// Raises the <see cref="E:PostAdded" /> event.
		/// </summary>
		/// <param name="e">The <see cref="SystemEventEventArgs" /> instance containing the event data.</param>
		protected void OnPostAdded(SystemEventEventArgs e)
		{
			if (PostAdded == null)
				return;

			PostAdded(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:PostUpdated" /> event.
		/// </summary>
		/// <param name="e">The <see cref="SystemEventEventArgs" /> instance containing the event data.</param>
		protected void OnPostUpdated(SystemEventEventArgs e)
		{
			if (PostUpdated == null)
				return;

			PostUpdated(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:AttachmentAdded" /> event.
		/// </summary>
		/// <param name="e">The <see cref="SystemEventEventArgs" /> instance containing the event data.</param>
		protected void OnAttachmentAdded(SystemEventEventArgs e)
		{
			if (AttachmentAdded == null)
				return;

			AttachmentAdded(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:AttachmentUpdated" /> event.
		/// </summary>
		/// <param name="e">The <see cref="SystemEventEventArgs" /> instance containing the event data.</param>
		protected void OnAttachmentUpdated(SystemEventEventArgs e)
		{
			if (AttachmentUpdated == null)
				return;

			AttachmentUpdated(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:AttachmentDownloaded" /> event.
		/// </summary>
		/// <param name="e">The <see cref="SystemEventEventArgs" /> instance containing the event data.</param>
		protected void OnAttachmentArrived(SystemEventEventArgs e)
		{
			if (AttachmentArrived == null)
				return;

			AttachmentArrived(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:CollectionAdded" /> event.
		/// </summary>
		/// <param name="e">The <see cref="SystemEventEventArgs" /> instance containing the event data.</param>
		protected void OnCollectionAdded(SystemEventEventArgs e)
		{
			if (CollectionAdded == null)
				return;

			CollectionAdded(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:CollectionUpdated" /> event.
		/// </summary>
		/// <param name="e">The <see cref="SystemEventEventArgs" /> instance containing the event data.</param>
		protected void OnCollectionUpdated(SystemEventEventArgs e)
		{
			if (CollectionUpdated == null)
				return;

			CollectionUpdated(this, e);
		}
		#endregion


		#region Public Method
		public void Subscribe(string webSocketChannelID, SystemEventType eventType, WebSocketCommandData data)
		{
			OnEventSubscribed(new SystemEventSubscribeEventArgs(webSocketChannelID, eventType, data));
		}

		public void UnSubscribe(string webSocketChannelID, SystemEventType eventType, WebSocketCommandData data)
		{
			OnEventUnSubscribed(new SystemEventSubscribeEventArgs(webSocketChannelID, eventType, data));
		}

		public void TriggerPostAddedEvent(params string[] IDs)
		{
			OnPostAdded(new SystemEventEventArgs(IDs));
		}

		public void TriggerPostUpdatedEvent(params string[] IDs)
		{
			OnPostUpdated(new SystemEventEventArgs(IDs));
		}

		public void TriggerAttachmentAddedEvent(params string[] IDs)
		{
			OnAttachmentAdded(new SystemEventEventArgs(IDs));
		}

		public void TriggerAttachmentUpdatedEvent(params string[] IDs)
		{
			OnAttachmentUpdated(new SystemEventEventArgs(IDs));
		}

		public void TriggerAttachmentArrivedEvent(params string[] IDs)
		{
			OnAttachmentArrived(new SystemEventEventArgs(IDs));
		}

		public void TriggerCollectionAddedEvent(params string[] IDs)
		{
			OnCollectionAdded(new SystemEventEventArgs(IDs));
		}

		public void TriggerCollectionUpdatedEvent(params string[] IDs)
		{
			OnCollectionUpdated(new SystemEventEventArgs(IDs));
		}
		#endregion
	}
}
