using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder.Heuristics {
    public struct Debug : IHeuristic {
        public float EstimateCost(int x, int y, int goalX, int goalY) {
            return 0;
        }
    }
}
