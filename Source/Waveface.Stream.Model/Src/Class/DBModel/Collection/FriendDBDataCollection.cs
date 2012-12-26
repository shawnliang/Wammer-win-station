using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace Waveface.Stream.Model
{
	/// <summary>
	/// 
	/// </summary>
	public class FriendDBDataCollection : DBCollection<FriendDBData>
	{
		#region Var
		private static FriendDBDataCollection _instance;
		#endregion


		#region Property       
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
		public static FriendDBDataCollection Instance
        { 
            get
            {
                return _instance ?? (_instance = new FriendDBDataCollection());
            }
        }
		#endregion


		#region Constructor
		/// <summary>
		/// Prevents a default instance of the <see cref="PostDBDataCollection" /> class from being created.
		/// </summary>
		private FriendDBDataCollection()
			: base("friends")
		{
		}
		#endregion
	}
}