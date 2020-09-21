using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VFramework.UI
{
    public class ReusingScrollItemData : Dictionary<string, object>
    {
        public float m_size;

        public Bounds m_bounds;

        public Bounds GetBounds()
        {
            return new Bounds();
        }
    }
}


