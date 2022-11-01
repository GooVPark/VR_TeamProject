using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class QuizSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    [Header("Image Source")]
    [SerializeField] private Sprite selectSprite;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Image buttonImage;
    [Space(5)]

    public UnityEvent OnClick;

    private void OnEnable()
    {
        buttonImage.sprite = defaultSprite;
    }

    public void OnSelected()
    {
        OnClick?.Invoke();
    }

    public void OnHoverEntered()
    {
        buttonImage.sprite = selectSprite;
    }
    
    public void OnHoverExited()
    {
        buttonImage.sprite = defaultSprite;
    }

    public void Select()
    {
        buttonImage.sprite = selectSprite;
    }   

    public void Deselect()
    {
        buttonImage.sprite = defaultSprite;
    }
    


    public string GetText()
    {
        string text = this.text.text;
        return text;
    }

    public void SetEmpty()
    {
        text.text = "-";
    }

    public void SetText(string text)
    {
        this.text = GetComponentInChildren<TextMeshProUGUI>();
        this.text.text = text;
    }

    public void SetInteractable(bool value)
    {
        gameObject.GetComponent<Button>().interactable = value;
    }
}
