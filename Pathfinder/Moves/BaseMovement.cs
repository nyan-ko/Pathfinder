using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Nodes;

namespace Pathfinder.Moves {
    public abstract class BaseMovement {
        public abstract int dX { get; }
        public abstract int dY { get; }

        protected const float ACCEPTABLE_RANGE = 0.25F;

        //public float ApplyTo(Player player) {
        //    float cost = CalculateMovementCost(player, null);
        //    UpdateVelocity();
        //    return cost;
        //}

        protected abstract float CalculateCost(Player player, AbstractPathNode node);
        protected abstract void UpdateVelocity();
    }

    public enum Direction {
        Left, Right, None
    }
}
