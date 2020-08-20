using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathfinder.Projections;
using Terraria;
using Pathfinder.Structs;
using Microsoft.Xna.Framework;
using Nodes;

namespace Pathfinder.Moves {
    public abstract class BaseMovement {
        public int dX { get; protected set; }
        public int dY { get; protected set; }
        protected bool Jump;
        protected HorizontalDirection RelativeNodeDirection;

        protected const float ACCEPTABLE_RANGE = 0.25F;
        protected const float ACCEPTABLE_RANGE_SQUARED = ACCEPTABLE_RANGE * ACCEPTABLE_RANGE;
        protected static readonly ActionCost IMPOSSIBLE_COST = ActionCost.ImpossibleCost;
        protected const int IMPOSSIBLE_FRAME_COST = -1;

        public ActionCost CalculateCost(ref PlayerProjection player) {
            var goalLocation = player.position.ClampToClosestTile() + new PixelPosition(dX * 16, dY * -16);

            bool standingStill = player.velocity == PixelPosition.Zero;
            bool playerGoingWrongWay = !standingStill && (player.velocity.X < 0 && player.lastDirection == 1 ||
                player.velocity.X > 0 && player.lastDirection == -1);

            if (!player.ValidPosition(new TilePosition(goalLocation), true, RelativeNodeDirection))
                return IMPOSSIBLE_COST;

            int turnFrames = 0;

            if (playerGoingWrongWay)
                UpdateTurnAround(ref player, out turnFrames);

            UpdateMovementTowardsGoal(ref player, goalLocation, out int frames);

            return ActionCost.CreateActionCost(turnFrames, frames, Jump);
        }

        protected abstract void UpdateTurnAround(ref PlayerProjection player, out int frames);
        protected abstract void UpdateMovementTowardsGoal(ref PlayerProjection player, PixelPosition goal, out int frames);

        public static BaseMovement[] GetAllMoves() {
            BaseMovement[] movements = new BaseMovement[8];

            movements[0] = new Pillar(1);
            movements[1] = new Ascend(1, 1, HorizontalDirection.Right);
            movements[2] = new Walk(1, HorizontalDirection.Right);
            movements[3] = new Descend(1, -1, HorizontalDirection.Right);
            movements[4] = new Fall(-1);
            movements[5] = new Descend(-1, -1, HorizontalDirection.Left);
            movements[6] = new Walk(-1, HorizontalDirection.Left);
            movements[7] = new Ascend(-1, 1, HorizontalDirection.Left);

            return movements;
        }
    }

    public enum HorizontalDirection : sbyte {
        Left = -1, None, Right = 1
    }
}
