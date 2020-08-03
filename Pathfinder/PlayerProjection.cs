using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using OTAPI.Tile;
using Pathfinder.Moves;
using Nodes;

namespace Pathfinder {
    public struct PlayerProjection {  // trace on
        public Vector2 position;
        public Vector2 velocity;

        public int width;
        public int height;

        public float maxRunSpeed;
        public float runAcceleration;
        public float runSlowdown;
        public float jumpSpeed;
        public int jump;
        public bool jumping;

        public float maxFallSpeed;
        public float gravity;

        private byte _lastDirection;
        public HorizontalDirection direction;

        public PlayerProjection(Player player) {
            position = player.position;
            velocity = player.velocity;
            width = player.width;
            height = player.height;

            maxRunSpeed = player.maxRunSpeed;
            runAcceleration = player.runAcceleration;
            runSlowdown = player.runSlowdown;
            jumpSpeed = Player.jumpSpeed;
            jump = player.jump;
            jumping = false;

            maxFallSpeed = player.maxFallSpeed;
            gravity = player.gravity;
            _lastDirection = 1;
            direction = HorizontalDirection.None;
        }

        public bool ValidPosition(Vector2 tilePosition, bool canBeInAir) {
            if (tilePosition.X > Main.maxTilesX || tilePosition.Y > Main.maxTilesY) {
                throw new InvalidOperationException("Position passed was likely in pixels instead of tiles.");
            }

            const int MIDDLE_TILE = 0;
            const int SIDE_TILE = 1;

            BitsByte onGround = (byte)(canBeInAir ? 2 : 0);

            int x = (int)tilePosition.X;
            int y = (int)tilePosition.Y;

            bool middleFree = true;
            for (int j = y; j > y - 2; j--) {
                if (Main.tile[x, j]?.active() ?? true) {
                    middleFree = false;
                }
            }

            if (!canBeInAir)
                onGround[MIDDLE_TILE] = Main.tile[x, y - 1]?.active() ?? false;

            if (!middleFree) {
                return false;
            }

            bool sideFree = true;
            for (int j = y; j > y - 2; j--) {
                if (Main.tile[x - 1, j]?.active() ?? true) {
                    sideFree = false;
                }
            }

            if (!canBeInAir)
                onGround[SIDE_TILE] = Main.tile[x - 1, y - 1]?.active() ?? false;

            if (!sideFree) {
                sideFree = true;
                for (int j = y; j > y - 2; j--) {
                    if (Main.tile[x + 1, j]?.active() ?? true) {
                        sideFree = false;
                    }
                }

                if (!onGround[SIDE_TILE] && !canBeInAir) {
                    onGround[SIDE_TILE] = Main.tile[x - 1, y - 1]?.active() ?? false;
                }
            }

            //inAir = !onGround[MIDDLE_TILE] && !onGround[SIDE_TILE];  // used to be ref
            return middleFree && sideFree && (canBeInAir || (onGround[MIDDLE_TILE] && onGround[SIDE_TILE]));
        }

        public void UpdateTurnAround() {
            if (velocity.X < maxRunSpeed) {
                if (velocity.X < runSlowdown) {
                    velocity.X += runSlowdown;
                }
                velocity.X += runAcceleration;
            }
            position += velocity;
        }

        public void UpdateHorizontalMovement() {
            if (velocity.X < maxRunSpeed) {
                velocity.X += runAcceleration;
            }
            position += velocity;
        }

        public void AdjustRunFieldsForTurningAround(HorizontalDirection direction) {
            int multiplier = (int)direction * _lastDirection;
            velocity.X *= multiplier;
            runSlowdown *= multiplier;
        }

        public Vector2 Center {
            get {
                return new Vector2(position.X + (width / 2), position.Y + (height / 2));
            }
            set {
                position = new Vector2(value.X - (width / 2), value.Y - (height / 2));
            }
        }

        public Vector2 Left {
            get {
                return new Vector2(position.X, position.Y + (height / 2));
            }
            set {
                position = new Vector2(value.X, value.Y - (height / 2));
            }
        }

        public Vector2 Right {
            get {
                return new Vector2(position.X + width, position.Y + (height / 2));
            }
            set {
                position = new Vector2(value.X - width, value.Y - (height / 2));
            }
        }

        public Vector2 Top {
            get {
                return new Vector2(position.X + (width / 2), position.Y);
            }
            set {
                position = new Vector2(value.X - (width / 2), value.Y);
            }
        }

        public Vector2 TopLeft {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        public Vector2 TopRight {
            get {
                return new Vector2(position.X + width, position.Y);
            }
            set {
                position = new Vector2(value.X - width, value.Y);
            }
        }

        public Vector2 Bottom {
            get {
                return new Vector2(position.X + (width / 2), position.Y + height);
            }
            set {
                position = new Vector2(value.X - (width / 2), value.Y - height);
            }
        }

        public Vector2 BottomLeft {
            get {
                return new Vector2(position.X, position.Y + height);
            }
            set {
                position = new Vector2(value.X, value.Y - height);
            }
        }

        public Vector2 BottomRight {
            get {
                return new Vector2(position.X + width, position.Y + height);
            }
            set {
                position = new Vector2(value.X - width, value.Y - height);
            }
        }
    }
}
