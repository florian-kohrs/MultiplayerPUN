using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDesign : ScriptableObject, ISerializationCallbackReceiver
{

    public MapOccupation[,] unserializeableMap;

    public MapOccupation[,] UnserializeableMap
    {
        get
        {
            if (unserializeableMap == null)
                unserializeableMap = new MapOccupation[dimensions.x, dimensions.y];
            return unserializeableMap;
        }
    }

    public Vector2Int dimensions;

    private void OnValidate()
    {
        Vector2Int length = new Vector2Int(UnserializeableMap.GetLength(0), UnserializeableMap.GetLength(1));
        if (length != dimensions)
        {
            MapOccupation[,] newMap = new MapOccupation[dimensions.x, dimensions.y];
            for (int x = 0; x < length.x && x < dimensions.x; x++)
            {
                for (int y = 0; y < length.y && y< dimensions.y; y++)
                {
                    newMap[x,y] = UnserializeableMap[x,y];  
                }
            }
            unserializeableMap = newMap;
            OnBeforeSerialize();
        }
    }


    // A list that can be serialized
    [SerializeField] private List<Package<MapOccupation>> serializable;

    // A package to store our stuff
    [Serializable]
    public struct Package<TElement>
    {
        public int Index0;
        public int Index1;
        public TElement Element;
        public Package(int idx0, int idx1, TElement element)
        {
            Index0 = idx0;
            Index1 = idx1;
            Element = element;
        }

        public Vector2Int Index => new Vector2Int(Index0, Index1);

    }

    public void DestroyAt(Vector2Int index)
    {
        Place(index, null);
    }

    public void Place(Vector2Int index, MapOccupation occupation)
    {
        UnserializeableMap[index.x,index.y] = occupation;
        UpdateSerialzable();
        //serializable[index.y * dimensions.x + index.x].Element = occupation;
    }

    public void OnBeforeSerialize()
    {
        // Convert our unserializable array into a serializable list
        UpdateSerialzable();
    }

    protected void UpdateUnserializeable()
    {
        int indexX = 0;
        int indexY = 0;
        try
        {     
            foreach (var package in serializable)
            {
                indexX = package.Index0;
                indexY = package.Index1;    
                UnserializeableMap[indexX, indexY] = package.Element;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"out of bounds! With index x being: {indexX} and indexY {indexY}. Map has dimesions of: {new Vector2Int(UnserializeableMap.GetLength(0), UnserializeableMap.GetLength(1))}");
        }
    }

    protected void UpdateSerialzable()
    {
        serializable = new List<Package<MapOccupation>>();
        int x = 0;
        int y = 0;
        try
        {
            for (x = 0; x < dimensions.x; x++)
            {
                for (y = 0; y < dimensions.y; y++)
                {
                    serializable.Add(new Package<MapOccupation>(x, y, UnserializeableMap[x, y]));
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"out of bounds! With index x being: {x} and indexY {y}. Map has dimesions of: {new Vector2Int(UnserializeableMap.GetLength(0), UnserializeableMap.GetLength(1))}");
        }
    }

    public void OnAfterDeserialize()
    {
        // Convert the serializable list into our unserializable array
        UpdateUnserializeable();
    }

    public List<Package<MapOccupation>> GetMapDesign()
    {
        UpdateSerialzable();
        return serializable;
    }

}