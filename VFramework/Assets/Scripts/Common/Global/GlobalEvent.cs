using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEvent 
{

    #region NetMessage

    /// <summary>
    /// 创建房间失败
    /// </summary>
    public const string CreateRoomFail = "CreateRoomFail";

    /// <summary>
    /// 创建房间成功
    /// </summary>
    public const string CreateRoomSuccess = "CreateRoomSuccess";

    /// <summary>
    /// 玩家进入房间
    /// </summary>
    public const string OnPlayerEnteredRoom = "OnPlayerEnteredRoom";

    /// <summary>
    /// 离开房间
    /// </summary>
    public const string OnLeftRoom = "OnLeftRoom";

    /// <summary>
    /// 房间列表更新
    /// </summary>
    public const string OnRoomListUpdate = "OnRoomListUpdate";

    public const string ReadyUpdate = "ReadyUpdate";

    /// <summary>
    /// 属性更新
    /// </summary>
    public const string PlayerPropertiesUpdate = "PlayerPropertiesUpdate";

    #endregion

}
