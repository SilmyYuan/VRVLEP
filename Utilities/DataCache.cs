using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace VRVLEP.Utilities
{
    /// <summary>
    /// 缓存帮助类
    /// </summary>
    public static class DataCache
    {
        //创建静态只读的IMemoryCache
        public static readonly IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());

        /// <summary>
        ///  存在则True
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsExists(string key)
        {
            return cache.Get(key) != null;
        }

        /// <summary>
        ///  删除
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static void Delete(string key)
        {
            cache.Remove(key);
        }

        /// <summary>
        /// 获取当前应用程序指定key的Cache值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetCache(string key)
        {
            return cache.Get(key);
        }

        /// <summary>
        /// 设置当前应用程序指定key的Cache值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objObject"></param>
        public static void SetCache(string key, object obj)
        {
            cache.Set(key, obj);
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="objObject"></param>
        /// <param name="absoluteExpiration"> </param>
        public static void SetCache(string key, object obj, DateTime absoluteExpiration)
        {
            cache.Set(key, obj, absoluteExpiration);
        }
    }
}
