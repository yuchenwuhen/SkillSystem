using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework.Common;
using VFramework.UI;

public class GameManager : MonoBehaviour
{
    private CameraController m_cameraController;

    private void Awake()
    {

        float x = Random.Range(0, 10);
        float y = Random.Range(0, 10);

        GameObject player;
        if (PhotonNetwork.IsConnected)
        {
            player = PhotonNetwork.Instantiate("Player", new Vector3(x, y, 0), Quaternion.identity, 0);
            //m_followCameraController.targetTransform = player.transform;

            if (player.GetComponent<PhotonView>() != null)
            {
                if (player.GetComponent<PhotonView>().IsMine)
                {
                    m_cameraController = Camera.main.GetComponent<CameraController>();
                    m_cameraController.target = player.transform;
                }
            }
        }
        else
        {
            //测试
#if UNITY_EDITOR
            player = GameObjectPool.Instance.CreateObject("Player");

            m_cameraController = Camera.main.GetComponent<CameraController>();
            m_cameraController.target = player.transform;
#endif
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            if (UIManager.GetUI<LobbyWindow>() != null)
            {
                UIManager.CloseUIWindow<LobbyWindow>();
            }
            //StartCoroutine(SpawnAsteroid());
        }
    }

    // Start is called before the first frame update
    void Start()
    {


    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
