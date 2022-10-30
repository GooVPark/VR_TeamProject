using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum IdleMode { STAND, SIT }

public class SitStandSettButton : MonoBehaviour
{
    public Image sitImage;
    public Button sitButton;
    public Image standImage;
    public Button standButton;

    [SerializeField] private Sprite sitEnabledSprite;
    [SerializeField] private Sprite sitDisabledSprite;

    [SerializeField] private Sprite standEnabledSprite;
    [SerializeField] private Sprite standDisabledSprite;

    public IdleMode mode;


    public void SwitchIdleMode()
    {
        switch (mode)
        {
            case IdleMode.STAND:

                mode = IdleMode.SIT;

                sitImage.sprite = sitEnabledSprite;
                standImage.sprite = standDisabledSprite;

                sitButton.interactable = false;
                standButton.interactable = true;

                break;
            case IdleMode.SIT:

                mode = IdleMode.STAND;

                standImage.sprite = standEnabledSprite;
                sitImage.sprite = sitDisabledSprite;

                sitButton.interactable = true;
                standButton.interactable = false;

                break;
        }
    }
}
