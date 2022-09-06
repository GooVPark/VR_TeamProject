using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToastTypeAndMessage : Toast
{
    public TMP_Text type;

    public override void SetToastMeesage(ToastJson toast)
    {
        message.text = toast.text;
        type.text = toast.type;
    }
}
