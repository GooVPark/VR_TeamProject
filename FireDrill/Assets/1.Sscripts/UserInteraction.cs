using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInteraction : MonoBehaviour
{
    public void ShowUserInteractionView()
    {

    }


    private IEnumerator PressTimer(float threshold)
    {
        float elapsedTime = 0f;
        WaitForFixedUpdate fixedTime = new WaitForFixedUpdate();

        while (elapsedTime > threshold)
        {
            elapsedTime += Time.deltaTime;
            yield return fixedTime;
        }
    }
}
