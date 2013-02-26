using System;
using System.Diagnostics;

namespace Waveface.Stream.WindowsClient
{
	[DebuggerDisplayAttribute("{Name}({ID})")]
	public class Device
	{
		#region Var
		private long _remainingBackUpCount;
		#endregion

		#region Property
		/// <summary>
		/// Gets or sets the ID.
		/// </summary>
		/// <value>
		/// The ID.
		/// </value>
		public string ID { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the remaining back up count.
		/// </summary>
		/// <value>
		/// The remaining back up count.
		/// </value>
		public long RemainingBackUpCount
		{
			get
			{
				return _remainingBackUpCount;
			}
			internal set
			{
				if (_remainingBackUpCount == value)
					return;

				_remainingBackUpCount = value;
				OnRemainingBackUpCountChanged(EventArgs.Empty);
			}
		}
		#endregion


		#region Event
		public event EventHandler RemainingBackUpCountChanged;
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="Device" /> class.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="name">The name.</param>
		public Device(string id, string name)
		{
			this.ID = id;
			this.Name = name;
		}
		#endregion


		#region Protected Method
		protected void OnRemainingBackUpCountChanged(EventArgs e)
		{
			this.RaiseEvent(RemainingBackUpCountChanged, e);
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return Name;
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (Object.ReferenceEquals(this, obj))
				return true;

			if (this.GetType() != obj.GetType())
				return false;

			return this.ID.Equals((obj as Device).ID);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return this.ID.GetHashCode();
		}
		#endregion
	}
}
