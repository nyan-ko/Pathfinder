using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using ReLogic;
using Pathfinder;
using Pathfinder.Moves;
using Pathfinder.Heuristics;
using Microsoft.Xna.Framework;

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
        public NodeStatus Status;
        public int HeapIndex;

        public float Cost => CostFromStart + HeuristicCostToGoal;

        public int X => _x;

        public int Y => _y;

        public int ParentX => pX;

        public int ParentY => pY;

        protected AbstractPathNode(int x, int y, ActionCost cost, INode goal) {
            _x = x;
            _y = y;
            ActionCost = cost;

            HeuristicCostToGoal = CalculateHeuristicCostToGoal(goal);
            HeapIndex = -1;
            Status = NodeStatus.Open;
        }

        public void SetParent(INode parent) {
            pX = parent.X;
            pY = parent.Y;
        }

        private float CalculateHeuristicCostToGoal(INode goal) => AStarPathfinder.Heuristic.EstimateCost(this, goal);  //TODO change to a globally held heuristic function or pass as parameter
    }

    public class JumpNode {
        public PlayerProjection projectionAtThisNode;

        public JumpNode(PlayerProjection projection) {
            projectionAtThisNode = projection;
        }
    }

    public class JumpNodeCollection : AbstractPathNode {
        public List<JumpNode> Nodes { get; private set; }
        public byte ParentJumpNodeIndex;

        public static JumpNodeCollection GetStartingNode(int x, int y, INode goal) {
            return new JumpNodeCollection(x, y, goal);
        }

        public JumpNodeCollection(int x, int y, INode goal) : base(x, y, ActionCost.ImpossibleCost, goal) {
            Nodes = new List<JumpNode>(8);  // arbitrary initial capacity, yet to find out if this is a good initial capacity
        }

        public void AddNode(JumpNode node) {
            Nodes.Add(node);
        }

        public bool ContainsJump(int jump) {
            for (int i = 0; i < Nodes.Count; ++i) {
                if (Nodes[i].projectionAtThisNode.jump == jump) {
                    return true;
                }
            }
            return false;
        }
    }

    public class AStarPathfinder : IPathFinder {
        private const float MINIMUM_IMPROVEMENT = 0.1F;
        public static readonly IHeuristic Heuristic = new Manhattan();

        public int ExploreLimit { get; set; } = 500;

        private BaseMovement[] availableMoves;
        private BinaryNodeHeap<JumpNodeCollection> openSet;
        private Dictionary<int, JumpNodeCollection> nodeHashDictionary;
        private JumpNodeCollection startNode;
        private JumpNodeCollection endNode;
    
        public AStarPathfinder(JumpNodeCollection start, JumpNodeCollection end) {
            startNode = start;
            endNode = end;
            availableMoves = BaseMovement.GetAllMoves();
            nodeHashDictionary = new Dictionary<int, JumpNodeCollection>();
            openSet = new BinaryNodeHeap<JumpNodeCollection>();
        }

        public INode Start => startNode;

        public INode End => endNode;
        

        public IPath FindPath() {
            bool foundPath = false;
            openSet.Add(startNode);
            int count = 0;

            while (!foundPath) { 
                JumpNodeCollection currentNode = openSet.TakeLowest();

                if (currentNode.X == endNode.X && currentNode.Y == endNode.Y) {
                    foundPath = true;
                    break;
                }

                SearchNeighbours(currentNode);
                count++;

                if (count > ExploreLimit) {
                    break;
                }
            }

            List<JumpNodeCollection> retracedSteps = RetraceSteps();

        }

        private List<JumpNodeCollection> RetraceSteps() {
            List<JumpNodeCollection> steps = new List<JumpNodeCollection>();
            int hash = PathfindingUtils.GetNodeHash(endNode.X, endNode.Y);
            JumpNodeCollection currentNode = GetNode(-1, -1, hash);

            while (currentNode.X != startNode.X && currentNode.Y != startNode.Y) {
                hash = PathfindingUtils.GetNodeHash(currentNode.ParentX, currentNode.ParentY);
                JumpNodeCollection parentNode = GetNode(-1, -1, hash);
                steps.Add(parentNode);
                currentNode = parentNode;
            }

            return steps;
        }

        private void SearchNeighbours(JumpNodeCollection parent) {
            for (byte i = 0; i < parent.Nodes.Count; i++) {
                foreach (BaseMovement movement in availableMoves) {
                    int currentX = parent.X + movement.dX;
                    int currentY = parent.Y + movement.dY;
                    var movementProjection = parent.Nodes[i].projectionAtThisNode;
                    ActionCost nodeCost = movement.CalculateCost(ref movementProjection);

                    if (nodeCost.TotalCost != float.MaxValue) {
                        int hash = PathfindingUtils.GetNodeHash(currentX, currentY);
                        var neighbouringNode = GetNode(currentX, currentY, hash);

                        if (!neighbouringNode.ContainsJump(movementProjection.jump)) {
                            neighbouringNode.AddNode(new JumpNode(movementProjection));
                        }

                        float newNeighbourCost = parent.CostFromStart + nodeCost.TotalCost;

                        if (neighbouringNode.CostFromStart - newNeighbourCost > MINIMUM_IMPROVEMENT) {
                            neighbouringNode.ActionCost = nodeCost;
                            neighbouringNode.CostFromStart = newNeighbourCost;
                            neighbouringNode.SetParent(parent);
                            neighbouringNode.ParentJumpNodeIndex = i;
                            if (neighbouringNode.HeapIndex == -1) {
                                openSet.Add(neighbouringNode);
                            }
                            else {
                                openSet.Update(neighbouringNode);
                            }
                        }
                    }
                }
            }
        }

        private JumpNodeCollection GetNode(int x, int y, int hash) {
            if (nodeHashDictionary.ContainsKey(hash)) {
                return nodeHashDictionary[hash];
            }
            else {
                JumpNodeCollection node = new JumpNodeCollection(x, y, endNode);
                nodeHashDictionary.Add(hash, node);
                return node;
            }
        }
    }

}
