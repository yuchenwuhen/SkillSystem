using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Profiling;

namespace VFramework.Common
{
    /// <summary>
    /// 本地资源信息存储类
    /// </summary>
    public class AssetsData
    {
        public string assetPath;            //资源路径

        /// <summary>
        /// 资源引用次数
        /// </summary>
        public int refCount = 0;

        public string assetName = "";

        private long objectsSize = -1;
        private long bundleSize = -1;

        private Object[] assets;
        public Object[] Assets
        {
            get
            {
                return assets;
            }
            set
            {
                assets = value;
                objectsSize = 0;
                if (assets != null)
                {
                    foreach (var item in assets)
                    {
                        objectsSize += Profiler.GetRuntimeMemorySizeLong(item);
                    }
                }
            }
        }

        public AssetsData(string path)
        {
            assetPath = path;
            assetName = Path.GetFileNameWithoutExtension(path);

            objectsSize = -1;
            bundleSize = -1;
        }

        public T GetAssets<T>() where T : Object
        {
            foreach (var item in assets)
            {
                if (item.GetType() == typeof(T))
                {
                    return (T)item;
                }
            }
            return default(T);
        }

        /// <summary>
        /// 获取资源的占用内存大小
        /// </summary>
        /// <returns></returns>
        public long GetObjectsMemorySize()
        {
            return objectsSize;
        }
    }
}


