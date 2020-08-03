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
        private int deltaX;
        private int deltaY;

        public Descend(int dX, int dY) {
            deltaX = dX;
            if (dY > 0) {
                dY = 0;
            }
            deltaY = dY;
        }

        public override int dX {
            get => deltaX;
            set => deltaX = value;
        }

        public override int dY {
            get => deltaY;
            set => deltaY = value;
        }

        public override bool Jump => false;
   
        protected override void UpdateTurnAround(ref PlayerProjection player, out int frames) {
            frames = 0;
            player.AdjustRunFieldsForTurningAround(player.direction);
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

                if (player.velocity.Y < player.maxFallSpeed) {
                    player.velocity.Y += player.gravity;
                }
                else if (player.velocity.Y > player.maxFallSpeed) {
                    player.velocity.Y = player.maxFallSpeed;
                }

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
