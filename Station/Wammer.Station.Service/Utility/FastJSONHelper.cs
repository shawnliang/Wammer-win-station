
namespace Wammer.Utility
{
	public static class FastJSONHelper
	{
		public static string ToFastJSON(this object obj)
		{
			return fastJSON.JSON.Instance.ToJSON(obj, false, false, false, false);
		}
	}
}
