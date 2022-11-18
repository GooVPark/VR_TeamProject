using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeachBubble : MonoBehaviour
{
    public TMP_Text bubbleText;
    public GameObject bubble;

    public void ShowBubble(string message)
    {
        bubble.gameObject.SetActive(true);
        bubbleText.text = message;
        Debug.Log("Show Bubble  " + bubble.transform.position);

    }

    public void HideBubble()
    {
        bubble.gameObject.SetActive(false);
        bubbleText.text = "";
    }
}
