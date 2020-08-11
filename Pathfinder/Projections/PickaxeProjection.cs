using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder.Projections {
    public struct PickaxeProjection {
        public byte MiningSpeed;
        public byte PickaxePower;

        public PickaxeProjection(byte speed, byte power) {
            MiningSpeed = speed;
            PickaxePower = power;
        }
    }
}
