using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerGuide : MonoBehaviour
{
    public GameObject controllerGuide;

    private bool isEnabled = false;

    public void OnSelect()
    {
        if(isEnabled)
        {
            controllerGuide.SetActive(false);
            isEnabled = false;
        }
        else
        {
            controllerGuide.SetActive(true);
            isEnabled = true;
        }
    }
}
