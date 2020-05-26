using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Common
{
    public enum ShakeType
    {
        Horizontal,
        Vertical,
        Random
    }

    public delegate void ShakeDelegate();
    public class ShakeCamera : Singleton<ShakeCamera>
    {
        #region internal struct
        struct ShakeData
        {
            public Vector3 positionShake;//震动幅度
            public float cycleTime;     //震动周期
            public int cycleCount;      //震动次数
            public bool fixShake;       //为真时每次幅度相同，反之则递减
            public bool bothDir;        //双向震动
            public float radius;        //随机半径，仅针对随机模式

            public ShakeData(Vector3 _positionShake, float _cycleTime, int _cycleCount, bool _fixShake, bool _bothDir, float _radius = 1)
            {
                positionShake = _positionShake;
                radius = _radius;
                cycleTime = _cycleTime;
                cycleCount = _cycleCount;
                fixShake = _fixShake;
                bothDir = _bothDir;
            }
        }
        #endregion

        public const ShakeType TestShakeType = ShakeType.Random;

        private ShakeData[] m_shakeDatas = new ShakeData[3]
        {
        new ShakeData(new Vector3(0.3f,0,0),0.03f,5,false,true),
        new ShakeData(new Vector3(0,0.3f,0),0.03f,5,false,true),
        new ShakeData(new Vector3(0,0,0),0.03f,5,false,true,0.2f)
        };

        public Vector3 positionShake;//震动幅度
        public Vector3 angleShake;   //震动角度
        public float cycleTime = 0.2f;//震动周期
        public int cycleCount = 6;    //震动次数
        public bool fixShake = false; //为真时每次幅度相同，反之则递减
        public bool unscaleTime = false;//不考虑缩放时间
        public bool bothDir = true;//双向震动

        float currentTime;
        int curCycle;
        Vector3 curPositonShake;
        Vector3 curAngleShake;
        float curFovShake;
        Vector3 startPosition;
        Vector3 startAngles;
        Transform myTransform;

        private ShakeDelegate m_callBack = null;

        private ShakeType m_shakeType;

        public void StartByShakeType(Transform transform, ShakeType shakeType, ShakeDelegate endCallback)
        {
            myTransform = transform;
            m_callBack = endCallback;
            m_shakeType = shakeType;

            var shakeTypeInt = (int)shakeType;
            if (shakeTypeInt < m_shakeDatas.Length)
            {
                var data = m_shakeDatas[shakeTypeInt];
                positionShake = data.positionShake;
                cycleTime = data.cycleTime;
                cycleCount = data.cycleCount;
                fixShake = data.fixShake;
                bothDir = data.bothDir;
            }
            //先设置默认参数，特殊需要再添加
            angleShake = Vector3.zero;
            unscaleTime = false;

            //开始执行
            Init();
        }

        private bool m_startShake = false;
        void Init()
        {
            currentTime = 0f;
            curCycle = 0;
            curPositonShake = positionShake;
            curAngleShake = angleShake;
            startPosition = myTransform.localPosition;
            startAngles = myTransform.localEulerAngles;

            SetRandomShake();

            m_startShake = true;
        }

        private void SetRandomShake()
        {
            if (m_shakeType == ShakeType.Random)
            {
                var randomAngle = Random.Range(-180, 180);
                var x = m_shakeDatas[(int)m_shakeType].radius * Mathf.Cos(randomAngle * Mathf.PI / 180);
                var y = m_shakeDatas[(int)m_shakeType].radius * Mathf.Sin(randomAngle * Mathf.PI / 180);
                positionShake = new Vector3(x, y, 0);
                curPositonShake = positionShake;
            }
        }

        public void Update()
        {
            if (!m_startShake)
            {
                return;
            }

            if (curCycle >= cycleCount)
            {
                if (m_callBack != null)
                {
                    m_startShake = false;
                    m_callBack();
                }
                return;
            }

            float deltaTime = unscaleTime ? Time.unscaledDeltaTime : Time.deltaTime;
            currentTime += deltaTime;
            while (currentTime >= cycleTime)
            {
                SetRandomShake();

                currentTime -= cycleTime;
                curCycle++;
                if (curCycle >= cycleCount)
                {
                    myTransform.localPosition = startPosition;
                    myTransform.localEulerAngles = startAngles;
                    return;
                }

                if (!fixShake)
                {
                    if (positionShake != Vector3.zero)
                        curPositonShake = (cycleCount - curCycle) * positionShake / cycleCount;
                    if (angleShake != Vector3.zero)
                        curAngleShake = (cycleCount - curCycle) * angleShake / cycleCount;
                }
            }

            if (curCycle < cycleCount)
            {
                float offsetScale = Mathf.Sin((bothDir ? 2 : 1) * Mathf.PI * currentTime / cycleTime);
                if (positionShake != Vector3.zero)
                    myTransform.localPosition = startPosition + curPositonShake * offsetScale;
                if (angleShake != Vector3.zero)
                    myTransform.localEulerAngles = startAngles + curAngleShake * offsetScale;
            }
        }
    }
}

