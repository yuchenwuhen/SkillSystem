using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Skill
{
    /// <summary>
    /// 近身释放器
    /// </summary>
    public class MeleeSkillDeployer : SkillDeployer
    {
        public override void DeploySkill()
        {
            CalculateTargets();

            ImpactTargets();
        }
    }
}


