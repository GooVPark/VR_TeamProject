using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildColliderTrigger : MonoBehaviour
{
    public void Activate()
    {
        GetComponent<InteractableObject>().enabled = true;
    }

    public void Deactivate()
    {
        GetComponent<InteractableObject>().enabled = false;
    }
}
