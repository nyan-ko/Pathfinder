using Pathfinder.Moves;
using Pathfinder.Structs;
using System;
using Terraria;

namespace Pathfinder.Projections
{
    public struct PlayerProjection
    {  // trace on
        public static readonly PlayerProjection Empty = new PlayerProjection();

        public PixelPosition position;
        public PixelPosition velocity;  // TODO make this private and just have it be a get
        public int width;
        public int height;

        public int jump;
        private bool canJump;
        public float gravity;
        public float maxRunSpeed;
        public float runAcceleration;
        public float runSlowdown;
        public float accessoryRunSpeed;

        public sbyte Direction;
        public PickaxeProjection[] pickaxes;
        private PlayerStats _stats;

        public PlayerProjection(Player player)
        {
            position = new PixelPosition(player.position);
            velocity = new PixelPosition(player.velocity);
            width = player.width;
            height = player.height;

            jump = 15;
            canJump = false;
            gravity = Player.defaultGravity;
            Direction = 1;
            maxRunSpeed = player.maxRunSpeed;
            runAcceleration = player.runAcceleration;
            runSlowdown = player.runSlowdown;
            accessoryRunSpeed = player.accRunSpeed;

            _stats = new PlayerStats(player);
            pickaxes = new PickaxeProjection[1];
            ScanInventory();
        }

        private void ScanInventory()
        {
        }

