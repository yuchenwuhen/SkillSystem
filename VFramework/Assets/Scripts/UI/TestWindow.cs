using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VFramework.Common;
using VFramework.UI;

public class TestWindow : UIWindowBase
{

    public Button button;
    public override void OnOpen()
    {
        AddOnClickListener("Button", OnClickReturnMainMenu);
        //GetGameObject("Button").GetComponent<Button>().onClick.AddListener(OnClickReturnMainMenu);
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
    }
}
