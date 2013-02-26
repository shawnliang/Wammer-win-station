
namespace Waveface.Stream.Model
{
	/// <summary>
	/// 
	/// </summary>
	public class LocationDBDataCollection : DBCollection<LocationDBData>
	{
		#region Var
		private static LocationDBDataCollection _instance;
		#endregion


		#region Property
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
		public static LocationDBDataCollection Instance
		{
			get
			{
				return _instance ?? (_instance = new LocationDBDataCollection());
			}
		}
		#endregion


		#region Constructor
		/// <summary>
		/// Prevents a default instance of the <see cref="PostDBDataCollection" /> class from being created.
		/// </summary>
		private LocationDBDataCollection()
			: base("locations")
		{
		}
		#endregion
	}
}