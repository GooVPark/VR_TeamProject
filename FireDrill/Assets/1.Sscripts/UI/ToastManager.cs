using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson;

public class ToastManager : MonoBehaviour
{
    public Toast singleTextAndTypeToast;

    private Coroutine toastDuration;
    private IEnumerator ToastDuration(Toast toast, float duration)
    {
        yield return new WaitForSeconds(duration);
    }

    public void OnToast(string code, Toast toast)
    {
        ToastJson toastJson = DataManager.Instance.toastsByCode[code];
        toast.SetToastMeesage(toastJson);
    }
}
