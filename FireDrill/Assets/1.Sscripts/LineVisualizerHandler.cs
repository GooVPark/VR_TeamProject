using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LineVisualizerHandler : MonoBehaviour
{
    [SerializeField] private XRInteractorLineVisual lineVisual;
    [SerializeField] private XRRayInteractor rayInteractor;
    private bool isPressed;

    private void Start()
    {
        Debug.Log("Set Handler");
        lineVisual = GetComponent<XRInteractorLineVisual>();
        rayInteractor = GetComponent<XRRayInteractor>();
        InputManager.rightTriggerButton += LineVisualize;
    }

    public void LineVisualize(bool value)
    {
        Debug.Log("LineVisualize Value: " + value);
        if(value)
        {
            lineVisual.enabled = true;
            rayInteractor.enabled = true;
        }
        else
        {
            lineVisual.enabled = false;
            rayInteractor.enabled = false;
        }
    }
}
