
namespace Wammer.Station
{
	class PingHandler: HttpHandler
	{
		protected override void HandleRequest()
		{
			this.RespondSuccess();
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
