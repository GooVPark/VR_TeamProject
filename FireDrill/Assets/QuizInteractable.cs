using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class QuizInteractable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<XRSimpleInteractable>().enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<XRSimpleInteractable>().enabled = false;
    }
}
