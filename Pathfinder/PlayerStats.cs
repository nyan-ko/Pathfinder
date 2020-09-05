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
        public float gravity;
        public float maxRunSpeed;
        public float runAcceleration;
        public float runSlowdown;
        public float accessoryRunSpeed;

        public PlayerStats(Player player) {
            jumpSpeed = Player.jumpSpeed;
            maxFallSpeed = player.maxFallSpeed;
            gravity = Player.defaultGravity;
            maxRunSpeed = player.maxRunSpeed;
            runAcceleration = player.runAcceleration;
            runSlowdown = player.runSlowdown;
            accessoryRunSpeed = player.accRunSpeed;
        }
    }
}
