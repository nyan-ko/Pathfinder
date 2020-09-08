using System;
using Terraria;
using Pathfinder.Movements;

namespace Pathfinder.Structures {
    public struct PlayerProjection {  // trace on
        public static readonly PlayerProjection Empty = new PlayerProjection();

        public PixelPosition position { get; private set; }
        public PixelPosition velocity;  // TODO make this private and just have it be a get
        public int width;
        public int height;

        public int jump;
        private bool canJump;

        private sbyte Direction;
        public CollisionType CollisionType;

        private PlayerStats _stats;

        public PlayerProjection(Player player) {
            position = new PixelPosition(player.position);
            velocity = new PixelPosition(player.velocity);
            width = player.width;
            height = player.height;
            jump = 15;
            canJump = false;
            Direction = 1;
            _stats = new PlayerStats(player);
            CollisionType = 0;
        }

        // currently unused
        public bool ValidPosition(TilePosition tilePosition, HorizontalDirection playerDirectionRelativeToBlock) {
            int offset = (int)playerDirectionRelativeToBlock == -1 ? -1 : 1;

            for (int x = tilePosition.X - offset; x != tilePosition.X; x += offset) {
                for (int y = tilePosition.Y; y < tilePosition.Y + 3; y++) {
                    if (PathfindingUtils.IsTileSolid(x, y)) {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool ShouldUseMidairTurn => velocity.Y < 0; 

        public bool IsGoingRightWay => velocity.X * Direction >= 0;

        public void InvertDirection() => Direction = (sbyte)-Direction;

        public void SetInitialDirection(HorizontalDirection direction) => Direction = (sbyte)direction;

        #region Updates

        public void UpdateMidairTurnAroundMovement() {
            IncrementJumpMovement();
            IncrementTurnAroundMovement();
            UpdatePositionInWorld();
        }

        public void UpdateFallingTurnAroundMovement() { 
            IncrementFallMovement();
            IncrementTurnAroundMovement();
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

        // clamps velocity to prevent collision with surrounding tiles
        private void UpdatePositionInWorld() {
            DecrementJump();
            CollisionType = 0;
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
            PixelPosition projectedPosition = position + velocity;
            for (int x = lowerXBound; x < upperXBound; ++x) {
                for (int y = lowerYBound; y < upperYBound; ++y) {
                    if (PathfindingUtils.IsTileSolid(x, y)) {
                        int pX = x * 16;
                        int pY = y * 16;

                        if (PathfindingUtils.IsEntityIntersectingWithEntity(projectedPosition.X, projectedPosition.Y, width, height, pX, pY, 16, 16)) { // dumb dumb never triggers
                            var tile = Main.tile[x, y];

                            if (position.Y + height <= pY) {
                                yIntersectionLastX = x;
                                yIntersectionLastY = y;
                                if (yIntersectionLastX != xIntersectionLastX) {
                                    jump = 15;
                                    canJump = true;
                                    CollisionType |= CollisionType.Down;
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
                                        CollisionType |= CollisionType.Right;
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
                                        CollisionType |= CollisionType.Left;
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
                                jump = 0;
                                CollisionType |= CollisionType.Up;
                                velocité.Y = pY + (16 - position.Y);
                                if (yIntersectionLastY == xIntersectionLastY) {
                                    velocité.X = velocity.X;
                                }
                            }
                        }
                    }
                }
            }
            //bool velocityChanged = velocity != velocité;
            velocity = velocité;
            position += velocity;
            //return velocityChanged;
        }

        #endregion

        #region Increments
        private void IncrementHorizontalMovement() {
            if (Direction >= 0) {
                if (velocity.X < _stats.maxRunSpeed) {
                    velocity.X += _stats.runAcceleration;
                }
                else if (velocity.X < _stats.accessoryRunSpeed && velocity.Y == 0) {
                    velocity.X += _stats.runAcceleration * 0.2F;
                }
                else if (velocity.Y == 0 && velocity.X > _stats.runSlowdown) {
                    velocity.X -= _stats.runSlowdown;
                }
            }
            else {
                if (velocity.X > -_stats.maxRunSpeed) {
                    velocity.X -= _stats.runAcceleration;
                }
                else if (velocity.X > -_stats.accessoryRunSpeed && velocity.Y == 0) {
                    velocity.X -= _stats.runAcceleration * 0.2F;
                }
                else if (velocity.Y == 0 && velocity.X < -_stats.runSlowdown) {
                    velocity.X += _stats.runSlowdown;
                }
            }
        }

        private void IncrementJumpMovement() {
            if (jump > 0 && canJump) {
                velocity.Y = -_stats.jumpSpeed;
            }
            else {
                IncrementFallMovement();
            }
        }

        private void IncrementFallMovement() {
            if (velocity.Y < _stats.maxFallSpeed) {
                velocity.Y += _stats.gravity;
            }
            else if (velocity.Y > _stats.maxFallSpeed) {
                velocity.Y = _stats.maxFallSpeed;
            }
            canJump = false;
        }

        private void IncrementTurnAroundMovement() {
            if (Direction <= 0) {
                if (velocity.X > 0) {
                    velocity.X -= _stats.runSlowdown + _stats.runAcceleration;
                }
            }
            else {
                if (velocity.X < 0) {
                    velocity.X += _stats.runSlowdown + _stats.runAcceleration;
                }
            }
        }

        private void DecrementJump() {
            if (jump <= 0) {
                return;
            }
            jump--;
        }
        #endregion

        #region Estimates
        public float EstimateTimeToWalkDistance(float pixelDeltaDistance) {
            if (pixelDeltaDistance == 0) {
                return 0;
            }

            float multiplier = Math.Sign(pixelDeltaDistance);
            pixelDeltaDistance = Math.Abs(pixelDeltaDistance);
            float velocity = this.velocity.X * multiplier;
            float positionChange = 0;
            int frameCount = 0;
            const int LIMIT = 60 * 60;

            while (true) {
                frameCount++;

                if (velocity < _stats.maxRunSpeed) {
                    velocity += _stats.runAcceleration;
                }

                positionChange += velocity;

                if (positionChange >= pixelDeltaDistance) {
                    return frameCount + 1 - (positionChange / pixelDeltaDistance);
                }
                else if (frameCount > LIMIT) {
                    return float.MaxValue;
                }
            }
        }

        public float EstimateTimeToFallDistance(float pixelDeltaDistance) {
            if (pixelDeltaDistance == 0) {
                return 0;
            }

            pixelDeltaDistance = Math.Abs(pixelDeltaDistance);
            float velocity = this.velocity.Y;
            float positionChange = 0;
            int frameCount = 0;
            const int LIMIT = 60 * 60;

            while (true) {
                frameCount++;

                if (velocity < _stats.maxFallSpeed) {
                    velocity += _stats.gravity;
                }

                positionChange += velocity;

                if (positionChange >= pixelDeltaDistance) {
                    return frameCount + 1 - (positionChange / pixelDeltaDistance);
                }
                else if (frameCount > LIMIT) {
                    return float.MaxValue;
                }
            }
        }

        public float EstimateTimeToJumpDistance(float pixelDeltaDistance) {
            if (pixelDeltaDistance == 0) {
                return 0;
            }

            pixelDeltaDistance = Math.Abs(pixelDeltaDistance);
            float velocity = _stats.jumpSpeed;
            float positionChange = 0;
            int frameCount = 0;
            const int LIMIT = 60 * 60;

            while (true) {
                frameCount++;
                positionChange += velocity;

                if (positionChange >= pixelDeltaDistance) {
                    return frameCount + 1 - (positionChange / pixelDeltaDistance);
                }
                else if (frameCount > LIMIT) {
                    return float.MaxValue;
                }
            }
        }
        #endregion

        public bool IsBodyIntersectingWithTile(int tileX, int tileY) {
            return PathfindingUtils.IsEntityIntersectingWithEntity(position.X, position.Y, width, height, tileX * 16, tileY * 16, 16, 16);
        }

        public bool IsOriginIntersectingWithTile(float tilePixelX, float tilePixelY) {
            return PathfindingUtils.IsEntityIntersectingWithEntity(position.X, position.Y, 0, 0, tilePixelX, tilePixelY, 16, 16);
        }

        public bool IsInCorrectRelativePosition(PixelPosition comparePosition, int xDirection, int yDirection) {
            return PathfindingUtils.IsPositionInCorrectRelativePosition(comparePosition.X, comparePosition.Y, position.X, position.Y, xDirection, yDirection);
        }
    }
}
