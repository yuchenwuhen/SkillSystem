using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using VFramework.Animation;

[CustomEditor(typeof(FrameAnimation))]
public class FrameAnimationEditor : Editor {
    FrameAnimation frameAnimation;
    const int BigSeparator = 3;
	const int SmallSeparator = 1;

    static string newAnimName = "animName";

    Dictionary<string, bool> animFoldMap = new Dictionary<string, bool>();
    List<string> removeAnimTable = new List<string>();

	void OnSceneGUI()
    {
        FrameAnimation frameAnimation = (FrameAnimation)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Separator(BigSeparator);

        frameAnimation = (FrameAnimation)target;

        // add action		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("新建动画");
		newAnimName = GUILayout.TextField(newAnimName);
		EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        bool addAnim1 = GUILayout.Button("新建一阶段动画");
        bool addAnim3 = GUILayout.Button("新建三阶段动画");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if( GUILayout.Button("保存") )
            Save();

        if( GUILayout.Button("读取") )
            Load();
            
        EditorGUILayout.EndHorizontal();

		if( (addAnim1 || addAnim3) && !frameAnimation.HasAnim(newAnimName) )
		{
            MyAnimation anim = new MyAnimation();
            anim.name = newAnimName;
            anim.oneStage = addAnim1;
            frameAnimation.animations.Add(anim);
		}
		Separator(BigSeparator);

        DoRemoveAction();

		// del action
		//DoRemoveAction();

		for (int i=0; i<frameAnimation.animations.Count; ++i)
		{
			MyAnimation anim = frameAnimation.animations[i];
			DrawAnim(anim);
		}
    }

    void DrawAnim(MyAnimation anim )
    {
        if (!animFoldMap.ContainsKey(anim.name))
		{
			animFoldMap[anim.name] = false;
		}

        EditorGUILayout.BeginHorizontal();
		animFoldMap[anim.name] = EditorGUILayout.Foldout(animFoldMap[anim.name], "Anim: " + anim.name);

		if (GUILayout.Button("播放"))
		{
            frameAnimation.UpdateAnimRuntimeData(anim);
            frameAnimation.Play(anim.name, null, true);
		}
		if (GUILayout.Button("删除"))
		{
            RemoveAnim(anim.name);
		}

		EditorGUILayout.EndHorizontal();
		if (!animFoldMap[anim.name])
			return;

        EditorGUILayout.BeginHorizontal();
		GUILayout.Label("loop");
		anim.loop = EditorGUILayout.Toggle(anim.loop);	
		EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
		GUILayout.Label("sprite path");
		anim.spritePath = EditorGUILayout.TextField(anim.spritePath != null ? anim.spritePath : "");	
		EditorGUILayout.EndHorizontal();

        if( anim.oneStage )
        {
            EditorGUILayout.BeginHorizontal();
		    GUILayout.Label("sprite start index");
		    anim.spriteStart = EditorGUILayout.IntField(anim.spriteStart);	
		    EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
		    GUILayout.Label("sprite number");
		    anim.spriteNum = EditorGUILayout.IntField(anim.spriteNum);	
		    EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("totle frame number");
            int frameLenthBk = anim.frameLenth;
            anim.frameLenth = EditorGUILayout.IntField(anim.frameLenth);
            if( frameLenthBk != anim.frameLenth )
                anim.framePerSprite = FrameDistribution(anim.spriteNum, anim.frameLenth);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("frame per sprite");
            string framePerSpriteBk = anim.framePerSprite;
            anim.framePerSprite = EditorGUILayout.TextField(anim.framePerSprite);
            if( framePerSpriteBk != anim.framePerSprite )
                anim.frameLenth = FrameSum(anim.framePerSprite);
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            anim.stageData[0] = EditorGUILayout.Vector3IntField("第一阶段", anim.stageData[0]);
            anim.stageData[1] = EditorGUILayout.Vector3IntField("第一阶段", anim.stageData[1]);
            anim.stageData[2] = EditorGUILayout.Vector3IntField("第一阶段", anim.stageData[2]);
        }
    }

    string FrameDistribution(int spriteNum, int frameNum)
    {
        string strRet = "";
        if( spriteNum == 0 )
            return strRet;

        // 平均一个sprite播放的帧数的整数部分
        int oneSpriteFrameNum = frameNum / spriteNum;
        // 一个sprite播放的帧数的余数部分
        int frameNumRemainder = frameNum % spriteNum;
        int remainderAdd = 0;

        for( int i=0; i<spriteNum; i++ )
        {
            int frameCount = oneSpriteFrameNum;
            remainderAdd += frameNumRemainder;

            if( remainderAdd >= spriteNum )
            {
                remainderAdd -= spriteNum;
                frameCount += 1;
            }

            strRet += frameCount.ToString();
            if( i != spriteNum - 1 )
                strRet += " ";
        }

        return strRet;
    }

    int FrameSum(string strFrames)
    {
        int sum = 0;
        char[] aa = { ' '};
        string[] frames = strFrames.Split(aa);

        for( int i = 0; i < frames.Length; i++ )
        {
            int ii;
            if( int.TryParse(frames[i], out ii) )
                sum += ii;
        }

        return sum;
    }

    void Separator(int s=1)
	{
		for (int i=0; i<s; ++i)
		{
			EditorGUILayout.Separator();
		}
	}

    void RemoveAnim(string animName)
	{
		if (!removeAnimTable.Contains(animName))
			removeAnimTable.Add(animName);
	}

    void DoRemoveAction()
	{
		foreach(var animName in removeAnimTable)
		{
			for( int i=0; i<frameAnimation.animations.Count; i++ )
            {
                if( frameAnimation.animations[i].name == animName )
                {
                    frameAnimation.animations.RemoveAt(i);
                    break;
                }
            }
		}
		removeAnimTable.Clear();
	}

    void Save()
    {
        string strDirectory = Application.dataPath;
        string defaultName = "Temp";

        if( !string.IsNullOrEmpty(frameAnimation.animationFilePath) )
        {
            string filePath = Application.dataPath + "/Resources/" + frameAnimation.animationFilePath; 

            strDirectory = Path.GetDirectoryName(filePath);
            defaultName = Path.GetFileNameWithoutExtension(filePath);
        }

        string strPath = EditorUtility.SaveFilePanel("保存动画文件", strDirectory, defaultName, "txt");

        if( !string.IsNullOrEmpty(strPath) )
        {
            frameAnimation.Save(strPath);
            AssetDatabase.Refresh();
        }
    }

    void Load()
    {
        string[] filters = {"anim file", "txt"};
        string defaultPath = Application.dataPath + "/Resources/Animations";

        if( !string.IsNullOrEmpty(frameAnimation.animationFilePath) )
        {
            string filePath = Application.dataPath + "/Resources/" + frameAnimation.animationFilePath; 
            defaultPath = Path.GetDirectoryName(filePath);
        }

        string strPath = EditorUtility.OpenFilePanelWithFilters("加载动画文件", defaultPath, filters);

        if (!string.IsNullOrEmpty(strPath))
            frameAnimation.LoadWithFullPath(strPath);
    }
}
