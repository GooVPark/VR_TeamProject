using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToastEventHandler : MonoBehaviour
{
    [SerializeField] private string code;
    [SerializeField] private float duration;
    [SerializeField] private Toast toast;


    public void OnToastEvent()
    {
        ToastJson json = DataManager.Instance.toastsByCode[code];

        toast.SetToastMeesage(json);
        toast.Activate(duration);
    }
}
