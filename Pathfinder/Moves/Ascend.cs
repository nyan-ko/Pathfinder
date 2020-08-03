﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Nodes;
using Terraria;

namespace Pathfinder.Moves {
    public class Ascend : BaseMovement {
        private int deltaX;
        private int deltaY;

        public override int dX {
            get => deltaX;
            set => deltaX = value;
        }

        public override int dY {
            get => deltaY;
            set => deltaY = value;
        }

        public override bool Jump => true;

        public Ascend(int dX, int dY) {
            deltaX = dX;
            if (dY < 0) {
                dY = 0;
            }
            deltaY = dY;
        }

        protected override void UpdateTurnAround(ref PlayerProjection player, out int frames) {
            frames = 0;
            player.AdjustRunFieldsForTurningAround(player.direction);
            while (player.velocity.X < 0) {
                player.UpdateTurnAround();
                frames++;
            }
        }

        protected override void UpdateMovementTowardsGoal(ref PlayerProjection player, Vector2 goal, out int frames) {
            frames = 0;

            player.velocity.Y = -player.jumpSpeed;
            player.jumping = true;

            Vector2 position = player.Center;
            float previousDistance = float.MaxValue;
            float distance = position.Distance(goal);
            while (distance > ACCEPTABLE_RANGE_SQUARED) {

                if (player.jump <= 0) {
                    frames = IMPOSSIBLE_FRAME_COST;
                    player.Center = position;
                    return;
                }
                player.jump--;

                player.UpdateHorizontalMovement();
                distance = position.Distance(goal);

                if (previousDistance > distance) {
                    previousDistance = distance;
                }
                else {
                    frames = IMPOSSIBLE_FRAME_COST;
                    player.Center = position;
                    return;
                }

                frames++;
            }

            player.Center = position;
        }
    }
}
