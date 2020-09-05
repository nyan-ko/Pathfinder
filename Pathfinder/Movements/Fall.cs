using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Nodes;
using Pathfinder.Projections;
using Pathfinder.Structs;

namespace Pathfinder.Moves {
    public class Fall : BaseMovement {

        public Fall(int deltaY) {
            dY = deltaY;
        }

        protected override bool IsPlayerInCorrectRelativePosition(PlayerProjection player, PixelPosition basePosition) {
            return player.IsInCorrectRelativePosition(basePosition, 0, 1);
        }

        protected override PlayerProjection ApplyMovement(PlayerProjection player) {
            player.UpdateFallMovement();
            return player;
        }
    }
}
