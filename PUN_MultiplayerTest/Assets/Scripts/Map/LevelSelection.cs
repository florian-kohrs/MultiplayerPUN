using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelSelection : MonoBehaviour
{


    public List<MapDesign> maps;

    protected int selectedMap;

    public MapDesign SelectedMapDesign => maps[selectedMap]; 

    public TextMeshProUGUI text;

    public event Action<MapDesign> onSelectedMapChanged;

    public int SelectedMap
    {
        get
        {
            return selectedMap;
        }
        protected set
        {
            selectedMap = value;
            selectedMap %= maps.Count;
            selectedMap += maps.Count;
            selectedMap %= maps.Count;
            text.text = "Map " + selectedMap;
            onSelectedMapChanged?.Invoke(SelectedMapDesign);
        }
    }

    public void Next()
    {
        SelectedMap++;
    }

    public void Previous()
    {
        SelectedMap--;
    }

}
