using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using VFramework.UI;
using VFramework.Common;
using Photon.Realtime;
using Photon.Pun.Demo.Asteroids;

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

    #endregion

}
