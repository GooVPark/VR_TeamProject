using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LoundgeScene : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject playerObject = PhotonNetwork.Instantiate("Player", transform.position, transform.rotation);
    }
}
