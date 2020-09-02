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
    public class Pillar : BaseMovement {

        public Pillar(int deltaY) {
            dY = deltaY;
        }

        protected override bool IsPlayerInCorrectRelativePosition(PlayerProjection player, PixelPosition basePosition) {
            basePosition.Y += FULL_BLOCK_PIXEL_OFFSET_FROM_CORNER;
            return player.IsInCorrectRelativePosition(basePosition, 0, -1);
        }

        protected override PlayerProjection ApplyMovement(PlayerProjection player) {
            player.UpdateJumpMovement();
            return player;
        }
    }
}
