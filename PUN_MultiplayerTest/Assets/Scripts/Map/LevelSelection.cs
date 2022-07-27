using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelSelection : MonoBehaviour
{


    public List<MapDesign> maps;

    protected int selectedMap;

    public TextMeshProUGUI text;

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
