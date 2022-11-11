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

    public GameObject npc;
    [Space(5)]

    [Header("Area A")]
    public Paroxe.PdfRenderer.PDFViewer view;
    [Header("Area B")]
    public GameObject dummyExtinguisher;

    public int targetRoomState;

    public RoomState_GoToA roomStateGoToA;
    public RoomState_GoToB roomStateGoToB;
    public RoomState_SelectMRPlayer roomStateGoToC;

    public delegate void EventMessage(string message);
    public EventMessage eventMessage;   

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        roomSceneManager.requiredPlayer = PhotonNetwork.CurrentRoom.PlayerCount;
        DataManager.Instance.UpdateRoomState(roomSceneManager.roomNumber, true);
        areaA.gameObject.SetActive(false);
        areaA.playerCount = 0;
        areaA.targetObjects.Clear();
        areaB.gameObject.SetActive(false);
        areaB.playerCount = 0;
        areaB.targetObjects.Clear();
        areaC.gameObject.SetActive(false);
        areaC.playerCount = 0;
        areaC.targetObjects.Clear();
        areaMR.gameObject.SetActive(false);
        areaMR.playerCount = 0;

        objectA.gameObject.SetActive(false);
        objectB.gameObject.SetActive(false);
        objectC.gameObject.SetActive(false);

        eventMessage += eventSyncronizer.OnSendMessage;

        #region Area A Initizlize

        view.CurrentPageIndex = 0;
        
        #endregion

        #region Area B Initailzie

        DataManager.Instance.InitializeQuizScore(NetworkManager.User.email);
        NetworkManager.User.hasExtingisher = false;
        roomSceneManager.player.HasExtinguisher = false;
        roomSceneManager.player.QuizScore = -1;

        if (user.userType == UserType.Lecture)
        {
            eventMessage = null;
            eventMessage += eventSyncronizer.OnSendMessage;

            string message = $"{EventMessageType.QUIZ}";
            eventMessage?.Invoke(message);
        }
        npc.SetActive(false);
        #endregion

        #region Area C Initialize

        dummyExtinguisher.SetActive(true);

        objectC.GetComponent<TrainingManager>().ClearFire();
        Extinguisher extinguisher = FindObjectOfType<Extinguisher>();
        if (extinguisher != null)
        {
            Destroy(extinguisher.gameObject);
        }

        if(!NetworkManager.User.hasExtingisher)
        {
            FireObject[] fireObjects = FindObjectsOfType<FireObject>();

            foreach (var fire in fireObjects)
            {
                Destroy(fire.gameObject);
            }
        }

        #endregion

        switch (targetRoomState)
        {
            case 0:

                roomSceneManager.RoomState = roomStateGoToA;

                break;
            case 1:

                roomSceneManager.RoomState = roomStateGoToB;

                break;
            case 2:

                roomSceneManager.RoomState = roomStateGoToC;
                npc.SetActive(true);
                npc.GetComponent<Animator>().SetInteger("AnimationState", 1);

                break;
        }
    }

}
