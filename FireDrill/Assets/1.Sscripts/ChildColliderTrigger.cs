using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChildColliderTrigger : MonoBehaviourPun
{
    public void Activate()
    {
        GetComponent<InteractableObject>().enabled = true;
        photonView.RPC(nameof(SetParent), RpcTarget.Others);
    }

    [PunRPC]
    private void SetParent()
    {
        transform.SetParent(null, true);
    }

    public void Deactivate()
    {
        //GetComponent<InteractableObject>().enabled = false;
    }
}
