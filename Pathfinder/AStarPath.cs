using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathfinder.Input;
using Nodes;

namespace Pathfinder {
    public class AStarPath : IPath {
        public bool Complete { get; private set; }
        public PathfinderTriggersSet Path { get; private set; }

        public AStarPath(bool complete, PathfinderTriggersSet path) {
            Complete = complete;
            Path = path;
        }
    }
}
