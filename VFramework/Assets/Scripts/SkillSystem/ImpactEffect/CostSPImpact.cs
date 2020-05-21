using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework.Character;

namespace VFramework.Skill
{
    public class CostSPImpact : IImpactEffect
    {
        public void Execute(SkillDeployer deployer)
        {
            CharacterStatus characterStatus = deployer.SkillData.owner.GetComponent<CharacterStatus>();
            characterStatus.sp -= deployer.SkillData.costSP;
        }
    }
}


