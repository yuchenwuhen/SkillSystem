using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Skill
{
    /// <summary>
    /// 影响效果算法接口
    /// </summary>
    public interface IImpactEffect
    {
        void Execute(SkillDeployer deployer);
    }
}


