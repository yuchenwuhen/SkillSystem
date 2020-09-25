using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using VFramework.UI;
using VFramework.Common;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    public void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log("网络初始化");
    }

    #region 网络连接状态

    /// <summary>
    /// 连接到master
    /// </summary>
    public override void OnConnectedToMaster()
    {
        if (UIManager.GetUI<LobbyWindow>() == null)
        {
            UIManager.OpenUIWindow<LobbyWindow>();
        }

        UIManager.CloseUIWindow<LoginWindow>();
    }

    /// <summary>
    /// 创建房间成功
    /// </summary>
    public override void OnJoinedRoom()
    {
        EventMgr.Instance.TriggerEvent(GlobalEvent.CreateRoomSuccess);

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                {GlobalDef.PLAYER_LOADED_LEVEL, false}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    /// <summary>
    /// 有玩家进入房间
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        EventMgr.Instance.TriggerEvent<Player,bool>(GlobalEvent.OnPlayerEnteredRoom, newPlayer,true);
    }

    /// <summary>
    /// 房间列表更新
    /// </summary>
    /// <param name="roomList"></param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        EventMgr.Instance.TriggerEvent<List<RoomInfo>>(GlobalEvent.OnRoomListUpdate, roomList);
    }

    /// <summary>
    /// 离开房间
    /// </summary>
    public override void OnLeftRoom()
    {
        EventMgr.Instance.TriggerEvent(GlobalEvent.OnLeftRoom);
        
    }

    /// <summary>
    /// 玩家离开房间
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        EventMgr.Instance.TriggerEvent<Player, bool>(GlobalEvent.OnPlayerEnteredRoom, otherPlayer, false);
    }

    /// <summary>
    /// 创建房间失败
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnCreateRoomFailed,returnCode" + returnCode + "message:" + message);

        EventMgr.Instance.TriggerEvent(GlobalEvent.CreateRoomFail);
    }

    /// <summary>
    /// 属性更新
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <param name="changedProps"></param>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {

        EventMgr.Instance.TriggerEvent<Player, ExitGames.Client.Photon.Hashtable>(GlobalEvent.PlayerPropertiesUpdate, targetPlayer, changedProps);
        
    }


    #endregion

}
