using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nodes;

namespace Pathfinder.Heuristics {
    public struct Manhattan : IHeuristic {
        public float EstimateCost(INode start, INode goal) {
            return Math.Abs(start.X - goal.X) + Math.Abs(start.Y - goal.Y);
        }
    }
}
