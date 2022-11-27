using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public delegate void ControllerInputReadEvent(bool value);
    public static ControllerInputReadEvent PrimaryReaded;
    public static ControllerInputReadEvent SecondaryReaded;

    public delegate void LeftSecondaryButtonEvent();
    public static LeftSecondaryButtonEvent leftSecondaryButton;

    public delegate void LeftPrimaryButtonEvent();
    public static LeftPrimaryButtonEvent leftPrimaryButton;

    public delegate void LeftTriggerButtonEvent(bool value);
    public static LeftTriggerButtonEvent leftTriggerButton;
    public static LeftTriggerButtonEvent leftTriggerTouchEvent;

    public delegate void RightTriggerButtonEvent(bool value);
    public static RightTriggerButtonEvent rightTriggerButton;
    public static RightTriggerButtonEvent rightTriggerTouchEvent;

    public delegate void BoolEvent(bool value);

    public delegate void FloatEvent(float value);
    public static FloatEvent onRightTriggerValue;
    public static FloatEvent onRightTriggerTouched;
    public static FloatEvent onRightThumbsValue;
    public static FloatEvent onRightGrabValue;

    public static FloatEvent onLeftTriggerValue;
    public static FloatEvent onLeftTriggerTouched;
    public static FloatEvent onLeftThumbsValue;
    public static FloatEvent onLeftGrabValue;

    public static InputManager Instance;

    [Header("Left Hand Button Input")]
    public InputActionReference leftStart = null;
    public InputActionReference leftPrimary = null;
    public InputActionReference leftSecondary = null;

    public InputActionReference leftTrigger = null;
    public InputActionReference leftTriggerValue = null;
    public InputActionReference leftTriggerTouched = null;
    public InputActionReference leftGrabVAlue = null;
    public InputActionReference leftAirTab = null;

    public InputActionReference leftStick = null;

    public InputActionReference leftControllerDeviceRotation = null;
    public InputActionReference leftControllerPosition = null;
    [Space(5)]

    [Header("Right Hand Button Input")]
    public InputActionReference rightPrimary = null;
    public InputActionReference rightSecondary = null;

    public InputActionReference rightTrigger = null;
    public InputActionReference rightTriggerValue = null;
    public InputActionReference rightTriggerTouched = null;
    public InputActionReference rightGrabValue = null;
    public InputActionReference rightAirTab = null;

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

    private static bool isRightHandPrimaryPerformed = false;
    public static bool IsRightHandPrimaryPerformed { get { return isRightHandPrimaryPerformed; } }

    private static bool isRightHandSecondaryPerformed = false;

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
            DontDestroyOnLoad(gameObject);
        }

        leftStart.action.started += LeftStart;
        leftPrimary.action.started += LeftPrimary;
        leftSecondary.action.started += LeftSecondary;

        leftStick.action.started += OnLeftStickStarted;
        leftStick.action.performed += OnLeftStickPerformed;
        leftStick.action.canceled += OnLeftStickCanceled;

        leftTrigger.action.started += OnLeftTriggerStarted;
        leftTrigger.action.performed += OnLeftTriggerPerformed;
        leftTrigger.action.canceled += OnLeftTriggerCanceled;

        leftControllerPosition.action.performed += OnLeftControllerPosition;
        leftControllerDeviceRotation.action.performed += OnLeftHandDeviceRotate;

        rightPrimary.action.started += RightPrimary;
        rightPrimary.action.performed += RightHandPrimaryPerformed;
        rightPrimary.action.canceled += RightHandPrimaryCanceled;

        rightSecondary.action.started += RightSecondary;

        rightStick.action.started += OnRightStickStarted;
        rightStick.action.performed += OnRightStickPerformed;
        rightStick.action.canceled += OnRightStickCanceled;

        rightTrigger.action.started += OnRightTriggerStarted;
        rightTrigger.action.performed += OnRightTriggerPerformed;
        rightTrigger.action.canceled += OnRightTriggerCanceled;

        //Point
        rightTriggerValue.action.performed += OnRightTriggerValuePerformed;
        leftTriggerValue.action.performed += OnLeftTriggerValuePerformed;
        

        //ThumsUp
        rightAirTab.action.started += OnRightThumbsUpStarted;
        rightAirTab.action.canceled += OnRightThumbsUpCanceled;
        leftAirTab.action.started += OnLeftThumbsUpStarted;
        leftAirTab.action.canceled += OnLeftThumbsUpCanceled;

        //Flex
        rightGrabValue.action.performed += OnRightGrabPerformed;
        leftGrabVAlue.action.performed += OnLeftGrabValuePerformed;

        //TriggerTouched
        rightTriggerTouched.action.started += OnRightTriggerTouchedStarted;
        rightTriggerTouched.action.canceled += OnRightTriggerTouchedCanceled;

        leftTriggerTouched.action.started += OnLeftTriggerTouchedStarted;
        leftTriggerTouched.action.canceled += OnLeftTriggerTouchedCanceled;
    }

    private void Start()
    {
    }

    #region Left Hand Inputs
    private void LeftStart(InputAction.CallbackContext context)
    {
        isLeftHandStartContextValue = !isLeftHandStartContextValue;

        if(isLeftHandStartContextValue)
        {
            switch (NetworkManager.User.userType)
            {
                case UserType.Lecture:
                    ViewManager.Show<LectureUIManager>();
                    break;
                case UserType.Student:
                    ViewManager.Show<StudentUIManager>(); 
                    break;
            }
        }
        else
        {
            ViewManager.ToMain();
        }
    }

    private void LeftPrimary(InputAction.CallbackContext context)
    {
        leftPrimaryButton?.Invoke();
    }

    private void LeftSecondary(InputAction.CallbackContext context)
    {
        leftSecondaryButton?.Invoke();
    }

    private void OnLeftStickStarted(InputAction.CallbackContext context)
    {
        isLeftHandStickActivated = true;
    }

    private void OnLeftStickPerformed(InputAction.CallbackContext context)
    {
        leftHandStickContextValue = context.ReadValue<Vector2>();
    }

    private void OnLeftStickCanceled(InputAction.CallbackContext context)
    {
        isLeftHandStickActivated = false;
    }

    private void OnLeftControllerPosition(InputAction.CallbackContext context)
    {
        Vector3 currPosition = context.ReadValue<Vector3>();
        float distance = Vector3.Distance(prevPosition, currPosition);
        if (distance > 0.02f)
        {
            isHook = true;
            hookingDirection = prevPosition - currPosition;
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

    private void OnLeftTriggerValuePerformed(InputAction.CallbackContext context)
    {
        onLeftTriggerValue?.Invoke(1 - context.ReadValue<float>());
    }

    private void OnLeftTriggerTouchedStarted(InputAction.CallbackContext context)
    {
        onLeftTriggerTouched?.Invoke(0);
        leftTriggerTouchEvent?.Invoke(true);
    }

    private void OnLeftTriggerTouchedCanceled(InputAction.CallbackContext context)
    {
        onLeftTriggerTouched?.Invoke(1);
        leftTriggerTouchEvent?.Invoke(false);
    }
    private void OnLeftTriggerStarted(InputAction.CallbackContext context)
    {
        Debug.Log("Start");
        leftTriggerButton?.Invoke(true);
    }

    private void OnLeftTriggerPerformed(InputAction.CallbackContext context)
    {
        leftTriggerButton?.Invoke(true);
    }

    private void OnLeftTriggerCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("Cancel");
        leftTriggerButton?.Invoke(false);
    }

    private void OnLeftGrabValuePerformed(InputAction.CallbackContext context)
    {
        onLeftGrabValue?.Invoke(context.ReadValue<float>());
    }

    private void OnLeftThumbsUpStarted(InputAction.CallbackContext context)
    {
        onLeftThumbsValue?.Invoke(0f);
    }

    private void OnLeftThumbsUpCanceled(InputAction.CallbackContext context)
    {
        onLeftThumbsValue?.Invoke(1f);
    }
    #endregion


    #region Right Hand Inputs

    private void RightPrimary(InputAction.CallbackContext context)
    {
        
    }

    private void RightHandPrimaryPerformed(InputAction.CallbackContext context)
    {
        isRightHandPrimaryPerformed = true;

        PrimaryReaded?.Invoke(IsRightHandPrimaryPerformed);
    }

    private void RightHandPrimaryCanceled(InputAction.CallbackContext context)
    {
        isRightHandPrimaryPerformed = false;

        PrimaryReaded?.Invoke(IsRightHandPrimaryPerformed);
    }

    private void RightSecondary(InputAction.CallbackContext context)
    {
        isRightHandSecondaryPerformed = !isRightHandSecondaryPerformed;

        SecondaryReaded?.Invoke(isRightHandSecondaryPerformed);
    }

    private void OnRightStickStarted(InputAction.CallbackContext context)
    {

    }

    private void OnRightStickPerformed(InputAction.CallbackContext context)
    {
        Vector2 contextValue = context.ReadValue<Vector2>();
    }

    private void OnRightStickCanceled(InputAction.CallbackContext context)
    {
        
    }

    private void OnRightTriggerStarted(InputAction.CallbackContext context)
    {
        rightTriggerButton?.Invoke(true);
    }

    private void OnRightTriggerPerformed(InputAction.CallbackContext context)
    {
        rightTriggerButton?.Invoke(true);
    }

    private void OnRightTriggerCanceled(InputAction.CallbackContext context)
    {
        rightTriggerButton?.Invoke(false);
    }    

    private void OnRightTriggerValuePerformed(InputAction.CallbackContext context)
    {
        onRightTriggerValue?.Invoke(1 - context.ReadValue<float>());
    }

    private void OnRightTriggerTouchedStarted(InputAction.CallbackContext context)
    {
        onRightTriggerTouched?.Invoke(0);
        rightTriggerTouchEvent?.Invoke(true);
    }

    private void OnRightTriggerTouchedCanceled(InputAction.CallbackContext context)
    {
        onRightTriggerTouched?.Invoke(1);
        rightTriggerTouchEvent?.Invoke(false);
    }

    private void OnRightThumbsUpStarted(InputAction.CallbackContext context)
    {
        onRightThumbsValue?.Invoke(0f);
    }

    private void OnRightThumbsUpCanceled(InputAction.CallbackContext context)
    {
        onRightThumbsValue?.Invoke(1f);
    }

    private void OnRightGrabPerformed(InputAction.CallbackContext context)
    {
        onRightGrabValue?.Invoke(context.ReadValue<float>());
    }
    #endregion

    #region Input Timer

    public IEnumerator PressTimer(float threshold)
    {
        float elapsedTime = 0f;
        WaitForFixedUpdate fixedTime = new WaitForFixedUpdate();

        while(elapsedTime > threshold)
        {
            elapsedTime += Time.deltaTime;
            yield return fixedTime;
        }
    }

    #endregion
}
