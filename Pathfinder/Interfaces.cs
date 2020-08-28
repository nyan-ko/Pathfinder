using Nodes;
using Pathfinder.Input;

namespace Pathfinder
{
    public interface IPathFinder
    {
        INode Start { get; }
        INode End { get; }

        IPath FindPath();
    }

    public interface IPath
    {
        bool Complete { get; }

        PathfinderTriggersSet Path { get; }
    }
}