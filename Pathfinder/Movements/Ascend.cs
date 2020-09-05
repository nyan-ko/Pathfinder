using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using Nodes;
using Pathfinder.Structs;
using Pathfinder.Projections;
using Terraria;

namespace Pathfinder.Moves {
    public class Ascend : BaseMovement {

        public Ascend(int deltaX, int deltaY, HorizontalDirection direction) {
            dX = deltaX;
            if (dY > 0) {
                dY = 0;
            }
            dY = deltaY;
            RelativeNodeDirection = direction;
        }

        protected override bool IsPlayerInCorrectRelativePosition(PlayerProjection player, PixelPosition basePosition) {
            basePosition.X += RelativeNodeDirection == HorizontalDirection.Right ? 0 : FULL_BLOCK_PIXEL_OFFSET_FROM_CORNER;
            basePosition.Y += FULL_BLOCK_PIXEL_OFFSET_FROM_CORNER;
            return player.IsInCorrectRelativePosition(basePosition, (int)RelativeNodeDirection, -1);
        }

        protected override PlayerProjection ApplyMovement(PlayerProjection player) {
            player.UpdateMovingJumpMovement();
            return player;
        }
    }
}
