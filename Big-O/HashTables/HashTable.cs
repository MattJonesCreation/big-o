using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTables
{
    /// <summary>
    /// A key / value associative collection
    /// </summary>
    public class HashTable<TKey, TValue>
    {
        /// <summary>
        /// If the array exceeds this fill percentage, it will grow. In this example, total number of items is used
        /// </summary>
        private const double _fillFactor = 0.75;

        /// <summary>
        /// The maximum number of items to store before growing. Cached value of the fill factor calculation
        /// </summary>
        private int _maxItemsAtCurrentSize;

        // The number of items in the hash table
        private int _count;

        // The array where the items are stored
        private HashTableArray<TKey, TValue> _array;

        // Construct hash table with default capacity
        public HashTable() : this(1000)
        {

        }

        /// <summary>
        /// Constructs a hash table with the specified capacity
        /// </summary>
        /// <param name="initialCapacity"></param>
        public HashTable(int initialCapacity)
        {
            if (initialCapacity < 1)
            {
                throw new ArgumentOutOfRangeException("initialCapacity");
            }

            _array = new HashTableArray<TKey, TValue>(initialCapacity);

            // when the count exceeds this value, the next add will cause the array to grow
            _maxItemsAtCurrentSize = (int)(initialCapacity * _fillFactor) + 1;
        }

        /// <summary>
        /// Adds the key/value pair to the hash table. If the key already exists in the 
        /// hash table an ArgumentException will be thrown
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            if (_count >= _maxItemsAtCurrentSize)
            {
                // allocate more space 
                HashTableArray<TKey, TValue> largerArray = new HashTableArray<TKey, TValue>(_array.Capacity * 2);

                // and re-add each item to the new array 
                foreach (HashTableNodePair<TKey, TValue> node in _array.Items)
                {
                    largerArray.Add(node.Key, node.Value);
                }

                // the larger array is now the hash table storage 
                _array = largerArray;

                // update the new max items cached value
                _maxItemsAtCurrentSize = (int)(_array.Capacity * _fillFactor) + 1;
            }

            _array.Add(key, value);
            _count += 1;
        }

        /// <summary>
        /// Removes the item from the hash table whose key matches the specified key. 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            bool removed = _array.Remove(key);
            if (removed)
            {
                _count -= 1;
            }

            return removed;
        }

        /// <summary>
        /// Gets and sets the value with the specified key. Argument Exception is thrown
        /// if the key does not already exist in the hash table. 
        /// </summary>
        /// <param name="key">The key value to retrieve</param>
        /// <returns>The value associated with the specified key</returns>
        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (!_array.TryGetValue(key, out value))
                {
                    throw new ArgumentException("key");
                }

                return value;
            }
            set
            {
                _array.Update(key, value);
            }
        }

        /// <summary>
        /// Finds and returns the value for the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _array.TryGetValue(key, out value);
        }
    }

    /// <summary>
    /// The fixed size array of the nodes in the hash table
    /// </summary>
    /// <typeparam name="TKey">The key type of the hash table</typeparam>
    /// <typeparam name="TValue">The value type of the hash table</typeparam>
    public class HashTableArray<TKey, TValue>
    {
        private HashTableArrayNode<TKey, TValue>[] _array;

        /// <summary>
        /// Constructs a new hash table array with the specified capacity 
        /// </summary>
        /// <param name="capacity"></param>
        public HashTableArray(int capacity)
        {
            _array = new HashTableArrayNode<TKey, TValue>[capacity];
            for (int i = 0; i < capacity; i++)
            {
                _array[i] = new HashTableArrayNode<TKey, TValue>();
            }
        }

        /// <summary>
        /// Adds the key/value pair to the node. If the key already exists in the node array an 
        /// ArgumentException will be thrown
        /// </summary>
        /// <param name="key">The key of the item being added</param>
        /// <param name="value">The value of the item being added</param>
        public void Add(TKey key, TValue value)
        {
            _array[GetIndex(key)].Add(key, value);
        }

        /// <summary>
        /// Updates the value of the existing key/value pair in the node array. 
        /// If the key doesn't exist, throw argument exception
        /// </summary>
        /// <param name="key">The key of the item being updated</param>
        /// <param name="value">The updated value</param>
        public void Update(TKey key, TValue value)
        {
            _array[GetIndex(key)].Update(key, value);
        }

        private int GetIndex(TKey key)
        {
            int hash = Djb2(key.ToString());
            int index = hash % _array.Length;
            return index;
        }

        private static int Djb2(string input)
        {
            int hash = 5381;

            foreach (int c in input.ToCharArray())
            {
                unchecked
                {
                    hash = ((hash << 5) + hash) + c;
                }
            }

            return hash;
        }

        /// <summary>
        /// Removes the item from the node array whose key matches the specified key. 
        /// </summary>
        /// <param name="key">The key of the item to remove</param>
        /// <returns>True if the item was removed, false otherwise</returns>
        public bool Remove(TKey key)
        {
            return _array[GetIndex(key)].Remove(key);
        }

        /// <summary>
        /// Finds and returns the value for the specified key.
        /// </summary>
        /// <param name="key">The key whose value is sought</param>
        /// <param name="value">The value associated with the specified key</param>
        /// <returns>True if the value was found, false otherwise</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _array[GetIndex(key)].TryGetValue(key, out value);
        }

        /// <summary>
        /// The capacity of the hash table array
        /// </summary>
        public int Capacity { get { return _array.Length; } }

        /// <summary>
        /// Remove every item from the hash table array
        /// </summary>
        public void Clear()
        {
            foreach (HashTableArrayNode<TKey, TValue> node in _array)
            {
                node.Clear();
            }
        }

        /// <summary>
        /// Returns an enumerator for all of the values in the node array
        /// </summary>
        public IEnumerable<TValue> Values
        {
            get
            {
                foreach (HashTableArrayNode<TKey, TValue> node in _array)
                {
                    foreach (var value in node.Values)
                    {
                        yield return value;
                    }
                }
            }
        }

        /// <summary>
        /// Returns an enumerator for all of the keys in the node array
        /// </summary>
        public IEnumerable<TKey> Keys
        {
            get
            {
                foreach (HashTableArrayNode<TKey, TValue> node in _array)
                {
                    foreach (var key in node.Keys)
                    {
                        yield return key;
                    }
                }
            }
        }

        /// <summary>
        /// Returns an enumerator for all of the keys in the node array
        /// </summary>
        public IEnumerable<HashTableNodePair<TKey, TValue>> Items
        {
            get
            {
                foreach (HashTableArrayNode<TKey, TValue> node in _array)
                {
                    foreach (HashTableNodePair<TKey, TValue> pair in node.Items)
                    {
                        yield return pair;
                    }
                }
            }
        }
    }

    /// <summary>
    /// The Hash table data chain
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class HashTableArrayNode<TKey, TValue>
    {
        /// <summary>
        /// This list contains the actual data in the hash table. It chains together data collisions. 
        /// It would be possible to use a list only when there is a collisions to avoid allocating the list unnecessarily but this 
        /// approach makes the implementation easier to follow for this sample. 
        /// </summary>
        private LinkedList<HashTableNodePair<TKey, TValue>> _items;

        /// <summary>
        /// Adds the key/value pair to the node. If the key already exists in the list an 
        /// ArgumentException will be thrown
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            // lazy init the linked list 
            if (_items == null)
            {
                _items = new LinkedList<HashTableNodePair<TKey, TValue>>();
            }
            else
            {
                // Multiple items might collide and exist in this list - but each 
                // key should only be in the list once 
                foreach (HashTableNodePair<TKey, TValue> pair in _items)
                {
                    if (pair.Key.Equals(key))
                    {
                        throw new ArgumentException("The collection already contains the key");
                    }
                }
            }

            // if we made it this far - add the item
            _items.AddFirst(new HashTableNodePair<TKey, TValue>(key, value));
        }

        /// <summary>
        /// Updates the values of the existing key/value pair in the list. 
        /// If the key does not exist in the list an argument exception will be thrown
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Update(TKey key, TValue value)
        {
            bool updated = false;

            if (_items != null)
            {
                // Multiple items might collide and exist in this list - but each 
                // key should only be in the list once 
                foreach (HashTableNodePair<TKey, TValue> pair in _items)
                {
                    if (pair.Key.Equals(key))
                    {
                        pair.Value = value;
                        updated = true;
                        break;
                    }
                }
            }

            if (!updated)
            {
                throw new ArgumentException("The collection does not contain the key");
            }
        }

        /// <summary>
        /// Removes the item from the list whose key matches the specified key.
        /// </summary>
        /// <param name="key">The key of the item to remove</param>
        /// <returns>True if the item was removed, false otherwise</returns>
        public bool Remove(TKey key)
        {
            bool removed = false;
            if (_items != null)
            {
                LinkedListNode<HashTableNodePair<TKey, TValue>> current = _items.First;
                while (current != null)
                {
                    if (current.Value.Key.Equals(key))
                    {
                        _items.Remove(current);
                        removed = true;
                        break;
                    }

                    current = current.Next;
                }
            }

            return removed;
        }

        /// <summary>
        /// Remove all items in the list
        /// </summary>
        public void Clear()
        {
            if (_items != null)
            {
                _items.Clear();
            }
        }

        /// <summary>
        /// Returns an enumerator for all of the values in the list
        /// </summary>
        public IEnumerable<TValue> Values
        {
            get
            {
                if (_items != null)
                {
                    foreach (HashTableNodePair<TKey, TValue> pair in _items)
                    {
                        yield return pair.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Returns an enumerator for all of the keys in the list
        /// </summary>
        public IEnumerable<TKey> Keys
        {
            get
            {
                if (_items != null)
                {
                    foreach (HashTableNodePair<TKey, TValue> pair in _items)
                    {
                        yield return pair.Key;
                    }
                }
            }
        }

        /// <summary>
        /// Returns an enumerator for all of the keys in the list
        /// </summary>
        public IEnumerable<HashTableNodePair<TKey, TValue>> Items
        {
            get
            {
                if (_items != null)
                {
                    foreach (HashTableNodePair<TKey, TValue> pair in _items)
                    {
                        yield return pair;
                    }
                }
            }
        }

        /// <summary>
        /// Finds and returns the value for the specified key.
        /// </summary>
        /// <param name="key">The key whose value is sought</param>
        /// <param name="value">The value associated with the specified key</param>
        /// <returns>True if found, false othersie</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            bool found = false;

            if (_items != null)
            {
                foreach (HashTableNodePair<TKey, TValue> pair in _items)
                {
                    if (pair.Key.Equals(key))
                    {
                        value = pair.Value;
                        found = true;
                        break;
                    }
                }
            }

            return found;
        }
    }

    public class HashTableNodePair<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public HashTableNodePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}
