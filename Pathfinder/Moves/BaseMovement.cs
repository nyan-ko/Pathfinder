using Pathfinder.Projections;
using Pathfinder.Structs;

namespace Pathfinder.Moves
{
    public abstract class BaseMovement
    {
        public int dX { get; protected set; }
        public int dY { get; protected set; }
        protected bool Jump;
        protected HorizontalDirection RelativeNodeDirection;

        protected const float ACCEPTABLE_RANGE = 0.25F;
        protected const float ACCEPTABLE_RANGE_SQUARED = ACCEPTABLE_RANGE * ACCEPTABLE_RANGE;
        protected static readonly ActionCost IMPOSSIBLE_COST = ActionCost.ImpossibleCost;
        protected const int IMPOSSIBLE_FRAME_COST = -1;

        public ActionCost CalculateCost(int previousX, int previousY, ref PlayerProjection player)
        {
            //var goalLocation = player.position + new PixelPosition(dX * 16, dY * -16);
            var goalLocation = new PixelPosition((previousX + dX) * 16, (previousY - dY) * 16);
            bool standingStill = player.velocity.X == 0;
            bool playerGoingWrongWay = !standingStill && ((player.velocity.X < 0 && RelativeNodeDirection == HorizontalDirection.Right) ||
                (player.velocity.X > 0 && RelativeNodeDirection == HorizontalDirection.Left));

            if (!player.ValidPosition(new TilePosition(goalLocation), true, RelativeNodeDirection))
                return IMPOSSIBLE_COST;

            int turnFrames = 0;

            if (playerGoingWrongWay)
                UpdateTurnAround(ref player, out turnFrames);
            else if (standingStill)
                player.SetDirection(RelativeNodeDirection);

            UpdateMovementTowardsGoal(ref player, goalLocation, out int frames);

            return ActionCost.CreateActionCost(turnFrames, frames, Jump);
        }

        protected abstract void UpdateTurnAround(ref PlayerProjection player, out int frames);

        protected abstract void UpdateMovementTowardsGoal(ref PlayerProjection player, PixelPosition goal, out int frames);

        public static BaseMovement[] GetAllMoves()
        {
            BaseMovement[] movements = new BaseMovement[9];

            movements[0] = new Pillar(1);
            movements[1] = new Ascend(1, 1, HorizontalDirection.Right);
            movements[2] = new Walk(1, HorizontalDirection.Right);
            movements[3] = new Descend(1, -1, HorizontalDirection.Right);
            movements[4] = new Fall(-1);
            movements[5] = new Descend(-1, -1, HorizontalDirection.Left);
            movements[6] = new Walk(-1, HorizontalDirection.Left);
            movements[7] = new Ascend(-1, 1, HorizontalDirection.Left);
            movements[8] = new Stay();

            return movements;
        }
    }

    public enum HorizontalDirection : sbyte
    {
        Left = -1, None, Right = 1
    }
}