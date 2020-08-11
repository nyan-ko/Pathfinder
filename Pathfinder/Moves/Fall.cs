using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Nodes;

namespace Pathfinder.Moves {
    public class Fall : BaseMovement {

        public Fall(int deltaY) {
            dX = 0;
            dY = deltaY;
        }

        protected override void UpdateMovementTowardsGoal(ref PlayerProjection player, Vector2 goal, out int frames) {
            throw new NotImplementedException();
        }

        protected override void UpdateTurnAround(ref PlayerProjection player, out int frames) {
            throw new NotImplementedException();
        }
    }
}
