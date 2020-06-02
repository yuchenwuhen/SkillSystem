using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework.Common;

public class Read : MonoBehaviour
{
    float TotleVolume = 1;

    private void Awake()
    {
        AudioPlayManager.Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioPlayManager.PlayMusic2D("boss2", 1, 1f);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 20), "Save"))
        {
            AudioPlayManager.StopMusic2D(1);
        }

        if (GUI.Button(new Rect(10, 40, 100, 20), "Show"))
        {
            RecordTable table = RecordManager.GetData("GameSettingData");
            Debug.Log(table["TotleVolume"].GetFloat());
        }
    }
}
