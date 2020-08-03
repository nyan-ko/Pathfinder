using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nodes;

namespace Pathfinder {
    public class BinaryNodeHeap<T> where T : AbstractPathNode {
        private T[] nodes;
        public int Size { get; private set; }  // exclusive upper limit

        public BinaryNodeHeap(int size = 1024) {
            Size = size;
            nodes = new T[size]; 
        }

        public void Add(T node) {
            if (Size >= nodes.Length - 1) {
                Array.Resize(ref nodes, nodes.Length << 1);
            }
            Size++;
            node.HeapIndex = Size;
            nodes[Size] = node;
            MaintainHeapStructure(node);
        }

        public T TakeLowest() {
            if (Size == 0) {
                throw new Exception();
            }
            T result = nodes[0];
            T movedNode = nodes[Size];

            UpdateNode(movedNode, 0);
            nodes[Size] = null;

            Size--;

            result.HeapIndex = -1;
            if (Size < 2) {
                return result;
            }

            int index = 0;
            int childIndex = 1;

            float movedNodeCost = movedNode.Cost;
            do {
                T child = nodes[childIndex];
                float childCost = child.Cost;

                if (childIndex < Size) {
                    T rightChild = nodes[++childIndex];
                    float rightChildCost = rightChild.Cost;
                    if (rightChildCost < childCost) {
                        childIndex++;
                        child = rightChild;
                        childCost = rightChildCost;
                    }
                }

                if (childCost <= movedNodeCost) {
                    break;
                }

                UpdateNode(movedNode, childIndex);
                UpdateNode(child, index);
                index = childIndex;
            }
            while ((childIndex <<= 1) <= Size);

            result.Status = NodeStatus.Closed;
            return result;
        }

        public void Update(T node) => MaintainHeapStructure(node);

        private void UpdateNode(T node, int newIndex) {
            nodes[newIndex] = node;
            node.HeapIndex = newIndex;
        }

        private void MaintainHeapStructure(T node) {
            int index = node.HeapIndex;
            int parentIndex = (int)((uint)index >> 1);
            T parentNode = nodes[parentIndex];
            float cost = node.Cost;
            while (index > 0 && parentNode.Cost > cost) {

                UpdateNode(parentNode, index);
                UpdateNode(node, parentIndex);

                index = parentIndex;
                parentIndex = (int)((uint)index >> 1);
                parentNode = nodes[parentIndex];
            }
        }

        public bool Empty => Size == 0;
    }
}
