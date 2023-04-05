using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoTextDisplay : MonoBehaviour
{

    public TextMeshProUGUI infoText;

    protected static InfoTextDisplay instance;

    public void Awake()
    {
        instance = this;
        infoText.text = "";
    }

    public static void DisplayTextFor(string text, float duration)
    {
        instance.StopAllCoroutines();
        instance.infoText.text = text;
        instance.DoDelayed(duration, () => instance.infoText.text = "");
    }

}
