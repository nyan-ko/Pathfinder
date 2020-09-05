using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Terraria;
using ReLogic;
using Pathfinder;
using Pathfinder.Projections;
using Pathfinder.Moves;
using Pathfinder.Heuristics;
using Pathfinder.Input;
using Pathfinder.Structs;
using Microsoft.Xna.Framework;
using Terraria.GameContent;

namespace Nodes
{

    public enum NodeStatus : byte {
        Open,
        Closed
    }

    public interface INode {
        int X { get; }
        int Y { get; }
    }

    public abstract class AbstractPathNode : INode {
        private int _x;
        private int _y;
        private int pX;
        private int pY;
        public ActionCost ActionCost;
        public float CostFromStart;
        public float HeuristicCostToGoal;
        public short HeapIndex;

        public float Cost => CostFromStart != -1 ? CostFromStart + HeuristicCostToGoal : float.MaxValue;
        public int X => _x;
        public int Y => _y;
        public int ParentX => pX;
        public int ParentY => pY;

        protected AbstractPathNode(int x, int y, int goalX, int goalY) {
            _x = x;
            _y = y;
            ActionCost = ActionCost.ImpossibleCost;
            CostFromStart = -1;
            HeapIndex = -1;
        }

        public void SetParent(INode parent) {
            pX = parent.X;
            pY = parent.Y;
            SetAdditionalParentFields(parent);
        }

        protected abstract void SetAdditionalParentFields(INode parent);

        public override string ToString() {
            return $"X:{_x} Y:{_y} Cost:{Cost}";
        }
    }

    public class JumpNode : AbstractPathNode {
        public PlayerProjection Projection;
        public PixelPosition ParentVelocity;
        public int Input;
        private JumpNodeCollection _collection;

        public JumpNode(int x, int y, int goalX, int goalY, PlayerProjection projection, int input) : base(x, y, goalX, goalY) {
            Projection = projection;
            Input = input;

            HeuristicCostToGoal = CalculateHeuristic(goalX, goalY);
        }

        public void LinkToCollection(JumpNodeCollection collection) {
            _collection = collection;
            collection.AddNode(this);
        }

        protected override void SetAdditionalParentFields(INode parent) {
            ParentVelocity = ((JumpNode)parent).Projection.velocity;
        }

        private float CalculateHeuristic(int x, int y) {
            float horizontalCost = Projection.EstimateTimeToWalkDistance((x - X) * 16);
            float verticalCost = 0;

            if (y > Y) {
                verticalCost = Projection.EstimateTimeToFallDistance((y - Y) * 16);
            }
            else {
                verticalCost = Projection.EstimateTimeToJumpDistance((y - Y) * 16);
            }

            // best to worser heuristics
            const float MAGIC_HEURISTIC_MULTIPLIER = 1.41421F;  // pure magic, this somehow results in drastically less nodes explored and an overall much more accurate heuristic compared to functions without it
            return (float)Math.Sqrt(horizontalCost * horizontalCost + verticalCost * verticalCost) * MAGIC_HEURISTIC_MULTIPLIER;
            //return Math.Max(horizontalCost, verticalCost) * MAGIC_HEURISTIC_MULTIPLIER;  
            //return Math.Max(horizontalCost, verticalCost);
            //return horizontalCost + verticalCost;
            //return (float)Math.Sqrt(horizontalCost * horizontalCost + verticalCost * verticalCost);
        }
    }

    public class JumpNodeCollection {
        public Dictionary<PixelPosition, JumpNode> Nodes;  // TODO private these and a bunch of other unnecessarily public fields
        public byte ParentJumpNodeIndex;
        public byte JumpNodeIndex;

        public int X;
        public int Y;

        public JumpNodeCollection(int x, int y) {
            Nodes = new Dictionary<PixelPosition, JumpNode>(8);  // arbitrary initial capacity, yet to find out if this is a good initial capacity

            X = x;
            Y = y;
        }

        public void AddNode(JumpNode node) {
            Nodes.Add(node.Projection.velocity, node);
        }

        public bool ContainsProjection(PlayerProjection projection) {
            if (Nodes.Count == 0) {
                return false;
            }

            foreach (var node in Nodes.Values) {
                if (node.Projection.velocity == projection.velocity) {
                    return true;
                }
            }

            return false;
        }

        public JumpNode this[PixelPosition velocity] {
            get {
                if (Nodes.ContainsKey(velocity)) {
                    return Nodes[velocity];
                }
                else {
                    throw new InvalidOperationException();
                }
            }
        }
    }

    public class AStarPathfinder : IPathFinder {
        private const float MINIMUM_IMPROVEMENT = 0.1F;
        public static readonly IHeuristic Heuristic = new Manhattan();

        public int ExploreLimit { get; set; } = 500;

        private int[] HeuristicWeights;
        private BaseMovement[] availableMoves;
        private BinaryNodeHeap<JumpNode> openSet;
        private Dictionary<long, JumpNodeCollection> nodeHashDictionary;
        private JumpNode startNode;
        private JumpNode endNode;

