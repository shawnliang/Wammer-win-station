using System;

namespace Waveface.Stream.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class LoginedEventArgs : EventArgs
	{
		#region Var
		private String _response;
		#endregion

		#region Property
		/// <summary>
		/// Gets the response.
		/// </summary>
		/// <value>
		/// The response.
		/// </value>
		public String Response
		{
			get
			{
				return _response ?? string.Empty;
			}
			private set
			{
				_response = value;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="LoginedEventArgs" /> class.
		/// </summary>
		/// <param name="response">The response.</param>
		public LoginedEventArgs(string response)
		{
			this.Response = response;
		}
		#endregion
	}
}
