using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Common
{
    public class AssetsLoadController 
    {
        private Dictionary<string, AssetsData> assetsCaches = new Dictionary<string, AssetsData>();

        private LoaderBase loader;

        private AssetsLoadType m_loadType;
        private bool m_useCache;

        public AssetsLoadController(AssetsLoadType loadType, bool useCache)
        {
            m_loadType = loadType;
            m_useCache = useCache;
            if (loadType == AssetsLoadType.Resources)
            {
                loader = new ResourcesLoader(this);
            }
        }

        #region 加载资源
        public AssetsData LoadAssets(string path)
        {
            return LoadAssetsLogic(path, () =>
            {
                if (assetsCaches.ContainsKey(path))
                {
                    return true;
                }
                return false;
            }
                , (p) =>
                {
                    return loader.LoadAssets(p);

                });
        }
        public AssetsData LoadAssets<T>(string path) where T : Object
        {
            return LoadAssetsLogic(path,
                () =>
                {
                    if (assetsCaches.ContainsKey(path))
                    {
                        T res = assetsCaches[path].GetAssets<T>();
                        if (res != null)
                            return true;
                    }
                    return false;
                }
                ,
                (p) =>
                {
                    return loader.LoadAssets<T>(p);

                });
        }
        public AssetsData LoadAssetsLogic(string path, CallBackR<bool> checkContainsAssets, CallBackR<AssetsData, string> loadMethod)
        {
            LoadAssetsDependencie(path);
            AssetsData assets = null;
            if (checkContainsAssets())
            {
                assets = assetsCaches[path];
            }
            else
            {

                assets = loadMethod(path);
                if (assets == null)
                {
                    Debug.LogError("资源加载失败：" + path);
                    return assets;
                }
                else
                {
                    if (assetsCaches.ContainsKey(path))
                    {
                        List<Object> asList = new List<Object>(assetsCaches[path].Assets);
                        foreach (var item in assets.Assets)
                        {
                            if (!asList.Contains(item))
                            {
                                asList.Add(item);
                            }
                        }
                        assetsCaches[path].Assets = asList.ToArray();
                        assets = assetsCaches[path];
                    }
                    else
                    {
                        if (m_useCache)
                        {
                            assetsCaches.Add(path, assets);
                        }
                    }
                }
            }
            if (m_useCache)
            {
                assets.refCount++;
                AssetsUnloadHandler.MarkUseAssets(assets, loader.IsHaveDependencies(path));
            }
            return assets;
        }
        private void LoadAssetsDependencie(string path)
        {
            string[] dependenciesNames = loader.GetAllDependenciesName(path);
            // Debug.LogWarning("DestoryAssets:" + name + "=>" + string.Join("\n", dependenciesNames));
            foreach (var item in dependenciesNames)
            {
                LoadAssets(item);
            }
        }
        public void LoadAsync(string path, System.Type assetType, CallBack<Object> callBack)
        {
            MonoBehaviourRuntime.Instance.StartCoroutine(LoadAssetsIEnumerator(path, assetType, callBack));
        }
        /// <summary>
        /// 异步加载资源逻辑
        /// </summary>
        /// <param name="path"></param>
        /// <param name="assetType"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        private IEnumerator LoadAssetsIEnumerator(string path, System.Type assetType, CallBack<Object> callBack)
        {
            yield return LoadAssetsIDependencieEnumerator(path);

            if (assetsCaches.ContainsKey(path))
            {
                AssetsData assets = assetsCaches[path];
                if (m_useCache)
                {
                    assets.refCount++;
                    AssetsUnloadHandler.MarkUseAssets(assets, loader.IsHaveDependencies(path));
                }
                if (callBack != null)
                {
                    callBack(assets.Assets[0]);
                }
            }
            else
            {
                yield return loader.LoadAssetsIEnumerator(path, assetType, (assets) =>
                {
                    if (m_useCache)
                    {
                        assetsCaches.Add(path, assets);
                    }
                    if (m_useCache)
                    {
                        assets.refCount++;
                        AssetsUnloadHandler.MarkUseAssets(assets, loader.IsHaveDependencies(path));
                    }
                    if (callBack != null)
                    {
                        callBack(assets.Assets[0]);
                    }
                });
            }
            yield return 0;
        }
        /// <summary>
        /// 异步加载依赖包
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private IEnumerator LoadAssetsIDependencieEnumerator(string path)
        {
            string[] dependenciesNames = loader.GetAllDependenciesName(path);
            // Debug.LogWarning("LoadAssetsIDependencieEnumerator:" + name + "=>" + string.Join("\n", dependenciesNames));
            foreach (var item in dependenciesNames)
            {
                yield return LoadAssetsIEnumerator(item, null, null);
            }
        }
        #endregion

        #region 卸载资源相关
        /// <summary>
        /// 资源引用数减少（该资源的依赖也会减少）
        /// </summary>
        /// <param name="path"></param>
        public void DestoryAssetsCounter(string path)
        {
            if (assetsCaches.ContainsKey(path))
            {
                AssetsData assets = assetsCaches[path];
                assets.refCount--;
                if (assets.refCount < 0)
                {
                    Debug.LogError("资源引用计数错误：(" + assets.refCount + ") " + assets.assetPath);
                    assets.refCount = 0;
                }
                if (assets.refCount <= 0)
                {
                    AssetsUnloadHandler.DiscardAssets(assets);
                }

                string[] dependenciesNames = loader.GetAllDependenciesName(path);
                //Debug.LogWarning("DestoryAssets:" + name + "=>" + string.Join("\n", dependenciesNames));
                foreach (var item in dependenciesNames)
                {
                    DestoryAssetsCounter(item);
                }
            }
            else
            {
                if (m_useCache)
                {
                    Debug.LogError("未加载资源，不能Destroy ：" + path);
                }

            }


        }

        public void ReleaseAll(bool isForceAB)
        {
            foreach (var item in assetsCaches)
            {
                UnloadAssets(item.Value, isForceAB);
            }
            assetsCaches.Clear();
        }
        /// <summary>
        /// 直接释放资源（引用数为0时起作用）
        /// </summary>
        /// <param name="path"></param>
        public void Release(string path)
        {
            AssetsData assets = null;
            if (assetsCaches.TryGetValue(path, out assets))
            {
                if (assets.refCount <= 0)
                {
                    UnloadAssets(assets, true);
                    assetsCaches.Remove(path);
                    Debug.LogWarning("彻底释放" + path);
                }
            }
        }

        public void UnloadAssets(AssetsData assets, bool isForceAB)
        {
            if (!m_useCache)
                return;
            if (assets.Assets != null && isForceAB)
            {
                foreach (var item in assets.Assets)
                {
                    Debug.LogWarning("释放资源" + item);
                    UnloadObject(item);
                }
                assets.Assets = null;
            }
            //if (assets.AssetBundle != null)
            //    assets.AssetBundle.Unload(isForceAB);
        }
        private void UnloadObject(Object obj)
        {

            if (obj == null)
            {
                return;
            }
            //Debug.Log("UnloadObject " + obj.GetType()+" :"+obj.name);
            if (obj is Shader)
            {
                return;
            }

            if (!(obj is GameObject)
                && !(obj is Component)
                && !(obj is AssetBundle)
                )
            {
                Resources.UnloadAsset(obj);
            }
            else if ((obj is GameObject)
                || (obj is Component))
            {
                if (m_loadType == AssetsLoadType.AssetBundle)
                    Object.DestroyImmediate(obj, true);
            }
            else
            {
                AssetBundle ab = (AssetBundle)obj;
                ab.Unload(true);
            }
        }
        #endregion
    }
}


