﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nodes;

namespace Pathfinder.Heuristics {
    public struct Manhattan : IHeuristic {
        public float EstimateCost(int x, int y, int goalX, int goalY) {
            return Math.Abs(x - goalX) + Math.Abs(y - goalY);
        }
    }
}