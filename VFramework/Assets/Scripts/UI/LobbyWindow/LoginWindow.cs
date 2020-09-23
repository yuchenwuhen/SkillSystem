using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VFramework.Common;
using VFramework.UI;

public class LoginWindow : UIWindowBase
{
    #region private constants

    const string playerNamePrefKey = "PlayerName";

    #endregion

    #region Private Serializable Fields

    [SerializeField]
    private InputField m_nameField;

    #endregion

    #region override

    public override void OnInit()
    {
        m_nameField.onEndEdit.AddListener(SetPlayerName);
    }

    public override void OnOpen(params object[] args)
    {
        base.OnOpen(args);

        InitName();

        AddOnClickListener("LoginBtn", OnLoginButtonClicked);
    }

    void InitName()
    {
        string defaultName = string.Empty;
        if (m_nameField != null && PlayerPrefs.HasKey(playerNamePrefKey))
        {
            defaultName = PlayerPrefs.GetString(playerNamePrefKey);
            m_nameField.text = defaultName;
        }

        PhotonNetwork.NickName = defaultName;
    }

    public override void OnUIDestroy()
    {
        m_nameField.onEndEdit.RemoveAllListeners();
    }

    #endregion

    #region public methods

    public void OnLoginButtonClicked(InputUIOnClickEvent e)
    {
        string playerName = m_nameField.text;

        if (!playerName.Equals(""))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.LogError("Player Name is invalid.");
        }
    }


    public void SetPlayerName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Player Name is null or empty");
            return;
        }
        PhotonNetwork.NickName = value;


        PlayerPrefs.SetString(playerNamePrefKey, value);
    }

    #endregion

}
