using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using ReLogic;
using Pathfinder;
using Pathfinder.Moves;

namespace Nodes
{

    public enum NodeStatus {
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
        private float _cost;
        private NodeStatus status;

        public int HeapIndex { get; set; }

        public int X => _x;

        public int Y => _y;

        public NodeStatus Status => status;

        public int ParentX => pX;

        public int ParentY => pY;

        public float Cost => _cost;

        protected AbstractPathNode(int x, int y, float cost, INode parent) {
            _x = x;
            _y = y;
            _cost = cost;

            pX = parent.X;
            pY = parent.Y;

            status = NodeStatus.Open;
        }
    }

    public class WorldNode {  // potentially unnecessary
        public int Z { get; private set; }
        
        public WorldNode(int z) {
            Z = z;
        }
    }

    public class ZWorldNodeCollection : AbstractPathNode {
        private List<WorldNode> nodes;

        public ZWorldNodeCollection(int x, int y, float cost, INode parent) : base(x, y, cost, parent) {
            nodes = new List<WorldNode>(8);  // arbitrary initial capacity, yet to find out if this is a good initial capacity
        }

        public void AddNode(WorldNode node) {
            if (!ContainsZ(node.Z))
                nodes.Add(node);
        }

        private bool ContainsZ(int z) {
            int length = nodes.Count;
            if (length == 0)
                return false;

            for (int i = 0; i < length; i++) {
                if (nodes[i].Z == z)
                    return true;
            }

            return false;
        }
    }

    public class AStarPathfinder : IPathFinder {

        private PlayerProjection player;
        private BaseMovement[] availableMoves;
        private BinaryNodeHeap<ZWorldNodeCollection> openSet;

        private ZWorldNodeCollection startNode;
        private ZWorldNodeCollection endNode;
    
        public AStarPathfinder(ZWorldNodeCollection start, ZWorldNodeCollection end) {
            startNode = start;
            endNode = end;
            openSet = new BinaryNodeHeap<ZWorldNodeCollection>();
        }

        public INode Start => startNode;

        public INode End => endNode;

        public float CalculateCost(INode node) {
            


        }

        

        public IPath FindPath() {

            bool foundPath = false;

            openSet.Add(startNode);
            
            while (!foundPath) {

                ZWorldNodeCollection node = openSet.TakeLowest();


            }
        }


    }

}
