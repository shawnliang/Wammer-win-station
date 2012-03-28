using System;
using System.IO;



namespace Wammer.Station
{
	class ResouceDirGetHandler: HttpHandler
	{
		private string resourceBasePath;

		public ResouceDirGetHandler(string ResouceDirGetHandler)
		{
			this.resourceBasePath = ResouceDirGetHandler;
		}

		protected override void HandleRequest()
		{
			RespondSuccess(
				new GetResourceDirResponse
				{
					status = 200,
					api_ret_code = 0,
					api_ret_message = "success",
					timestamp = DateTime.UtcNow,
					path = Path.IsPathRooted(resourceBasePath) ?
							resourceBasePath : Path.GetFullPath(resourceBasePath)
				});
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}

	class ResouceDirSetHandler: HttpHandler
	{
		protected override void HandleRequest()
		{
			string path = Parameters["path"];

			if (path == null || !Path.IsPathRooted(path))
				throw new FormatException("path is null or not a full path");
				
			StationRegistry.SetValue("resourceBasePath", path);

			RespondSuccess();
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}

	public class GetResourceDirResponse : Cloud.CloudResponse
	{
		public string path { get; set; }

		public GetResourceDirResponse()
			:base()
		{
		}
	}
}
