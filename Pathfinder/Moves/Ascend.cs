using Pathfinder.Projections;
using Pathfinder.Structs;

namespace Pathfinder.Moves
{
    public class Ascend : BaseMovement
    {
        public Ascend(int deltaX, int deltaY, HorizontalDirection direction)
        {
            dX = deltaX;
            if (dY < 0)
            {
                dY = 0;
            }
            dY = deltaY;
            RelativeNodeDirection = direction;
        }

        protected override void UpdateTurnAround(ref PlayerProjection player, out int frames)
        {
            frames = 0;
            player.SetDirection(RelativeNodeDirection);
            while (!player.IsGoingRightWay)
            {
                player.UpdateTurnAroundMovement();
                frames++;
            }
        }

        protected override void UpdateMovementTowardsGoal(ref PlayerProjection player, PixelPosition goal, out int frames)
        {
            frames = 0;
            float previousDistance = float.MaxValue;

            while (!player.IsTileOriginIntersectingWithTile(goal.X, goal.Y))
            {
                player.UpdateMovingJumpMovement();
                float distance = player.position.Distance(goal.X, goal.Y);

                if (distance < previousDistance)
                {
                    previousDistance = distance;
                }
                else if (!player.WillTileOriginIntersectWithTile(goal.X, goal.Y))
                {
                    frames = IMPOSSIBLE_FRAME_COST;
                    return;
                }

                frames++;
            }
        }
    }
}