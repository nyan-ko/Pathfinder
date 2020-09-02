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
        protected HorizontalDirection RelativeNodeDirection;

        protected const float ACCEPTABLE_RANGE = 0.25F;
        protected const float ACCEPTABLE_RANGE_SQUARED = ACCEPTABLE_RANGE * ACCEPTABLE_RANGE;
        protected static readonly ActionCost IMPOSSIBLE_COST = ActionCost.ImpossibleCost;
        protected const int IMPOSSIBLE_FRAME_COST = -1;
        protected const int FULL_BLOCK_PIXEL_OFFSET_FROM_CORNER = 15;

        public ActionCost SimulateMovement(int previousX, int previousY, ref PlayerProjection player) {
            //var goalLocation = player.position + new PixelPosition(dX * 16, dY * -16);
            var goalLocation = new PixelPosition(previousX * 16, previousY * 16);
            bool standingStill = player.velocity.X == 0;
            bool playerGoingWrongWay = !standingStill && ((RelativeNodeDirection == HorizontalDirection.None) || (player.velocity.X < 0 && RelativeNodeDirection == HorizontalDirection.Right) ||
                (player.velocity.X > 0 && RelativeNodeDirection == HorizontalDirection.Left));

            //if (!player.ValidPosition(new TilePosition(goalLocation), true, RelativeNodeDirection))
            //    return IMPOSSIBLE_COST;

            int turnFrames = 0;

            if (playerGoingWrongWay)
                turnFrames = SimulateProjectionCostToTurnAround(ref player);
            else if (standingStill && RelativeNodeDirection != HorizontalDirection.None)
                player.SetDirection(RelativeNodeDirection);
            
            int frames = SimulateProjectionCostToGoal(ref player, goalLocation);

            return ActionCost.CreateActionCost(turnFrames, frames);
        }

        private int SimulateProjectionCostToTurnAround(ref PlayerProjection player) {
            int frames = 0;
            player.SetDirection(-player.Direction);

            while (!player.IsGoingRightWay) {
                if (player.ShouldUseMidairTurn)
                    player.UpdateMidairTurnAroundMovement();
                else
                    player.UpdateFallingTurnAroundMovement();
                frames++;
            }

            return frames;
        }

        private int SimulateProjectionCostToGoal(ref PlayerProjection player, PixelPosition basePosition) {
            int frames = 0;
            var lastPosition = new PixelPosition(-1, -1);

            while (player.IsOriginIntersectingWithTile(basePosition.X, basePosition.Y)) {
                player = ApplyMovement(player);
                
                if (player.position != lastPosition) {
                    lastPosition = player.position;  // as long as the player is moving and hasn't come to a dead stop (i.e. collided with terrain), we're good
                }
                else {
                    return IMPOSSIBLE_FRAME_COST;
                }

                frames++;
            }

            if (IsPlayerInCorrectRelativePosition(player, basePosition)) {
                var newTilePosition = player.position.ClampToClosestTile();
                dX = (int)(newTilePosition.X - basePosition.X) / 16;
                dY = (int)(newTilePosition.Y - basePosition.Y) / 16;

                return frames;
            }

            return IMPOSSIBLE_FRAME_COST;
        }

        protected abstract bool IsPlayerInCorrectRelativePosition(PlayerProjection player, PixelPosition basePosition);
        protected abstract PlayerProjection ApplyMovement(PlayerProjection player);

        public static BaseMovement[] GetAllMoves() {
            BaseMovement[] movements = new BaseMovement[8];

            movements[0] = new Pillar(1);
            movements[1] = new Ascend(1, -1, HorizontalDirection.Right);
            movements[2] = new Walk(1, HorizontalDirection.Right);
            movements[3] = new Descend(1, 1, HorizontalDirection.Right);
            movements[4] = new Fall(1);
            movements[5] = new Descend(-1, 1, HorizontalDirection.Left);
            movements[6] = new Walk(-1, HorizontalDirection.Left);
            movements[7] = new Ascend(-1, -1, HorizontalDirection.Left);

            return movements;
        }
    }

    public enum HorizontalDirection : sbyte {
        Left = -1, None = 0, Right = 1
    }
}
