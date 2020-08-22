using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using Terraria;
using Nodes;
using Pathfinder.Projections;
using Pathfinder.Structs;

namespace Pathfinder.Moves {
    public class Walk : BaseMovement {

        private int xTilePixelOffset;

        public Walk(int deltaX, HorizontalDirection direction) {
            dX = deltaX;
            RelativeNodeDirection = direction;

            xTilePixelOffset = deltaX == 1 ? 0 : 15;
        }

        protected override void UpdateTurnAround(ref PlayerProjection player, out int frames) {
            frames = 0;
            player.AdjustRunFieldsForTurningAround(RelativeNodeDirection);
            while (player.velocity.X < 0) {
                player.UpdateTurnAroundMovement();
                frames++;
            }
        }

        protected override void UpdateMovementTowardsGoal(ref PlayerProjection player, PixelPosition goal, out int frames) {
            frames = -1;
            int goalX = (int)goal.X;
            int goalY = (int)goal.Y;
            float previousDistance = float.MaxValue;
            if (!player.WillTileOriginIntersectWithTile(goalX, goalY)) {
                frames = 0;
                do {
                    player.UpdateHorizontalMovement();
                    float distance = player.TileOriginCenter.Distance(goalX + xTilePixelOffset, goalY + 7);

                    if (distance < previousDistance) {
                        previousDistance = distance;
                    }
                    else {
                        //frames = IMPOSSIBLE_FRAME_COST;
                        return;
                    }

                    frames++;
                }
                while (!player.WillTileOriginIntersectWithTile(goalX, goalY));
            }
        }
    }
}
