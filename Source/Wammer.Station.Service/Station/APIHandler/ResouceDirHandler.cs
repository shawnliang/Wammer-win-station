using System;
using System.IO;
using Waveface.Stream.Model;

namespace Wammer.Station
{
	internal class ResouceDirGetHandler : HttpHandler
	{
		private readonly string resourceBasePath;

		public ResouceDirGetHandler(string ResouceDirGetHandler)
		{
			resourceBasePath = ResouceDirGetHandler;
		}

		public override void HandleRequest()
		{
			RespondSuccess(
				new GetResourceDirResponse
					{
						status = 200,
						api_ret_code = 0,
						api_ret_message = "success",
						timestamp = DateTime.UtcNow,
						path = Path.IsPathRooted(resourceBasePath)
								? resourceBasePath
								: Path.GetFullPath(resourceBasePath)
					});
		}

		public override object Clone()
		{
			return MemberwiseClone();
		}
	}

	internal class ResouceDirSetHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			string path = Parameters["path"];

			if (path == null || !Path.IsPathRooted(path))
				throw new FormatException("path is null or not a full path");

			StationRegistry.SetValue("resourceBasePath", path);

			RespondSuccess();
		}

		public override object Clone()
		{
			return MemberwiseClone();
		}
	}

	public class GetResourceDirResponse : CloudResponse
	{
		public string path { get; set; }
	}
}