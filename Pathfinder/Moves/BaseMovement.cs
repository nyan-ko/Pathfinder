using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Microsoft.Xna.Framework;
using Nodes;

namespace Pathfinder.Moves {
    public abstract class BaseMovement {
        public abstract int dX { get; }
        public abstract int dY { get; }

        protected const float ACCEPTABLE_RANGE = 0.25F;
        protected const float ACCEPTABLE_RANGE_SQUARED = ACCEPTABLE_RANGE * ACCEPTABLE_RANGE;

        //public float ApplyTo(Player player) {
        //    float cost = CalculateMovementCost(player, null);
        //    UpdateVelocity();
        //    return cost;
        //}

        protected abstract float CalculateCost(ref PlayerProjection player, AbstractPathNode node);
        //protected abstract void UpdateVelocity();
    }

    public enum HorizontalDirection {
        Left, Right, None
    }
}
