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

            UseObject(position, rotation, go);

            return go;
        }

        public GameObject CreateObject(string key, Vector3 position, Quaternion rotation, GameObject parent)
        {
            GameObject go = CreateObject(key, position, rotation);

            if (go != null && parent != null)
            {
                go.transform.SetParent(parent.transform);
            }

            return go;
        }

        public GameObject CreateObject(string key)
        {
            GameObject go = FindUsableObject(key);

            if (go == null)
            {
                go = AddObject(key);
            }

            UseObject(go);

            return go;
        }

        public GameObject CreateObject(GameObject prefab)
        {
            return CreateObject(prefab.name);
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

        private void UseObject(GameObject go)
        {
            go.transform.rotation = Quaternion.identity;
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
            go.name = go.name.Replace("(Clone)", "");

            if (!go.GetComponent<PrefabData>())
            {
                go.AddComponent<PrefabData>().pathName = key;
            }
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

            // StartCoroutine(DelayCollect(go, delay));
            DelayCollect(go);
        }

        private void DelayCollect(GameObject go)
        {

            //go 名字一般为xx(Clone);
            var data = go.GetComponent<PrefabData>();
            if (data == null)
            {
                Debug.LogError("can't collect obj, missing PrefabData component" + go.name);
            }

            string key = data.pathName;
            if (m_cacheList.ContainsKey(key))
            {
                if (m_cacheList[key].Contains(go))
                {
                    Debug.LogError("collect repeat" + go.name);
                }
                else
                {
                    m_cacheList[key].Add(go);
                }
            }
            else
            {
                m_cacheList.Add(key, new List<GameObject>());
                m_cacheList[key].Add(go);
            }

            //重设父节点 防止被删除
            go.transform.SetParent(null);
            go.SetActive(false);
        }

        private IEnumerator DelayCollect(GameObject go, float delay)
        {
            yield return new WaitForSeconds(delay);
            //go 名字一般为xx(Clone);
            var data = go.GetComponent<PrefabData>();
            if (data == null)
            {
                Debug.LogError("can't collect obj, missing PrefabData component" + go.name);
            }

            string key = data.pathName;
            if (m_cacheList.ContainsKey(key))
            {
                if (m_cacheList[key].Contains(go))
                {
                    Debug.LogError("collect repeat" + go.name);
                }
                else
                {
                    m_cacheList[key].Add(go);
                }
            }
            else
            {
                m_cacheList.Add(key, new List<GameObject>());
                m_cacheList[key].Add(go);
            }

            //重设父节点 防止被删除
            go.transform.SetParent(null);
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


