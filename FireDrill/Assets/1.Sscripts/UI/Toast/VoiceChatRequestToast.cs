using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceChatRequestToast : Toast
{
    public ButtonInteractor accept;
    public ButtonInteractor deaccept;

    private void OnEnable()
    {
        accept.gameObject.SetActive(true);
        deaccept.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        accept.gameObject.SetActive(false);
        deaccept.gameObject.SetActive(false);
    }
}
