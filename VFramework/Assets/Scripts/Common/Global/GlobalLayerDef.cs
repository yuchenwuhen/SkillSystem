using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Common
{
    /// <summary>
    /// 层级全局函数
    /// </summary>
    public class GlobalLayerDef
    {
        public static int StaticSceneCheckLayer = 1 << LayerMask.NameToLayer("StaticScene");

        public static int StaticSceneLayer = LayerMask.NameToLayer("StaticScene");
    }
}

