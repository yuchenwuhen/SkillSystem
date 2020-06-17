using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Common
{
    /// <summary>
    /// 脚本单例类
    /// </summary>
    /// T 继承MonoSingleton<T> 代表是子类，能执行Awake方法
    /// <typeparam name="T"></typeparam>
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T m_instance;
        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindObjectOfType<T>();
                    if (m_instance == null)
                    {
                        //new GameObject会执行Awake方法
                        m_instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                    }
                    else
                    {
                        m_instance.AwakeInit();
                    }
                }
                return m_instance;
            }
        }

        public void Awake()
        {
            //允许自行挂载游戏物体中
            if (m_instance == null)
            {
                m_instance = this as T;
                AwakeInit();
            }
        }

        public virtual void AwakeInit()
        {

        }
    }
}


