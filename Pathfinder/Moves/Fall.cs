using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Nodes;

namespace Pathfinder.Moves {
    public class Fall : BaseMovement {
        public override int dX {
            get => 0;
            set { }
        }

        public override int dY {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override bool Jump => false;

        protected override void UpdateMovementTowardsGoal(ref PlayerProjection player, Vector2 goal, out int frames) {
            throw new NotImplementedException();
        }

        protected override void UpdateTurnAround(ref PlayerProjection player, out int frames) {
            throw new NotImplementedException();
        }
    }
}
