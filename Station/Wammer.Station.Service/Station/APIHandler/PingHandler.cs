namespace Wammer.Station
{
	internal class PingHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			RespondSuccess();
		}

		public override object Clone()
		{
			return MemberwiseClone();
		}
	}
}