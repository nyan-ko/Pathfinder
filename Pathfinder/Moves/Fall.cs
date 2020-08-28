using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Nodes;
using Pathfinder.Projections;
using Pathfinder.Structs;

namespace Pathfinder.Moves {
    public class Fall : BaseMovement {

        public Fall(int deltaY) {
            dX = 0;
            dY = deltaY;
        }

        protected override void UpdateTurnAround(ref PlayerProjection player, out int frames) {
            frames = 0;
            player.SetDirection(-player.Direction);
            while (!player.IsGoingRightWay) {
                player.UpdateTurnAroundMovement();
                frames++;
            }
        }

        protected override void UpdateMovementTowardsGoal(ref PlayerProjection player, PixelPosition goal, out int frames) {
            frames = 0;
            float previousDistance = float.MaxValue;

            while (!player.IsTileOriginIntersectingWithTile(goal.X, goal.Y)) {
                player.UpdateFallMovement();
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
