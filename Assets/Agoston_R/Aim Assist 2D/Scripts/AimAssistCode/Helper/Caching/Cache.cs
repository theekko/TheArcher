using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.Helper.Caching
{
    /// <summary>
    /// Improve performance by storing which game objects have certain components on them.
    ///
    /// Cuts down on GetComponent calls and queries.
    /// </summary>
    /// <typeparam name="T">Type of component stored for a given game object</typeparam>
    public class Cache<T> where T : Component
    {
        private static Cache<T> _instance;
        public static Cache<T> Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Cache<T>();
                }
                return _instance;
            }
        }

        private Cache()
        {
        }

        private readonly IDictionary<int, T> _store = new Dictionary<int, T>();

        /// <summary>
        /// Registers a new item to the cache
        ///
        /// HAS TO BE CALLED EVERY TIME a new item of type T has been instantiated to the scene.
        /// </summary>
        /// <param name="item">item to add</param>
        public void RegisterItem(T item)
        {
            _store.Add(item.gameObject.GetInstanceID(), item);
        }

        /// <summary>
        /// Check whether the store is empty
        /// </summary>
        /// <returns>true if the store has no elements inside, false otherwise</returns>
        public bool IsEmpty()
        {
            return _store.Count == 0;
        }

        /// <summary>
        /// Replace the store with the given elements.
        /// </summary>
        /// <param name="items">elements to add to the store</param>
        public void StoreItems(IEnumerable<T> items)
        {
            _store.Clear();
            foreach (var i in items)
            {
                _store.Add(i.gameObject.GetInstanceID(), i);
            }
        }

        /// <summary>
        /// Returns the elements stored in the cache in a readonly fashion.
        /// </summary>
        /// <returns>the elements of the cache</returns>
        public ICollection<T> FindAll()
        {
            return _store.Values;
        }

        /// <summary>
        /// Remove the given item if it is in the store.
        ///
        /// </summary>
        /// <param name="item">item to remove</param>
        /// <returns>true if the item is found and removed, false otherwise.</returns>
        public bool RemoveItem(T item)
        {
            return _store.Remove(item.gameObject.GetInstanceID());
        }

        /// <summary>
        /// Tries to find the given component on the added object and saves it if not found in storage.
        ///
        /// If there's no T component on the object in question, it stores the null and will return that later.
        /// </summary>
        /// <param name="obj">the object in question whom we check for the T component</param>
        /// <returns>the stored component if already present, the found component if present or null.</returns>
        [CanBeNull]
        public T FindOrInsert(Component obj)
        {
            if (obj == null)
            {
                return null;
            }
            
            var instanceId = obj.GetInstanceID();
            if (_store.TryGetValue(instanceId, out var comp))
            {
                return comp;
            }
            
            _store.Add(instanceId, obj.GetComponent<T>());
            return _store[instanceId];
        }

        /// <summary>
        /// Clear all items from the store.
        /// </summary>
        public void Purge()
        {
            _store.Clear();
        }
    }
}