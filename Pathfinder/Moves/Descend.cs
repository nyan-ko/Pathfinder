using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Microsoft.Xna.Framework;
using Nodes;

namespace Pathfinder.Moves {
    public class Descend : BaseMovement {

        public Descend(int deltaX, int deltaY, HorizontalDirection nodeRelativeDirection) {
            dX = deltaX;
            if (dY > 0) {
                dY = 0;
            }
            dY = deltaY;
            RelativeNodeDirection = nodeRelativeDirection;
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
            Vector2 position = player.Center;
            float previousDistance = float.MaxValue;
            float distance = position.Distance(goal);
            while (distance > ACCEPTABLE_RANGE_SQUARED) {
                player.UpdateHorizontalMovement(); 

                distance = position.Distance(goal);

                if (previousDistance > distance) {
                    previousDistance = distance;
                }
                else {
                    frames = IMPOSSIBLE_FRAME_COST;
                    player.Center = position;
                    return;
                }

                frames++;
            }
            player.Center = position;
        }
    }
}
