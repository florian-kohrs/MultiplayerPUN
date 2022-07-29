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

    [MenuItem("Assets/Create/Custom/CubeMapOccupation")]
    public static void NewMapCubeOccupation()
    {
        AssetCreator.CreateAsset<MapCubeOccupationObject>(BASE_FOLDER_NAME + "MapOccupation/Cubes");
    }

    [MenuItem("Assets/Create/Custom/Accessoire")]
    public static void NewAccessoire()
    {
        AssetCreator.CreateAsset<EquipableItemAsset>(BASE_FOLDER_NAME + "Accessoire");
    }

    [MenuItem("Assets/Create/Custom/Map")]
    public static void NewMap()
    {
        AssetCreator.CreateAsset<MapDesign>(BASE_FOLDER_NAME + "Map");
    }

    [MenuItem("Assets/Create/Custom/ItemList")]
    public static void NewItemList()
    {
        AssetCreator.CreateAsset<MapOccupationList>(BASE_FOLDER_NAME + "ItemList");
    }



}
