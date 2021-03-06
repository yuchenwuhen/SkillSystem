﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Skill
{
    public class DeployerConfigFactory
    {
        public static IAttackSelector CreateAttackSelector(SkillData data)
        {
            //skillData.selectorType
            //选取对象命名规则
            //VFramework.Skill.+枚举名+AttackSelector
            string className = string.Format("VFramework.Skill.{0}AttackSelector", data.selectorType);
            return CreateObject<IAttackSelector>(className);
        }

        public static IImpactEffect[] CreateImpactEffects(SkillData data)
        {
            IImpactEffect[] impacts = new IImpactEffect[data.skillImpactTypes.Length];

            for (int i = 0; i < impacts.Length; i++)
            {
                string className = string.Format("VFramework.Skill.{0}Impact", data.skillImpactTypes[i]);
                impacts[i] = CreateObject<IImpactEffect>(className);
            }

            return impacts;
        }

        public static T CreateObject<T>(string className) where T : class
        {
            //获取类型
            Type type = Type.GetType(className);
            //创建对象
            return Activator.CreateInstance(type) as T;
        }
    }
}

