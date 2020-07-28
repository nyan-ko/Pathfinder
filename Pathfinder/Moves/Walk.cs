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
        private int deltaX;
        private HorizontalDirection direction;

        public override int dX => deltaX;

        public override int dY => 0;

        public Walk(int dX, HorizontalDirection relativeDirection) {
            deltaX = dX;
            direction = relativeDirection;

        }

        protected override float CalculateCost(ref PlayerProjection player, AbstractPathNode currentNode) {
            int     frames                  = 0;
            bool    headingLeft             = direction == HorizontalDirection.Left;
            int     multiplier              = headingLeft ? -1 : 1;  // allows movement calculations to work in both directions
            float   projectedPosition       = headingLeft ? player.TopLeft.X : player.TopRight.X;
            int     goalLocation            = (int)(projectedPosition + dX * 16);
            float   projectedVelocity       = player.velocity.X;

            float   maxRunSpeed             = player.maxRunSpeed * multiplier;
            float   runAcceleration         = player.runAcceleration * multiplier;

            bool    standingStill           = projectedVelocity == 0;
            bool    playerGoingWrongWay     = !standingStill && (projectedVelocity < 0 && direction == HorizontalDirection.Right ||
                projectedVelocity > 0 && direction == HorizontalDirection.Left);

            bool inAir = false;  // unused
            if (!player.ValidPosition(new Vector2(goalLocation / 16, player.position.Y / 16), ref inAir))
                return float.MaxValue;

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

            while (Math.Abs(projectedPosition - goalLocation) > ACCEPTABLE_RANGE) {
                if (projectedVelocity < maxRunSpeed) {
                    projectedVelocity += runAcceleration;
                }
                projectedPosition += projectedVelocity;
                frames++;
            }

            return frames;  // return value will likely be changed in the future to more accurately reflect the cost of this movement
        }
    }
}
