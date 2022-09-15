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

    public Image progressBar;

    [SerializeField] private List<FireObject> fireObjects;
    [SerializeField] private Transform[] fireSpot;
    private bool isEnd = false;

    private void Start()
    {

    }

    private void Update()
    {
        currentProgress = GetCurrentProgress();

        if (NetworkManager.User.hasExtingisher)
        {
            photonView.RPC(nameof(SetProgressGaugeRPC), RpcTarget.All, currentProgress / totalProgress);
        }

        GetTrainingProgress?.Invoke(currentProgress / totalProgress);
    }

    [PunRPC]
    private void SetProgressGaugeRPC(float value)
    {
        progressBar.fillAmount = value;
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

    public void SpawnFire()
    {
        for (int i = 0; i < fireSpot.Length; i++)
        {
            GameObject firePrefab = PhotonNetwork.Instantiate("FireObject", fireSpot[i].position, Quaternion.identity);
            FireObject fire = firePrefab.GetComponent<FireObject>();

            fireObjects.Add(fire);
        }

                for(int i = 0; i < fireObjects.Count; i++)
        {
            fireObjects[i].fireObjectIndex = i;
            //fireObjects[i].onFireObjectTriggerd += SyncFireObject;
        }
        totalProgress = GetTotalProgress();
    }
}
