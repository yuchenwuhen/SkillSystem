using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Read : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(data.num);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 20), "Test"))
        {
            RecordTable table = RecordManager.GetData("cc");
            table.SetRecord("key", 2);
        }

        if (GUI.Button(new Rect(10, 40, 100, 20), "Result"))
        {
            RecordTable table = RecordManager.GetData("cc");
            Debug.Log(table["key"].num);
        }
    }
}
