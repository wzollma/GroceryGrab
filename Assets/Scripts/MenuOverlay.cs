using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuOverlay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI text1;
    [SerializeField] TextMeshProUGUI text2;

    public bool isDone()
    {
        if (text1.enabled)
        {
            titleText.enabled = false;
            text1.enabled = false;
            text2.enabled = true;
            return false;
        }
        else
            text2.enabled = false;

        return true;
    }
}
