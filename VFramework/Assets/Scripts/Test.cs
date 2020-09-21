using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VFramework.UI;

public delegate void TestHandler(PointerEventData pointEventData);

public class Test : MonoBehaviour,IPointerDownHandler
{
    public TestHandler testHandler;

    //PointerEventHandler
    void Awake()
    {
        //UIManager.Init();
    }
    // Start is called before the first frame update
    void Start()
    {
        //UIManager.OpenUIWindow<TestWindow>();

        //GameObject cc = Instantiate( Resources.Load<GameObject>("Audio/Cube"));
        //GameObject cc = Instantiate( Resources.Load<GameObject>("audio/cube"));
    }


    // Update is called once per frame
    void Update()
    {
        Debug.Log("测试");
        if (Input.GetKey(KeyCode.A))
        {
            Debug.Log("anjian");
        }
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    UIManager.CloseUIWindow<TestWindow>();
        //}
        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    UIManager.OpenUIWindow<TestWindow>();
        //}
        //Debug.Log("测试" + Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
}
