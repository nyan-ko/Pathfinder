using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Microsoft.Xna.Framework;
using Pathfinder.Structs;
using Pathfinder.Projections;
using Nodes;

namespace Pathfinder.Moves {
    public class Descend : BaseMovement {

        public Descend(int deltaX, int deltaY, HorizontalDirection nodeRelativeDirection) {
            dX = deltaX;
            if (dY < 0) {
                dY = 0;
            }
            dY = deltaY;
            RelativeNodeDirection = nodeRelativeDirection;
        }

        protected override bool IsPlayerInCorrectRelativePosition(PlayerProjection player, PixelPosition basePosition) {
            basePosition.X += RelativeNodeDirection == HorizontalDirection.Right ? 0 : FULL_BLOCK_PIXEL_OFFSET_FROM_CORNER;
            return player.IsInCorrectRelativePosition(basePosition, (int)RelativeNodeDirection, 1);
        }

        protected override PlayerProjection ApplyMovement(PlayerProjection player) {
            player.UpdateMovingFallMovement();
            return player;
        }
    }
}
