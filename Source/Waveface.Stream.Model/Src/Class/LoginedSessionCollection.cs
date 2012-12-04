namespace Waveface.Stream.Model
{
	/// <summary>
	/// 
	/// </summary>
	public class LoginedSessionCollection : DBCollection<LoginedSession>
	{
		#region Static Var

		private static LoginedSessionCollection instance;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="LoginedSessionCollection"/> class.
		/// </summary>
		private LoginedSessionCollection()
			: base("LoginedSession")
		{
		}

		#endregion

		#region Public Static Method

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static LoginedSessionCollection Instance
		{
			get { return instance ?? (instance = new LoginedSessionCollection()); }
		}

		#endregion
	}
}