using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public delegate void TestHandler(PointerEventData pointEventData);

public class Test : MonoBehaviour,IPointerDownHandler
{
    public TestHandler testHandler;

    //PointerEventHandler

    // Start is called before the first frame update
    void Start()
    {
        testHandler = TestC;
        testHandler = TestB;
        testHandler(null);
    }

    void TestC(PointerEventData pointEventData)
    {
        Debug.Log("CC");
    }

    void TestB(PointerEventData pointEventData)
    {
        Debug.Log("DD");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("测试" + Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
}
