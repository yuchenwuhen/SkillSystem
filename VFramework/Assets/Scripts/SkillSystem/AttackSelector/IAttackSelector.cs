using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Skill
{
    public interface IAttackSelector
    {
        /// <summary>
        /// 搜索目标
        /// </summary>
        /// <param name="data">技能数据</param>
        /// <param name="skillMaster">技能所在组件Transform</param>
        /// <returns></returns>
        Transform[] SelectTarget(SkillData data, Transform skillMaster);
    }
}


