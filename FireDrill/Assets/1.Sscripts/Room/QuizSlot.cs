using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private void OnEnable()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
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
        this.text.text = text;
    }

    public void SetInteractable(bool value)
    {
        gameObject.GetComponent<Button>().interactable = value;
    }
}
