using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

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

	public class ProductInfoCollection: Collection<ProductInfo>
	{
		private static ProductInfoCollection instance;

		static ProductInfoCollection()
		{
			instance = new ProductInfoCollection();
		}

		private ProductInfoCollection()
			:base("products")
		{
		}

		public static ProductInfoCollection Instance
		{
			get { return instance; }
		}
	}
}
