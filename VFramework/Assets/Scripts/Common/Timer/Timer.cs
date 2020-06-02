
namespace VFramework.Common
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 时间管理类
    /// 使用：Timer.DelayCallBack(5f, (object[] objs) => { Debug.Log("5秒调用"); });
    /// </summary>
    public class Timer
    {
        private static List<TimerEvent> m_timeList = new List<TimerEvent>();

        public static void Init()
        {
            ApplicationManager.onApplicationUpdateCallback += Update;
        }

        static void Update()
        {
            for (int i = 0; i < m_timeList.Count; i++)
            {
                TimerEvent timer = m_timeList[i];
                timer.Update();

                if (timer.isDone)
                {
                    timer.CompeleteTimer();
                    //计算次数之后，isDone为true说明事件完全完成
                    if (timer.isDone)
                    {
                        m_timeList.Remove(timer);
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// 延迟调用
        /// </summary>
        /// <param name="delayTime"></param>
        /// <param name="callBack"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static TimerEvent DelayCallBack(float delayTime, TimerCallBack callBack, params object[] objs)
        {
            return AddTimer(delayTime, false, 0, null, callBack, objs);
        }

        /// <summary>
        /// 延迟调用
        /// </summary>
        /// <param name="delayTime">间隔时间</param>
        /// <param name="isIgnoreTimeScale">是否忽略时间缩放</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="objs">回调函数的参数</param>
        /// <returns></returns>
        public static TimerEvent DelayCallBack(float delayTime, bool isIgnoreTimeScale, TimerCallBack callBack, params object[] objs)
        {
            return AddTimer(delayTime, isIgnoreTimeScale, 0, null, callBack, objs);
        }

        /// <summary>
        /// 间隔一定时间重复调用
        /// </summary>
        /// <param name="spaceTime">间隔时间</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="objs">回调函数的参数</param>
        /// <returns></returns>
        public static TimerEvent CallBackOfIntervalTimer(float spaceTime, TimerCallBack callBack, params object[] objs)
        {
            return AddTimer(spaceTime, false, -1, null, callBack, objs);
        }

        /// <summary>
        /// 间隔一定时间重复调用
        /// </summary>
        /// <param name="spaceTime">间隔时间</param>
        /// <param name="isIgnoreTimeScale">是否忽略时间缩放</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="objs">回调函数的参数</param>
        /// <returns></returns>
        public static TimerEvent CallBackOfIntervalTimer(float spaceTime, bool isIgnoreTimeScale, TimerCallBack callBack, params object[] objs)
        {
            return AddTimer(spaceTime, isIgnoreTimeScale, -1, null, callBack, objs);
        }

        /// <summary>
        /// 间隔一定时间重复调用
        /// </summary>
        /// <param name="spaceTime">间隔时间</param>
        /// <param name="isIgnoreTimeScale">是否忽略时间缩放</param>
        /// <param name="timerName">Timer的名字</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="objs">回调函数的参数</param>
        /// <returns></returns>
        public static TimerEvent CallBackOfIntervalTimer(float spaceTime, bool isIgnoreTimeScale, string timerName, TimerCallBack callBack, params object[] objs)
        {
            return AddTimer(spaceTime, isIgnoreTimeScale, -1, timerName, callBack, objs);
        }

        /// <summary>
        /// 有限次数的重复调用
        /// </summary>
        /// <param name="spaceTime">间隔时间</param>
        /// <param name="callBackCount">重复调用的次数</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="objs">回调函数的参数</param>
        /// <returns></returns>
        public static TimerEvent CallBackOfIntervalTimer(float spaceTime, int callBackCount, TimerCallBack callBack, params object[] objs)
        {
            return AddTimer(spaceTime, false, callBackCount, null, callBack, objs);
        }

        /// <summary>
        /// 有限次数的重复调用
        /// </summary>
        /// <param name="spaceTime">间隔时间</param>
        /// <param name="isIgnoreTimeScale">是否忽略时间缩放</param>
        /// <param name="callBackCount">重复调用的次数</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="objs">回调函数的参数</param>
        /// <returns></returns>
        public static TimerEvent CallBackOfIntervalTimer(float spaceTime, bool isIgnoreTimeScale, int callBackCount, TimerCallBack callBack, params object[] objs)
        {
            return AddTimer(spaceTime, isIgnoreTimeScale, callBackCount, null, callBack, objs); ;
        }

        /// <summary>
        /// 有限次数的重复调用
        /// </summary>
        /// <param name="spaceTime">间隔时间</param>
        /// <param name="isIgnoreTimeScale">是否忽略时间缩放</param>
        /// <param name="callBackCount">重复调用的次数</param>
        /// <param name="timerName">Timer的名字</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="objs">回调函数的参数</param>
        /// <returns></returns>
        public static TimerEvent CallBackOfIntervalTimer(float spaceTime, bool isIgnoreTimeScale, int callBackCount, string timerName, TimerCallBack callBack, params object[] objs)
        {
            return AddTimer(spaceTime, isIgnoreTimeScale, callBackCount, timerName, callBack, objs);
        }

        /// <summary>
        /// 添加TimerEvent
        /// </summary>
        /// <param name="intervalTime">事件触发间隔时间</param>
        /// <param name="isIgnoreTimeScale">忽略TimeScale</param>
        /// <param name="callBackCount">重复次数 -1无限重复</param>
        /// <param name="timerName">名字</param>
        /// <param name="callBack">间隔时间回调</param>
        /// <param name="objs">回调参数</param>
        /// <returns></returns>
        public static TimerEvent AddTimer(float intervalTime, bool isIgnoreTimeScale, int callBackCount, string timerName, TimerCallBack callBack, params object[] objs)
        {
            TimerEvent timer = new TimerEvent();

            timer.timerInterval = intervalTime;
            timer.isIgnoreTimeScale = isIgnoreTimeScale;
            timer.repeatCount = callBackCount;
            timer.timerCallback = callBack;
            timer.objs = objs;

            timer.ResetTimer();

            m_timeList.Add(timer);

            return timer;
        }

        /// <summary>
        /// 移除TimerEvent
        /// </summary>
        /// <param name="timer">timerEevent</param>
        /// <param name="isCallBack">是否触发回调</param>
        public static void DestroyTimer(TimerEvent timer, bool isCallBack = false)
        {
            //Debug.Log("DestroyTimer " + timer.m_timerName + " isTest " + (timer == test));

            if (m_timeList.Contains(timer))
            {
                if (isCallBack)
                {
                    timer.CallBackTimer();
                }

                m_timeList.Remove(timer);
            }
            else
            {
                Debug.LogError("Timer DestroyTimer error: dont exist timer " + timer);
            }
        }

        /// <summary>
        /// 移除TimerEvent
        /// </summary>
        /// <param name="timerName">名字</param>
        /// <param name="isCallBack">是否触发回调</param>
        public static void DestroyTimer(string timerName, bool isCallBack = false)
        {
            //Debug.Log("DestroyTimer2  ----TIMER " + timerName);
            for (int i = 0; i < m_timeList.Count; i++)
            {
                TimerEvent te = m_timeList[i];
                if (te.timerName.Equals(timerName))
                {
                    DestroyTimer(te, isCallBack);
                }
            }
        }

        /// <summary>
        /// 移除所有TimerEvent
        /// </summary>
        /// <param name="isCallBack"></param>
        public static void DestroyAllTimer(bool isCallBack = false)
        {
            for (int i = 0; i < m_timeList.Count; i++)
            {
                if (isCallBack)
                {
                    m_timeList[i].CallBackTimer();
                }
            }

            m_timeList.Clear();
        }

        public static void ResetTimer(TimerEvent timer)
        {
            if (m_timeList.Contains(timer))
            {
                timer.ResetTimer();
            }
            else
            {
                Debug.LogError("Timer ResetTimer error: dont exist timer " + timer);
            }
        }

        public static void ResetTimer(string timerName)
        {
            for (int i = 0; i < m_timeList.Count; i++)
            {
                var e = m_timeList[i];

                if (e.timerName.Equals(timerName))
                {
                    ResetTimer(e);
                }
            }
        }

    }
}
