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
        private Dictionary<string, List<GameObject>> m_cacheList ;

        public override void AwakeInit()
        {
            base.AwakeInit();
            m_cacheList = new Dictionary<string, List<GameObject>>();
        }

        /// <summary>
        /// 根据名字创建对象，需要提前调用Tools菜单中的GeneratorRes方法
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
            GameObject prefab = ResourcesMgr.Load<GameObject>(key);
            GameObject go = GameObject.Instantiate(prefab);
            if (!m_cacheList.ContainsKey(key))
            {
                m_cacheList.Add(key, new List<GameObject>());
            }
            m_cacheList[key].Add(go);

            return go;
        }

        private GameObject FindUsableObject(string key)
        {
            if (m_cacheList.ContainsKey(key))
            {
                return m_cacheList[key].Find(g => !g.activeInHierarchy);
            }
            return null;
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="go"></param>
        /// <param name="delay">延迟时间</param>
        public void CollectObject(GameObject go, float delay = 0)
        {
            StartCoroutine(DelayCollect(go, delay));
        }

        private IEnumerator DelayCollect(GameObject go, float delay)
        {
            yield return new WaitForSeconds(delay);
            go.SetActive(false);
        }

        public void Clear(string key)
        {
            if (m_cacheList.ContainsKey(key))
            {
                for (int i = m_cacheList[key].Count-1; i >= 0; i--)
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
            foreach(var key in keyList)
            {
                Clear(key);
            }
        }
    }
}


