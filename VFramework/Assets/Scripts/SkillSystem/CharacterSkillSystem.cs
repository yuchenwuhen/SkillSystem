using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework.Common;
using VFramework.Animation;

namespace VFramework.Skill
{
    [RequireComponent(typeof(SkillManager))]
    /// <summary>
    /// 
    /// </summary>
    public class CharacterSkillSystem : MonoBehaviour
    {
        private SkillManager m_skillManager;
        private SkillData m_skillData;
        private FrameAnimation m_frameAnimation;

        private void Start()
        {
            m_skillManager = this.GetComponent<SkillManager>();
            m_frameAnimation = this.GetComponent<FrameAnimation>();
        }

        public void AttackUseSkill(int id)
        {
            //准备技能
            m_skillData = m_skillManager.PrepareSkill(id);
            if (m_skillData == null)
            {
                return;
            }
            //播放动画
            m_frameAnimation.Play(m_skillData.animationNamePre, () =>
            {
                //前摇结束 生成技能
                DeploySkill();
                m_frameAnimation.Play(m_skillData.animationName, () =>
                {
                    m_frameAnimation.Play(m_skillData.animationNameEnd, () =>
                    {

                    });
                });
            });
            //如果单攻
            Transform target = SelectTarget();
        }

        private Transform SelectTarget()
        {
            Transform[] target = new SectorAttackSelector().SelectTarget(m_skillData, transform);
            return target.Length != 0 ? target[0] : null;
        }

        private void DeploySkill()
        {
            m_skillManager.GenerateSkill(m_skillData);
        }

        /// <summary>
        /// 使用随机技能
        /// </summary>
        public void AttackRandomSkill()
        {
            var usableSkills = m_skillManager.skillDatas.FindAll(t => m_skillManager.PrepareSkill(t.skillID) != null);
            if (usableSkills.Length == 0)
            {
                return;
            }

            int index = UnityEngine.Random.Range(0, usableSkills.Length);
            AttackUseSkill(index);
        }
    }
}


