using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkipList
{
    public class SkipList<T> : IEnumerable<T> where T : IComparable
    {
        #region "Properties"
        /// <summary>
        /// Gets the number of elements in the skip list.
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get { return _count; }
        }
        #endregion

        const int MAX_LEVEL = 16;

        const double P = 0.5d;
        int _level;
        int _count;
        Node<T> _header;
        Random _rand = new Random();

        #region "Node Class"
        internal class Node<TT> : IDisposable where TT : IComparable
        {
            public Node<TT>[] NextNodes;

            public TT Value;// = new TT();
            /// <summary>
            /// Node constructor
            /// </summary>
            /// <param name="pLevel">Level of the node</param>
            /// <param name="pValue">Value to store in the node</param>
            public Node(int pLevel, TT pValue)
            {
                NextNodes = new Node<TT>[pLevel];
                Value = pValue;
            }

            /// <summary>
            /// Node constructor
            /// </summary>
            /// <param name="pLevel">Level of the node</param>
            public Node(int pLevel)
            {
                NextNodes = new Node<TT>[pLevel];
            }

            /// <summary>
            /// Node constructor
            /// </summary>
            public Node()
            {
                NextNodes = new Node<TT>[1];
            }

            /// <summary>
            /// Dispose
            /// </summary>
            public void Dispose()
            {
                for (int i = 0; i < NextNodes.Length; i++)
                {
                    NextNodes[i] = null;
                }
            }
        }
        #endregion

        #region "SkipList Constuctor"
        /// <summary>
        /// Skip list constructor
        /// </summary>
        public SkipList()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the skip list to default values
        /// </summary>
        private void Initialize()
        {
            _count = 0;
            _level = 1;
            _header = new Node<T>(MAX_LEVEL);

            for (Int16 i = 0; i < MAX_LEVEL; i++)
            {
                _header.NextNodes[i] = _header;
            }
        }
        #endregion

        #region "SkipList Private Functions"
        /// <summary>
        /// Generates the level at which the next node will reside.
        /// </summary>
        /// <returns>Level</returns>
        private int GenerateNewLevel()
        {
            int level = 1;

            while (_rand.NextDouble() < P && level < MAX_LEVEL && level <= _level)
            {
                level += 1;
            }

            return level;
        }

        /// <summary>
        /// Search the skip list for a given value.
        /// </summary>
        /// <param name="pValue">Value to search for in the skip list.</param>
        /// <param name="pNode">Node which contains the given value.</param>
        /// <returns>True if the value is found in the skip list.</returns>
        private bool Search(T pValue, out Node<T> pNode)
        {
            Node<T>[] nodesToUpdate;

            return Search(pValue, out pNode, out nodesToUpdate);
        }

        /// <summary>
        /// Search the skip list for a given value.
        /// </summary>
        /// <param name="pValue">Value to search for in the skip list.</param>
        /// <param name="pNodesToUpdate">Nodes whose pointers will need updating if the value is inserted.</param>
        /// <returns>True if the value is found in the skip list.</returns>
        private bool Search(T pValue, out Node<T>[] pNodesToUpdate)
        {
            Node<T> retval;
            return Search(pValue, out retval, out pNodesToUpdate);
        }

        /// <summary>
        /// Search the skip list for a given value.
        /// </summary>
        /// <param name="pValue">Value to search for in the skip list.</param>
        /// <param name="pNode">Node which contains the given value.</param>
        /// <param name="pNodesToUpdate">Nodes whose pointers will need updating if the value is inserted.</param>
        /// <returns>True if the value is found in the skip list.</returns>
        private bool Search(T pValue, out Node<T> pNode, out Node<T>[] pNodesToUpdate)
        {
            if (pValue == null)
            {
                throw new ArgumentNullException("pValue");
            }
            //Start at the beginning of our list
            Node<T> currentNode = _header;
            Node<T>[] nodesToUpdate = new Node<T>[MAX_LEVEL];
            bool found = false;

            //While we have a level to evaluate

            for (int i = _level - 1; i >= 0; i--)
            {
                //If the next node is not null and is less than our search value use it as our next search node
                while (currentNode.NextNodes[i] != _header && currentNode.NextNodes[i].Value.CompareTo(pValue) < 0)
                {
                    currentNode = currentNode.NextNodes[i];
                }

                //We dropped down a level in our skip list.  Mark it as a node that will need updating
                nodesToUpdate[i] = currentNode;
            }

            //This node will either be greater than or equal to our search value
            currentNode = currentNode.NextNodes[0];

            //If it is equal we have found the value
            if (currentNode != _header && currentNode.Value.CompareTo(pValue) == 0)
            {
                found = true;
            }

            pNode = currentNode;
            pNodesToUpdate = nodesToUpdate;
            return found;
        }

        /// <summary>
        /// Inserts the given value into the skip list.
        /// </summary>
        /// <param name="pValue">Value that will be inserted into the skip list.</param>
        /// <param name="pNodesToUpdate">Nodes that will need updating to point to our inserted value.</param>
        /// <remarks>
        /// You will want to run a search before inserting.
        /// This will get all the nodes that will need their pointers updated when the new value is inserted.
        /// </remarks>
        private void Insert(T pValue, Node<T>[] pNodesToUpdate)
        {
            //Get the level of our new node
            int level = GenerateNewLevel();

            //If it is the new highest node
            if (level > _level)
            {
                //Update the new level to point to our null node
                for (int i = _level; i < level; i++)
                {
                    pNodesToUpdate[i] = _header;
                }
                _level = level;
            }

            Node<T> insertNode = new Node<T>(level, pValue);

            //Update pointers based off where we dropped down in the skip list:
            for (int i = 0; i < level; i++)
            {
                //Our node must point to what the previous node WAS pointing to
                insertNode.NextNodes[i] = pNodesToUpdate[i].NextNodes[i];

                //Previous node must now point to our inserted node
                pNodesToUpdate[i].NextNodes[i] = insertNode;
            }

            _count += 1;
        }
        #endregion

        #region "Public Functions"
        public bool Contains(T pValue)
        {
            Node<T> n;
            return Search(pValue, out n);
        }
        /// <summary>
        /// Copies the skip list to a one-dimensional System.Array object, starting at the specified index.
        /// </summary>
        /// <param name="pArray">The one-dimentional System.Array object that is the destination of the objects copied from the skip list.  The System.Array must have zero-based index.</param>
        /// <param name="pIndex">The zero-based index in the array at which copying begins.</param>
        /// <remarks></remarks>
        public void CopyTo(ref Array pArray, int pIndex)
        {
            if (pArray == null)
            {
                throw new ArgumentNullException("pArray");
            }
            if (pIndex < 0 || pIndex >= pArray.Length)
            {
                throw new ArgumentOutOfRangeException("Invalid starting index");
            }
            if (pIndex + _count > pArray.Length)
            {
                throw new Exception("Copying at index will result in values being trucated.");
            }

            //Start at the beginning of our list
            Node<T> currentNode = _header.NextNodes[0];

            //While we are not at the end copy the value to the array
            while (currentNode != _header)
            {
                pArray.SetValue(currentNode.Value, pIndex);
                currentNode = currentNode.NextNodes[0];
                pIndex += 1;
            }
        }

        /// <summary>
        /// Adds an element with the specified value to the skip list object.
        /// </summary>
        /// <param name="pValue">Value of the element to add.</param>
        public void Add(T pValue)
        {
            Node<T>[] nodesToUpdate;// = new Node<T>[MAX_LEVEL + 1];

            //Search for where our value will be inserted
            Search(pValue, out nodesToUpdate);
            //Insert our new value
            Insert(pValue, nodesToUpdate);
        }

        /// <summary>
        /// Removes all elements from the skip list object.
        /// </summary>
        public void Clear()
        {
            if (Count == 0) { return; }

            //Start at the beginning of our list
            Node<T> currentNode = _header.NextNodes[0];
            Node<T> previousNode = null;

            //While we are not at the end of the array
            while (currentNode != _header)
            {
                //Move to the next node
                previousNode = currentNode;
                currentNode = currentNode.NextNodes[0];
                //Dispose of the previous
                previousNode.Dispose();
            }

            //Reinitialize the skip list as blank
            Initialize();
        }

        /// <summary>
        /// Retrieves the element stored at the specified zero-based index.
        /// </summary>
        /// <param name="pIndex">Zero-based index at which to retrieve the element.</param>
        /// <returns>Element stored at the specified index.</returns>
        public T Index(int pIndex)
        {
            if (pIndex > _count || pIndex < 0)
            {
                throw new IndexOutOfRangeException("Invalid index");
            }

            //Start at the beginning of the skip list
            Node<T> currentNode = _header;

            //Cycle until we have reached the element
            for (int i = 0; i <= pIndex; i++)
            {
                currentNode = currentNode.NextNodes[0];
            }

            return currentNode.Value;
        }

        /// <summary>
        /// Returns the first element of the skip list.
        /// </summary>
        /// <returns>First element of the skip list</returns>
        public T First()
        {
            return _header.NextNodes[0].Value;
        }

        /// <summary>
        /// Removes the element with the specified value from the skip list.
        /// </summary>
        /// <param name="pValue">Value of the element to remove.</param>
        public void Remove(T pValue)
        {
            Node<T>[] nodesToUpdate;
            Node<T> currentNode;

            //Find the value we want to remove
            if (Search(pValue, out currentNode, out nodesToUpdate))
            {
                //Update the previous nodes to point to the next node
                for (int i = 0; i < _level; i++)
                {
                    if (nodesToUpdate[i].NextNodes[i] == currentNode)
                    {
                        nodesToUpdate[i].NextNodes[i] = currentNode.NextNodes[i];
                    }
                }

                //Dispose of the removed node
                currentNode.Dispose();

                //Check if that node was the node with the highest level
                while (_level > 1 && _header.NextNodes[_level - 1] == _header)
                {
                    _level -= 1;
                }

                _count -= 1;
            }
        }

        /// <summary>
        /// Gets the values contained in the skip list object.
        /// </summary>
        /// <returns>ICollection containing all values in the skip list.</returns>
        public ICollection Values
        {
            get
            {
                //Start at the beginning of the skip list
                Node<T> currentNode = _header.NextNodes[0];

                ArrayList retval = new ArrayList();
                //While we are not at the end
                while (currentNode != _header)
                {
                    //Add the value to the collection
                    retval.Add(currentNode.Value);
                    currentNode = currentNode.NextNodes[0];
                }

                return retval;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new SkipListEnumerator<T>(_header);
        }

        private IEnumerator GetEnumerator1()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator1();
        }
        #endregion

        #region "Enumerator"
        public class SkipListEnumerator<TT> : IEnumerator<TT> where TT : IComparable
        {
            private Node<TT> _header;
            private Node<TT> _current;
            internal SkipListEnumerator(Node<TT> pHeader)
            {
                _current = pHeader;
            }

            public TT Current 
            {
                get 
                {
                    if (_current == _header)
                    {
                        throw new InvalidOperationException();
                    }

                    return _current.Value;
                }
            }

            object IEnumerator.Current 
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                _current = _current.NextNodes[0];
                return _current != _header;
            }

            public void Reset()
            {
                _current = _header;
            }

            public void Dispose()
            {
                _header = null;
                _current = null;
            }
        }
        #endregion
    }
}
