using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LineVisualizerHandler : MonoBehaviour
{
    public enum HandType { Right, Left}
    public HandType handType;

    [SerializeField] private XRInteractorLineVisual lineVisual;
    [SerializeField] private XRRayInteractor rayInteractor;
    private bool isPressed;

    private void Start()
    {
        lineVisual = GetComponent<XRInteractorLineVisual>();
        rayInteractor = GetComponent<XRRayInteractor>();

        switch (handType)
        {
            case HandType.Right:
                InputManager.rightTriggerTouchEvent = null;
                InputManager.rightTriggerTouchEvent += LineVisualize;
                break;
            case HandType.Left:
                InputManager.leftTriggerTouchEvent = null;
                InputManager.leftTriggerTouchEvent += LineVisualize;
                break;
        }
    }

    public void LineVisualize(bool value)
    {
        if(value)
        {
            if(NetworkManager.Instance.inFireControl)
            {
                return;
            }
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
