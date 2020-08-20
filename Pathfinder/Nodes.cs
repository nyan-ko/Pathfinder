using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using ReLogic;
using Pathfinder;
using Pathfinder.Projections;
using Pathfinder.Moves;
using Pathfinder.Heuristics;
using Pathfinder.Input;
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
            HeuristicCostToGoal = CalculateHeuristicCostToGoal(goalX, goalY);
            HeapIndex = -1;
        }

        public void SetParent(INode parent) {
            pX = parent.X;
            pY = parent.Y;
        }

        private float CalculateHeuristicCostToGoal(int x, int y) => AStarPathfinder.Heuristic.EstimateCost(_x, _y, x, y);  //TODO change to a globally held heuristic function or pass as parameter
    }

    public class JumpNode {
        public PlayerProjection ProjectionAtThisNode;
        public byte Input;

        public JumpNode(PlayerProjection projection, byte input) {
            ProjectionAtThisNode = projection;
            Input = input;
        }
    }

    public class JumpNodeCollection : AbstractPathNode {
        public List<JumpNode> Nodes { get; private set; }
        public byte ParentJumpNodeIndex;

        public JumpNodeCollection(int x, int y, int goalX, int goalY) : base(x, y, goalX, goalY) {
            Nodes = new List<JumpNode>(8);  // arbitrary initial capacity, yet to find out if this is a good initial capacity
        }

        public void AddNode(JumpNode node) {
            Nodes.Add(node);
        }

        public bool ContainsJump(int jump) {
            if (Nodes.Count == 0)
                return false;

            for (int i = 0; i < Nodes.Count; ++i) {
                if (Nodes[i].ProjectionAtThisNode.jump == jump) {
                    return true;
                }
            }
            return false;
        }
    }

    public class AStarPathfinder : IPathFinder {
        private const float MINIMUM_IMPROVEMENT = 0.1F;
        public static readonly IHeuristic Heuristic = new Manhattan();

        public int ExploreLimit { get; set; } = 2500;

        private BaseMovement[] availableMoves;
        private BinaryNodeHeap<JumpNodeCollection> openSet;
        private Dictionary<long, JumpNodeCollection> nodeHashDictionary;
        private JumpNodeCollection startNode;
        private JumpNodeCollection endNode;
    
        public AStarPathfinder(int startX, int startY, int endX, int endY, Player startingProjection) {
            startNode = new JumpNodeCollection(startX, startY, endX, endY) { CostFromStart = 0 };
            startNode.Nodes.Add(new JumpNode(new PlayerProjection(startingProjection), 0));
            endNode = new JumpNodeCollection(endX, endY, endX, endY);
            availableMoves = BaseMovement.GetAllMoves();
            nodeHashDictionary = new Dictionary<long, JumpNodeCollection>();
            openSet = new BinaryNodeHeap<JumpNodeCollection>();
        }

        public IEnumerable<INode> DebugGetExploredNodes => nodeHashDictionary.Values;

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

                if (count >= ExploreLimit) {
                    break;
                }
            }

            if (foundPath) {
                PathfinderTriggersSet triggersSet = new PathfinderTriggersSet();
                List<Trigger> triggers = new List<Trigger>();
                int lastJumpNodeIndex = 0;

                foreach (var step in RetraceSteps()) {
                    triggers.Add(new Trigger(step.Nodes[lastJumpNodeIndex].Input, step.ActionCost.TotalCost, step.CostFromStart));
                    lastJumpNodeIndex = step.ParentJumpNodeIndex;
                }

                triggersSet.SetList(triggers);

                return new AStarPath(true, triggersSet);
            }

            return null; // idk what to do yet for this
        }

        private List<JumpNodeCollection> RetraceSteps() {
            List<JumpNodeCollection> steps = new List<JumpNodeCollection>();
            long hash = PathfindingUtils.GetNodeHash(endNode.X, endNode.Y);
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
                byte input = 1;

                foreach (BaseMovement movement in availableMoves) {
                    int currentX = parent.X + movement.dX;
                    int currentY = parent.Y - movement.dY;
                    var movementProjection = parent.Nodes[i].ProjectionAtThisNode;
                    var nodeCost = movement.CalculateCost(ref movementProjection);

                    if (nodeCost.TotalCost != float.MaxValue) {
                        long hash = PathfindingUtils.GetNodeHash(currentX, currentY);
                        var neighbouringNode = GetNode(currentX, currentY, hash);

                        if (!neighbouringNode.ContainsJump(movementProjection.jump)) {
                            neighbouringNode.AddNode(new JumpNode(movementProjection, input));
                        }

                        float costToGetHere = parent.CostFromStart + nodeCost.TotalCost;
                        float newNeighbourCost = costToGetHere + neighbouringNode.HeuristicCostToGoal;

                        if (neighbouringNode.Cost - newNeighbourCost > MINIMUM_IMPROVEMENT) {
                            neighbouringNode.ActionCost = nodeCost;
                            neighbouringNode.CostFromStart = costToGetHere;
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

                    input++;
                }
            }
        }

        private JumpNodeCollection GetNode(int x, int y, long hash) {
            if (nodeHashDictionary.ContainsKey(hash)) {
                return nodeHashDictionary[hash];
            }
            else {
                JumpNodeCollection node = new JumpNodeCollection(x, y, endNode.X, endNode.Y);
                nodeHashDictionary.Add(hash, node);
                return node;
            }
        }
    }

}