        public AStarPathfinder(int startX, int startY, int endX, int endY, Player startingProjection) {
            startNode = new JumpNode(startX, startY, endX, endY, new PlayerProjection(startingProjection), byte.MaxValue) { CostFromStart = 0 };
            endNode = new JumpNode(endX, endY, endX, endY, PlayerProjection.Empty, byte.MaxValue) { CostFromStart = -1 };
            availableMoves = BaseMovement.GetAllMoves();
            HeuristicWeights = new int[] { 1, 2, 3, 5, 10 };
            nodeHashDictionary = new Dictionary<long, JumpNodeCollection>();
            openSet = new BinaryNodeHeap<JumpNode>();
        }

        public IEnumerable<JumpNodeCollection> debug2 { get 
                {
                lock (nodeHashDictionary) {
                    return nodeHashDictionary?.Values;
                }
            }
        }

        public INode Start => startNode;

        public INode End => endNode;

        public IPath FindPath() {
            bool foundPath = false;
            int count = 0;
            openSet.Add(startNode);
            var startCollection = new JumpNodeCollection(startNode.X, startNode.Y);
            startNode.LinkToCollection(startCollection);
            nodeHashDictionary.Add(PathfindingUtils.GetNodeHash(startNode.X, startNode.Y), startCollection);
            JumpNode currentNode = startNode;

            while (!foundPath) {
                currentNode = openSet.TakeLowest();

                bool reachedEnd = currentNode.Projection.IsBodyIntersectingWithTile(endNode.X, endNode.Y);
                if (reachedEnd) {
                    foundPath = true;
                    break;
                }

                SearchNeighbours(currentNode);
                count++;

                if (count >= ExploreLimit) {
                    break;
                }
            }

            if (foundPath) {
                PathfinderTriggersSet triggersSet = new PathfinderTriggersSet();
                List<Trigger> triggers = new List<Trigger>();

                var retracedSteps = RetraceSteps(currentNode);
                foreach (var step in retracedSteps) {
                    if (step.Input < 256 && step.Input > 0) {
                        triggersSet.AddTrigger((byte)step.Input, step.ActionCost.TotalCost, step.CostFromStart - step.ActionCost.TotalCost);
                    }
                }

                triggersSet.SortInputList();

                return new AStarPath(true, triggersSet);
            }

            return null; // idk what to do yet for this
        }

        private List<JumpNode> RetraceSteps(JumpNode lastNode) {
            List<JumpNode> steps = new List<JumpNode>();                        // TODO make this less weirdChamp
            long hash = PathfindingUtils.GetNodeHash(lastNode.X, lastNode.Y);  // gets the node that exists in the node dictionary since certain instances may not have a set parent (i.e. endNode and startNode)
            var currentNode = GetNode(hash, lastNode.Projection.velocity);

            while (currentNode.X != startNode.X || currentNode.Y != startNode.Y) {
                steps.Add(currentNode);
                hash = PathfindingUtils.GetNodeHash(currentNode.ParentX, currentNode.ParentY);
                currentNode = GetNode(hash, currentNode.ParentVelocity);
            }

            return steps;
        }

        private void SearchNeighbours(JumpNode parent) {
            byte input = 1;

            foreach (BaseMovement movement in availableMoves) {
                var movementProjection = parent.Projection;
                var nodeCost = movement.SimulateMovement(parent.X, parent.Y, ref movementProjection);
                int currentX = parent.X + movement.dX;
                int currentY = parent.Y + movement.dY;

                if (nodeCost.TotalCost != float.MaxValue) {
                    long hash = PathfindingUtils.GetNodeHash(currentX, currentY);
                    var neighbouringNode = GetNode(currentX, currentY, hash, movementProjection, input);
                    float costToGetHere = parent.CostFromStart + nodeCost.TotalCost;
                    float newNeighbourCost = costToGetHere + neighbouringNode.HeuristicCostToGoal;

                    if (neighbouringNode.Cost - newNeighbourCost > MINIMUM_IMPROVEMENT) {
                        neighbouringNode.ActionCost = nodeCost;
                        neighbouringNode.CostFromStart = costToGetHere;
                        neighbouringNode.SetParent(parent);
                        if (neighbouringNode.HeapIndex == -1) {
                            openSet.Add(neighbouringNode);
                        }
                        else {
                            openSet.Update(neighbouringNode);
                        }
                    }
                }

                input++;
            }
        }

        private JumpNode GetNode(long hash, PixelPosition velocity) {
            return nodeHashDictionary[hash][velocity];
        }

        private JumpNode GetNode(int x, int y, long hash, PlayerProjection z, int input) {
            if (nodeHashDictionary.ContainsKey(hash)) {
                var collection = nodeHashDictionary[hash];
                if (collection.ContainsProjection(z))
                    return collection[z.velocity];
                JumpNode node = new JumpNode(x, y, endNode.X, endNode.Y, z, input);
                node.LinkToCollection(collection);
                return node;
            }
            else {
                JumpNodeCollection nodeCollection = new JumpNodeCollection(x, y);
                JumpNode node = new JumpNode(x, y, endNode.X, endNode.Y, z, input);
                node.LinkToCollection(nodeCollection);
                nodeHashDictionary.Add(hash, nodeCollection);
                return node;
            }
        }
    }

}
