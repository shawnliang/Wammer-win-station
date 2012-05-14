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


		public static ProductInfo GetCurrentVersion()
		{
			return new ProductInfo
			       	{
			       		ThumbnailExtensionIsDat = true
			       	};
		}
	}

	public class ProductInfoCollection : Collection<ProductInfo>
	{
		private static readonly ProductInfoCollection instance;

		static ProductInfoCollection()
		{
			instance = new ProductInfoCollection();
		}

		private ProductInfoCollection()
			: base("products")
		{
		}

		public static ProductInfoCollection Instance
		{
			get { return instance; }
		}
	}
}