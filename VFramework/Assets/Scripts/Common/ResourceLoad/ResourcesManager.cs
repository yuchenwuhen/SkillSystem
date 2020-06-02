using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Common
{
    public static class ResourcesManager
    {
        private static AssetsLoadType m_loadType = AssetsLoadType.Resources;
        public static AssetsLoadType LoadType { get { return m_loadType; } }

        public static bool UseCache
        {
            get; private set;
        }

        private static AssetsLoadController loadAssetsController;

#if UNITY_EDITOR
        //UnityEditor模式下编译完成后自动初始化
        [UnityEditor.InitializeOnLoadMethod]
#endif
        private static void Initialize()
        {
            Initialize(AssetsLoadType.Resources, false);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="loadType"></param>
        /// <param name="useCache"></param>
        public static void Initialize(AssetsLoadType loadType, bool useCache)
        {
            //if (isInit)
            //    return;


            if (loadType == AssetsLoadType.AssetBundle)
            {
                useCache = true;
            }
            if (!Application.isPlaying)
            {
                useCache = false;
            }
            UseCache = useCache;
            ResourcesManager.m_loadType = loadType;
            ReleaseAll();
            //GameInfoCollecter.AddAppInfoValue("AssetsLoadType", loadType);

            loadAssetsController = new AssetsLoadController(loadType, useCache);
            //Debug.Log("ResourceManager初始化 AssetsLoadType:" + loadType + " useCache:" + useCache);
        }

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
            string path = ResourcesConfigManager.GetLoadPath(m_loadType, name);

            AssetsData assets = loadAssetsController.LoadAssets<T>(path);
            Debug.Log(assets.assetName +":"+ assets.GetObjectsMemorySize());
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

        public static void ReleaseByPath(string path)
        {
            loadAssetsController.Release(path);
        }

        /// <summary>
        /// 释放资源 （通过 ResourceManager.Load<>() 加载出来的）
        /// </summary>
        /// <param name="unityObject"></param>
        /// <param name="times"></param>
        public static void DestoryAssetsCounter(Object unityObject, int times = 1)
        {
            DestoryAssetsCounter(unityObject.name, times);
        }

        public static void DestoryAssetsCounter(string name, int times = 1)
        {
            if (!ResourcesConfigManager.GetIsExitRes(name))
                return;
            string path = ResourcesConfigManager.GetLoadPath(m_loadType, name);
            if (times <= 0)
                times = 1;
            for (int i = 0; i < times; i++)
            {
                loadAssetsController.DestoryAssetsCounter(path);
            }
        }

        /// <summary>
        /// 卸载所有资源
        /// </summary>
        /// <param name="isForceAB">是否强制卸载bundle（true:bundle包和资源一起卸载；false：只卸载bundle包）</param>
        public static void ReleaseAll(bool isForceAB = true)
        {
            if (loadAssetsController != null)
                loadAssetsController.ReleaseAll(isForceAB);
            //ResourcesConfigManager.ClearConfig();
        }
    }
}


