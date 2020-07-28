using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Pathfinder.Moves;
using Nodes;

namespace Pathfinder {
    public struct PlayerProjection {  // trace on
        public Vector2 Position { get; private set; }
        public Vector2 Velocity { get; private set; }


        public T ApplyMovements<T>(BaseMovement[] movements) where T : AbstractPathNode {
            T[] openNodes = new T[8];


        }
    }
}
