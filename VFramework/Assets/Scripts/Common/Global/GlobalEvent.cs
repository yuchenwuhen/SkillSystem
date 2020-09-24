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

    public const string ReadyUpdate = "ReadyUpdate";

    /// <summary>
    /// 属性更新
    /// </summary>
    public const string PlayerPropertiesUpdate = "PlayerPropertiesUpdate";

    #endregion

}
