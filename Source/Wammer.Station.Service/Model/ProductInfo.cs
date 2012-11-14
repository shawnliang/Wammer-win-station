using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Wammer.Model
{
	[BsonIgnoreExtraElements]
	public class ProductInfo
	{
		[BsonId]
		public BsonObjectId id { get; set; }

		[BsonIgnoreIfNull]
		[BsonDefaultValue(false)]
		public bool ThumbnailExtensionIsDat { get; set; }

		[BsonIgnoreIfNull]
		[BsonDefaultValue(false)]
		public bool UsingChangeLogsInsteadOfUserTracks { get; set; }

		[BsonIgnoreIfNull]
		[BsonDefaultValue(false)]
		public bool PlaceAttachmentsByDate { get; set; }

		public static ProductInfo GetCurrentVersion()
		{
			return new ProductInfo
			{
				ThumbnailExtensionIsDat = true,
				UsingChangeLogsInsteadOfUserTracks = true,
				PlaceAttachmentsByDate = true
			};
		}
	}

	public class ProductInfoCollection : Collection<ProductInfo>
	{
		#region Var
		private static ProductInfoCollection _instance; 
		#endregion

		#region Property
		public static ProductInfoCollection Instance
		{
			get { return _instance ?? (_instance = new ProductInfoCollection()); }
		}
		#endregion

		private ProductInfoCollection()
			: base("products")
		{
		}
	}
}