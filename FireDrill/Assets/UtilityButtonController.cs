using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UtilityButtonController : MonoBehaviourPun
{
    public GameObject levelSelectionUI;
    public RoomSceneManager roomManager;

    public RoomState_GoToA roomStateGoToA;
    public RoomState_GoToB roomStateGoToB;
    public RoomState_SelectMRPlayer roomStateGoToC;

    public RoomState_Initialize roomStateInitialize;

    public void OnSelect()
    {
        if(levelSelectionUI.activeSelf)
        {
            levelSelectionUI.SetActive(false);
        }
        else
        {
            levelSelectionUI.SetActive(true);
        }
    }

    public void LeaveAll()
    {
        photonView.RPC(nameof(LeaveALLRPC), RpcTarget.All);   
    }

    [PunRPC]
    public void LeaveALLRPC()
    {
        roomManager.LeaveRoom();

        levelSelectionUI.SetActive(false);
    }

    public void SetLevelA()
    {
        photonView.RPC(nameof(SetLevelARPC), RpcTarget.All);
    }

    [PunRPC]
    public void SetLevelARPC()
    {
        roomManager.RoomState = roomStateInitialize;
        roomManager.RoomState = roomStateGoToA;

        levelSelectionUI.SetActive(false);
    }

    public void SetLevelB()
    {
        photonView.RPC(nameof(SetLevelBRPC), RpcTarget.All);
    }

    [PunRPC]
    public void SetLevelBRPC()
    {
        roomManager.RoomState = roomStateInitialize;
        roomManager.RoomState = roomStateGoToB;

        levelSelectionUI.SetActive(false);
    }

    public void SetLevelC()
    {
        photonView.RPC(nameof(SetLevelCRPC), RpcTarget.All);
    }

    [PunRPC]
    public void SetLevelCRPC()
    {
        roomManager.RoomState = roomStateInitialize;
        roomManager.RoomState = roomStateGoToC;

        levelSelectionUI.SetActive(false);
    }
}
