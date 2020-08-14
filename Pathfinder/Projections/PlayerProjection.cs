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
using Pathfinder.Structs;
using Nodes;

namespace Pathfinder.Projections {
    public struct PlayerProjection {  // trace on
        public PixelPosition position;
        public PixelPosition velocity;  // TODO make this private and just have it be a get
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

        public PlayerProjection(Player player) {
            position = new PixelPosition(player.position);
            velocity = new PixelPosition(player.velocity);
            width = player.width;
            height = player.height;

            jump = player.jump;
            jumping = false;
            gravity = Player.defaultGravity;
            onGround = true;
            lastDirection = 1;
            maxRunSpeed = player.maxRunSpeed;
            runAcceleration = player.runAcceleration;
            runSlowdown = player.runSlowdown;
            accessoryRunSpeed = player.accRunSpeed;

            boundingBox = new Rectangle((int)position.X, (int)position.Y, width - 1, height - 1);
            _stats = new PlayerStats(player);
            pickaxes = new PickaxeProjection[1];
            ScanInventory();
        }

        private void ScanInventory() {
            
        }

        public bool ValidPosition(TilePosition tilePosition, bool canBeInAir, HorizontalDirection playerDirectionRelativeToBlock) {
            int offset = (int)playerDirectionRelativeToBlock;

            for (int x = tilePosition.X + offset; x != tilePosition.X; x -= offset) {
                for (int y = tilePosition.Y; y < tilePosition.Y + 3; y++) {
                    if (!PathfindingUtils.IsTileAir(x, y)) {
                        return false;
                    }
                }
            }

            return true;
        }

        public void UpdateTurnAroundMovement() {
            IncrementTurnAroundMovement();
            UpdatePositionInWorld();
        }

        public void UpdateHorizontalMovement() {
            IncrementHorizontalMovement();
            UpdatePositionInWorld();
        }

        public void UpdateFallMovement() {
            IncrementFallMovement();
            UpdatePositionInWorld();
        }

        public void UpdateMovingFallMovement() {
            IncrementFallMovement();
            IncrementHorizontalMovement();
            UpdatePositionInWorld();
        }

        public void UpdateJumpMovement() {
            IncrementJumpMovement();
            UpdatePositionInWorld();
        }

        public void UpdateMovingJumpMovement() {
            IncrementJumpMovement();
            IncrementHorizontalMovement();
            UpdatePositionInWorld();
        }

        #region Increments
        private void IncrementHorizontalMovement() {
            if (velocity.X < maxRunSpeed) {
                velocity.X += runAcceleration;
            }
            else if (velocity.X < accessoryRunSpeed && velocity.Y == 0) {
                velocity.X += runAcceleration * 0.2F;
            }
        }

        private void IncrementJumpMovement() {
            if (jump > 0) {
                if (velocity.Y == 0) {
                    jump = 0;
                }
                else {
                    velocity.Y = -_stats.jumpSpeed;
                }
            }
            else {
                IncrementFallMovement();
            }
        }

        private void IncrementFallMovement() {
            if (velocity.Y < _stats.maxFallSpeed) {
                velocity.Y += gravity;
            }
            else if (velocity.Y > _stats.maxFallSpeed) {
                velocity.Y = _stats.maxFallSpeed;
            }
        }

        private void IncrementTurnAroundMovement() {
            if (velocity.X < runSlowdown) {
                velocity.X += runSlowdown;
            }
        }
        #endregion

        // clamps velocity to prevent collision with surrounding tiles
        private bool UpdatePositionInWorld() {
            // this method is taken from Collision.TileCollision()
            int lowerXBound = (int)(position.X / 16) - 1;
            int lowerYBound = (int)(position.Y / 16) - 1;
            int upperXBound = (int)((position.X + width) / 16) + 2;
            int upperYBound = (int)((position.Y + height) / 16) + 2;
            int xIntersectionLastX = -1;
            int xIntersectionLastY = -1;
            int yIntersectionLastX = -1;
            int yIntersectionLastY = -1;
            PixelPosition velocité = velocity;
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
            bool velocityChanged = velocity != velocité;
            velocity = velocité;
            position += velocity;
            return velocityChanged;
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

        #region Entity Position thing i cant think of a word rn
        public PixelPosition Center {
            get {
                return new PixelPosition(position.X + (width / 2), position.Y + (height / 2));
            }
            set {
                position = new PixelPosition(value.X - (width / 2), value.Y - (height / 2));
            }
        }

        public PixelPosition Left {
            get {
                return new PixelPosition(position.X, position.Y + (height / 2));
            }
            set {
                position = new PixelPosition(value.X, value.Y - (height / 2));
            }
        }

        public PixelPosition Right {
            get {
                return new PixelPosition(position.X + width, position.Y + (height / 2));
            }
            set {
                position = new PixelPosition(value.X - width, value.Y - (height / 2));
            }
        }

        public PixelPosition Top {
            get {
                return new PixelPosition(position.X + (width / 2), position.Y);
            }
            set {
                position = new PixelPosition(value.X - (width / 2), value.Y);
            }
        }

        public PixelPosition TopLeft {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        public PixelPosition TopRight {
            get {
                return new PixelPosition(position.X + width, position.Y);
            }
            set {
                position = new PixelPosition(value.X - width, value.Y);
            }
        }

        public PixelPosition Bottom {
            get {
                return new PixelPosition(position.X + (width / 2), position.Y + height);
            }
            set {
                position = new PixelPosition(value.X - (width / 2), value.Y - height);
            }
        }

        public PixelPosition BottomLeft {
            get {
                return new PixelPosition(position.X, position.Y + height);
            }
            set {
                position = new PixelPosition(value.X, value.Y - height);
            }
        }

        public PixelPosition BottomRight {
            get {
                return new PixelPosition(position.X + width, position.Y + height);
            }
            set {
                position = new PixelPosition(value.X - width, value.Y - height);
            }
        }
        #endregion
    }
}
