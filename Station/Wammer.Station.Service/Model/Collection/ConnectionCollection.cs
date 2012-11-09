using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Builders;


namespace Wammer.Model
{
	/// <summary>
	/// 
	/// </summary>
	public class ConnectionCollection : Collection<LoginedSession>
	{
		#region Static Var

		private static ConnectionCollection instance;

		#endregion

		#region Constructor

		static ConnectionCollection()
		{
			instance = new ConnectionCollection();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionCollection"/> class.
		/// </summary>
		private ConnectionCollection()
			: base("ConnectedDevices")
		{
		}

		#endregion

		#region Public Static Method


		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static ConnectionCollection Instance
		{
			get { return instance; }
		}

		#endregion
	}
}
