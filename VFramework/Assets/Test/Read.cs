using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework.Common;

public class Read : MonoBehaviour
{
    float TotleVolume = 1;
    // Start is called before the first frame update
    void Start()
    {
        var go = Instantiate(ResourcesManager.Load<GameObject>("Cube"));
        go.GetComponent<MeshRenderer>().material = ResourcesManager.Load<Material>("TestCube");
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 20), "Save"))
        {
            TotleVolume = 0.3f;
            RecordManager.SaveRecord("GameSettingData", "TotleVolume", TotleVolume);
        }

        if (GUI.Button(new Rect(10, 40, 100, 20), "Show"))
        {
            RecordTable table = RecordManager.GetData("GameSettingData");
            Debug.Log(table["TotleVolume"].GetFloat());
        }
    }
}
