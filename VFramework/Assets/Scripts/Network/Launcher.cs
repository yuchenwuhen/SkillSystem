using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using VFramework.UI;

namespace VFramework.Net
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields

        /// <summary>
        /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
        /// </summary>
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte m_maxPlayersPerRoom = 4;

        #endregion


        #region Private Fields


        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "1";


        #endregion


        #region MonoBehaviour CallBacks


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            UIManager.Init();
        }


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            //Connect();
            UIManager.OpenUIWindow<LoginWindow>();
        }


        #endregion

    }
}