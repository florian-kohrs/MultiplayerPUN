using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkGameManager : MonoBehaviourPunCallbacks
{

    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;

    public GameCycle gameCycle;

    #region Photon Callbacks


    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }


    //public override void OnPlayerEnteredRoom(Player other)
    //{
    //    Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


    //        LoadArena();
    //    }
    //}


    //public override void OnPlayerLeftRoom(Player other)
    //{
    //    Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects


    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


    //        LoadArena();
    //    }
    //}


    #endregion


    #region Public Methods

    private void Start()
    {
        InstantiatePlayer();
    }

    public void StartGame()
    {
        gameCycle.StartGame(10, 0);
    }

    public GameObject InstantiatePlayer()
    {
        if (GameManager.Player == null)
        {
            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                GameObject player;
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                if(PhotonNetwork.IsConnected)
                    player = PhotonNetwork.Instantiate("Player/" + playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity);
                else
                    player = Instantiate(playerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);

                GameManager.Player = player;
            }
        }
        return GameManager.Player;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }


    #endregion

  
    void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        }
        Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

}
