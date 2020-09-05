using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder.Projections {
    public struct MiningTool {
        public byte MiningSpeed;
        public byte PickaxePower;

        public MiningTool(byte speed, byte power) {
            MiningSpeed = speed;
            PickaxePower = power;
        }
    }
}
