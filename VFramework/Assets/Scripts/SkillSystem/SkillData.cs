using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Skill
{
    public enum SkillImpactType
    {
        CostStrength,
        Damage
    }

    public enum SkillAttackType
    {
        Group,
        Single
    }

    public enum SelectorType
    {
        Sector,
        Rectangle
    }

    /// <summary>
    /// 技能数据类
    /// </summary>
    public class SkillData
    {
        /// <summary>
        /// 技能ID
        /// </summary>
        public int skillID;
        /// <summary>
        /// 技能名称
        /// </summary>
        public string name;
        /// <summary>
        /// 技能描述
        /// </summary>
        public string description;
        /// <summary>
        /// 冷却时间
        /// </summary>
        public float coolTime;
        /// <summary>
        /// 冷却剩余时间
        /// </summary>
        public float coolRemain;
        /// <summary>
        /// 消耗体力值
        /// </summary>
        public int costStrength;
        /// <summary>
        /// 攻击距离
        /// </summary>
        public float attackDistance;
        /// <summary>
        /// 攻击目标tags
        /// </summary>
        public string[] attackTargetTags = { "Enemy" };
        /// <summary>
        /// 攻击目标对象数组
        /// </summary>
        [HideInInspector]
        public Transform[] attackTargets;
        /// <summary>
        /// 技能影响类型
        /// </summary>
        public SkillImpactType[] skillImpactTypes;
        /// <summary>
        /// 下次连击ID
        /// </summary>
        public int nextBatterId;
        /// <summary>
        /// 伤害比率
        /// </summary>
        public float attackRatio;
        /// <summary>
        /// 持续时间
        /// </summary>
        public float durationTime;
        /// <summary>
        /// 伤害间隔
        /// </summary>
        public float attackInterval;
        /// <summary>
        /// 技能prefab名称
        /// </summary>
        public string prefabName;
        /// <summary>
        /// 技能prefab
        /// </summary>
        [HideInInspector]
        public GameObject skillPrefab;
        /// <summary>
        /// 技能所属
        /// </summary>
        [HideInInspector]
        public GameObject owner;
        /// <summary>
        /// 动画名称
        /// </summary>
        public string animationName;
        /// <summary>
        /// 受击特效名称
        /// </summary>
        public string hitFxName;
        /// <summary>
        /// 受击特效预制件
        /// </summary>
        [HideInInspector]
        public GameObject hitFxPrefab;
        /// <summary>
        /// 技能等级
        /// </summary>
        public int level;
        /// <summary>
        /// 技能类型 单攻，群攻
        /// </summary>
        public SkillAttackType attackType;
        /// <summary>
        /// 选择类型 扇形（圆形），矩形
        /// </summary>
        public SelectorType selectorType;
    }
}


