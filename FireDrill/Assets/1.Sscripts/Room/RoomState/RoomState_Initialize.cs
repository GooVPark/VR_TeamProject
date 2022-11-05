using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomState_Initialize : RoomState
{
    [Header("Event Area")]
    public EventArea areaA;
    public EventArea areaB;
    public EventArea areaC;
    public EventAreaMR areaMR;
    [Space(5)]

    [Header("Room Objects")]
    public GameObject objectA;
    public GameObject objectB;
    public GameObject objectC;
    [Space(5)]

    [Header("Area B")]
    public GameObject dummyExtinguisher;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        areaA.gameObject.SetActive(false);
        areaB.gameObject.SetActive(false);
        areaC.gameObject.SetActive(false);

        objectA.gameObject.SetActive(false);
        objectB.gameObject.SetActive(false);
        objectC.gameObject.SetActive(false);

        dummyExtinguisher.SetActive(true);

        Extinguisher extinguisher = FindObjectOfType<Extinguisher>();
        if(extinguisher != null)
        {
            Destroy(extinguisher.gameObject);
        }

        NetworkManager.User.hasExtingisher = false;
        roomSceneManager.player.HasExtinguisher = false;
    }

}
