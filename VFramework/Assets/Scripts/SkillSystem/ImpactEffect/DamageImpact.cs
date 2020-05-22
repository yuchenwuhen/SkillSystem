using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework.Character;

namespace VFramework.Skill
{
    public class DamageImpact : IImpactEffect
    {
        public void Execute(SkillDeployer deployer)
        {
            for (int i = 0; i < deployer.SkillData.attackTargets.Length; i++)
            {
                CharacterStatus characterStatus = deployer.SkillData.attackTargets[i].GetComponent<CharacterStatus>();
                characterStatus.hp -= (int)deployer.SkillData.attackRatio * 1;
            }
        }
    }
}


