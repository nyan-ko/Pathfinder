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

        public override int dX {
            get => deltaX;
            set => deltaX = value;
        }

        public override int dY {
            get => 0;
            set { }
        }

        public override bool Jump => false;

        public Walk(int dX) {
            deltaX = dX;
        }

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
            int goalLocation = (int)goal.X;
            while (Math.Abs(player.position.X - goalLocation) > ACCEPTABLE_RANGE) {
                player.UpdateHorizontalMovement();
                frames++;
            }
        }
    }
}
