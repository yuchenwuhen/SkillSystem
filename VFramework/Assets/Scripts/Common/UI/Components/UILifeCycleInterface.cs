using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.UI
{
    public interface UILifeCycleInterface
    {
        void Init(string UIEventKey, int id = 0);

        void Dispose();
    }
}


