using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeachBubble : MonoBehaviour
{
    public GameObject bubble;

    public void ShowBubble()
    {
        bubble.SetActive(true);
    }

    public void HideBubble()
    {
        bubble.SetActive(false);
    }
}
