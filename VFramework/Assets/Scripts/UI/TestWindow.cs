using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VFramework.Common;
using VFramework.UI;

public class TestWindow : UIWindowBase
{

    public override void OnOpen()
    {
        AddOnClickListener("Button", OnClickReturnMainMenu);
        AddBeginDragListener("Drag", OnBeginDrag);
        AddOnDragListener("Drag", OnDrag);
        AddEndDragListener("Drag", OnEndDrag);

        AddLongPressListener("LongPress", OnLongPress);

        EventMgr.Instance.AddListener("UIEvent", OnUIEvent);

        //GetJoyStick("JoyStick").onJoyStick += Joy;

        GetReusingScrollRect("ScrollRect").SetItem("Image_item");
        GetReusingScrollRect("ScrollRect").Init(UIEventKey, 0);

        List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();

        for (int i = 0; i < 100; i++)
        {
            data.Add(new Dictionary<string, object>());
        }

        GetReusingScrollRect("ScrollRect").SetData(data);
    }

    public override void OnInit()
    {
        base.OnInit();
        Graphic graphic = gameObject.GetComponent<Graphic>();
        if (graphic != null)
        {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 0);
        }
    }

    private void Joy(Vector3 dir)
    {
        Debug.Log("方向" + dir);
    }

    public override void OnRefresh()
    {
        base.OnRefresh();
    }

    void OnUIEvent(string str)
    {
        Debug.Log("Test UI Event");
    }

    public override IEnumerator EnterAnim(UIAnimCallBack animComplete, UICallBack callBack, params object[] objs)
    {
        //yield return new WaitForSeconds(0.2f);

        AnimSystem.UguiAlpha(gameObject, 0, 1, callBack: (object[] obj) =>
        {
            StartCoroutine(base.EnterAnim(animComplete, callBack, objs));
        });

        AnimSystem.UguiMove(GetGameObject("Button"), new Vector3(0, 0, 0), new Vector3(0, 70, 0));

        yield return new WaitForEndOfFrame();
    }

    public override IEnumerator ExitAnim(UIAnimCallBack animComplete, UICallBack callBack, params object[] objs)
    {
        AnimSystem.UguiAlpha(gameObject, null, 0,1, callBack: (object[] obj) =>
        {
            StartCoroutine(base.ExitAnim(animComplete, callBack, objs));
        });

        AnimSystem.UguiMove(GetGameObject("Button"), new Vector3(0, 70, 0), new Vector3(0, 0, 0));

        yield return new WaitForEndOfFrame();
    }

    void OnClickReturnMainMenu(InputUIOnClickEvent e)
    {
        Debug.Log("回到主场景....");
        EventMgr.Instance.TriggerEvent("UIEvent");
    }

    void OnBeginDrag(InputUIOnBeginDragEvent e)
    {
        Debug.Log("开始拖拽...." + e.m_dragPosition);
    }

    void OnDrag(InputUIOnDragEvent e)
    {
        Debug.Log("拖拽中...." + e.m_dragPosition);
    }

    void OnEndDrag(InputUIOnEndDragEvent e)
    {
        Debug.Log("结束拖拽...." + e.m_dragPosition);
    }

    void OnLongPress(InputUILongPressEvent e)
    {
        Debug.Log("长按...." + e.m_type+":" +e.m_t);
    }
}
