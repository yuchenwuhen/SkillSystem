using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework.Tools;

namespace VFramework.Common
{
    public class ResourcesLoader : LoaderBase
    {
        public ResourcesLoader(AssetsLoadController loadAssetsController) : base(loadAssetsController)
        {
        }

        public override AssetsData LoadAssets(string path)
        {
            string s = PathTool.RemoveExtension(path);
            AssetsData rds = null;
            UnityEngine.Object ass = Resources.Load(s);
            if (ass != null)
            {
                rds = new AssetsData(path);
                rds.Assets = new UnityEngine.Object[] { ass };
            }
            else
            {
                Debug.LogError("加载失败,Path:" + path);
            }
            return rds;
        }

        public override AssetsData LoadAssets<T>(string path)
        {
            string s = PathTool.RemoveExtension(path);
            AssetsData rds = null;
            T ass = Resources.Load<T>(s);
            if (ass != null)
            {
                rds = new AssetsData(path);
                rds.Assets = new UnityEngine.Object[] { ass };
            }
            else
            {
                Debug.LogError("加载失败,Path:" + path);
            }
            return rds;
        }

        public override IEnumerator LoadAssetsIEnumerator(string path, System.Type resType, CallBack<AssetsData> callBack)
        {
            AssetsData rds = null;
            string s = PathTool.RemoveExtension(path);
            ResourceRequest ass = null;
            if (resType != null)
            {
                ass = Resources.LoadAsync(s, resType);
            }
            else
            {
                ass = Resources.LoadAsync(s);
            }
            yield return ass;

            if (ass.asset != null)
            {
                rds = new AssetsData(path);
                rds.Assets = new UnityEngine.Object[] { ass.asset };
            }
            else
            {
                Debug.LogError("加载失败,Path:" + path);
            }

            if (callBack != null)
                callBack(rds);
            yield return new WaitForEndOfFrame();
        }
    }
}


