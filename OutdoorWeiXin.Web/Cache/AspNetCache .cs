using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutdoorWeiXin.Web.Cache
{
    public class AspNetCache : ICacheStorage
    {
        /// <summary>
        /// 缓存保留时间，单位为分钟
        /// </summary>
        private readonly string _cacheTime = "120";

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">Key值</param>
        /// <param name="value">要缓存的数据</param>
        public void Add(string key, object value)
        {
            HttpContext.Current.Cache.Insert(
                key,
                value,
                null,
                DateTime.Now.AddMinutes(string.IsNullOrEmpty(_cacheTime) ? 5 : Convert.ToInt32(_cacheTime)),
                TimeSpan.Zero);
        }

        /// <summary>
        /// 根据指定的key移除缓存
        /// </summary>
        /// <param name="key">Key值</param>
        public void Remove(string key)
        {
            HttpContext.Current.Cache.Remove(key);
        }

        /// <summary>
        /// 获取指定key的缓存数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam> 
        /// <param name="key">缓存Key值</param>
        /// <returns>返回指定数据类型的缓存数据</returns>
        public T Get<T>(string key)
        {
            if (HttpContext.Current.Cache[key] == null)
            {
                return default(T);
            }
            return (T)HttpContext.Current.Cache[key];
        }
    }
}