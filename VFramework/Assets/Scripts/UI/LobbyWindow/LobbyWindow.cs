using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VFramework.UI;
using VFramework.Common;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class LobbyWindow : UIWindowBase
{
    #region SerializeField

    [SerializeField]
    private Text m_netStatus;

    [SerializeField]
    private GameObject m_selectionPanel;

    [SerializeField]
    private GameObject m_createRoomPanel;

    [SerializeField]
    private GameObject m_joinRandomRoomPanel;

    [SerializeField]
    private GameObject m_roomListPanel;
    [SerializeField]
    private GameObject m_roomListContent;

    [SerializeField]
    private GameObject m_insideRoomPanel;

    [SerializeField]
    private GameObject m_startGameButton;

    [SerializeField]
    private InputField m_roomNameInputField;

    [SerializeField]
    private InputField m_maxPlayersInputField;

    #endregion

    #region private variable

    private Dictionary<string, RoomInfo> m_cachedRoomList;
    private Dictionary<int, GameObject> m_playerListEntries;
    private Dictionary<string, GameObject> m_roomListEntries;

    #endregion

    #region Monobehavior Callback

    private void Awake()
    {
        m_cachedRoomList = new Dictionary<string, RoomInfo>();
        m_roomListEntries = new Dictionary<string, GameObject>();
    }

    public override void OnOpen(params object[] args)
    {
        base.OnOpen(args);

        SetActivePanel(m_selectionPanel.name);

        AddOnClickListener("CreateRoomButton", CreateRoomButton);
        AddOnClickListener("SubCreateRoomButton", OnCreateRoomButtonClicked);
        AddOnClickListener("CancelButton", OnBackButtonClicked);
        AddOnClickListener("JoinRandomRoomButton", OnJoinRandomRoomButtonClicked);
        AddOnClickListener("LeaveGameButton", OnLeaveGameButtonClicked);
        AddOnClickListener("RoomListButton", OnRoomListButtonClicked);
        AddOnClickListener("BackButton", OnBackLobbyButtonClicked);
        AddOnClickListener("StartGameButton", OnStartGameButtonClicked);

        EventMgr.Instance.AddListener(GlobalEvent.CreateRoomFail,BackToSelectPanel);
        EventMgr.Instance.AddListener(GlobalEvent.OnLeftRoom, OnLeftRoom);
        EventMgr.Instance.AddListener<List<RoomInfo>>(GlobalEvent.OnRoomListUpdate, OnRandomListUpdate);
        EventMgr.Instance.AddListener(GlobalEvent.CreateRoomSuccess, Net_OnCreateRoomSuccess);
        EventMgr.Instance.AddListener(GlobalEvent.ReadyUpdate, LocalPlayerPropertiesUpdated);
        EventMgr.Instance.AddListener<Player,bool>(GlobalEvent.OnPlayerEnteredRoom, Net_OnPlayerEnteredRoom);
        EventMgr.Instance.AddListener<Player, Hashtable>(GlobalEvent.PlayerPropertiesUpdate,PlayerPropertiesUpdate);
    }

    private void Update()
    {
        m_netStatus.text = string.Format("    Connection Status: {0}" ,PhotonNetwork.NetworkClientState);
    }

    #endregion

    #region Panel change

    private void SetActivePanel(string activePanel)
    {
        m_selectionPanel.SetActive(activePanel.Equals(m_selectionPanel.name));
        m_createRoomPanel.SetActive(activePanel.Equals(m_createRoomPanel.name));
        m_joinRandomRoomPanel.SetActive(activePanel.Equals(m_joinRandomRoomPanel.name));
        m_roomListPanel.SetActive(activePanel.Equals(m_roomListPanel.name));
        m_insideRoomPanel.SetActive(activePanel.Equals(m_insideRoomPanel.name));
    }

    #region join lobby

    public void OnBackLobbyButtonClicked(InputUIOnClickEvent e)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        SetActivePanel(m_selectionPanel.name);
    }

    public void OnRoomListButtonClicked(InputUIOnClickEvent e)
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        SetActivePanel(m_roomListPanel.name);
    }

    #endregion

    #region create Room

    void CreateRoomButton(InputUIOnClickEvent e)
    {
        SetActivePanel(m_createRoomPanel.name);
    }

    /// <summary>
    /// 取消创建房间
    /// </summary>
    /// <param name="e"></param>
    void OnBackButtonClicked(InputUIOnClickEvent e)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        BackToSelectPanel();
    }

    /// <summary>
    /// 退回选择界面
    /// </summary>
    void BackToSelectPanel()
    {
        SetActivePanel(m_selectionPanel.name);
    }

    /// <summary>
    /// 创建房间
    /// </summary>
    public void OnCreateRoomButtonClicked(InputUIOnClickEvent e)
    {
        string roomName = m_roomNameInputField.text;
        roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

        byte maxPlayers;
        byte.TryParse(m_maxPlayersInputField.text, out maxPlayers);
        maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 8);

        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 10000 };

        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    /// <summary>
    /// 有玩家进入房间
    /// </summary>
    public void Net_OnPlayerEnteredRoom(string e, Player newPlayer,bool enterRoom)
    {
        if (enterRoom)
        {
            GameObject entry = GameObjectPool.Instance.CreateObject("UI/PlayerListEntryItem");
            entry.transform.SetParent(m_insideRoomPanel.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<PlayerListEntryItem>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

            m_playerListEntries.Add(newPlayer.ActorNumber, entry);     
        }
        else
        {
            GameObjectPool.Instance.CollectObject(m_playerListEntries[newPlayer.ActorNumber].gameObject);
            m_playerListEntries.Remove(newPlayer.ActorNumber);
        }

        m_startGameButton.gameObject.SetActive(CheckPlayersReady());

    }

    /// <summary>
    /// 创建房间成功
    /// </summary>
    public void Net_OnCreateRoomSuccess()
    {
        SetActivePanel(m_insideRoomPanel.name);

        if (m_playerListEntries == null)
        {
            m_playerListEntries = new Dictionary<int, GameObject>();
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject entry = GameObjectPool.Instance.CreateObject("UI/PlayerListEntryItem");
            entry.transform.SetParent(m_insideRoomPanel.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<PlayerListEntryItem>().Initialize(p.ActorNumber, p.NickName);

            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(GlobalDef.PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<PlayerListEntryItem>().SetPlayerReady((bool)isPlayerReady);
            }

            m_playerListEntries.Add(p.ActorNumber, entry);
        }

        m_startGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    private bool CheckPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(GlobalDef.PLAYER_READY, out isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public void LocalPlayerPropertiesUpdated()
    {
        m_startGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    #endregion

    #region join room

    public void OnJoinRandomRoomButtonClicked(InputUIOnClickEvent e)
    {
        SetActivePanel(m_joinRandomRoomPanel.name);

        PhotonNetwork.JoinRandomRoom();
    }

    #endregion

    #region roomlist update

    public void OnRandomListUpdate(string e,List<RoomInfo> roomList)
    {
        ClearRoomListView();

        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    private void UpdateRoomListView()
    {
        foreach (RoomInfo info in m_cachedRoomList.Values)
        {
            GameObject entry = GameObjectPool.Instance.CreateObject("UI/RoomListEntryItem");
            entry.transform.SetParent(m_roomListContent.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<RoomListEntryItem>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

            m_roomListEntries.Add(info.Name, entry);
        }
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (m_cachedRoomList.ContainsKey(info.Name))
                {
                    m_cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            // Update cached room info
            if (m_cachedRoomList.ContainsKey(info.Name))
            {
                m_cachedRoomList[info.Name] = info;
            }
            // Add new room info to cache
            else
            {
                m_cachedRoomList.Add(info.Name, info);
            }
        }
    }

    private void ClearRoomListView()
    {
        foreach (GameObject entry in m_roomListEntries.Values)
        {
            GameObjectPool.Instance.CollectObject(entry.gameObject);
        }

        m_roomListEntries.Clear();
    }

    #endregion

    #region leave room

    public void OnLeftRoom()
    {
        SetActivePanel(m_selectionPanel.name);

        foreach (GameObject entry in m_playerListEntries.Values)
        {
            GameObjectPool.Instance.CollectObject(entry.gameObject);
        }

        m_playerListEntries.Clear();
        m_playerListEntries = null;
    }

    public void OnLeaveGameButtonClicked(InputUIOnClickEvent e)
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region start game

    public void OnStartGameButtonClicked(InputUIOnClickEvent e)
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        UIManager.CloseUIWindow<LobbyWindow>();

        PhotonNetwork.LoadLevel("3D");
    }

    #endregion

    #region 属性更新

    void PlayerPropertiesUpdate(string e,Player targetPlayer, Hashtable changedProps)
    {
        if (m_playerListEntries == null)
        {
            m_playerListEntries = new Dictionary<int, GameObject>();
        }

        GameObject entry;
        if (m_playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(GlobalDef.PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<PlayerListEntryItem>().SetPlayerReady((bool)isPlayerReady);
            }
        }

        m_startGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    #endregion

    #endregion
}
