using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework.Character;
using VFramework.Common;

namespace VFramework.Skill
{
    /// <summary>
    /// 扇形（圆形）选区
    /// </summary>
    public class SectorAttackSelector : IAttackSelector
    {
        public Transform[] SelectTarget(SkillData data, Transform skillMaster)
        {
            //根据技能数据中的标签 获取所有目标
            List<Transform> targets = new List<Transform>();
            for (int i = 0; i < data.attackTargetTags.Length; i++)
            {
                GameObject[] tempGoArray = GameObject.FindGameObjectsWithTag(data.attackTargetTags[i]);
                targets.AddRange(tempGoArray.Select(t => t.transform));
            }

            //判断攻击范围（扇形、圆形）
            targets = targets.FindAll(t => 
                (Vector3.Distance(t.position, skillMaster.position) <= data.attackDistance) &&
                (Vector3.Angle(skillMaster.forward,t.position-skillMaster.position) <= data.attackAngle)
            );

            //筛选活的角色
            targets = targets.FindAll(t => t.GetComponent<CharacterStatus>().hp >= 0);

            //返回目标（单攻/群攻）
            Transform[] result = targets.ToArray();
            if (data.attackType == SkillAttackType.Group || result.Length == 0)
                return result;

            Transform min = result.GetMin(t => Vector3.Distance(skillMaster.position, t.position));
            return new Transform[] { min };
        }
    }
}


