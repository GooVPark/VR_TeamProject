using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteractor : MonoBehaviour
{
    public delegate void ButtonEvent();
    public ButtonEvent onClick;

    public void OnSelect()
    {
        onClick?.Invoke();
    }
}
