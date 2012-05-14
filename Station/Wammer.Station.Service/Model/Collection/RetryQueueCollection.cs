namespace Wammer.Model
{
	public class RetryQueueCollection : Collection<GenericData>
	{
		private static readonly RetryQueueCollection instance;

		static RetryQueueCollection()
		{
			instance = new RetryQueueCollection();
		}

		private RetryQueueCollection()
			: base("retry_queue")
		{
		}

		public static RetryQueueCollection Instance
		{
			get { return instance; }
		}
	}
}