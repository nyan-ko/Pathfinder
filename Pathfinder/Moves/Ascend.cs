using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Nodes;
using Terraria;

namespace Pathfinder.Moves {
    public class Ascend : BaseMovement {
        private int deltaX;
        private uint deltaY;
        private HorizontalDirection direction;

        public override int dX => deltaX;

        public override int dY => (int)deltaY;

        public Ascend(int dX, uint dY, HorizontalDirection initialDirection) {
            deltaX = dX;
            deltaY = dY;
            direction = initialDirection;
        }

        protected override float CalculateCost(ref PlayerProjection player, AbstractPathNode node) {
            //copied from Walk.CalculateCost

            int     frames = 0;
            bool    headingLeft = direction == HorizontalDirection.Left;
            int     multiplier = headingLeft ? -1 : 1;  // allows movement calculations to work in both directions
            Vector2   projectedPosition = headingLeft ? player.TopLeft : player.TopRight;
            Vector2 goalLocation = projectedPosition + new Vector2(dX * 16, 0);
            Vector2 projectedVelocity = player.velocity;

            float maxRunSpeed = player.maxRunSpeed * multiplier;
            float runAcceleration = player.runAcceleration * multiplier;

            bool standingStill = projectedVelocity == Vector2.Zero;
            bool playerGoingWrongWay = !standingStill && (projectedVelocity.X < 0 && direction == HorizontalDirection.Right ||
                projectedVelocity.X > 0 && direction == HorizontalDirection.Left);

            var tile = Main.tile[(int)(goalLocation.X / 16), (int)(goalLocation.Y / 16)];
            if (tile != null || !tile.active()) {
                return -1;
            }

            if (playerGoingWrongWay) {  // calculate the number of frames necessary for player to turn around and reset velocity

                projectedVelocity.X *= multiplier;
                float runTurnSpeed = player.runSlowdown * multiplier;

                while (projectedVelocity.X < 0) {
                    if (projectedVelocity.X < runTurnSpeed) {
                        projectedVelocity.X += runTurnSpeed;
                    }
                    projectedVelocity.X += runAcceleration;
                    projectedPosition += projectedVelocity;
                    frames++;
                }
            }

            projectedVelocity.Y = -player.jumpSpeed;

            while (projectedPosition.Distance(goalLocation) > ACCEPTABLE_RANGE_SQUARED) {
                if (projectedVelocity.X < maxRunSpeed) {
                    projectedVelocity.X += runAcceleration;
                }
                projectedPosition += projectedVelocity;
                frames++;
            }

            return frames;
        }
    }
}
