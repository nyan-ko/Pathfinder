using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nodes;

namespace Pathfinder.Heuristics {
    public interface IHeuristic {
        float EstimateCost(INode start, INode goal);


    }
}
