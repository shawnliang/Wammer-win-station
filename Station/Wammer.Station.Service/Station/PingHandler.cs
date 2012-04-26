
namespace Wammer.Station
{
	class PingHandler: HttpHandler
	{
		public override void HandleRequest()
		{
			this.RespondSuccess();
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
