using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hookable : MonoBehaviour
{
    public Transform controller;

    public void OnHookEnter()
    {
        Debug.Log("On Hook Enter");
        StartCoroutine(Hooked());
    }

    public void OnHookExit()
    {
        Debug.Log("On Hook Exit");
        StopAllCoroutines();
    }

    private IEnumerator Hooked()
    {
        while (true)
        {
            if(InputManager.IsHook)
            {
                Debug.Log("Hookable Hook");
                GetComponent<Rigidbody>().AddForce(InputManager.HookingDirection * 5000f);
                break;
            }


            yield return null;
        }
    }
}
