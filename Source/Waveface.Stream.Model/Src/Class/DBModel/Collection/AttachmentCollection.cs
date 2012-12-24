using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.Model
{
	public class AttachmentCollection : DBCollection<Attachment>
	{
		#region Var
		private static AttachmentCollection _instance;
		#endregion


		#region Property
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
		public static AttachmentCollection Instance
		{
			get
			{
				return _instance ?? (_instance = new AttachmentCollection());
			}
		}
		#endregion


		#region Constructor
		/// <summary>
		/// Prevents a default instance of the <see cref="AttachmentCollection" /> class from being created.
		/// </summary>
		private AttachmentCollection()
			: base("attachments")
		{
			collection.EnsureIndex(new IndexKeysBuilder().Ascending("group_id"));
			collection.EnsureIndex(new IndexKeysBuilder().Ascending("md5"));
		} 
		#endregion
	}
}
