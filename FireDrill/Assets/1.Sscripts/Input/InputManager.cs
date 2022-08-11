using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [Header("Left Hand Button Input")]
    public InputActionReference leftStart = null;
    public InputActionReference leftPrimary = null;
    public InputActionReference leftSecondary = null;

    public InputActionReference leftStick = null;

    public InputActionReference leftControllerDeviceRotation = null;
    public InputActionReference leftControllerPosition = null;
    [Space(5)]

    [Header("Right Hand Button Input")]
    public InputActionReference rightPrimary = null;
    public InputActionReference rightSecondary = null;

    public InputActionReference rightStick = null;

    public InputActionReference rightControllerRotation = null;
    

    #region LeftHand Context Values

    private static bool isLeftHandPrimaryContextValue = false;
    public static bool IsLeftHandPrimaryContextValue { get { return isLeftHandPrimaryContextValue; } }

    private static bool isLeftHandSecondaryContextValue = false;
    public static bool IsLeftHandSecondaryContextValue { get { return isLeftHandSecondaryContextValue; } }

    private static bool isLeftHandStartContextValue = false;
    public static bool IsLeftHandStartContextValue { get { return isLeftHandStartContextValue; } set { isLeftHandStartContextValue = value; } }

    private static bool isLeftHandStickActivated = false;
    public static bool IsLeftHandStickActivated { get { return isLeftHandStickActivated; } }

    private static bool isHook = false;
    public static bool IsHook { get { return isHook; } }
    private static Vector3 hookingDirection = Vector3.zero;
    public static Vector3 HookingDirection { get { return hookingDirection; } }

    private static Vector2 leftHandStickContextValue;
    public static Vector2 LeftHandStickContextValue { get { return leftHandStickContextValue; } }

    private Vector3 prevPosition;

    #endregion


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
        }

        leftStart.action.started += LeftStart;
        leftPrimary.action.started += LeftPrimary;
        leftSecondary.action.started += LeftSecondary;

        leftStick.action.started += OnLeftStickStarted;
        leftStick.action.performed += OnLeftStickPerformed;
        leftStick.action.canceled += OnLeftStickCanceled;

        leftControllerPosition.action.performed += OnLeftControllerPosition;
        leftControllerDeviceRotation.action.performed += OnLeftHandDeviceRotate;

        rightPrimary.action.started += RightPrimary;
        rightSecondary.action.started += RightSecondary;

        rightStick.action.started += OnRightStickStarted;
        rightStick.action.performed += OnRightStickPerformed;
        rightStick.action.canceled += OnRightStickCanceled;
    }

    private void Start()
    {
    }

    #region Left Hand Inputs
    private void LeftStart(InputAction.CallbackContext context)
    {
        Debug.Log("Left Start Button");
        isLeftHandStartContextValue = !isLeftHandStartContextValue;

        if(isLeftHandStartContextValue)
        {
            ViewManager.Show<LectureUIManager>();
        }
        else
        {
            ViewManager.ToMain();
        }
    }

    private void LeftPrimary(InputAction.CallbackContext context)
    {
        Debug.Log("Left Primary Button");
        ViewManager.ShowLast();
    }

    private void LeftSecondary(InputAction.CallbackContext context)
    {
        Debug.Log("Left Secondary Button");
    }

    private void OnLeftStickStarted(InputAction.CallbackContext context)
    {
        Debug.Log("Left Stick Started");
        isLeftHandStickActivated = true;
    }

    private void OnLeftStickPerformed(InputAction.CallbackContext context)
    {
        leftHandStickContextValue = context.ReadValue<Vector2>();
    }

    private void OnLeftStickCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("Left Stick Canceled");
        isLeftHandStickActivated = false;
    }

    private void OnLeftControllerPosition(InputAction.CallbackContext context)
    {
        Vector3 currPosition = context.ReadValue<Vector3>();
        float distance = Vector3.Distance(prevPosition, currPosition);
        if (distance > 0.02f)
        {
            Debug.Log("Hook");
            isHook = true;
            hookingDirection = prevPosition - currPosition;
            Debug.Log(hookingDirection);
        }
        else
        {
            isHook = false;
        }
        prevPosition = context.ReadValue<Vector3>();
    }

    private void OnLeftHandDeviceRotate(InputAction.CallbackContext context)
    {
        
    }
    #endregion


    #region Right Hand Inputs

    private void RightPrimary(InputAction.CallbackContext context)
    {
        Debug.Log("Right Primary Input");
    }

    private void RightSecondary(InputAction.CallbackContext context)
    {
        Debug.Log("Rigth Secondary Input");
    }

    private void OnRightStickStarted(InputAction.CallbackContext context)
    {
        Debug.Log("Right Stick Started");
    }

    private void OnRightStickPerformed(InputAction.CallbackContext context)
    {
        Vector2 contextValue = context.ReadValue<Vector2>();

        Debug.Log(contextValue);
    }

    private void OnRightStickCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("Right Stick Canceled");
    }
    #endregion
}
