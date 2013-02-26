
namespace Wammer.Station
{
	public interface INamedTask : ITask
	{
		string Name { get; }
	}
}
