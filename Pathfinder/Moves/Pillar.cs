using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Nodes;

namespace Pathfinder.Moves {
    public class Pillar : BaseMovement {

        public Pillar(int deltaY) {
            dY = deltaY;
        }

        protected override void UpdateMovementTowardsGoal(ref PlayerProjection player, Vector2 goal, out int frames) {
            frames = 0;

            Vector2 position = player.Center;

        }

        protected override void UpdateTurnAround(ref PlayerProjection player, out int frames) {
            frames = 0;
            player.AdjustRunFieldsForTurningAround(-PathfindingUtils.RecklessAbsolute(player.lastDirection));
            while (player.velocity.X < 0) {
                player.UpdateTurnAround();
                frames++;
            }
        }
    }
}
