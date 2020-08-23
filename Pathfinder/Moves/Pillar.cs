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
    public class Pillar : BaseMovement {

        public Pillar(int deltaY) {
            dY = deltaY;
        }

        protected override void UpdateTurnAround(ref PlayerProjection player, out int frames) {
            frames = 0;
            player.AdjustRunFieldsForTurningAround(-player.Direction);
            while (player.velocity.X < 0) {
                player.UpdateTurnAroundMovement();
                frames++;
            }
        }

        protected override void UpdateMovementTowardsGoal(ref PlayerProjection player, PixelPosition goal, out int frames) {
            frames = 0;
            int goalX = (int)goal.X;
            int goalY = (int)goal.Y;
            float previousDistance = float.MaxValue;
            while (!player.WillTileOriginIntersectWithTile(goalX, goalY)) {
                player.UpdateJumpMovement();
                float distance = player.position.Distance(goalX, goalY);

                if (distance < previousDistance) {
                    previousDistance = distance;
                }
                else if (!player.WillTileOriginIntersectWithTile(goalX, goalY)) {
                    frames = IMPOSSIBLE_FRAME_COST;
                    return;
                }

                frames++;
            }
        }
    }
}
