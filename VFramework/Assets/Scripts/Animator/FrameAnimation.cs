using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using LitJson;
using VFramework.Common;

namespace VFramework.Animation
{
    public class MyAnimation
    {
        public string name;
        public bool loop = true;
        public bool oneStage;
        public int frameLenth;
        public string spritePath;
        public int spriteStart;
        public int spriteNum;
        public string framePerSprite = "";
        public Vector3Int[] stageData = new Vector3Int[3];

        // 下面是运行时数据
        public Sprite[] sprites;
        public int[] frameSpriteIndex;
    }

    public enum RenderStyle
    {
        Normal,
        White,
    }

    public class FrameAnimation : MonoBehaviour
    {
        public string animationFilePath;

        public List<MyAnimation> animations = new List<MyAnimation>();

        public SpriteRenderer spriteRenderer;

        static Material materialNormal;
        static Material materialWhite;

        // 当前播放的动画
        MyAnimation curAnim;

        // 从动画播放开始一共过了多少帧
        uint animFrameCount = 0;
        // 动画当前播放帧
        uint animFrame = 0;
        // 动画播完一个完整帧序列后的回调
        System.Action animCallback;
        // 当前帧所用到的sprite
        Sprite curSprite;

        bool bPause = false;

        private void Awake()
        {
            if (materialNormal == null)
                materialNormal = spriteRenderer.material;

            if (materialWhite == null)
                materialWhite = Resources.Load<Material>("Materials/Sprites-White");

            if (!string.IsNullOrEmpty(animationFilePath))
                //Load(Application.dataPath + "/Resources/" + animationFilePath + ".manim");
                Load(animationFilePath);
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (curAnim != null)
            {
                if (animCallback != null && animFrameCount == curAnim.frameLenth)
                    animCallback();
            }
        }

        public void LateUpdate()
        {
            if (Time.timeScale == 0)
                return;

            if (bPause)
                return;

            if (curAnim != null)
            {
                if (curAnim.loop)
                    animFrame = animFrameCount % (uint)curAnim.frameLenth;
                else
                {
                    if (animFrameCount < (uint)curAnim.frameLenth)
                        animFrame = animFrameCount % (uint)curAnim.frameLenth;
                    else
                        animFrame = (uint)curAnim.frameLenth - 1;
                }

                curSprite = curAnim.sprites[curAnim.frameSpriteIndex[animFrame]];

                if (spriteRenderer != null)
                    spriteRenderer.sprite = curSprite;

                animFrameCount++;
            }
        }

        public void Pause(bool pause)
        {
            bPause = pause;
        }

        public void SetRenderStyle(RenderStyle renderStyle)
        {
            if (renderStyle == RenderStyle.Normal)
            {
                spriteRenderer.material = materialNormal;
            }
            else if (renderStyle == RenderStyle.White)
            {
                spriteRenderer.material = materialWhite;
            }
        }

        // 返回这一帧所用的sprite
        public Sprite GetSpriteThisFrame()
        {
            return curSprite;
        }

        public bool HasAnim(string name)
        {
            for (int i = 0; i < animations.Count; i++)
            {
                if (animations[i].name == name)
                    return true;
            }

            return false;
        }

        public void Save(string strPath)
        {
            //filePath = strPath;
            var animsJsonData = new JsonData();

            for (int i = 0; i < animations.Count; i++)
            {
                MyAnimation anim = animations[i];
                var oneAnimJsonData = new JsonData();
                animsJsonData.Add(oneAnimJsonData);
                oneAnimJsonData["name"] = anim.name;
                oneAnimJsonData["loop"] = anim.loop;
                oneAnimJsonData["oneStage"] = anim.oneStage;
                oneAnimJsonData["spritePath"] = anim.spritePath;

                if (animations[i].oneStage)
                {
                    oneAnimJsonData["spriteStart"] = anim.spriteStart;
                    oneAnimJsonData["spriteNum"] = anim.spriteNum;
                    oneAnimJsonData["frameLenth"] = anim.frameLenth;
                    oneAnimJsonData["framePerSprite"] = anim.framePerSprite;
                }
                else
                {
                    var stagesJsonData = new JsonData();
                    oneAnimJsonData["stages"] = stagesJsonData;

                    for (int j = 0; j < 3; j++)
                    {
                        var oneStageJsonData = new JsonData();
                        stagesJsonData.Add(oneStageJsonData);

                        oneStageJsonData["startSprite"] = anim.stageData[j].x;
                        oneStageJsonData["spriteNum"] = anim.stageData[j].y;
                        oneStageJsonData["frameNum"] = anim.stageData[j].z;
                    }
                }
            }

            FilePathHelp.SaveStringToFile(strPath, animsJsonData.ToJson());
        }

        public void Play(string animName, System.Action callback = null, bool forceReplay = false)
        {
            if (curAnim != null && curAnim.name == animName && !forceReplay)
                return;

            for (int i = 0; i < animations.Count; i++)
            {
                if (animations[i].name == animName)
                {
                    animCallback = callback;
                    curAnim = animations[i];
                    break;
                }
            }

            animFrameCount = 0;
        }

        public void Load(string strPath)
        {
            Object textObj = Resources.Load(strPath);
            if (textObj == null)
                return;

            TextAsset textAsset = textObj as TextAsset;
            string str = Encoding.UTF8.GetString(textAsset.bytes);

            //string str = GlobalFunctions.LoadFileToString(strPath);
            JsonData animsJsonData = JsonMapper.ToObject(str);
            Load(animsJsonData);
        }

