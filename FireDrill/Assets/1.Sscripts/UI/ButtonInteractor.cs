using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonInteractor : MonoBehaviour
{
    public delegate void ButtonEvent();
    public ButtonEvent onClick;

    public UnityEvent OnClick;

    public Image image;
    public TMP_Text text;
    public BoxCollider collider;

    public bool interactable = false;
    public bool isHovered = false;

    private void Start()
    {
        collider.size = new Vector2(image.rectTransform.rect.width / 100, image.rectTransform.rect.height / 100);
    }

    public void OnSelect()
    {
        if (isHovered)
        {
            Debug.Log("On Select");
            onClick?.Invoke();
            OnClick?.Invoke();
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
