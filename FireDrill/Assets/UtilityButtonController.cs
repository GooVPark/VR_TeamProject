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

    public GameObject forceExitToast;

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
        forceExitToast.SetActive(true);
        levelSelectionUI.SetActive(false);
    }

    [PunRPC]
    public void LeaveALLRPC()
    {
        roomManager.LeaveRoom();
    }

    public void SetLevelA()
    {
        photonView.RPC(nameof(SetLevelARPC), RpcTarget.All);

        levelSelectionUI.SetActive(false);
    }

    [PunRPC]
    public void SetLevelARPC()
    {
        roomStateInitialize.targetRoomState = 0;
        roomManager.RoomState = roomStateInitialize;
    }

    public void SetLevelB()
    {
        photonView.RPC(nameof(SetLevelBRPC), RpcTarget.All);

        levelSelectionUI.SetActive(false);
    }

    [PunRPC]
    public void SetLevelBRPC()
    {
        roomStateInitialize.targetRoomState = 1;
        roomManager.RoomState = roomStateInitialize;
    }

    public void SetLevelC()
    {
        photonView.RPC(nameof(SetLevelCRPC), RpcTarget.All);

        levelSelectionUI.SetActive(false);
    }

    [PunRPC]
    public void SetLevelCRPC()
    {
        roomStateInitialize.targetRoomState = 2;
        roomManager.RoomState = roomStateInitialize;
    }
}
