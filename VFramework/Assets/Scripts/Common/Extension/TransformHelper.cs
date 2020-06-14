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

        /// <summary>
        /// 找到子物体挂T组件的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="currentTF"></param>
        /// <returns></returns>
        public static GameObject[] FindChildByComponent<T>(this Transform currentTF) where T : class
        {
            List<GameObject> cpList = new List<GameObject>();
            if (currentTF==null)
            {
                return cpList.ToArray();
            }

            currentTF.FindChildListByComponent<T>(cpList);

            return cpList.ToArray();
        }

        private static void FindChildListByComponent<T>(this Transform self,List<GameObject> cpList) where T : class
        {
            for (int i = 0; i < self.transform.childCount; i++)
            {
                Transform child = self.transform.GetChild(i);

                T component = child.GetComponent<T>();
                if (component != null)
                {
                    cpList.Add(child.gameObject);
                }

                if (child.childCount > 0)
                {
                    child.FindChildListByComponent<T>(cpList);
                }
            }
        }

    }

}


