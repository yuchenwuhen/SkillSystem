using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Common
{
    public interface IResetable
    {
        void OnReset();
    }

    public class GameObjectPool : MonoSingleton<GameObjectPool>
    {
        private Dictionary<string, List<GameObject>> m_cacheList;

        public override void AwakeInit()
        {
            base.AwakeInit();
            m_cacheList = new Dictionary<string, List<GameObject>>();
        }

        /// <summary>
        /// 根据名字创建对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public GameObject CreateObject(string key, Vector3 position, Quaternion rotation)
        {
            GameObject go = FindUsableObject(key);

            if (go == null)
            {
                go = AddObject(key);
            }


            if (go == null)
            {
                Debug.LogError("GameObjectPool 加载失败：" + name);
                return go;
            }

            AssetsUnloadHandler.MarkUseAssets(name);

            UseObject(position, rotation, go);

            return go;
        }

        private void UseObject(Vector3 position, Quaternion rotation, GameObject go)
        {
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.SetActive(true);

            var resetScript = go.GetComponent<IResetable>();
            if (resetScript != null)
            {
                resetScript.OnReset();
            }
        }

        private GameObject AddObject(string key)
        {
            GameObject prefab = ResourcesManager.Load<GameObject>(key);
            if (prefab == null)
            {
                Debug.LogError("cant load resource " + key);
            }
            GameObject go = GameObject.Instantiate(prefab);
            //if (!m_cacheList.ContainsKey(key))
            //{
            //    m_cacheList.Add(key, new List<GameObject>());
            //}
            //m_cacheList[key].Add(go);

            return go;
        }

        private GameObject FindUsableObject(string key)
        {
            if (m_cacheList.ContainsKey(key))
            {
                if (m_cacheList[key].Count > 0)
                {
                    var go = m_cacheList[key][0];
                    m_cacheList[key].Remove(go);
                    return go;
                }
                return null;
            }
            return null;
        }

        /// <summary>
        /// 创建物体（parent不为null时，position为本地坐标）
        /// </summary>
        /// <param name="objName"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <param name="isSetActive"></param>
        /// <returns></returns>
        private GameObject CreateObject(string objName, Vector3 position, Quaternion rotation, GameObject prefab, GameObject parent = null, bool isSetActive = true)
        {
            GameObject go = null;
            string name = objName;
            if (string.IsNullOrEmpty(name))
            {
                name = prefab.name;
            }

            go = CreateObject(name, position,rotation);

            if (isSetActive)
                go.SetActive(true);

            if (parent == null)
            {
                go.transform.SetParent(null);
            }
            else
            {
                go.transform.SetParent(parent.transform);
                go.transform.position = position;
            }
            return go;
        }

        /// <summary>
        /// 从对象池取出一个对象，如果没有，则直接创建它
        /// </summary>
        /// <param name="name">对象名</param>
        /// <param name="parent">要创建到的父节点</param>
        /// <returns>返回这个对象</returns>
        public GameObject CreateGameObjectByPool(string name,Vector3 position, Quaternion rotation, GameObject prefab, GameObject parent, bool isSetActive = true)
        {
            return CreateObject(name, position, rotation, prefab, parent, isSetActive);
        }

        public GameObject CreateGameObjectByPool(string name, Vector3 position, Quaternion rotation, GameObject parent = null, bool isSetActive = true)
        {
            return CreateObject(name, position, rotation, null, parent, isSetActive);
        }

        /// <summary>
        /// 回收对象
        /// 因为resources目录索引是prefab name并且唯一,所以根据GameObject来判断
        /// </summary>
        /// <param name="go"></param>
        /// <param name="delay">延迟时间</param>
        public void CollectObject(GameObject go, float delay = 0)
        {
            if (go == null)
            {
                return;
            }

            StartCoroutine(DelayCollect(go, delay));
        }

        private IEnumerator DelayCollect(GameObject go, float delay)
        {
            yield return new WaitForSeconds(delay);
            //go 名字一般为xx(clone);
            string name = go.name.Replace("(Clone)", "");
            if (m_cacheList.ContainsKey(name))
            {
                m_cacheList[name].Add(go);
            }
            else
            {
                m_cacheList.Add(name, new List<GameObject>());
                m_cacheList[name].Add(go);
            }
            go.SetActive(false);
        }

        public void Clear(string key)
        {
            if (m_cacheList.ContainsKey(key))
            {
                for (int i = m_cacheList[key].Count - 1; i >= 0; i--)
                {
                    Destroy(m_cacheList[key][i]);
                }
                m_cacheList[key].Clear();
                m_cacheList.Remove(key);
            }
        }

        public void ClearAll()
        {
            List<string> keyList = new List<string>(m_cacheList.Keys);
            foreach (var key in keyList)
            {
                Clear(key);
            }
        }
    }
}


