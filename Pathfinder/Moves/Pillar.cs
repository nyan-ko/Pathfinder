﻿using System;
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

        protected override void UpdateTurnAround(ref PlayerProjection player, out int frames) {
            frames = 0;
            player.AdjustRunFieldsForTurningAround(-player.lastDirection);
            while (player.velocity.X < 0) {
                player.UpdateTurnAroundMovement();
                frames++;
            }
        }

        protected override void UpdateMovementTowardsGoal(ref PlayerProjection player, PixelPosition goal, out int frames) {
            frames = -1;
            int goalX = (int)goal.X;
            int goalY = (int)goal.Y;
            float previousDistance = float.MaxValue;
            if (!player.WillTileOriginIntersectWithTile(goalX, goalY)) {
                frames = 0;
                do {
                    player.UpdateJumpMovement();
                    float distance = player.TileOriginCenter.Distance(goalX + 7, goalY + 15);

                    if (distance < previousDistance) {
                        previousDistance = distance;
                    }
                    else {
                        //frames = IMPOSSIBLE_FRAME_COST;
                        return;
                    }

                    frames++;
                }
                while (!player.WillTileOriginIntersectWithTile(goalX, goalY));
            }
        }
    }
}
