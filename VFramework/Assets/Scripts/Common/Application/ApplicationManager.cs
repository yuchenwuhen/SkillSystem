
namespace VFramework.Common
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using System.Threading;

    public class ApplicationManager : MonoSingleton<ApplicationManager>
    {

        public override void AwakeInit()
        {
            Timer.Init();
        }

        private void Update()
        {
            if (onApplicationUpdateCallback != null)
            {
                onApplicationUpdateCallback();
            }
        }

        #region 程序生命周期事件派发

        public static ApplicationVoidCallback onApplicationUpdateCallback = null;

        #endregion
    }

    public delegate void ApplicationBoolCallback(bool status);
    public delegate void ApplicationVoidCallback();

}
