using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetMenue : MonoBehaviour
{

    private const string BASE_FOLDER_NAME = "ScriptableObjects/";


    [MenuItem("Assets/Create/Custom/MapOccupation")]
    public static void NewMapOccupation()
    {
        AssetCreator.CreateAsset<MapOccupationObject>(BASE_FOLDER_NAME + "MapOccupation");
    }

    [MenuItem("Assets/Create/Custom/Accessoire")]
    public static void NewAccessoire()
    {
        AssetCreator.CreateAsset<EquipableItemAsset>(BASE_FOLDER_NAME + "Accessoire");
    }



}
