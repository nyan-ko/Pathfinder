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
        public abstract int dX { get; set; }
        public abstract int dY { get; set; }
        public abstract bool Jump { get; }

        protected const float ACCEPTABLE_RANGE = 0.25F;
        protected const float ACCEPTABLE_RANGE_SQUARED = ACCEPTABLE_RANGE * ACCEPTABLE_RANGE;
        protected static readonly ActionCost IMPOSSIBLE_COST = ActionCost.ImpossibleCost;
        protected const int IMPOSSIBLE_FRAME_COST = int.MaxValue;

        //public float ApplyTo(Player player) {
        //    float cost = CalculateMovementCost(player, null);
        //    UpdateVelocity();
        //    return cost;
        //}

        public ActionCost CalculateCost(ref PlayerProjection player) {
            Vector2 goalLocation = player.Center + new Vector2(dX * 16, dY * 16);

            bool standingStill = player.velocity == Vector2.Zero;
            bool playerGoingWrongWay = !standingStill && (player.velocity.X < 0 && player.direction == HorizontalDirection.Right ||
                player.velocity.X > 0 && player.direction == HorizontalDirection.Left);

            if (!player.ValidPosition(goalLocation / 16f, true))
                return IMPOSSIBLE_COST;

            int turnFrames = 0;

            if (playerGoingWrongWay)
                UpdateTurnAround(ref player, out turnFrames);

            UpdateMovementTowardsGoal(ref player, goalLocation, out int frames);

            return new ActionCost(turnFrames, frames, Jump);
        }

        protected abstract void UpdateTurnAround(ref PlayerProjection player, out int frames);
        protected abstract void UpdateMovementTowardsGoal(ref PlayerProjection player, Vector2 goal, out int frames);

        public static BaseMovement[] GetAllMoves() {
            BaseMovement[] movements = new BaseMovement[8];

            movements[0] = new Pillar(1);
            movements[1] = new Ascend(1, 1);
            movements[2] = new Walk(1);
            movements[3] = new Descend(1, -1);
            movements[4] = new Fall();
            movements[5] = new Descend(-1, -1);
            movements[6] = new Walk(-1);
            movements[7] = new Ascend(-1, 1);

            return movements;
        }
    }

    public enum HorizontalDirection {
        Left = -1, None, Right = 1
    }
}
