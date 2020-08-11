using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nodes;

namespace Pathfinder {
    public interface IPathFinder {
        INode Start { get; }
        INode End { get; }

        IPath FindPath();
    }

    public interface IPath {
        bool Complete { get; }
        bool Optimal { get; }

        AbstractPathNode[] Path { get; }
    }
}
