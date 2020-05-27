using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Common
{
    public static class ResourcesManager
    {
        /// <summary>
        /// 加载资源
        /// 注意释放资源，方法： DestoryAssetsCounter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T Load<T>(string name) where T : Object
        {
            T res = null;
            string path = ResourcesConfigManager.GetLoadPath(loadType, name);

            AssetsData assets = loadAssetsController.LoadAssets<T>(path);
            if (assets != null)
            {
                res = assets.GetAssets<T>();

            }
            if (res == null)
            {
                Debug.LogError("Error=> Load Name :" + name + "  Type:" + typeof(T).FullName + "\n" + " Load Object:" + res);
            }
            return res;
        }
    }
}


