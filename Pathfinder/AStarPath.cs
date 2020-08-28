using Pathfinder.Input;

namespace Pathfinder
{
    public class AStarPath : IPath
    {
        public bool Complete { get; private set; }
        public PathfinderTriggersSet Path { get; private set; }

        public AStarPath(bool complete, PathfinderTriggersSet path)
        {
            Complete = complete;
            Path = path;
        }
    }
}