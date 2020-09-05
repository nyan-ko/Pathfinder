using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TItem = Terraria.Item;

namespace Pathfinder.Projections {
    public class Inventory {
        private int[] blocks;
        private int[] pickaxes;
        private TItem[] inventory;

        public Inventory(Player player) {
            inventory = player.inventory;

            List<int> blocks = new List<int>(4);
            List<int> pickaxes = new List<int>(2);
            List<int> axes = new List<int>(2);

            for (int i = 0; i < inventory.Length; i++) {

            }
        }
    }

    public enum ItemType {
        Block = 1, Pickaxe, Axe, 
    }

    public abstract class Item {
        public ItemType Type { get; private set; }

        public abstract short ItemID { get; protected set; }

        public abstract short Stack { get; protected set; }

        public abstract byte Prefix { get; protected set; }

        protected Item(short id, short stack, byte prefix, ItemType type) {
            ItemID = id;
            Stack = stack;
            Prefix = prefix;
            Type = type;
        }
    }

    public class Block {
        public bool IsPlatform { get; private set; }
        
        public Block()
    }

}
