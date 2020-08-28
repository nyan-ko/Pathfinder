namespace Pathfinder.Heuristics
{
    public struct Debug : IHeuristic
    {
        public float EstimateCost(int x, int y, int goalX, int goalY)
        {
            return 0;
        }
    }
}