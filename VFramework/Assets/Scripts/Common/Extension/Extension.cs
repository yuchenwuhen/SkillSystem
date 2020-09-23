using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Common
{
    public static class Extension
    {
        /// <summary>
        /// Checks whether vector is near to zero within a tolerance.
        /// </summary>
        public static bool IsZero(this Vector3 vector3)
        {
            return vector3.sqrMagnitude < 9.99999943962493E-11;
        }

        /// <summary>
        /// Checks whether vector is exceeding the magnitude within a small error tolerance.
        /// </summary>
        public static bool IsExceeding(this Vector3 vector3, float magnitude)
        {
            // Allow 1% error tolerance, to account for numeric imprecision.

            const float errorTolerance = 1.01f;

            return vector3.sqrMagnitude > magnitude * magnitude * errorTolerance;
        }
    }
}

