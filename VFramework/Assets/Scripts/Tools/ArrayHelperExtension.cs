using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Tools
{
    public static class ArrayHelperExtension
    {

        public static T Find<T>(this T[] array,Func<T,bool> condtional)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (condtional(array[i]))
                {
                    return array[i];
                }
            }
            return default(T);
        }

    }
}


