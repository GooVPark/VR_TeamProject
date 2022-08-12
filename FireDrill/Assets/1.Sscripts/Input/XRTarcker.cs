using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class XRTarcker : MonoBehaviourPun
{
    Transform xrOrigin;
    // Start is called before the first frame update
    void Start()
    {
        xrOrigin = GameObject.Find("XR Origin").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine) transform.position = xrOrigin.position;
    }
}
