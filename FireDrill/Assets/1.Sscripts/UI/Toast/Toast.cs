using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Toast : MonoBehaviour
{
    public TMP_Text message;

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public virtual void SetToastMessage(string message)
    {
        this.message.text = message;
    }
}