        public bool ValidPosition(TilePosition tilePosition, bool canBeInAir, HorizontalDirection playerDirectionRelativeToBlock)
        {
            int offset = (int)playerDirectionRelativeToBlock == -1 ? -1 : 1;

            for (int x = tilePosition.X - offset; x != tilePosition.X; x += offset)
            {
                for (int y = tilePosition.Y; y < tilePosition.Y + 3; y++)
                {
                    if (PathfindingUtils.IsTileSolid(x, y))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool IsGoingRightWay => velocity.X * Direction >= 0;

        public void SetDirection(HorizontalDirection direction) => Direction = (sbyte)direction;

        public void SetDirection(int direction) => Direction = (sbyte)direction;

        public void UpdateTurnAroundMovement()
        {  // TODO condense these
            IncrementFallMovement();
            IncrementTurnAroundMovement();
            UpdatePositionInWorld();
        }

        public void UpdateStopMovement()
        {
            IncrementFallMovement();
            IncrementStopMovement();
            UpdatePositionInWorld();
        }

        public void UpdateFallMovement()
        {
            IncrementFallMovement();
            UpdatePositionInWorld();
        }

        public void UpdateMovingFallMovement()
        {
            IncrementFallMovement();
            IncrementHorizontalMovement();
            UpdatePositionInWorld();
        }

        public void UpdateJumpMovement()
        {
            IncrementJumpMovement();
            UpdatePositionInWorld();
        }

        public void UpdateMovingJumpMovement()
        {
            IncrementJumpMovement();
            IncrementHorizontalMovement();
            UpdatePositionInWorld();
        }

        #region Increments

        private void IncrementHorizontalMovement()
        {
            if (Direction >= 0)
            {
                if (velocity.X < maxRunSpeed)
                {
                    velocity.X += runAcceleration;
                }
                else if (velocity.X < accessoryRunSpeed && velocity.Y == 0)
                {
                    velocity.X += runAcceleration * 0.2F;
                }
            }
            else
            {
                if (velocity.X > -maxRunSpeed)
                {
                    velocity.X -= runAcceleration;
                }
                else if (velocity.X > -accessoryRunSpeed && velocity.Y == 0)
                {
                    velocity.X -= runAcceleration * 0.2F;
                }
            }
        }

        private void IncrementJumpMovement()
        {
            if (jump > 0 && canJump)
            {
                velocity.Y = -_stats.jumpSpeed;
            }
            else
            {
                IncrementFallMovement();
            }
        }

        private void IncrementFallMovement()
        {
            if (velocity.Y < _stats.maxFallSpeed)
            {
                velocity.Y += gravity;
            }
            else if (velocity.Y > _stats.maxFallSpeed)
            {
                velocity.Y = _stats.maxFallSpeed;
            }
            canJump = false;
        }

        private void IncrementTurnAroundMovement()
        {
            if (Direction >= 0)
            {
                if (velocity.X > 0)
                {
                    velocity.X -= runSlowdown;
                }
            }
            else
            {
                if (velocity.X < 0)
                {
                    velocity.X += runSlowdown;
                }
            }
        }

        private void IncrementStopMovement()
        {
            if (Direction >= 0)
            {
                if (velocity.X > 0)
                {
                    velocity.X -= runSlowdown;
                }
                else
                {
                    velocity.X = 0;
                }
            }
            else
            {
                if (velocity.X < 0)
                {
                    velocity.X += runSlowdown;
                }
                else
                {
                    velocity.X = 0;
                }
            }
        }

        private void DecrementJump()
        {
            if (jump <= 0)
            {
                return;
            }
            jump--;
        }

        #endregion Increments

        // clamps velocity to prevent collision with surrounding tiles
        private void UpdatePositionInWorld()
        {
            DecrementJump();
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
            for (int x = lowerXBound; x < upperXBound; ++x)
            {
                for (int y = lowerYBound; y < upperYBound; ++y)
                {
                    if (PathfindingUtils.IsTileSolid(x, y))
                    {
                        int pX = x * 16;
                        int pY = y * 16;

                        if (PathfindingUtils.IsEntityIntersectingWithEntity(projectedPosition.X, projectedPosition.Y, width, height, pX, pY, 16, 16))
                        { // dumb dumb never triggers
                            var tile = Main.tile[x, y];

                            if (position.Y + height <= pY)
                            {
                                yIntersectionLastX = x;
                                yIntersectionLastY = y;
                                if (yIntersectionLastX != xIntersectionLastX)
                                {
                                    jump = 15;
                                    canJump = true;
                                    velocité.Y = pY - (position.Y + height);
                                }
                            }
                            else if (position.X + width <= pX && !Main.tileSolidTop[tile.type])
                            {
                                if (x >= 1 && Main.tile[x - 1, y] == null)
                                {
                                    Main.tile[x - 1, y] = new Tile();
                                }
                                if (x < 1 || (Main.tile[x - 1, y].slope() != 2 && Main.tile[x - 1, y].slope() != 4))
                                {
                                    xIntersectionLastX = x;
                                    xIntersectionLastY = y;
                                    if (xIntersectionLastY != yIntersectionLastY)
                                    {
                                        velocité.X = pX - (position.X + width);
                                    }
                                    if (yIntersectionLastX == xIntersectionLastX)
                                    {
                                        velocité.Y = velocity.Y;
                                    }
                                }
                            }
                            else if (position.X >= pX + 16 && !Main.tileSolidTop[tile.type])
                            {
                                if (Main.tile[x + 1, y] == null)
                                {
                                    Main.tile[x + 1, y] = new Tile();
                                }
                                if (Main.tile[x + 1, y].slope() != 1 && Main.tile[x + 1, y].slope() != 3)
                                {
                                    xIntersectionLastX = x;
                                    xIntersectionLastY = y;
                                    if (xIntersectionLastY != yIntersectionLastY)
                                    {
                                        velocité.X = pX + 16f - position.X;
                                    }
                                    if (yIntersectionLastX == xIntersectionLastX)
                                    {
                                        velocité.Y = velocity.Y;
                                    }
                                }
                            }
                            else if (position.Y >= pY + 16 && !Main.tileSolidTop[tile.type])
                            {
                                yIntersectionLastX = x;
                                yIntersectionLastY = y;
                                velocité.Y = pY + (16 - position.Y);
                                jump = 0;
                                if (yIntersectionLastY == xIntersectionLastY)
                                {
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

        public bool IsBodyIntersectingWithTile(int tileX, int tileY)
        {
            return PathfindingUtils.IsEntityIntersectingWithEntity(position.X, position.Y, width, height, tileX * 16, tileY * 16, 16, 16);
        }

        public bool WillTileOriginIntersectWithTile(float tilePixelX, float tilePixelY)
        {
            float projectedX = position.X + velocity.X;
            float projectedY = position.Y + velocity.Y;
            return PathfindingUtils.IsEntityIntersectingWithEntity(projectedX, projectedY, 16, 16, tilePixelX, tilePixelY, 16, 16);
        }

        public bool IsTileOriginIntersectingWithTile(float tilePixelX, float tilePixelY)
        {
            return PathfindingUtils.IsEntityIntersectingWithEntity(position.X, position.Y, 16, 16, tilePixelX, tilePixelY, 16, 16);
        }

        public float EstimateTimeToWalkDistance(float pixelDeltaDistance)
        {
            if (pixelDeltaDistance == 0)
            {
                return 0;
            }

            pixelDeltaDistance = Math.Abs(pixelDeltaDistance);
            float velocity = -(this.velocity.X * Direction);
            float positionChange = 0;
            int frameCount = 0;
            const int LIMIT = 120;

            while (true)
            {
                if (velocity <= maxRunSpeed)
                {
                    velocity += runAcceleration;
                }

                positionChange += velocity;

                if (positionChange >= pixelDeltaDistance)
                {
                    return frameCount + (positionChange / pixelDeltaDistance) - 1;
                }

                frameCount++;

                if (frameCount > LIMIT)
                {
                    return float.MaxValue;
                }
            }
        }

        public float EstimateTimeToFallDistance(float pixelDeltaDistance)
        {
            if (pixelDeltaDistance == 0)
            {
                return 0;
            }

            pixelDeltaDistance = Math.Abs(pixelDeltaDistance);
            float velocity = this.velocity.Y;
            float positionChange = 0;
            int frameCount = 0;
            const int LIMIT = 120;

            while (true)
            {
                if (velocity <= _stats.maxFallSpeed)
                {
                    velocity += gravity;
                }

                positionChange += velocity;

                if (positionChange >= pixelDeltaDistance)
                {
                    return frameCount + (positionChange / pixelDeltaDistance) - 1;
                }

                frameCount++;

                if (frameCount > LIMIT)
                {
                    return float.MaxValue;
                }
            }
        }

        public float EstimateTimeToJumpDistance(float pixelDeltaDistance)
        {
            if (pixelDeltaDistance == 0)
            {
                return 0;
            }

            pixelDeltaDistance = Math.Abs(pixelDeltaDistance);
            float velocity = _stats.jumpSpeed;
            float positionChange = 0;
            int frameCount = 0;
            const int LIMIT = 120;

            while (true)
            {
                positionChange += velocity;

                if (positionChange >= pixelDeltaDistance)
                {
                    return frameCount + (positionChange / pixelDeltaDistance) - 1;
                }

                frameCount++;

                if (frameCount > LIMIT)
                {
                    return float.MaxValue;
                }
            }
        }
    }
}