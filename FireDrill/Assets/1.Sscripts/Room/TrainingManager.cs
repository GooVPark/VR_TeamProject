using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TrainingManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public delegate void TrainingProgressEvent(float progress);
    public event TrainingProgressEvent GetTrainingProgress;

    [SerializeField] private float totalProgress;
    [SerializeField] private float currentProgress;

    public Image testImage;

    [SerializeField] private FireObject[] fireObjects;
    private bool isEnd = false;

    private void Start()
    {
        for(int i = 0; i < fireObjects.Length; i++)
        {
            fireObjects[i].fireObjectIndex = i;
            fireObjects[i].onFireObjectTriggerd += SyncFireObject;
        }
        totalProgress = GetTotalProgress();
    }

    private void Update()
    {
        currentProgress = GetCurrentProgress();
        GetTrainingProgress?.Invoke(currentProgress / totalProgress);

        if(currentProgress == 0)
        {

        }
    }

    private float GetTotalProgress()
    {
        float total = 0;
        foreach(FireObject fireObject in fireObjects)
        {
            total += fireObject.totalDuration;
        }

        return total;
    }

    public float GetCurrentProgress()
    {
        float current = 0;

        foreach (FireObject fireObject in fireObjects)
        {
            if (fireObject.isActiveAndEnabled)
            {
                current += fireObject.currentDuration;
            }
        }
        return current;
    }

    public void SyncFireObject(int fireObjectIndex, int flameIndex, bool state)
    {
        photonView.RPC(nameof(SyncFireObjectRPC), RpcTarget.Others, fireObjectIndex, flameIndex, state);
    }

    [PunRPC]
    public void SyncFireObjectRPC(int fireObjectIndex, int flameIndex, bool state)
    {
        fireObjects[fireObjectIndex].flames[flameIndex].gameObject.SetActive(state);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentProgress);
        }
        if (stream.IsReading)
        {
            currentProgress = (float)stream.ReceiveNext();
        }
    }
}
