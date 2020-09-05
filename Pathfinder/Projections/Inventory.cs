using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Pathfinder.Projections {
    public class Inventory {
        private int[] blocks;
        private int[] pickaxes;
        private Item[] inventory;

        public Inventory(Player player) {
            inventory = player.inventory;
        }
    }
}
