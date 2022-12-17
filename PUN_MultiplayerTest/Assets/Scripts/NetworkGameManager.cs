using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkGameManager : MonoBehaviourPunCallbacks
{

    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;

    public GameCycle gameCycle;

    public LevelSelection levelSelection;

    protected static NetworkGameManager instance;

    public static bool HasInstance =>  instance != null;

    private void Awake()
    {
        instance = this;

        if(GameCycle.instance != null)
            GameCycle.instance.Initialize();
    }

    protected List<Action<Photon.Realtime.Player>> OnPlayerConnected = new List<Action<Photon.Realtime.Player>>();
    protected List<Action<Photon.Realtime.Player>> OnPlayerDisconnected = new List<Action<Photon.Realtime.Player>>();

    public static void AddPlayerJoinedListener(Action<Photon.Realtime.Player> callback)
    {
        instance.OnPlayerConnected.Add(callback);
    }


    public static void AddPlayerDisconnectedListener(Action<Photon.Realtime.Player> callback)
    {
        instance.OnPlayerDisconnected.Add(callback);
    }


    #region Photon Callbacks


    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        OnPlayerDisconnected.ForEach(f => f(otherPlayer));
    }


    public override void OnPlayerEnteredRoom(Player other)
    {
        OnPlayerConnected.ForEach(f => f(other));
    }



    #endregion


    #region Public Methods

    private void Start()
    {
        InstantiatePlayer();
    }

    public void StartGame()
    {
        gameCycle.StartGame(10, UnityEngine.Random.Range(0,999999), levelSelection.SelectedMap);
        levelSelection.gameObject.SetActive(false);
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
