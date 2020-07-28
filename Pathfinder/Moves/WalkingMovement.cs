using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using Terraria;
using Nodes;

namespace Pathfinder.Moves {
    public class WalkingMovement : BaseMovement {
        private int deltaX;
        private Direction direction;

        public override int dX => deltaX;

        public override int dY => 0;

        public WalkingMovement(int dX, Direction relativeDirection) {
            deltaX = dX;
            direction = relativeDirection;

        }

        protected override float CalculateCost(Player player, AbstractPathNode currentNode) {
            int     frames                  = 0;
            int     multiplier              = direction == Direction.Left ? -1 : 1;  // allows movement calculations to work in both directions
            float   projectedPosition       = player.position.X;
            int     goalLocation            = (int)(projectedPosition + dX * 16);
            float   projectedVelocity       = player.velocity.X;

            float   maxRunSpeed             = player.maxRunSpeed * multiplier;
            float   runAcceleration         = player.runAcceleration * multiplier;

            bool    standingStill           = projectedVelocity == 0;
            bool    playerGoingWrongWay     = !standingStill && (projectedVelocity < 0 && direction == Direction.Right ||
                projectedVelocity > 0 && direction == Direction.Left);

            if (playerGoingWrongWay) {  // calculate the number of frames necessary for player to turn around and reset velocity

                projectedVelocity *= multiplier;
                float   runTurnSpeed        = player.runSlowdown * multiplier;

                while (projectedVelocity < 0) {
                    if (projectedVelocity < runTurnSpeed) {
                        projectedVelocity += runTurnSpeed;
                    }
                    projectedVelocity += runAcceleration;
                    projectedPosition += projectedVelocity;
                    frames++;
                }
            }

            while (Math.Abs(projectedPosition - goalLocation) < ACCEPTABLE_RANGE) {
                if (projectedVelocity < maxRunSpeed) {
                    projectedVelocity += runAcceleration;
                }
                projectedPosition += projectedVelocity;
                frames++;
            }

            return frames;
        }

        protected override void UpdateVelocity() {
            throw new NotImplementedException();
        }
    }
}
