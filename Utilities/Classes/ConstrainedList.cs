using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utiliities
{
    /// <summary>
    /// A list with a maximum number of items. When the list is full, added items will replace older items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConstrainedList<T> : IList<T>, ICollection<T>, IEnumerable<T> where T : class
    {
        private T[] itemArray;
        private int oldestItemIndex;
        public bool replaceIfAtMax;
        private int count;

        private struct IndexMap
        {
            public int internalIndex;
            public int externalIndex;

            public static IndexMap None = new IndexMap { internalIndex = -1, externalIndex = -1 };

            public static bool operator ==(IndexMap map1, IndexMap map2)
            {
                return map1.internalIndex == map2.internalIndex && map1.externalIndex == map2.externalIndex;
            }

            public static bool operator !=(IndexMap map1, IndexMap map2)
            {
                return !(map1 == map2);
            }

            public override int GetHashCode()
            {
                return (27 + (internalIndex.GetHashCode())) * 27 + externalIndex.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }

                if (!(obj is IndexMap))
                {
                    return false;
                }

                // Return true if the fields match
                return ((IndexMap)obj) == this;
            }
        }

        private IndexMap lastMappedIndex;

        /// <summary>
        /// Constructs a ConstrainedList
        /// </summary>
        /// <param name="maxItems">The maximum number of objects this ConstrainedList can store</param>
        /// <param name="replaceIfAtMax">Whether to replace the oldest object in the list if no space is found when adding an object to the ConstrainedList. Defaults to true</param>
        public ConstrainedList(int maxItems, bool replaceIfAtMax = true)
        {
            itemArray = new T[maxItems];
            oldestItemIndex = 0;
            this.replaceIfAtMax = replaceIfAtMax;
            count = 0;
            lastMappedIndex = IndexMap.None;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ConstrainedListEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            //Refers to above method
            return GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the ConstrainedList. The added object will not necessarily be added to the end of the list; it may be in the middle, at the beginning, or may replace another object.
        /// </summary>
        /// <param name="item">The object to add to the ConstrainedList</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the ConstrainedList is full and replaceIfAtMax is false.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="item"/> is null.</exception>
        public void Add(T item)
        {
            if (Count == MaxItems && replaceIfAtMax == false)
            {
                throw new InvalidOperationException("ConstrainedList is full, no more items can be added");
            }

            EnsureNotNull(item);

            int addIndex = oldestItemIndex;

            //First, try to find an empty slot and put an item in it.
            for (int i = 0; i < itemArray.Length; i++)
            {
                int actualIndex = (i + oldestItemIndex) % itemArray.Length;
                if (itemArray[actualIndex] == null)
                {
                    addIndex = actualIndex;
                    break;
                }
            }

            //TODO: possibly optimize this to change older item instead of creating a new one.
            //Or allow Insert to be overloaded so it can be implemented per-class instead of having to do Reflection wizardry
            if (itemArray[addIndex] == null)
            {
                Count++;
            }

            itemArray[addIndex] = item;
            oldestItemIndex = (oldestItemIndex + 1) % itemArray.Length;
        }

        /// <summary>
        /// Removes all items from the ConstrainedList.
        /// </summary>
        public void Clear()
        {
            itemArray.Fill(null);
            Count = 0;
        }

        /// <summary>
        /// Determines whether the ConstrainedList contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the ConstrainedList</param>
        /// <returns>true if item is found in the ConstrainedList; otherwise, false.</returns>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="item"/> is null.</exception>
        public bool Contains(T item)
        {
            EnsureNotNull(item);
            return itemArray.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the ConstrainedList to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ConstrainedList.</param>
        /// <param name="arrayIndex">The index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            //this.ToArray() must be used to get indexes correct
            Array.Copy(this.ToArray(), 0, array, arrayIndex, MaxItems);
        }

        /// <summary>
        /// Gets the number of elements contained in the ConstrainedList.
        /// </summary>
        public int Count
        {
            get
            {
                return count;
            }
            private set
            {
                count = value;

                //if (count != itemArray.Where(item => item != null).Count())
                //{
                //    Debugger.Break();
                //}

                lastMappedIndex = IndexMap.None;
            }
        }

        /// <summary>
        /// Gets the maximum number of elements that can be contained in the ConstrainedList.
        /// </summary>
        public int MaxItems
        {
            get
            {
                return itemArray.Length;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the ConstrainedList is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the ConstrainedList.
        /// </summary>
        /// <param name="item">The object to remove from the ConstrainedList.</param>
        /// <returns>true if <paramref name="item"/> was removed from the ConstrainedList; otherwise, false.</returns>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="item"/> is null.</exception>
        public bool Remove(T item)
        {
            EnsureNotNull(item);

            int internalIndex = Array.IndexOf(itemArray, item);
            if (internalIndex >= 0)
            {
                itemArray[internalIndex] = null;
                Count--;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines the index of a specific item in the ConstrainedList.
        /// </summary>
        /// <param name="item">The object to locate in the ConstrainedList.</param>
        /// <returns>The index of item if found in the ConstrainedList; otherwise, -1.</returns>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="item"/> is null.</exception>
        public int IndexOf(T item)
        {
            EnsureNotNull(item);

            int internalIndex = Array.IndexOf(itemArray, item);

            if (internalIndex < 0)
            {
                return -1;
            }
            else
            {
                return InternalIndexToExternalIndex(internalIndex);
            }
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();

            //NOTE: this implementation does not work!
            //EnsureNotNull(item);

            //if (Count == MaxItems)
            //{
            //    throw new InvalidOperationException("ConstrainedList is full");
            //}
            //else
            //{
            //    int internalIndexAbove = ExternalIndexToInternalIndex(index);
            //    int internalIndexBelow = ExternalIndexToInternalIndex(index - 1);

            //    if (itemArray[internalIndex] != null)
            //    {
            //        //We need to shift stuff around to make an insertion point

            //        //Find the nearest null value
            //        int diff = 1;

            //        bool found = false;
            //        int foundIndex = -1;

            //        while (!found)
            //        {
            //            int upperCheck = internalIndex + diff;
            //            int lowerCheck = internalIndex - diff;

            //            if (upperCheck < itemArray.Length && itemArray[upperCheck] == null)
            //            {
            //                found = true;
            //                foundIndex = upperCheck;
            //            }
            //            else if (lowerCheck >= 0 && itemArray[lowerCheck] == null)
            //            {
            //                found = true;
            //                foundIndex = lowerCheck;
            //            }
            //        }

            //        //shift stuff up
            //        if (foundIndex > internalIndex)
            //        {
            //            for (int i = foundIndex; i > internalIndex; i--)
            //            {
            //                itemArray[i] = itemArray[i - 1];
            //            }
            //        }
            //        else //shift stuff down but dont shift the one at the internal index so that our new item is inserted at the correct point
            //        {
            //            for (int i = foundIndex; i < internalIndex - 1; i--)
            //            {
            //                itemArray[i] = itemArray[i + 1];
            //            }
            //            internalIndex--;
            //        }
            //    }

            //    itemArray[internalIndex] = item;
            //    Count++;
            //}
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            int internalIndex = ExternalIndexToInternalIndex(index);
            itemArray[internalIndex] = null;
            Count--;
        }

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get or set.</param>
        /// <returns>The item at the specified index.</returns>
        public T this[int index]
        {
            get
            {
                return itemArray[ExternalIndexToInternalIndex(index)];
            }
            set
            {
                EnsureNotNull(value);
                itemArray[ExternalIndexToInternalIndex(index)] = value;
            }
        }

        private int InternalIndexToExternalIndex(int internalIndex)
        {
            if (Count == MaxItems)
            {
                return internalIndex;
            }

            return itemArray.Take(internalIndex + 1).Count(item => item != null) - 1;
        }

        private int ExternalIndexToInternalIndex(int externalIndex)
        {
            if (externalIndex < 0)
            {
                throw new IndexOutOfRangeException();
            }

            if (externalIndex >= Count)
            {
                throw new IndexOutOfRangeException();
            }

            if (Count == MaxItems)
            {
                return externalIndex;
            }

            int curInternalIndex = 0;
            int curExternalIndex = -1;
            // Only use the lastMappedIndex if its less than the index we're searching for
            if (lastMappedIndex != IndexMap.None && lastMappedIndex.externalIndex <= externalIndex)
            {
                curInternalIndex = lastMappedIndex.internalIndex;
                curExternalIndex = lastMappedIndex.externalIndex - 1;
            }

            while (true)
            {
                if (itemArray[curInternalIndex] != null)
                {
                    curExternalIndex++;

                    if (curExternalIndex == externalIndex)
                    {
                        break;
                    }
                }

                curInternalIndex++;

            }

            lastMappedIndex.externalIndex = curExternalIndex;
            lastMappedIndex.internalIndex = curInternalIndex;

            return curInternalIndex;
        }

        //public T[] ToArray()
        //{
        //    return itemArray.Where(item => item != null).ToArray();
        //}

        private void EnsureNotNull(T item)
        {
            if (item == null)
            {
                throw new ArgumentException("Item cannot be null! ConstrainedLists cannot store null items", "item");
            }
        }

        private class ConstrainedListEnumerator<T2> : IEnumerator<T2> where T2 : class
        {
            private ConstrainedList<T2> constrainedList;
            private int curIndex;
            private T2 curItem;

            public ConstrainedListEnumerator(ConstrainedList<T2> constrainedList)
            {
                this.constrainedList = constrainedList;
                curIndex = -1;
                curItem = default(T2);
            }

            public T2 Current
            {
                get
                {
                    return curItem;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    //This refers to the other Current above.
                    return Current;
                }
            }

            public bool MoveNext()
            {
                do
                {
                    curIndex++;
                }
                while (curIndex < constrainedList.MaxItems && constrainedList.itemArray[curIndex] == null);

                if (curIndex < constrainedList.MaxItems)
                {
                    curItem = constrainedList.itemArray[curIndex];
                }

                return curIndex < constrainedList.MaxItems;
            }

            public void Reset()
            {
                curIndex = -1;
            }


            //See http://stackoverflow.com/a/538238/3897909
            #region Disposal
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool safeToFreeManagedResources)
            {
                if (safeToFreeManagedResources)
                {
                    if (constrainedList != null)
                    {
                        constrainedList = null;
                    }
                }
            }

            //Finalizer
            ~ConstrainedListEnumerator()
            {
                Dispose(false);
            }
            #endregion
        }
    }
}
