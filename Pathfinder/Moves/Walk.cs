using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using Terraria;
using Nodes;
using Pathfinder.Projections;
using Pathfinder.Structs;

namespace Pathfinder.Moves {
    public class Walk : BaseMovement {

        public Walk(int deltaX, HorizontalDirection direction) {
            dX = deltaX;
            RelativeNodeDirection = direction;
        }

        protected override bool IsPlayerInCorrectRelativePosition(PlayerProjection player, PixelPosition basePosition) {
            basePosition.X += RelativeNodeDirection == HorizontalDirection.Right ? 0 : FULL_BLOCK_PIXEL_OFFSET_FROM_CORNER;
            return player.IsInCorrectRelativePosition(basePosition, (int)RelativeNodeDirection, 0);
        }

        protected override PlayerProjection ApplyMovement(PlayerProjection player) {
            player.UpdateMovingFallMovement();
            return player;
        }
    }
}
