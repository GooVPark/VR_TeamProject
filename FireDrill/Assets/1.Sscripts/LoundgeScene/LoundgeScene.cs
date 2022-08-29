using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LoundgeScene : MonoBehaviourPunCallbacks
{
    public Transform playerTransforms;

    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        GameObject playerObject = PhotonNetwork.Instantiate("Player", transform.position, transform.rotation);
    }


}
