using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework.Character
{
    public class Character2DMove : CharacterMoveBase
    {
        #region 旋转函数
        /// <summary>
        /// 停止旋转
        /// </summary>
        public void StopRotate()
        {
            m_rotate = false;
        }

        /// <summary>
        /// 立刻旋转到指定方向
        /// </summary>
        /// <param name="dir"></param>
        public void InvokeRotate(Vector3 dir)
        {
            transform.rotation = GetQuaternion(dir.normalized);
        }

        /// <summary>
        /// 立刻旋转指定角度
        /// </summary>
        /// <param name="angle"></param>
        public void InvokeRotate(float angle)
        {
            transform.rotation = GetQuaternion(angle);
        }

        private bool m_rotate = false;
        private float m_rotateSpeed = 5;
        private Quaternion m_targetRotation;

        /// <summary>
        /// 平滑旋转到指定方向
        /// </summary>
        /// <param name="dir"></param>
        void SmoothRotate(Vector3 dir, float rotateSpeed)
        {
            //初始化
            m_rotate = true;
            m_rotateSpeed = rotateSpeed;
            m_targetRotation = GetQuaternion(dir.normalized);
        }

        /// <summary>
        /// 平滑旋转到指定角度
        /// </summary>
        /// <param name="dir"></param>
        void SmoothRotate(float angle, float rotateSpeed)
        {
            //初始化
            m_rotate = true;
            m_rotateSpeed = rotateSpeed;
            m_targetRotation = GetQuaternion(angle);
        }
        #endregion

        #region 移动函数

        private bool m_startRush = false;
        public bool IsRush
        {
            get
            {
                return m_startRush;
            }
        }
        private Coroutine m_rushCoroutine = null;

        private Action lastRushCallback = null;

        public void StopRush()
        {
            m_startRush = false;
            if (m_rushCoroutine != null)
            {
                StopCoroutine(m_rushCoroutine);
            }

            if (lastRushCallback != null)
            {
                lastRushCallback();
                lastRushCallback = null;
            }
        }

        public void RushTo(float rushDistance, Vector3 rushDir, float rushSpeed, int obstacleLayer, float checkRadius, Action rushCallback = null)
        {
            StopRush();

            m_startRush = true;
            lastRushCallback = rushCallback;
            m_rushCoroutine = StartCoroutine(RushIEnumerator(rushDistance, rushDir.normalized, rushSpeed, obstacleLayer, checkRadius, () => {
                lastRushCallback = null;
                rushCallback();
            }));
        }

        public void RushTo(float rushDistance, Vector3 rushDir, float rushSpeed, Action rushCallback = null)
        {
            StopRush();

            m_startRush = true;
            lastRushCallback = rushCallback;
            m_rushCoroutine = StartCoroutine(RushIEnumerator(rushDistance, rushDir.normalized, rushSpeed, -1, 0, () => {
                lastRushCallback = null;
                rushCallback();
            }));
        }

        IEnumerator RushIEnumerator(float rushDistance, Vector3 rushDir, float rushSpeed, int obstacleLayer, float checkRadius, Action rushCallback = null)
        {
            float rushAdd = 0;
            float moveAdd = 0;
            bool result = false;
            do
            {
                result = MoveTo(rushDir, rushSpeed, obstacleLayer, checkRadius, out moveAdd);
                rushAdd += moveAdd;
                yield return null;
            } while (result && rushAdd < rushDistance);
            //冲刺结束
            if (rushCallback != null)
            {
                rushCallback();
            }
            StopRush();
        }

        /// <summary>
        /// 移动一段距离
        /// </summary>
        /// <param name="moveDir">移动方向</param>
        /// <param name="moveSpeed">移动速度</param>
        /// <param name="obstacleLayer">障碍层级</param>
        /// <param name="checkRadius">检测半径</param>
        public void MoveFunc(Vector3 moveDir, float moveSpeed, int obstacleLayer, float checkRadius, bool smoothWall)
        {
            float moveAdd = 0;
            MoveTo(moveDir, moveSpeed, obstacleLayer, checkRadius, out moveAdd, smoothWall);
        }

        #endregion

        #region move and rotation help function

        /// <summary>
        /// 指定方向移动一帧距离
        /// </summary>
        /// <param name="moveDir">移动方向</param>
        /// <param name="moveSpeed">移动速度</param>
        /// <param name="obstacleLayer">障碍层级</param>
        /// <param name="checkRadius">障碍检测半径</param>
        /// <param name="moveAdd">移动距离</param>
        /// <returns></returns>
        bool MoveTo(Vector3 moveDir, float moveSpeed, int obstacleLayer, float checkRadius, out float moveAdd, bool smoothWall = false)
        {
            moveDir = moveDir.normalized;

            Vector3 vMove = moveDir * moveSpeed * Time.deltaTime;

            //存在浮点数误差
            float oldLocalPosX = transform.localPosition.x;
            float oldLocalPosY = transform.localPosition.y;

            Vector3 oldPos = transform.position;

            Collider2D collider2D = RaycastCircle(oldPos, checkRadius, moveDir, vMove.magnitude, obstacleLayer);

            if (collider2D != null)
            {
                if (!smoothWall || vMove.x == 0 || vMove.y == 0)
                {
                    moveAdd = 0;
                    return false;
                }
                else
                {
                    //尝试沿墙滑动,vMove方向x,y值必须有值，把vMove分解成两个方向,哪个方向能移动就朝哪个方向移动
                    Vector3 vTest = moveDir;
                    vTest.x = 0;
                    vMove = vTest * moveSpeed * Time.deltaTime;
                    transform.localPosition = new Vector3((oldLocalPosX + vMove.x), (oldLocalPosY + vMove.y), transform.localPosition.z);
                    collider2D = RaycastCircle(oldPos, checkRadius, vTest, vMove.magnitude, obstacleLayer);

                    if (collider2D != null)
                    {
                        vTest = moveDir;
                        vTest.y = 0;
                        vMove = vTest * moveSpeed * Time.deltaTime;
                        transform.localPosition = new Vector3((oldLocalPosX + vMove.x), (oldLocalPosY + vMove.y), transform.localPosition.z);
                        collider2D = RaycastCircle(oldPos, checkRadius, vTest, vMove.magnitude, obstacleLayer);

                        if (collider2D != null)
                        {
                            transform.localPosition = new Vector3((oldLocalPosX), (oldLocalPosY), transform.localPosition.z);
                            moveAdd = 0;
                            return false;
                        }
                    }
                }
            }

            moveAdd = vMove.magnitude;

            //存在浮点数误差
            transform.localPosition = new Vector3((oldLocalPosX + vMove.x), (oldLocalPosY + vMove.y), transform.localPosition.z);

            //transform.localPosition = oldLocalPos + vMove;

            return true;
        }

        /// <summary>
        /// 圆形检测
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="radius"></param>
        /// <param name="dir"></param>
        /// <param name="dis"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        Collider2D RaycastCircle(Vector3 origin, float radius, Vector3 dir, float dis, int layerMask)
        {
            Vector2 origin2D = new Vector2(origin.x, origin.y);
            Vector2 dir2D = new Vector2(dir.x, dir.y);

            RaycastHit2D[] hits = Physics2D.CircleCastAll(origin2D, radius, dir2D, dis, layerMask);
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject != this.gameObject && hit.collider.transform.parent != this.transform)
                {
                    return hit.collider;
                }
            }
            return null;
        }

        /// <summary>
        /// 检测单点，圆形检测
        /// </summary>
        /// <param name="point"></param>
        /// <param name="radius"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        Collider2D OverlapCircle(Vector2 point, float radius, int layerMask)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(point, radius, layerMask);
            foreach (var collider in colliders)
            {
                if (collider.gameObject != this.gameObject)
                {
                    return collider;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据方向获取旋转Quaternion
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private Quaternion GetQuaternion(Vector3 dir)
        {
            //LookRotation的含义就是让Z方向对齐第一个参数，y轴对齐第二参数对齐
            return Quaternion.LookRotation(Vector3.forward, -dir);
        }

        private Quaternion GetQuaternion(float angle)
        {
            return Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
        }

        #endregion
    }
}

