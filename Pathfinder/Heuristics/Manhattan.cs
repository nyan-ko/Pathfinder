using System;

namespace Pathfinder.Heuristics
{
    public struct Manhattan : IHeuristic
    {
        public float EstimateCost(int x, int y, int goalX, int goalY)
        {
            return Math.Abs(x - goalX) + Math.Abs(y - goalY);
        }
    }
}