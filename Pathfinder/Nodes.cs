using Pathfinder;
using Pathfinder.Heuristics;
using Pathfinder.Input;
using Pathfinder.Moves;
using Pathfinder.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Terraria;

namespace Nodes
{
    public enum NodeStatus : byte
    {
        Open,
        Closed
    }

    public interface INode
    {
        int X { get; }
        int Y { get; }
    }

    public abstract class AbstractPathNode : INode
    {
        public ActionCost ActionCost;
        public float CostFromStart;
        public float HeuristicCostToGoal;
        public short HeapIndex;

        public float Cost => CostFromStart != -1 ? CostFromStart + HeuristicCostToGoal : float.MaxValue;
        public int X { get; }
        public int Y { get; }
        public int ParentX { get; private set; }
        public int ParentY { get; private set; }

        protected AbstractPathNode(int x, int y, int goalX, int goalY)
        {
            X = x;
            Y = y;
            ActionCost = ActionCost.ImpossibleCost;

            CostFromStart = -1;
            //HeuristicCostToGoal = CalculateHeuristicCostToGoal(goalX, goalY);
            HeapIndex = -1;
        }

        public void SetParent(INode parent)
        {
            ParentX = parent.X;
            ParentY = parent.Y;
        }

        protected virtual float CalculateHeuristicCostToGoal(int x, int y) => AStarPathfinder.Heuristic.EstimateCost(X, Y, x, y);  //TODO change to a globally held heuristic function or pass as parameter
    }

    public class JumpNode : AbstractPathNode
    {
        public PlayerProjection Projection;
        public int ParentJump;
        public int Input;
        private JumpNodeCollection _collection;

        public JumpNode(int x, int y, int goalX, int goalY, PlayerProjection projection, int input) : base(x, y, goalX, goalY)
        {
            Projection = projection;
            Input = input;

            HeuristicCostToGoal = CalculateHeuristic(goalX, goalY);
        }

        public void LinkToCollection(JumpNodeCollection collection)
        {
            _collection = collection;
            collection.AddNode(this);
        }

        private float CalculateHeuristic(int x, int y)
        {
            float horizontalCost = Projection.EstimateTimeToWalkDistance((X - x) * 16);
            float verticalCost = 0;

            if (y > Y)
            {
                verticalCost = Projection.EstimateTimeToFallDistance((Y - y) * 16);
            }
            else
            {
                verticalCost = Projection.EstimateTimeToJumpDistance((Y - y) * 16);
            }

            return horizontalCost + verticalCost;
        }
    }

    public class JumpNodeCollection
    {
        public Dictionary<int, JumpNode> Nodes;  // TODO private these and a bunch of other unnecessarily public fields
        public byte ParentJumpNodeIndex;
        public byte JumpNodeIndex;

        public int X;
        public int Y;

        public JumpNodeCollection(int x, int y)
        {
            Nodes = new Dictionary<int, JumpNode>(8);  // arbitrary initial capacity, yet to find out if this is a good initial capacity

            X = x;
            Y = y;
        }

        public void AddNode(JumpNode node)
        {
            Nodes.Add(node.Projection.jump, node);
        }

        public bool ContainsProjection(PlayerProjection projection)
        {
            if (Nodes.Count == 0)
            {
                return false;
            }

            foreach (var node in Nodes.Values)
            {
                if (node.Projection.jump == projection.jump)
                {
                    return true;
                }
            }

            return false;
        }

