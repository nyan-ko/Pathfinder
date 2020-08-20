using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder.Heuristics {
    public struct Euclidean : IHeuristic {
        public float EstimateCost(int x, int y, int goalX, int goalY) {
            int dX = x - goalX;
            int dY = y - goalY;
            return dX * dX + dY + dY;
        }
    }
}
