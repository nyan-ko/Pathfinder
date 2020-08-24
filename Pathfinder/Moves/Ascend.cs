using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using Nodes;
using Pathfinder.Structs;
using Pathfinder.Projections;
using Terraria;

namespace Pathfinder.Moves {
    public class Ascend : BaseMovement {
        private int xTilePixelOffset;

        public Ascend(int deltaX, int deltaY, HorizontalDirection direction) {
            dX = deltaX;
            if (dY < 0) {
                dY = 0;
            }
            dY = deltaY;
            RelativeNodeDirection = direction;

            xTilePixelOffset = deltaX == 1 ? 0 : 15;
        }

        protected override void UpdateTurnAround(ref PlayerProjection player, out int frames) {
            frames = 0;
            player.AdjustVelocityForTurningAround(RelativeNodeDirection);
            while (player.velocity.X < 0) {
                player.UpdateTurnAroundMovement();
                frames++;
            }
        }

        protected override void UpdateMovementTowardsGoal(ref PlayerProjection player, PixelPosition goal, out int frames) {
            frames = 0;
            float previousDistance = float.MaxValue;

            while (!player.IsTileOriginIntersectingWithTile(goal.X, goal.Y)) {
                player.UpdateMovingJumpMovement();
                float distance = player.position.Distance(goal.X, goal.Y);
                    
                if (distance < previousDistance) {
                    previousDistance = distance;
                }
                else if (!player.WillTileOriginIntersectWithTile(goal.X, goal.Y)) {
                    frames = IMPOSSIBLE_FRAME_COST;
                    return;
                }

                frames++;
            }
        }
    }
}
