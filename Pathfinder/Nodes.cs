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

        public float Cost => (ActionCost.TotalCost + CostFromStart) + HeuristicCostToGoal;

        public int HeapIndex { get; set; }

        public int X => _x;

        public int Y => _y;

        public NodeStatus Status { get; set; }

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

    public class WorldNode {
        public PlayerProjection projectionAtThisNode;

        public WorldNode(PlayerProjection projection) {
            projectionAtThisNode = projection;
        }
    }

    public class ZWorldNodeCollection : AbstractPathNode {
        private List<WorldNode> nodes;

        public static ZWorldNodeCollection GetStartingNode(int x, int y, INode goal) {
            return new ZWorldNodeCollection(x, y, goal);
        }


        public ZWorldNodeCollection(int x, int y, INode goal) : base(x, y, ActionCost.ImpossibleCost, goal) {
            nodes = new List<WorldNode>(8);  // arbitrary initial capacity, yet to find out if this is a good initial capacity
        }

        public void AddNode(WorldNode node) {
            nodes.Add(node);
        }

        public bool ContainsJump(int jump) {
            for (int i = 0; i < nodes.Count; ++i) {
                if (nodes[i].projectionAtThisNode.jump == jump) {
                    return true;
                }
            }
            return false;
        }
    }

    public class AStarPathfinder : IPathFinder {
        private const float MINIMUM_IMPROVEMENT = 0.1F;
        public static readonly IHeuristic Heuristic = new Manhattan();

        private PlayerProjection player;
        private BaseMovement[] availableMoves;
        private BinaryNodeHeap<ZWorldNodeCollection> openSet;
        private Dictionary<int, ZWorldNodeCollection> nodeHashDictionary;

        private ZWorldNodeCollection startNode;
        private ZWorldNodeCollection endNode;
    
        public AStarPathfinder(ZWorldNodeCollection start, ZWorldNodeCollection end) {
            startNode = start;
            endNode = end;

            availableMoves = BaseMovement.GetAllMoves();
            nodeHashDictionary = new Dictionary<int, ZWorldNodeCollection>();
            openSet = new BinaryNodeHeap<ZWorldNodeCollection>();
        }

        public INode Start => startNode;

        public INode End => endNode;
        

        public IPath FindPath() {

            bool foundPath = false;

            openSet.Add(startNode);
            
            while (!foundPath) {
                ZWorldNodeCollection node = SearchNeighbours(ref player, openSet.TakeLowest());



            }
        }

        private ZWorldNodeCollection SearchNeighbours(ref PlayerProjection player, ZWorldNodeCollection parent) {

            ActionCost cost = ActionCost.ImpossibleCost;
            int dX = 0;
            int dY = 0;
            PlayerProjection bestProjection = player;
            bool actuallyFoundNewNode = false;

            foreach (BaseMovement movement in availableMoves) {
                int currentX = parent.X + movement.dX;
                int currentY = parent.Y + movement.dY;

                PlayerProjection currentProjection = player;
                ActionCost nodeCost = movement.CalculateCost(ref currentProjection);

                if (nodeCost.TotalCost != float.MaxValue) {
                    int hash = PathfindingUtils.GetNodeHash(currentX, currentY);
                    var neighbouringNode = GetNode(currentX, currentY, hash);

                    if (!neighbouringNode.ContainsJump(currentProjection.jump)) {
                        neighbouringNode.AddNode(new WorldNode(currentProjection));
                    }

                    float newNeighbourCost = parent.CostFromStart + nodeCost.TotalCost;

                    if (neighbouringNode.CostFromStart - newNeighbourCost > MINIMUM_IMPROVEMENT) {
                        neighbouringNode.CostFromStart = newNeighbourCost;
                        neighbouringNode.SetParent(parent);
                        if (neighbouringNode.Status == NodeStatus.Open) {
                            openSet.Add(neighbouringNode);
                        }
                        else {
                            openSet.Update(neighbouringNode);
                        }
                    }
                }

                if (nodeCost < cost) {
                    actuallyFoundNewNode = true;
                    dX = movement.dX;
                    dY = movement.dY;
                    cost = nodeCost;
                    bestProjection = currentProjection;
                }
            }

            return actuallyFoundNewNode ? new ZWorldNodeCollection(parent.X + dX, parent.Y + dY, endNode) : null;
        }

        private ZWorldNodeCollection GetNode(int x, int y, int hash) {
            if (nodeHashDictionary.ContainsKey(hash)) {
                return nodeHashDictionary[hash];
            }
            else {
                ZWorldNodeCollection node = new ZWorldNodeCollection(x, y, endNode);
                nodeHashDictionary.Add(hash, node);
                return node;
            }
        }
    }

}
