using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

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
        /// <param name="key"></param>
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

        /// <summary>
        /// 设置当前应用程序指定key的Cache值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="Minutes"></param>
        public static void SetCache(string key, object obj, int Minutes)
        {
            cache.Set(key, obj, DateTime.Now.AddMinutes((Minutes < 1 ? 5 : Minutes)));
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值 20m        
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public static void SetCache20M(string key, object obj)
        {
            SetCache(key, obj, 20);
        }

        /// <summary>
        /// 判断数据是否存在缓存中
        /// 存在:从缓存中拿取
        /// 否则:重新加载
        /// [无参]
        /// </summary>
        /// <param name="key">缓存名称</param>
        /// <param name="func"> 缓存失效时调用的方法</param>
        public static T GetCacheData<T>(string key, Func<T> func) where T : class
        {
            if (!IsExists(key))
            {
                T result = func();
                SetCache20M(key, result);
            }
            T t = GetCache(key) as T;
            return t;
        }

        /// <summary>
        /// 判断数据是否存在缓存中
        /// 存在:从缓存中拿取
        /// 否则:重新加载
        /// [无参]
        /// </summary>
        /// <param name="key">缓存名称</param>
        /// <param name="func"> 缓存失效时调用的方法</param>
        public static string GetCacheData(string key, Func<string> func)
        {
            if (!IsExists(key))
            {
                string result = func();
                SetCache20M(key, result);
            }
            return GetCache(key).ToString();
        }
    }
}
