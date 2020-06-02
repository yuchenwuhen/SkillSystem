
namespace VFramework.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 时间类
    /// </summary>
    public class TimerEvent
    {
        public string timerName = "";

        /// <summary>
        /// 重复调用次数，-1表示一直调用
        /// </summary>
        public int repeatCount = 0;
        public int currentRepeat = 0;

        /// <summary>
        /// 是否忽略TimeScale
        /// </summary>
        public bool isIgnoreTimeScale = false;
        /// <summary>
        /// 到时间回调
        /// </summary>
        public TimerCallBack timerCallback;
        /// <summary>
        /// 回调参数
        /// </summary>
        public object[] objs;

        public bool isDone = false;

        public bool isStop = false;

        /// <summary>
        /// 时间间隔
        /// </summary>
        public float timerInterval = 0;

        private float m_currentTimer = 0;

        public void Update()
        {
            if (isIgnoreTimeScale)
            {
                m_currentTimer += Time.unscaledDeltaTime;
            }
            else
            {
                m_currentTimer += Time.deltaTime;
            }

            if (m_currentTimer >= timerInterval)
            {
                isDone = true;
            }
        }

        /// <summary>
        /// 完成一轮计时，计时次数+1
        /// </summary>
        public void CompeleteTimer()
        {
            CallBackTimer();

            if (repeatCount > 0)
            {
                currentRepeat++;
            }

            if (currentRepeat != repeatCount)
            {
                isDone = false;
                m_currentTimer = 0;
            }
        }

        public void CallBackTimer()
        {
            if (timerCallback != null)
            {
                try
                {
                    timerCallback(objs);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }

        public void ResetTimer()
        {
            currentRepeat = 0;
            m_currentTimer = 0;
        }


    }

    public delegate void TimerCallBack(params object[] l_objs);
}
