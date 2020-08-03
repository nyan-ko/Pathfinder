using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Nodes;

namespace Pathfinder.Moves {
    public class Pillar : BaseMovement {
        private int deltaY;

        public override int dX {
            get => 0;
            set { }
        }

        public override int dY {
            get => deltaY;
            set => deltaY = value;
        }

        public override bool Jump => true;

        public Pillar(int dY) {
            deltaY = dY;
        }

        protected override void UpdateMovementTowardsGoal(ref PlayerProjection player, Vector2 goal, out int frames) {
            throw new NotImplementedException();
        }

        protected override void UpdateTurnAround(ref PlayerProjection player, out int frames) {
            throw new NotImplementedException();
        }
    }
}
