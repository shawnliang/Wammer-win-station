namespace Wammer.Station
{
	[APIHandlerInfo(APIHandlerType.FunctionAPI | APIHandlerType.ManagementAPI, "/availability/ping/")]
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