        public JumpNode this[int jump]
        {
            get
            {
                if (Nodes.ContainsKey(jump))
                {
                    return Nodes[jump];
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }

    public class AStarPathfinder : IPathFinder
    {
        private const float MINIMUM_IMPROVEMENT = 0.1F;
        public static readonly IHeuristic Heuristic = new Manhattan();

        public int ExploreLimit { get; set; } = 500;

        private int[] HeuristicWeights;
        private BaseMovement[] availableMoves;
        private BinaryNodeHeap<JumpNode> openSet;
        private Dictionary<long, JumpNodeCollection> nodeHashDictionary;
        private JumpNode startNode;
        private JumpNode endNode;

        public AStarPathfinder(int startX, int startY, int endX, int endY, Player startingProjection)
        {
            startNode = new JumpNode(startX, startY, endX, endY, new PlayerProjection(startingProjection), byte.MaxValue) { CostFromStart = 0 };
            endNode = new JumpNode(endX, endY, endX, endY, PlayerProjection.Empty, byte.MaxValue) { CostFromStart = -1 };
            availableMoves = BaseMovement.GetAllMoves();
            HeuristicWeights = new int[] { 1, 2, 3, 5, 10 };
            nodeHashDictionary = new Dictionary<long, JumpNodeCollection>();
            openSet = new BinaryNodeHeap<JumpNode>();
        }

        public IEnumerable<JumpNodeCollection> debug2
        {
            get
            {
                lock (nodeHashDictionary)
                {
                    return nodeHashDictionary?.Values;
                }
            }
        }

        public INode Start => startNode;

        public INode End => endNode;

        public IPath FindPath()
        {
            bool foundPath = false;
            openSet.Add(startNode);
            int count = 0;
            JumpNode currentNode = startNode;

            try
            {
                while (!foundPath)
                {
                    currentNode = openSet.TakeLowest();

                    bool reachedEnd = currentNode.Projection.IsBodyIntersectingWithTile(endNode.X, endNode.Y);
                    if (reachedEnd)
                    {
                        foundPath = true;
                        break;
                    }

                    SearchNeighbours(currentNode);
                    count++;
                    //DebugDraw();
                    Thread.Sleep(100);

                    if (count >= ExploreLimit)
                    {
                        break;
                    }
                }
            }
            catch
            {
                //return null;
            }

            if (foundPath)
            {
                PathfinderTriggersSet triggersSet = new PathfinderTriggersSet();
                List<Trigger> triggers = new List<Trigger>();

                var retracedSteps = RetraceSteps(currentNode);
                foreach (var step in retracedSteps.Skip(1))
                {
                    if (step.Input < 256 && step.Input > 0)
                        triggers.Add(new Trigger((byte)step.Input, step.ActionCost.TotalCost, step.CostFromStart - step.ActionCost.TotalCost));
                }
                triggers.Reverse();
                triggersSet.SetList(triggers);

                return new AStarPath(true, triggersSet);
            }

            return null; // idk what to do yet for this
        }

        private List<JumpNode> RetraceSteps(JumpNode lastNode)
        {
            if (lastNode.X == startNode.X && lastNode.Y == startNode.Y) { return new List<JumpNode>(); }

            List<JumpNode> steps = new List<JumpNode>();
            long hash = PathfindingUtils.GetNodeHash(lastNode.X, lastNode.Y);  // gets the node that exists in the node dictionary since certain instances may not have a set parent (i.e. endNode and startNode)
            var currentNode = GetNode(hash, lastNode.Projection.jump);

            while (currentNode.X != startNode.X || currentNode.Y != startNode.Y)
            {
                hash = PathfindingUtils.GetNodeHash(currentNode.ParentX, currentNode.ParentY);
                var parentNode = GetNode(hash, currentNode.ParentJump);
                steps.Add(parentNode);
                currentNode = parentNode;
            }

            return steps;
        }

        private void SearchNeighbours(JumpNode parent)
        {
            //for (byte i = 0; i < parent.Nodes.Count; i++) {
            byte input = 1;

            foreach (BaseMovement movement in availableMoves)
            {
                int currentX = parent.X + movement.dX;
                int currentY = parent.Y - movement.dY;
                var movementProjection = parent.Projection;
                var nodeCost = movement.CalculateCost(parent.X, parent.Y, ref movementProjection);

                if (nodeCost.TotalCost != float.MaxValue)
                {
                    long hash = PathfindingUtils.GetNodeHash(currentX, currentY);
                    var neighbouringNode = GetNode(currentX, currentY, hash, movementProjection, input);

                    //if (!neighbouringNode.ContainsVelocity(movementProjection.velocity)) {
                    //    neighbouringNode.AddNode(new JumpNode(movementProjection, input));
                    //    neighbouringNode.JumpNodeIndex++;
                    //}

                    float costToGetHere = parent.CostFromStart + nodeCost.TotalCost;
                    float newNeighbourCost = costToGetHere + neighbouringNode.HeuristicCostToGoal;

                    if (neighbouringNode.Cost - newNeighbourCost > MINIMUM_IMPROVEMENT)
                    {
                        neighbouringNode.ActionCost = nodeCost;
                        neighbouringNode.CostFromStart = costToGetHere;
                        neighbouringNode.SetParent(parent);
                        neighbouringNode.ParentJump = parent.Projection.jump;
                        //neighbouringNode.ParentJumpNodeIndex = parent.JumpNodeIndex;
                        if (neighbouringNode.HeapIndex == -1)
                        {
                            openSet.Add(neighbouringNode);
                        }
                        else
                        {
                            openSet.Update(neighbouringNode);
                        }
                    }
                }

                input++;
            }
            //}
        }

        private JumpNode GetNode(long hash, int jump)
        {
            return nodeHashDictionary[hash][jump];
        }

        private JumpNode GetNode(int x, int y, long hash, PlayerProjection z, int input)
        {
            if (nodeHashDictionary.ContainsKey(hash))
            {
                var collection = nodeHashDictionary[hash];
                if (collection.ContainsProjection(z))
                    return collection[z.jump];
                JumpNode node = new JumpNode(x, y, endNode.X, endNode.Y, z, input);
                node.LinkToCollection(collection);
                return node;
            }
            else
            {
                if (x == -1 && y == -1)
                    throw new Exception();
                JumpNodeCollection nodeCollection = new JumpNodeCollection(x, y);
                JumpNode node = new JumpNode(x, y, endNode.X, endNode.Y, z, input);
                node.LinkToCollection(nodeCollection);
                nodeHashDictionary.Add(hash, nodeCollection);
                return node;
            }
        }
    }
}