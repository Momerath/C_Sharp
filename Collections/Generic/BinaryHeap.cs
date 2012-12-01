/*
******************************************************************
Copyright (c) 2012, Ron Whittle
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above
      copyright notice, this list of conditions and the following
      disclaimer in the documentation and/or other materials provided
      with the distribution.
    * Redistributions in any form in whole or in part requires an email
      be sent to usage@mewkus.com where the Subject of the message is
      the name of the source file used and the Body of the message
      includes any URLs, product names, etc. where this code is being
      used.
    * Neither the name of mewkus.com nor the names of its contributors
      may be used to endorse or promote products derived from this 
      software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
POSSIBILITY OF SUCH DAMAGE.

******************************************************************
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace rwhittle.Collections.Generic {
    /// <summary>
    /// Represents a heap of objects. A heap is a semi-ordered tree in which the parent is larger (or smaller) than its children.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the BinaryHeap</typeparam>
    public class BinaryHeap<T> : IHeap<T> {
        T[] heap = new T[1];
        int position = 1;
        IComparer<T> comparer;
        int version = 0;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the BinaryHeap<T> class that is empty.
        /// </summary>
        public BinaryHeap() {
            comparer = Comparer<T>.Default;
        }

        /// <summary>
        /// Initializes a new instance of the BinaryHeap<T> class that is empty and uses the supplied IComparer<T> to determine ordering.
        /// </summary>
        /// <param name="comp">The IComparer<T> to use in the ordering of the heap.</param>
        public BinaryHeap(IComparer<T> comp) {
            if (comp == null) {
                comparer = Comparer<T>.Default;
            } else {
                comparer = comp;
            }
        }

        /// <summary>
        /// Initializes a new instance of the BinaryHeap<T> class that contains the elements from the supplied IEnumerable<T>.
        /// </summary>
        /// <param name="enumerable">The source for the initial elements of the heap.</param>
        public BinaryHeap(IEnumerable<T> enumerable) {
            comparer = Comparer<T>.Default;

            if (enumerable != null) {
                foreach (T item in enumerable) {
                    Add(item);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the BinaryHeap<T> class that contains the elements from the supplied IEnumerable<T>
        /// ordered by the supplied IComparer<T>.
        /// </summary>
        /// <param name="enumerable">The source for the initial elements of the heap.</param>
        /// <param name="comp">The IComparer<T> to use for ordering the heap.</param>
        public BinaryHeap(IEnumerable<T> enumerable, IComparer<T> comp) {
            if (comp == null) {
                comparer = Comparer<T>.Default;
            } else {
                comparer = comp;
            }

            if (enumerable != null) {
                foreach (T item in enumerable) {
                    Add(item);
                }
            }
        }
        #endregion

        #region Heap Methods
        /// <summary>
        /// Adds the supplied item to the heap, reordering it as needed.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public void Add(T item) {
            if (heap.Length <= position) {
                Array.Resize(ref heap, heap.Length * 2);
            }
            heap[position] = item;
            Swim(position);
            position++;
            version++;
        }

        /// <summary>
        /// Returns the top of the heap.
        /// </summary>
        /// <returns>Item at the top of the heap.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown when the heap is empty.</exception>
        public T Peek() {
            if (position <= 1) {
                throw new InvalidOperationException("The BinaryHeap<T> is empty");
            }

            return heap[1];
        }

        /// <summary>
        /// Removes the top of the heap and returns it.
        /// </summary>
        /// <returns>Item at the top of the heap</returns>
        /// <exception cref="System.InvalidOperationException">Thrown when the heap is empty.</exception>
        public T Pop() {
            if (position <= 1) {
                throw new InvalidOperationException("The BinaryHeap<T> is empty");
            }
            T result = heap[1];
            position--;
            heap[1] = heap[position];
            Sink(1);
            version++;

            return result;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Moves a node down the heap tree until the heap order is restored.
        /// </summary>
        /// <param name="node">Index of the node to be moved</param>
        private void Sink(int node) {
            int child = SelectLargestChild(node);
            while (child < position && comparer.Compare(heap[node], heap[child]) < 0) {
                heap[0] = heap[node];
                heap[node] = heap[child];
                heap[child] = heap[0];
                node = child;
                child = SelectLargestChild(node);
            }
        }

        /// <summary>
        /// Moves a node up the heap tree until the heap order is restored.
        /// </summary>
        /// <param name="node">Index of the node to be moved.</param>
        private void Swim(int node) {
            int parent = node / 2;
            while (node > 1 && comparer.Compare(heap[node], heap[parent]) > 0) {
                heap[0] = heap[node];
                heap[node] = heap[parent];
                heap[parent] = heap[0];
                node = parent;
                parent = node / 2;
            }
        }

        /// <summary>
        /// Returns the index of the largest child node of the specified parent node or the index
        /// where the next node will be inserted if there are no child nodes.
        /// </summary>
        /// <param name="leftChild">The index of the parent node</param>
        /// <returns>Index of the larger of the child nodes, or the index where the next node will
        /// be inserted if there are no child nodes.</returns>
        /// <remarks>"Largest" in this context is whichever node is considered "larger" by the
        /// IComparer being used.</remarks>
        private int SelectLargestChild(int node) {
            int leftChild = node;
            int rightChild = leftChild + 1;
            if (rightChild < position) {
                return comparer.Compare(heap[leftChild], heap[rightChild]) < 0 ? rightChild : leftChild;
            } else if (leftChild < position) {
                return leftChild;
            } else {
                return position;
            }
        }

        /// <summary>
        /// Returns the index in the heap of the specified item.
        /// </summary>
        /// <param name="item">Item to locate.</param>
        /// <returns>Index of the item, or -1 if not found.</returns>
        private int IndexOf(T item) {
            for (int i = 1; i < position; i++) {
                if (comparer.Compare(heap[i], item) == 0) {
                    return i;
                }
            }
            return -1;
        }
 
        #endregion

        #region Interface methods
        public IEnumerator<T> GetEnumerator() {
            return new BinaryHeap<T>.Enumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return new BinaryHeap<T>.Enumerator(this);
        }

        public void Clear() {
            position = 1;
        }

        public bool Contains(T item) {
            for(int i = 1; i < position; i++) {
                if (comparer.Compare(heap[i], item) == 0) {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex) {
            if (array == null) {
                throw new ArgumentNullException("Array is null.");
            }

            if (arrayIndex < array.GetLowerBound(0) || arrayIndex >= array.GetUpperBound(0)) {
                throw new IndexOutOfRangeException("Specified start index is out of range of the array.");
            }

            if (array.GetUpperBound(0) < arrayIndex + position - 1) {
                throw new ArgumentException("Array length too short to copy into at specified index.");
            }

            for (int i = 1; i < position; i++) {
                array[arrayIndex + i - 1] = heap[i];
            }
        }

        public int Count {
            get { return position - 1; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public bool Remove(T item) {
            int index = IndexOf(item);
            if (index != -1) {
                position--;
                heap[index] = heap[position];
                Sink(index);
                version++;
                return true;
            }
            return false;
        }
        #endregion

        #region Internal Classes
        public class HeapMinimum : Comparer<T> {
            public override int Compare(T x, T y) {
                Comparer<T> comp = Comparer<T>.Default;
                return 0 - comp.Compare(x, y);
            }
        }

        public struct Enumerator : IEnumerator<T>, IDisposable {
            BinaryHeap<T> heap;
            int index;
            int version;
            T current;

            public T Current {
                get {
                    if (index < 1) {
                        if (index == 0) {
                            throw new InvalidOperationException("Enumeration not started");
                        } else {
                            throw new InvalidOperationException("Enumeration ended");
                        }
                    }
                    return current;
                }
            }

            Object System.Collections.IEnumerator.Current {
                get {
                    if (index < 1) {
                        if (index == 0) {
                            throw new InvalidOperationException("Enumeration not started");
                        } else {
                            throw new InvalidOperationException("Enumeration ended");
                        }
                    }
                    return current;
                }
            }

            public void Dispose() {
                index = -1;
                current = default(T);
            }

            internal Enumerator(BinaryHeap<T> h) {
                heap = h;
                version = h.version;
                index = 0;
                current = default(T);
            }

            public bool MoveNext() {
                if (version != heap.version) {
                    throw new InvalidOperationException("Enumeration failed");
                }
                if (index == -1) {
                    return false;
                }
                index++;
                if (index == heap.position) {
                    index = -1;
                    current = default(T);
                    return false;
                }
                current = heap.heap[index];
                return true;
            }

            public void Reset() {
                if (version != heap.version) {
                    throw new InvalidOperationException("Enumeration failed");
                }
                index = 0;
                current = default(T);
            }
        }
        #endregion
    }
}