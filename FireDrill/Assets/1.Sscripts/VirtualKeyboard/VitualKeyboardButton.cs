using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyboardButtonState { lkr, ukr, len, uen, num, }

public class VitualKeyboardButton : MonoBehaviour
{
  
    private bool isHovered = false;
    [SerializeField] GameObject outline;

    public void OnHoverEntered()
    {
        HoverEvent(true);
    }

    public void OnHoverExited()
    {
        HoverEvent(false);
    }

    public void OnSelected()
    {
        
    }

    private void HoverEvent(bool value)
    {
        isHovered = value;
        outline.SetActive(value);
    }
}
