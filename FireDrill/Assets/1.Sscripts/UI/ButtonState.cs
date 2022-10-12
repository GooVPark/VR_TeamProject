using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonStateManager : MonoBehaviour
{
    public delegate void OnButtonEvent();
    public OnButtonEvent onButtonEvent;

    public enum IconState { Disable, On, Off }
    public IconState iconState;

    public GameObject on;
    public GameObject off;
    public GameObject disable;

    public Button button;

    public GameObject current;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => OnButton());

        //current = disable.gameObject;
    }

    public void ChangeIconState(IconState iconState)
    {
        Debug.Log("Begin ChangeIconState: " + iconState);
        current.SetActive(false);

        switch (iconState)
        {
            case IconState.Disable:
                Debug.Log("To Disable");
                current = disable;
                this.iconState = IconState.Disable;
                break;
            case IconState.On:
                Debug.Log("To On");
                current = on;
                this.iconState = IconState.On;
                break;
            case IconState.Off:
                Debug.Log("To Off");
                current = off;
                this.iconState = IconState.Off;
                break;
        }

        current.SetActive(true);
        Debug.Log("End ChangeIconState: " + iconState);
    }

    public void OnButton()
    {
        Debug.Log("On Button");
        Debug.Log("Begin: " + iconState);
        if(iconState == IconState.Off)
        {
            ChangeIconState(IconState.On);
        }
        else if(iconState == IconState.On)
        {
            ChangeIconState(IconState.Off);
        }

        onButtonEvent?.Invoke();

        Debug.Log("End: " + iconState);
    }
}
