using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder.Projections {
    [Flags]
    public enum CollisionType : byte {
        Up = 1, Down = 2, Left = 4, Right = 8
    }
}
