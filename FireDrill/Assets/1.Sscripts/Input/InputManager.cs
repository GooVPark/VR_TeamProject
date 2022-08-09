using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("Left Hand Button Input")]
    public InputActionReference leftStart = null;
    public InputActionReference leftPrimary = null;
    public InputActionReference leftSecondary = null;
    [Space(5)]

    [Header("Right Hand Button Input")]
    public InputActionReference rightPrimary = null;
    public InputActionReference rightSecondary = null;

    private void Awake()
    {
        leftStart.action.started += LeftStart;
        leftPrimary.action.started += LeftPrimary;
        leftSecondary.action.started += LeftSecondary;

        rightPrimary.action.started += RightPrimary;
        rightSecondary.action.started += RightSecondary;
    }

    private void LeftStart(InputAction.CallbackContext context)
    {
        Debug.Log("Left Start Button");
    }

    private void LeftPrimary(InputAction.CallbackContext context)
    {
        Debug.Log("Left Primary Button");
    }

    private void LeftSecondary(InputAction.CallbackContext context)
    {
        Debug.Log("Left Secondary Button");
    }

    private void RightPrimary(InputAction.CallbackContext context)
    {
        Debug.Log("Right Primary Input");
    }

    private void RightSecondary(InputAction.CallbackContext context)
    {
        Debug.Log("Rigth Secondary Input");
    }
}
