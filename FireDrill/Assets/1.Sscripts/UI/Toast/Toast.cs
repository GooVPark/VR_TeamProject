using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Toast : MonoBehaviour
{
    public float duration;
    public bool hasDurationTime;
    public TMP_Text message;

    private void OnEnable()
    {
        if (hasDurationTime)
        {
            toastDuration = StartCoroutine(ToastDuration(duration));
        }
    }

    private void OnDisable()
    {
        if(toastDuration != null)
        {
            StopCoroutine(toastDuration);
            toastDuration = null;
        }
    }

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

    private Coroutine toastDuration;

    private IEnumerator ToastDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }
}
