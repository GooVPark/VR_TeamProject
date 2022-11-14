using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PageController : MonoBehaviourPun
{
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void Update()
    {
        photonView.RPC(nameof(SyncPosition), RpcTarget.All, rectTransform.localPosition);
    }

    [PunRPC]
    private void SyncPosition(Vector3 position)
    {
        rectTransform.localPosition = position;
    }
}
