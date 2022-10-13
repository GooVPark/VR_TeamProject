using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteractor : MonoBehaviour
{
    public delegate void ButtonEvent();
    public ButtonEvent onClick;

    public bool interactable = false;
    public bool isHovered = false;

    public void OnSelect()
    {
        if (isHovered)
        {
            onClick?.Invoke();
        }
    }

    public void OnHoverEntered()
    {
        isHovered = true;
    }

    public void OnHoverExited()
    {
        isHovered = false;
    }
}
