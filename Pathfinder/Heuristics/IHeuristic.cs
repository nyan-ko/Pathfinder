namespace Pathfinder.Heuristics
{
    public interface IHeuristic
    {
        float EstimateCost(int x, int y, int goalX, int goalY);
    }
}