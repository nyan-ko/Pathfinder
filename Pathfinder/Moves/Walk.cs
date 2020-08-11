using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using OTAPI;
using Terraria;
using Nodes;

namespace Pathfinder.Moves {
    public class Walk : BaseMovement {

        public Walk(int deltaX, HorizontalDirection direction) {
            dX = deltaX;
            RelativeNodeDirection = direction;
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
            int goalLocation = (int)goal.X;
            while (Math.Abs(player.position.X - goalLocation) > ACCEPTABLE_RANGE) {
                player.UpdateHorizontalMovement();
                frames++;
            }
        }
    }
}
