using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SyncTest : MonoBehaviour, IPunObservable
{
    private Vector3 position;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }

        if (stream.IsReading)
        {
            transform.position = (Vector3)stream.ReceiveNext();
        }

    }

}
