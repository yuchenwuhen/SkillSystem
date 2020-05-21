using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Skill
{
    public abstract class SkillDeployer : MonoBehaviour
    {
        private SkillData m_skillData;
        public SkillData SkillData
        {
            get
            {
                return m_skillData;
            }
            set
            {
                m_skillData = value;
            }
        }

        private IAttackSelector selector;
        private IImpactEffect[] impactEffects;

        //初始化释放器
        private void InitDeployer()
        {
            //创建算法对象 skillData.VectorType
            selector = DeployerConfigFactory.CreateAttackSelector(m_skillData);

            //创建影响效果
            impactEffects = DeployerConfigFactory.CreateImpactEffects(m_skillData);
        }

        //执行算法对象
        //选区
        public void CalculateTargets()
        {
            m_skillData.attackTargets = selector.SelectTarget(m_skillData, transform);
        }

        public void ImpactTargets()
        {
            for (int i = 0; i < impactEffects.Length; i++)
            {
                impactEffects[i].Execute(this);
            }
        }

        //释放方式
        public abstract void DeploySkill();
    }
}


