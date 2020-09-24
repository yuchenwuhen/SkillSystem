using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using VFramework.UI;
using VFramework.Common;
using Photon.Realtime;
using Photon.Pun.Demo.Asteroids;
using ExitGames.Client.Photon;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #region 网络连接状态

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
                {AsteroidsGame.PLAYER_LOADED_LEVEL, false}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
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
