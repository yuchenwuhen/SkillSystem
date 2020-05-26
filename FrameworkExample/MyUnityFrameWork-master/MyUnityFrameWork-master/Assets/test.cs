using UnityEngine;
using System.Collections;

public class test : MonoBehaviour 
{
    //public GameObject cube;
	// Use this for initialization
	void Start () 
    {
        //AnimSystem.Move(gameObject, null,Vector3.zero, 0.5f, isLocal: true);
        //AnimSystem.Move(gameObject,,,,,,,,,,)
	}

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10,10,100,20),"Test"))
        {
            AudioPlayManager.TotleVolume = 0.7f;
            AudioPlayManager.SaveVolume();
            Debug.Log("保存成功");
        }

        if (GUI.Button(new Rect(10, 40, 100, 20), "Result"))
        {
            Debug.Log("读取值："+RecordManager.GetFloatRecord("GameSettingData", "TotleVolume", 1f));
        }
    }
}
