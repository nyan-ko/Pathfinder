using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Pathfinder {
    public class PlayerStats {

        public float jumpSpeed;
        public float maxFallSpeed;

        public PlayerStats(Player player) {

            jumpSpeed = Player.jumpSpeed;
            maxFallSpeed = player.maxFallSpeed;
        }
    }
}
