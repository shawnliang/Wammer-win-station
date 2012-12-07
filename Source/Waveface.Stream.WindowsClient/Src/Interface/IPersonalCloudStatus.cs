using System.Collections.Generic;

namespace Waveface.Stream.WindowsClient
{
	public interface IPersonalCloudStatus
	{
		IEnumerable<PersonalCloudNode> GetNodes(string user_id, string session_token, string apikey);
	}
}
