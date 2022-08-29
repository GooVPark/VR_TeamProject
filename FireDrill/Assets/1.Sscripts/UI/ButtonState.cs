using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonState : MonoBehaviour
{
    public enum IconState { Disable, On, Off }
    public IconState iconState;

    public Image targetImage;
    [SerializeField] private Sprite[] sprites;

    private Color disableColor = Color.gray;
    private Color onColor = Color.white;
    private Color offColor = Color.red;

    public void ChangeIconState(IconState iconState)
    {
        //targetImage.sprite = sprites[(int)iconState];
        Color color = Color.blue;
        switch (iconState)
        {
            case IconState.Disable:
                color = disableColor;
                break;
            case IconState.On:
                color = onColor;
                break;
            case IconState.Off:
                color = offColor;
                break;
        }

        targetImage.color = color;
    }


}
