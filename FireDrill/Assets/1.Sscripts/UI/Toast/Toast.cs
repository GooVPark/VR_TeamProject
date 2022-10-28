using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Toast : MonoBehaviour
{
    public bool hasDurationTime;
    public TMP_Text message;

    public void Activate(float duration)
    {
        gameObject.SetActive(true);
        StartCoroutine(ToastDuration(duration));
    }

    public virtual void SetToastMeesage(ToastJson toast)
    {

    }

    public virtual void SetToastMessage(string message)
    {
        this.message.text = message;
    }

    private IEnumerator ToastDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }
}
