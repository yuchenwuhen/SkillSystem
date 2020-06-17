using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Common
{
    public interface IPool<T>
    {
        /// <summary>
        /// 分配
        /// </summary>
        /// <returns></returns>
        T Allocate();

        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool Recycle(T obj);

        bool Contains(T obj);

        void RecycleAll();
    }

    public abstract class Pool<T> : IPool<T>
    {
        protected readonly Stack<T> m_cacheStack = new Stack<T>();

        protected IObjectFactory<T> m_factory;

        protected int m_maxCount = 12;

        public int CurCount
        {
            get { return m_cacheStack.Count; }
        }

        public virtual T Allocate()
        {
            return m_cacheStack.Count == 0
                ? m_factory.Create()
                : m_cacheStack.Pop();
        }

        public bool Contains(T obj)
        {
            if (m_cacheStack.Contains(obj))
            {
                return true;
            }
            return false;
        }

        public abstract bool Recycle(T obj);

        public abstract void RecycleAll();
    }

    public class SimpleObjectPool<T> : Pool<T>
    {
        readonly Action<T> m_resetMethod;

        public SimpleObjectPool(Func<T> factoryMethod, Action<T> resetMethod = null, int initCount = 0)
        {
            m_factory = new CustomObjectFactory<T>(factoryMethod);
            m_resetMethod = resetMethod;

            for (int i = 0; i < initCount; i++)
            {
                m_cacheStack.Push(m_factory.Create());
            }
        }

        public override bool Recycle(T obj)
        {
            if (m_resetMethod != null)
            {
                m_resetMethod(obj);
            }

            m_cacheStack.Push(obj);
            return true;
        }

        public override void RecycleAll()
        {
            m_cacheStack.Clear();
        }
    }

    /// <summary>
    /// I pool able.
    /// </summary>
    public interface IPoolable
    {
        void OnRecycled();
        bool IsRecycled { get; set; }
    }

    public class SafeObjectPool<T> : Pool<T> where T : IPoolable, new()
    {
        protected SafeObjectPool()
        {
            m_factory = new DefaultObjectFactory<T>();
        }

        public static SafeObjectPool<T> Instance
        {
            get { return SingletonProperty<SafeObjectPool<T>>.Instance; }
        }

        /// <summary>
        /// Init the specified maxCount and initCount.
        /// </summary>
        /// <param name="maxCount">Max Cache count.</param>
        /// <param name="initCount">Init Cache count.</param>
        public void Init(int maxCount, int initCount)
        {
            MaxCacheCount = maxCount;

            if (maxCount > 0)
            {
                initCount = Math.Min(maxCount, initCount);
            }

            if (CurCount < initCount)
            {
                for (var i = CurCount; i < initCount; ++i)
                {
                    Recycle(new T());
                }
            }
        }

        /// <summary>
        /// Gets or sets the max cache count.
        /// </summary>
        /// <value>The max cache count.</value>
        public int MaxCacheCount
        {
            get { return m_maxCount; }
            set
            {
                m_maxCount = value;

                if (m_cacheStack != null)
                {
                    if (m_maxCount > 0)
                    {
                        if (m_maxCount < m_cacheStack.Count)
                        {
                            int removeCount = m_cacheStack.Count - m_maxCount;
                            while (removeCount > 0)
                            {
                                m_cacheStack.Pop();
                                --removeCount;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Allocate T instance.
        /// </summary>
        public override T Allocate()
        {
            var result = base.Allocate();
            result.IsRecycled = false;
            return result;
        }

        /// <summary>
        /// Recycle the T instance
        /// </summary>
        /// <param name="t">T.</param>
        public override bool Recycle(T t)
        {
            if (t == null || t.IsRecycled)
            {
                return false;
            }

            if (m_maxCount > 0)
            {
                if (m_cacheStack.Count >= m_maxCount)
                {
                    t.OnRecycled();
                    return false;
                }
            }

            t.IsRecycled = true;
            t.OnRecycled();
            m_cacheStack.Push(t);

            return true;
        }

        public override void RecycleAll()
        {
            m_cacheStack.Clear();
        }
    }

    public interface IObjectFactory<T>
    {
        T Create();
    }

    public class DefaultObjectFactory<T> : IObjectFactory<T> where T : new()
    {
        public T Create()
        {
            return new T();
        }
    }

    public class CustomObjectFactory<T> : IObjectFactory<T>
    {
        protected Func<T> m_factoryMethod;

        public CustomObjectFactory(Func<T> factoryMethod)
        {
            m_factoryMethod = factoryMethod;
        }

        public T Create()
        {
            return m_factoryMethod();
        }
    }
}




