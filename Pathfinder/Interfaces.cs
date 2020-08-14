using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathfinder.Input;
using Nodes;

namespace Pathfinder {
    public interface IPathFinder {
        INode Start { get; }
        INode End { get; }

        IPath FindPath();
    }

    public interface IPath {
        bool Complete { get; }

        PathfinderTriggersSet Path { get; }
    }
}
