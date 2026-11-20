using System;
using System.Runtime.Caching;

namespace Elogbook.Core
{
    /// <summary>
    /// Quản lý token logout
    /// </summary>
    public class CacheManager
    {
        private static readonly ObjectCache _cache = MemoryCache.Default;

        /// <summary>
        /// Method to add an item to cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiration"></param>
        public void AddItem(string key, object value, TimeSpan expiration)
        {
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.Add(expiration)
            };

            _cache.Add(key, value, policy);
        }

        /// <summary>
        /// Method to get an item from cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetItem(string key)
        {
            return _cache.Get(key);
        }

        /// <summary>
        /// Method to remove an item from cache
        /// </summary>
        /// <param name="key"></param>
        public void RemoveItem(string key)
        {
            _cache.Remove(key);
        }
    }
}