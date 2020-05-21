using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework.Character;
using VFramework.Common;

namespace VFramework.Skill
{
    /// <summary>
    /// 技能管理器
    /// 初始化技能列表，判断技能生成条件,生成技能
    /// </summary>
    public class SkillManager : MonoBehaviour
    {
        public SkillData[] skillDatas;

        private CharacterStatus m_characterStatus;

        private void Awake()
        {
            m_characterStatus = GetComponent<CharacterStatus>();
        }

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < skillDatas.Length; i++)
            {
                InitData(skillDatas[i]);
            }
        }

        /// <summary>
        /// 初始化技能数据
        /// </summary>
        /// <param name=""></param>
        private void InitData(SkillData data)
        {
            data.skillPrefab = ResourcesMgr.Load<GameObject>(data.prefabName);
            data.owner = gameObject;
        }

        public SkillData PrepareSkill(int id)
        {
            //根据Id 查找技能数据
            SkillData data = skillDatas.Find<SkillData>(t => t.skillID == id);
            
            //判断条件
            if (data != null && data.coolRemain <= 0 && data.costSP <= m_characterStatus.sp)
            {
                return data;
            }
            return null;
        }

        /// <summary>
        /// 生成技能
        /// </summary>
        /// <param name="data"></param>
        public void GenerateSkill(SkillData data)
        {
            //创建技能预制件
            //GameObject go = Instantiate(data.skillPrefab, transform.position, Quaternion.identity);
            GameObject go = GameObjectPool.Instance.CreateObject(data.prefabName, transform.position, Quaternion.identity);

            //传递技能数据
            var skillDeploy = go.GetComponent<SkillDeployer>();
            skillDeploy.SkillData = data;
            skillDeploy.DeploySkill();

            //销毁预制件
            //Destroy(go,data.durationTime);
            GameObjectPool.Instance.CollectObject(go);

            //技能冷却
            StartCoroutine(CoolTimeDown(data));
        }

        /// <summary>
        /// 技能冷却
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IEnumerator CoolTimeDown(SkillData data)
        {
            data.coolRemain = data.coolTime;
            while (data.coolRemain > 0)
            {
                yield return null;
                data.coolRemain -= Time.deltaTime;
            }
        }
    }
}


