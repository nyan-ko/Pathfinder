using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nodes;

namespace Pathfinder {
    public class BinaryNodeHeap<T> where T : AbstractPathNode {
        private T[] nodes;
        public short Size { get; private set; } 

        public BinaryNodeHeap(short size = 1024) {
            Size = 0;
            nodes = new T[size]; 
        }

        public void Add(T node) {
            if (Size >= nodes.Length - 1) {
                Array.Resize(ref nodes, nodes.Length << 1);
            }
            node.HeapIndex = Size;
            nodes[Size] = node;
            Size++;
            MaintainHeapStructure(node);
        }

        public T TakeLowest() {
            if (Size == 0) {
                throw new Exception();
            }

            T result = nodes[0];
            T movedNode = nodes[Size - 1];

            UpdateNode(movedNode, 0);
            Size--;
            nodes[Size] = null;
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

                if (childIndex < Size - 1) {
                    T rightChild = nodes[childIndex + 1];
                    float rightChildCost = rightChild.Cost;
                    if (rightChildCost < childCost) {
                        childIndex++;
                        child = rightChild;
                        childCost = rightChildCost;
                    }
                }

                if (childCost >= movedNodeCost) {
                    break;
                }

                UpdateNode(movedNode, childIndex);
                UpdateNode(child, index);
                index = childIndex;
            }
            while ((childIndex = FindChildIndex(childIndex)) < Size);

            return result;
        }

        public void Update(T node) => MaintainHeapStructure(node);

        private void UpdateNode(T node, int newIndex) {
            nodes[newIndex] = node;
            node.HeapIndex = (short)newIndex;
        }

        private void MaintainHeapStructure(T node) {
            int index = node.HeapIndex;
            int parentIndex = FindParentIndex(index);
            T parentNode = nodes[parentIndex];
            float cost = node.Cost;
            while (index > 0 && parentNode.Cost > cost) {
                UpdateNode(parentNode, index);
                UpdateNode(node, parentIndex);

                index = parentIndex;
                parentIndex = FindParentIndex(index);
                parentNode = nodes[parentIndex];
            }
        }

        private int FindChildIndex(int parent) {
            return (parent << 1) + 1;
        }

        private int FindParentIndex(int child) {
            if (child == 0) {
                return 0;
            }
            return (child - 1) >> 1;
        }

        public bool Empty => Size == 0;
    }
}
