using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Common
{
    public static class TransformHelper
    {
        /// <summary>
        /// 查找后代指定名称的Transform
        /// </summary>
        /// <param name="currentTF"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Transform FindChildByName(this Transform currentTF,string name)
        {
            Transform child = currentTF.Find(name);
            if (child != null) return child;

            for (int i = 0; i < currentTF.childCount; i++)
            {
                child = FindChildByName(currentTF.GetChild(i), name);
                if (child != null) return child;
            }

            return null;
        }

        /// <summary>
        /// 查找后代指定名称的GameObject
        /// </summary>
        /// <param name="currentTF"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameObject FindChildByName(this GameObject currentTF, string name)
        {
            Transform go = currentTF.transform.FindChildByName(name);
            return go != null ? go.gameObject : null;
        }

    }

}


