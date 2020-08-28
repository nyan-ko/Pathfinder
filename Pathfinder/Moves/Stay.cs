using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathfinder.Projections;
using Pathfinder.Structs;

namespace Pathfinder.Moves {
    public class Stay : BaseMovement {

        public Stay() {
            dX = 0;
            dY = 0;
        }

        protected override void UpdateTurnAround(ref PlayerProjection player, out int frames) {
            frames = 0;
            player.SetDirection(-player.Direction);
            while (!player.IsGoingRightWay) {
                player.UpdateStopMovement();
                frames++;
            }
        }

        protected override void UpdateMovementTowardsGoal(ref PlayerProjection player, PixelPosition goal, out int frames) {
            UpdateTurnAround(ref player, out frames);
        }
    }
}
