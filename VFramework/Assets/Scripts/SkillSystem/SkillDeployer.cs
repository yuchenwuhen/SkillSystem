using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Skill
{
    public class SkillDeployer : MonoBehaviour
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

        //初始化释放器
        private void InitDeployer()
        {
            //创建算法对象 skillData.VectorType

            //创建影响效果
            for (int i = 0; i < m_skillData.skillImpactTypes.Length; i++)
            {

            }
        }

        //执行算法对象

        //释放方式
    }
}


