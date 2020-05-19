using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace VFramework.Tools
{
    public class Singleton<T> where T : class, new()
    {

        private static T _Instance;


        protected Singleton()
        {
        }

        public static T Instance
        {
            get
            {
                if (null == _Instance)
                {
                    _Instance = new T();
                }

                return _Instance;
            }
        }

        public static void Destroy()
        {
            _Instance = null;
        }
    }

    public static class SingletonProperty<T> where T : class
    {
        private static T mInstance;
        private static readonly object mLock = new object();

        public static T Instance
        {
            get
            {
                lock (mLock)
                {
                    if (mInstance == null)
                    {
                        mInstance = SingletonCreator.CreateSingleton<T>();
                    }
                }

                return mInstance;
            }
        }
    }

    public static class SingletonCreator
    {
        public static T CreateSingleton<T>() where T : class
        {
            // 获取私有构造函数
            var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

            // 获取无参构造函数
            var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);

            if (ctor == null)
            {
                throw new Exception("Non-Public Constructor() not found! in " + typeof(T));
            }

            // 通过构造函数，常见实例
            var retInstance = ctor.Invoke(null) as T;

            return retInstance;
        }
    }
}