        public void LoadWithFullPath(string strPath)
        {
            string str = FilePathHelp.LoadFileToString(strPath);
            JsonData animsJsonData = JsonMapper.ToObject(str);
            Load(animsJsonData);
        }

        public void Load(JsonData animsJsonData)
        {
            animations.Clear();

            for (int i = 0; i < animsJsonData.Count; i++)
            {
                var oneAnimJsonData = animsJsonData[i];
                MyAnimation anim = new MyAnimation();

                anim.name = (string)oneAnimJsonData["name"];

                anim.loop = true;
                if (oneAnimJsonData.Keys.Contains("loop"))
                    anim.loop = (bool)oneAnimJsonData["loop"];

                anim.oneStage = (bool)oneAnimJsonData["oneStage"];
                anim.spritePath = (string)oneAnimJsonData["spritePath"];

                if (anim.oneStage)
                {
                    anim.spriteStart = (int)oneAnimJsonData["spriteStart"];
                    anim.spriteNum = (int)oneAnimJsonData["spriteNum"];
                    anim.frameLenth = (int)oneAnimJsonData["frameLenth"];
                    anim.framePerSprite = (string)oneAnimJsonData["framePerSprite"];
                }
                else
                {
                    var stagesJsonData = oneAnimJsonData["stages"];

                    for (int j = 0; j < 3; j++)
                    {
                        var oneStageJsonData = stagesJsonData[j];

                        anim.stageData[j].x = (int)oneStageJsonData["startSprite"];
                        anim.stageData[j].y = (int)oneStageJsonData["spriteNum"];
                        anim.stageData[j].z = (int)oneStageJsonData["frameNum"];
                    }
                }

                UpdateAnimRuntimeData(anim);
                animations.Add(anim);
            }
        }

        // 更新一个动画的运行时数据
        public void UpdateAnimRuntimeData(MyAnimation anim)
        {
            if (!anim.oneStage)
                anim.frameLenth = anim.stageData[0].z + anim.stageData[1].z + anim.stageData[2].z;

            anim.sprites = new Sprite[anim.spriteNum];
            anim.frameSpriteIndex = new int[anim.frameLenth];

            Sprite[] sprites = Resources.LoadAll<Sprite>(anim.spritePath);

            if (sprites != null && sprites.Length > 0)
            {
                for (int i = 0; i < anim.spriteNum; i++)
                {
                    string spriteName = anim.spritePath.Substring(anim.spritePath.LastIndexOf('/') + 1);
                    spriteName = spriteName + "_" + (anim.spriteStart + i).ToString();

                    for (int j = 0; j < sprites.Length; j++)
                    {
                        if (sprites[j].name == spriteName)
                            anim.sprites[i] = sprites[j];
                    }
                }
            }

            if (anim.oneStage)
            {
                // 将字符串解析成整数数组
                bool rightFormate = false;
                char[] aa = { ' ' };
                string[] strFrames = anim.framePerSprite.Split(aa);
                int[] intFrames = null;

                if (strFrames != null && strFrames.Length == anim.spriteNum)
                {
                    rightFormate = true;
                    intFrames = new int[strFrames.Length];
                    int frameSum = 0;

                    for (int i = 0; i < strFrames.Length; i++)
                    {
                        int ii;
                        if (int.TryParse(strFrames[i], out ii))
                        {
                            intFrames[i] = ii;
                            frameSum += ii;
                        }
                        else
                        {
                            rightFormate = false;
                            break;
                        }
                    }

                    if (frameSum != anim.frameLenth)
                        rightFormate = false;
                }

                int frameIdx = 0;
                if (rightFormate)
                {
                    for (int i = 0; i < anim.spriteNum; i++)
                    {
                        for (int j = 0; j < intFrames[i]; j++)
                            anim.frameSpriteIndex[frameIdx++] = i;
                    }
                }
                else
                {
                    // 平均一个sprite播放的帧数的整数部分
                    int oneSpriteFrameNum = anim.frameLenth / anim.spriteNum;
                    // 一个sprite播放的帧数的余数部分
                    int frameNumRemainder = anim.frameLenth % anim.spriteNum;
                    int remainderAdd = 0;

                    for (int i = 0; i < anim.spriteNum; i++)
                    {
                        int frameNum = oneSpriteFrameNum;
                        remainderAdd += frameNumRemainder;

                        if (remainderAdd >= anim.spriteNum)
                        {
                            remainderAdd -= anim.spriteNum;
                            frameNum += 1;
                        }

                        for (int j = 0; j < frameNum; j++)
                            anim.frameSpriteIndex[frameIdx++] = i;
                    }
                }
            }
            else
            {
                int frameIdx = 0;

                for (int i = 0; i < 3; i++)
                {
                    // 平均一个sprite播放的帧数的整数部分
                    int oneSpriteFrameNum = anim.stageData[i].z / anim.stageData[i].y;
                    // 一个sprite播放的帧数的余数部分
                    int frameNumRemainder = anim.stageData[i].z % anim.stageData[i].y;
                    int remainderAdd = 0;

                    for (int j = 0; j < anim.stageData[i].y; j++)
                    {
                        int frameNum = oneSpriteFrameNum;
                        remainderAdd += frameNumRemainder;

                        if (remainderAdd >= anim.stageData[i].y)
                        {
                            remainderAdd -= anim.stageData[i].y;
                            frameNum += 1;
                        }

                        for (int k = 0; k < frameNum; k++)
                            anim.frameSpriteIndex[frameIdx++] = anim.stageData[i].x + j;
                    }
                }
            }
        }
    }
}


