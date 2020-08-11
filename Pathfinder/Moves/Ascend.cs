using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Nodes;
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
            player.AdjustRunFieldsForTurningAround(RelativeNodeDirection);
            while (player.velocity.X < 0) {
                player.UpdateTurnAround();
                frames++;
            }
        }

        protected override void UpdateMovementTowardsGoal(ref PlayerProjection player, Vector2 goal, out int frames) {
            frames = 0;
            if (player.jumping) {
                player.StartJumping();
            }
            int goalX = (int)(goal.X * 16);
            int goalY = (int)(goal.Y * 16);
            float previousDistance = float.MaxValue;
            while (!player.IsIntersectingWithTile(goalX, goalY)) {
                if (player.jump <= 0) {
                    frames = IMPOSSIBLE_FRAME_COST;
                }

                player.jump--;
                player.UpdateHorizontalMovement();
                float distance = player.Center.Distance
            }

            //frames = 0;

            //player.StartJumping();

            //Vector2 position = player.Center;
            //float previousDistance = float.MaxValue;
            //float distance = position.Distance(goal);
            //while (distance > ACCEPTABLE_RANGE_SQUARED) {
            //    if (player.jump <= 0) {
            //        frames = IMPOSSIBLE_FRAME_COST;
            //        player.Center = position;
            //        return;
            //    }

            //    player.jump--;
            //    player.UpdateHorizontalMovement();
            //    distance = position.Distance(goal);

            //    if (previousDistance > distance) {
            //        previousDistance = distance;
            //    }
            //    else {
            //        frames = IMPOSSIBLE_FRAME_COST;
            //        player.Center = position;
            //        return;
            //    }

            //    frames++;
            //}

            //player.Center = position;
        }
    }
}
