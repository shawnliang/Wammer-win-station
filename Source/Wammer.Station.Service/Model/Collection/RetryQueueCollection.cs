namespace Wammer.Model
{
	public class RetryQueueCollection : Collection<GenericData>
	{
		#region Var
		private static RetryQueueCollection _instance;
		#endregion

		#region Property
		public static RetryQueueCollection Instance
		{
			get { return _instance ?? (_instance = new RetryQueueCollection()); }
		}
		#endregion

		private RetryQueueCollection()
			: base("retry_queue")
		{
		}
	}
}