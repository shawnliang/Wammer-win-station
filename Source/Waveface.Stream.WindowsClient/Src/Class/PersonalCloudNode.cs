
namespace Waveface.Stream.WindowsClient
{
	public enum NodeType
	{
		Station,
		Tablet,
		Phone
	}

	public class PersonalCloudNode
	{
		public string Name { get; set; }
		public string Profile { get; set; }
		public string Status { get; set; }
		public NodeType Type { get; set; }
		public string Id { get; set; }
	}
}
