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

namespace Pathfinder.Projections {
    public struct PlayerProjection {  // trace on
        public Vector2 position;
        public Vector2 velocity;  // TODO make this private and just have it be a get
        public int width;
        public int height;

        public int jump;
        public bool jumping;
        public float gravity;
        public bool onGround;
        public float maxRunSpeed;
        public float runAcceleration;
        public float runSlowdown;
        public float accessoryRunSpeed;

        public sbyte lastDirection;
        public PickaxeProjection[] pickaxes;
        private Rectangle boundingBox;
        private PlayerStats _stats;

        public PlayerProjection(Player player, PlayerStats stats) {
            position = player.position;
            velocity = player.velocity;
            width = player.width;
            height = player.height;

            jump = player.jump;
            jumping = false;
            gravity = player.gravity;
            onGround = true;
            lastDirection = 1;
            maxRunSpeed = player.maxRunSpeed;
            runAcceleration = player.runAcceleration;
            runSlowdown = player.runSlowdown;
            accessoryRunSpeed = player.accRunSpeed;

            boundingBox = new Rectangle((int)position.X, (int)position.Y, width - 1, height - 1);
            _stats = stats;
            pickaxes = new PickaxeProjection[1];
            ScanInventory();
        }

        private void ScanInventory() {
            
        }

        public bool ValidPosition(Vector2 tilePosition, bool canBeInAir, HorizontalDirection playerDirectionRelativeToBlock) {
            if (tilePosition.X > Main.maxTilesX || tilePosition.Y > Main.maxTilesY) {
                throw new InvalidOperationException("Position passed was likely in pixels instead of tiles.");
            }

            int offset = (int)playerDirectionRelativeToBlock;

            for (int x = (int)(tilePosition.X + offset); x != tilePosition.X; x -= offset) {
                for (int y = (int)tilePosition.Y; y < tilePosition.Y + 3; y++) {
                    if (!PathfindingUtils.IsTileAir(x, y)) {
                        return false;
                    }
                }
            }

            return true;
        }

        public void UpdateTurnAround() {
            if (velocity.X < runSlowdown) {
                velocity.X += runSlowdown;
            }

            UpdatePosition();
        }

        public void UpdateHorizontalMovement() {
            if (velocity.X < maxRunSpeed) {
                velocity.X += runAcceleration;
            }
            else if (velocity.X < accessoryRunSpeed && velocity.Y == 0) {
                velocity.X += runAcceleration * 0.2F;
            }
        }

        public void UpdateFallMovement() {
            if (velocity.Y < _stats.maxFallSpeed) {
                velocity.Y += gravity;
            }
            else if (velocity.Y > _stats.maxFallSpeed) {
                velocity.Y = _stats.maxFallSpeed;
            }
        }

        public void StartJumping() {
            jumping = true;
            velocity.Y = -_stats.jumpSpeed;
        }

        public void UpdatePositionInWorld() {
            velocity = Collision.TileCollision(position, velocity, width, height);
            position += velocity;
        }

        public bool UpdatePositionInWorld(out Vector2 velocité) {
            // this whole method is taken from Collision.TileCollision()
            int lowerXBound = (int)(position.X / 16) - 1;
            int lowerYBound = (int)(position.Y / 16) - 1;
            int upperXBound = (int)((position.X + width) / 16) + 2;
            int upperYBound = (int)((position.Y + height) / 16) + 2;
            int xIntersectionLastX = -1;
            int xIntersectionLastY = -1;
            int yIntersectionLastX = -1;
            int yIntersectionLastY = -1;
            velocité = velocity;
            for (int x = lowerXBound; x < upperXBound; ++x) {
                for (int y = lowerYBound; y < upperYBound; ++y) {
                    if (!PathfindingUtils.IsTileAir(x, y)) {
                        int pX = x * 16;
                        int pY = y * 16;

                        if (IsIntersectingWithTile(pX, pY)) {
                            var tile = Main.tile[x, y];

                            if (position.Y + height <= pY) {
                                yIntersectionLastX = x;
                                yIntersectionLastY = y;
                                if (yIntersectionLastX != xIntersectionLastX) {
                                    velocité.Y = pY - (position.Y + height);
                                }
                            }
                            else if (position.X + width <= pX && !Main.tileSolidTop[tile.type]) {
                                if (x >= 1 && Main.tile[x - 1, y] == null) {
                                    Main.tile[x - 1, y] = new Tile();
                                }
                                if (x < 1 || (Main.tile[x - 1, y].slope() != 2 && Main.tile[x - 1, y].slope() != 4)) {
                                    xIntersectionLastX = x;
                                    xIntersectionLastY = y;
                                    if (xIntersectionLastY != yIntersectionLastY) {
                                        velocité.X = pX - (position.X + width);
                                    }
                                    if (yIntersectionLastX == xIntersectionLastX) {
                                        velocité.Y = velocity.Y;
                                    }
                                }
                            }
                            else if (position.X >= pX + 16 && !Main.tileSolidTop[tile.type]) {
                                if (Main.tile[x + 1, y] == null) {
                                    Main.tile[x + 1, y] = new Tile();
                                }
                                if (Main.tile[x + 1, y].slope() != 1 && Main.tile[x + 1, y].slope() != 3) {
                                    xIntersectionLastX = x;
                                    xIntersectionLastY = y;
                                    if (xIntersectionLastY != yIntersectionLastY) {
                                        velocité.X = pX + 16f - position.X;
                                    }
                                    if (yIntersectionLastX == xIntersectionLastX) {
                                        velocité.Y = velocity.Y;
                                    }
                                }
                            }
                            else if (position.Y >= pY + 16 && !Main.tileSolidTop[tile.type]) {
                                yIntersectionLastX = x;
                                yIntersectionLastY = y;
                                velocité.Y = pY + (16 - position.Y);
                                if (yIntersectionLastY == xIntersectionLastY) {
                                    velocité.X = velocity.X;
                                }
                            }
                        }
                    }
                }
            }
            return velocité != velocity;
        }

        private CollisionType DetermineCollisionType(int tilePixelX, int tilePixelY, ITile tile) {
            if (position.Y + height <= tilePixelY)
                return CollisionType.Bottom;
            if (position.X + width <= tilePixelX && !Main.tileSolidTop[tile.type])
                return CollisionType.Right;
            if (position.X >= tilePixelX + 16 && !Main.tileSolidTop[tile.type])
                return CollisionType.Left;
            if (position.Y >= tilePixelY + 16 && !Main.tileSolidTop[tile.type])
                return CollisionType.Top;
            return CollisionType.None;
        }

        public bool IsIntersectingWithTile(int tilePixelX, int tilePixelY) {
            int projectedX = (int)(position.X + velocity.X);
            int projectedY = (int)(position.Y + velocity.Y);
            return projectedX < tilePixelX + 16 &&
                projectedX + width > tilePixelX &&
                projectedY < tilePixelY + 16 &&
                projectedY + height > tilePixelY;
        }

        public void AdjustRunFieldsForTurningAround(HorizontalDirection direction) => AdjustRunFieldsForTurningAround((int)direction);

        public void AdjustRunFieldsForTurningAround(int direction) {
            int multiplier = direction * lastDirection;  
            velocity.X *= multiplier;
            runSlowdown *= multiplier;
            runAcceleration *= multiplier;
            maxRunSpeed *= multiplier;
            accessoryRunSpeed *= multiplier;

            lastDirection = (sbyte)direction; 
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
