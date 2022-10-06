using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LineVisualizerHandler : MonoBehaviour
{
    [SerializeField] private XRInteractorLineVisual lineVisual;
    private bool isPressed;

    private void Start()
    {
        Debug.Log("Set Handler");
        lineVisual = GetComponent<XRInteractorLineVisual>();
        InputManager.rightTriggerButton += LineVisualize;
    }

    public void LineVisualize(bool value)
    {
        Debug.Log("LineVisualize Value: " + value);
        if(value)
        {
            lineVisual.enabled = true;
        }
        else
        {
            lineVisual.enabled = false;
        }
    }
}
