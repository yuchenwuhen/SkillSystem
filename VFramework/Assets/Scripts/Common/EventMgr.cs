using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VFramework.Common
{
    public class EventMgr : MonoSingleton<EventMgr>
    {
        public enum EventPriority
        {
            Highest = 10,
            High = 100,
            AboveMedium = 150,
            Medium = 200,           // 默认优先级
            BelowMedium = 250,
            Low = 300,
            Lowest = 400,
        }

        public delegate void EventFunction(string ge);
        public delegate void EventFunction<U>(string ge, U arg1);
        public delegate void EventFunction<U, V>(string ge, U arg1, V arg2);
        public delegate void EventFunction<U, V, W>(string ge, U arg1, V arg2, W arg3);
        public delegate void EventFunction<U, V, W, M>(string ge, U arg1, V arg2, W arg3, M arg4);

        public class EventNode
        {
            public EventNode(Delegate function, bool removeOnLoadScene, EventPriority priority)
            {
                this.function = function;
                this.removeOnLoadScene = removeOnLoadScene;
                this.priority = priority;
            }

            public Delegate function;
            public bool removeOnLoadScene;
            public EventPriority priority;
        }

        public class EventNodeCompare : IComparer<EventNode>
        {
            public int Compare(EventNode x, EventNode y)
            {
                if (x == null)
                {
                    if (y == null)
                        return 0;
                    else
                        return -1;
                }
                else
                {
                    if (y == null)
                        return 1;
                    else
                        return ((int)x.priority) - ((int)y.priority);
                }
            }
        }

        public class MapNode
        {
            public bool dirty = false;
            public List<EventNode> nodeList = new List<EventNode>();
        }

        public class ObjEventNode
        {
            public ObjEventNode(string ge, Delegate function)
            {
                this.ge = ge;
                this.function = function;
            }

            public string ge;
            public Delegate function;
        }
        public class ObjEventMapNode
        {
            public List<ObjEventNode> nodeList = new List<ObjEventNode>();
        }

        private EventNodeCompare eventNodeCompare = new EventNodeCompare();
        private Dictionary<string, MapNode> eventMap;
        private Dictionary<object, ObjEventMapNode> objEventMap;

        public override void AwakeInit()
        {
            eventMap = new Dictionary<string, MapNode>();
            objEventMap = new Dictionary<object, ObjEventMapNode>();
        }

        // removeOnLoadScene决定此监听注册是否在加载场景时自动移除，大部分监听都应该选择自动移除，这是防止内存泄漏的保障机制
        public void AddListener(string ge, EventFunction ef, bool removeOnLoadScene = true, EventPriority priority = EventPriority.Medium)
        {
            AddListenerInternal(ge, ef, removeOnLoadScene, priority);
        }

        public void AddListener<U>(string gameEvent, EventFunction<U> ef, bool removeOnLoadScene = true, EventPriority priority = EventPriority.Medium)
        {
            AddListenerInternal(gameEvent, ef, removeOnLoadScene, priority);
        }

        public void AddListener<U, V>(string gameEvent, EventFunction<U, V> ef, bool removeOnLoadScene = true, EventPriority priority = EventPriority.Medium)
        {
            AddListenerInternal(gameEvent, ef, removeOnLoadScene, priority);
        }

        public void AddListener<U, V, W>(string gameEvent, EventFunction<U, V, W> ef, bool removeOnLoadScene = true, EventPriority priority = EventPriority.Medium)
        {
            AddListenerInternal(gameEvent, ef, removeOnLoadScene, priority);
        }

        public void AddListener<U, V, W, M>(string gameEvent, EventFunction<U, V, W, M> ef, bool removeOnLoadScene = true, EventPriority priority = EventPriority.Medium)
        {
            AddListenerInternal(gameEvent, ef, removeOnLoadScene, priority);
        }

        private void AddListenerInternal(string ge, Delegate ef, bool removeOnLoadScene, EventPriority priority)
        {
            // 如果event为空，那么返回
            if (ge == null || ge.Length == 0)
            {
                Debug.LogWarning("EventMgr" + "EventName Error");
                return;
            }

            // 如果第一次注册此类事件，需要创建对应链表
            MapNode mapNode;
            if (!eventMap.TryGetValue(ge, out mapNode))
            {
                mapNode = new MapNode();
                eventMap[ge] = mapNode;
            }

            // 如果重复注册则返回
            //foreach( EventNode node in nodeList )
            bool shouldSort = false;
            for (int i = 0; i < mapNode.nodeList.Count; ++i)
            {
                var node = mapNode.nodeList[i];

                if (node.priority != EventPriority.Medium)
                    shouldSort = true;

                if (node.function.Equals(ef))
                    return;
            }

            if (priority != EventPriority.Medium)
                shouldSort = true;

            mapNode.nodeList.Add(new EventNode(ef, removeOnLoadScene, priority));
            if (mapNode.nodeList.Count > 1 && shouldSort)
                mapNode.dirty = true;

            // yee
            // 如果第一次注册这个对象事件，需要创建对应链表
            if (ef.Target != null)
            {
                ObjEventMapNode objEventMapNode;
                if (!objEventMap.TryGetValue(ef.Target, out objEventMapNode))
                {
                    objEventMapNode = new ObjEventMapNode();
                    objEventMap[ef.Target] = objEventMapNode;
                }
                objEventMapNode.nodeList.Add(new ObjEventNode(ge, ef));
            }
        }

        public void RemoveListener(object obj)
        {
            // yee 新的销毁方案
            if (objEventMap != null && obj != null)
            {
                ObjEventMapNode objEventMapNode;
                if (!objEventMap.TryGetValue(obj, out objEventMapNode))
                {
                    return;
                }

                for (int i = 0; i < objEventMapNode.nodeList.Count; ++i)
                {
                    RemoveListenerInternal(objEventMapNode.nodeList[i].ge, objEventMapNode.nodeList[i].function);
                }

                objEventMap.Remove(obj);
            }

            /*if (eventMap != null && obj != null)
            {
                foreach( MapNode mapNode in eventMap.Values )
                {
                    for( int i=0; i<mapNode.nodeList.Count; ++i )
                    {
                        if( obj.Equals(mapNode.nodeList[i].function.Target) )
                        {	
                            mapNode.nodeList.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
            */
        }

        public void RemoveListener(string ge, EventFunction ef)
        {
            RemoveListenerInternal(ge, ef);
        }

        public void RemoveListener<U>(string ge, EventFunction<U> ef)
        {
            RemoveListenerInternal(ge, ef);
        }

        public void RemoveListener<U, V>(string ge, EventFunction<U, V> ef)
        {
            RemoveListenerInternal(ge, ef);
        }

        public void RemoveListener<U, V, W>(string ge, EventFunction<U, V, W> ef)
        {
            RemoveListenerInternal(ge, ef);
        }

        public void RemoveListener<U, V, W, M>(string ge, EventFunction<U, V, W, M> ef)
        {
            RemoveListenerInternal(ge, ef);
        }

        public void RemoveListenerInternal(string ge, Delegate ef)
        {
            MapNode mapNode;
            if (!eventMap.TryGetValue(ge, out mapNode))
                return;

            int nCount = 0;
            foreach (EventNode node in mapNode.nodeList)
            {
                if (node.function.Equals(ef))
                {
                    mapNode.nodeList.RemoveAt(nCount);
                    return;
                }

                nCount++;
            }
        }

        EventNode[] PrepareNodeList(string ge)
        {
            if (ge == null || ge.Length == 0)
            {
                //CoreEntry.logMgr.Log(LogLevel.WARNING, "EventMgr", "trigger null event");
                return null;
            }

            MapNode mapNode;
            if (!eventMap.TryGetValue(ge, out mapNode))
                return null;

            if (mapNode.dirty)
            {
                mapNode.nodeList.Sort(eventNodeCompare);
                mapNode.dirty = false;
            }

            // 将事件响应队列复制一个副本
            EventNode[] tpList = new EventNode[mapNode.nodeList.Count];
            mapNode.nodeList.CopyTo(tpList);
            return tpList;
        }

        public void TriggerEvent(string ge)
        {
            EventNode[] tpList = PrepareNodeList(ge);
            if (tpList == null)
                return;

            foreach (EventNode node in tpList)
            {
                EventFunction ef = node.function as EventFunction;
                if (ef != null)
                    ef(ge);
                else
                    Debug.LogError(string.Format("处理事件 \"{0}\" 时回调函数参数不匹配", ge));
            }
        }

        public void TriggerEvent<U>(string ge, U arg1)
        {
            EventNode[] tpList = PrepareNodeList(ge);
            if (tpList == null)
                return;

            foreach (EventNode node in tpList)
            {
                EventFunction<U> ef = node.function as EventFunction<U>;
                if (ef != null)
                    ef(ge, arg1);
                else
                    Debug.LogError(string.Format("处理事件 \"{0}\" 时回调函数参数不匹配", ge));
            }
        }

        public void TriggerEvent<U, V>(string ge, U arg1, V arg2)
        {
            EventNode[] tpList = PrepareNodeList(ge);
            if (tpList == null)
                return;

            foreach (EventNode node in tpList)
            {
                EventFunction<U, V> ef = node.function as EventFunction<U, V>;
                if (ef != null)
                    ef(ge, arg1, arg2);
                else
                    Debug.LogError(string.Format("处理事件 \"{0}\" 时回调函数参数不匹配", ge));
            }
        }

        public void TriggerEvent<U, V, W>(string ge, U arg1, V arg2, W arg3)
        {
            EventNode[] tpList = PrepareNodeList(ge);
            if (tpList == null)
                return;

            foreach (EventNode node in tpList)
            {
                EventFunction<U, V, W> ef = node.function as EventFunction<U, V, W>;
                if (ef != null)
                    ef(ge, arg1, arg2, arg3);
                else
                    Debug.LogError(string.Format("处理事件 \"{0}\" 时回调函数参数不匹配", ge));
            }
        }

        public void TriggerEvent<U, V, W, M>(string ge, U arg1, V arg2, W arg3, M arg4)
        {
            EventNode[] tpList = PrepareNodeList(ge);
            if (tpList == null)
                return;

            foreach (EventNode node in tpList)
            {
                EventFunction<U, V, W, M> ef = node.function as EventFunction<U, V, W, M>;
                if (ef != null)
                    ef(ge, arg1, arg2, arg3, arg4);
                else
                    Debug.LogError(string.Format("处理事件 \"{0}\" 时回调函数参数不匹配", ge));
            }
        }

        // 切换场景时移除removeOnLoadScene标志为true的监听对象
        public void OnLoadSceneRecycle()
        {
            foreach (MapNode mapNode in eventMap.Values)
            {
                for (int i = 0; i < mapNode.nodeList.Count;)
                {
                    if (mapNode.nodeList[i].removeOnLoadScene)
                        mapNode.nodeList.RemoveAt(i);
                    else
                        ++i;
                }
            }
        }
    }
}


