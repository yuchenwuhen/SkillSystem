using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Common
{
    public abstract class LoaderBase
    {
        protected AssetsLoadController m_assetsLoadController;
        public LoaderBase(AssetsLoadController assetsLoadController)
        {
            this.m_assetsLoadController = assetsLoadController;
        }

        public abstract IEnumerator LoadAssetsIEnumerator(string path, Type resType, CallBack<AssetsData> callBack);
        public abstract AssetsData LoadAssets(string path);

        public abstract AssetsData LoadAssets<T>(string path) where T : UnityEngine.Object;

        public virtual string[] GetAllDependenciesName(string name) { return new string[0]; }
        /// <summary>
        /// 判断资源是否含有依赖
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual bool IsHaveDependencies(string name) { return false; }
    }
}


