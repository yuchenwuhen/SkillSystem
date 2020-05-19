using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Common
{
    public static class ArrayHelperExtension
    {
        /// <summary>
        /// 根据条件查找数组元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="conditional"></param>
        /// <returns></returns>
        public static T Find<T>(this T[] array, Func<T, bool> conditional)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (conditional(array[i]))
                {
                    return array[i];
                }
            }
            return default(T);
        }

        /// <summary>
        /// 根据条件查找数组元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="conditional">比较条件</param>
        /// <returns></returns>
        public static T[] FindAll<T>(this T[] array, Func<T, bool> conditional)
        {
            List<T> list = new List<T>();
            for (int i = 0; i < array.Length; i++)
            {
                if (conditional(array[i]))
                {
                    list.Add(array[i]);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 获取数组最小值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Q"></typeparam>
        /// <param name="array"></param>
        /// <param name="conditional"></param>
        ///使用： int[] array = new int[5] { 1, 2, 3, 4, 5 };  array.GetMin(t => t);
        /// <returns></returns>
        public static T GetMin<T,Q>(this T[] array, Func<T,Q> conditional) where Q : IComparable
        {
            if (array.Length <= 0)
            {
                //T t=default(T),就是初始化，值类型的话，就是T t=0;引用类型的话，就是T t=null
                return default(T);
            }
            T min = array[0];
            for (int i = 0; i < array.Length; i++)
            {
                if (conditional(min).CompareTo(conditional(array[i])) > 0)
                {
                    min = array[i];
                }
            }
            return min;
        }

        /// <summary>
        /// 获取数组最大值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Q"></typeparam>
        /// <param name="array"></param>
        /// <param name="conditional"></param>
        /// <returns></returns>
        public static T GetMax<T, Q>(this T[] array, Func<T, Q> conditional) where Q : IComparable
        {
            if (array.Length <= 0)
            {
                //T t=default(T),就是初始化，值类型的话，就是T t=0;引用类型的话，就是T t=null
                return default(T);
            }
            T max = array[0];
            for (int i = 0; i < array.Length; i++)
            {
                if (conditional(max).CompareTo(conditional(array[i])) < 0)
                {
                    max = array[i];
                }
            }
            return max;
        }

        /// <summary>
        /// 降序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Q"></typeparam>
        /// <param name="array"></param>
        /// <param name="conditional"></param>
        /// <returns></returns>
        /// 3 2 1 5 4
        public static void OrderDescending<T,Q>(this T[] array, Func<T,Q> conditional) where Q : IComparable
        {
            for (int i = 0; i < array.Length; i++)
            {
                bool isSort = true;
                for (int j = 0; j < array.Length - 1 - i; j++)
                {
                    if (conditional(array[j]).CompareTo(conditional(array[j+1])) < 0)
                    {
                        var temp = array[j];
                        array[j] = array[j+1];
                        array[j+1] = temp;
                        isSort = false;
                    }
                }
                if (isSort)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 升序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Q"></typeparam>
        /// <param name="array"></param>
        /// <param name="conditional"></param>
        /// <returns></returns>
        /// 3 2 1 5 4
        public static void OrderAscending<T, Q>(this T[] array, Func<T, Q> conditional) where Q : IComparable
        {
            for (int i = 0; i < array.Length; i++)
            {
                bool isSort = true;
                for (int j = 0; j < array.Length - 1 - i; j++)
                {
                    if (conditional(array[j]).CompareTo(conditional(array[j + 1])) > 0)
                    {
                        var temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                        isSort = false;
                    }
                }
                if (isSort)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 筛选数组特定条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Q"></typeparam>
        /// <param name="array"></param>
        /// <param name="conditional"></param>
        /// array..Select(go => go.GetComponent<Enemy>());
        /// <returns></returns>
        public static Q[] Select<T,Q>(this T[] array, Func<T,Q> conditional)
        {
            List<Q> list = new List<Q>();
            for (int i = 0; i < array.Length; i++)
            {
                Q q = conditional(array[i]);
                if (q != null)
                {
                    list.Add(conditional(array[i]));
                }
            }
            return list.ToArray();
        }
    }
}


