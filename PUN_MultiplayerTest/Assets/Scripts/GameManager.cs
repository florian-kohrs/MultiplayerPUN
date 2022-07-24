using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager
{

    private static GameManager instance;

    public const string PLAYER_TAG_NAME = "Player";


    private static GameManager GM
    {
        get
        {
            if (instance == null)
                instance = new GameManager();
            return instance;
        }
    }

    private bool isGameFrozen;

    private bool isMovementBlocked;

    private bool isPlayerMovementBlocked;

    private bool isActionBlocked;

    private bool isPlayerActionBlocked;

    private bool isCameraBlocked;

    private GameObject player;

    private Camera playerMainCamera;

    private InputHandler inputHandler;

    private InputHandler _InputHandler
    {
        get 
        {
            if (inputHandler == null)
                inputHandler = Object.FindObjectOfType<InputHandler>();

            return inputHandler;
        } 
    }

    public static InputHandler InputHandler => GM._InputHandler;

    public static bool GameIsNotFrozen => !GM.isGameFrozen;

    public static Camera PlayerMainCamera
    {
        get
        {
            if (instance.playerMainCamera == null)
            {
                instance.playerMainCamera = Camera.main;
            }
            return instance.playerMainCamera;
        }
        set
        {
            instance.playerMainCamera = value;
        }
    }

    public static GameObject Player
    {
        get
        {
            if (GM.player == null)
                GM.player = GameObject.FindGameObjectWithTag(PLAYER_TAG_NAME);
            return GM.player;
        }
        set
        {
            if (value.tag == PLAYER_TAG_NAME)
            {
                GM.player = value;
            }
            else
            {
                Debug.LogError("Tried to set a gameobject as player that has not the " + PLAYER_TAG_NAME + " tag.");
            }
        }
    }

    public static T GetPlayerComponent<T>() where T : Component
    {
        return Player?.GetComponent<T>();
    }

    public static T GetPlayerComponentInChildren<T>() where T : Component
    {
        return Player?.GetComponentInChildren<T>();
    }

    public static void FreezeCamera()
    {
        GM.isCameraBlocked = true;
    }

    public static void UnfreezeCamera()
    {
        GM.isCameraBlocked = false;
    }


    public static void FreezePlayer()
    {
        FreezeCamera();
        DisablePlayerMovement(); 
        DisablePlayerActions();
    }

    public static void UnfreezePlayer()
    {
        UnfreezeCamera();
        EnablePlayerMovement();
        EnablePlayerActions();
    }

    public static void DisableMovement()
    {
        GM.isMovementBlocked = true;
    }

    public static void EnableMovement()
    {
        GM.isMovementBlocked = false;
    }

    public static void DisablePlayerMovement()
    {
        GM.isPlayerMovementBlocked = true;
    }

    public static void EnablePlayerMovement()
    {
        GM.isPlayerMovementBlocked = false;
    }

    public static void DisableActions()
    {
        GM.isActionBlocked = true;
    }

    public static void EnableActions()
    {
        GM.isActionBlocked = false;
    }


    public static void DisablePlayerActions()
    {
        GM.isPlayerActionBlocked = true;
    }

    public static void EnablePlayerActions()
    {
        GM.isPlayerActionBlocked = false;
    }

    public static bool CanCameraMove => GameIsNotFrozen && !GM.isCameraBlocked;

    public static bool AllowActions => GameIsNotFrozen && !GM.isActionBlocked;

    public static bool AllowPlayerActions => GameIsNotFrozen && !GM.isPlayerActionBlocked;

    public static bool AllowMovement => GameIsNotFrozen && !GM.isMovementBlocked;

    public static bool AllowPlayerMovement => GameIsNotFrozen && !GM.isPlayerMovementBlocked;

}
