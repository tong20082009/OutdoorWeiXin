using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutdoorWeiXin.Web.Cache
{
    public interface ICacheStorage
    {
        ///<summary>
        ///     新增缓存
        /// </summary>
        /// <param name="key">缓存的key</param>
        /// <param name="value">要缓存的数据</param>
        void Add(string key, object value);

        ///<summary>
        ///     根据缓存的key移除缓存
        /// </summary>
        /// <param name="key">缓存的key</param>
        void Remove(string key);

        ///<summary>
        ///     根据缓存的key获取缓存数据
        /// </summary>
        /// <typeparam name="T">缓存的数据类型</typeparam>
        /// <param name="key">key值</param>
        /// <returns>返回获取到的指定类型的缓存数据</returns>
        T Get<T>(string key);

    }
}