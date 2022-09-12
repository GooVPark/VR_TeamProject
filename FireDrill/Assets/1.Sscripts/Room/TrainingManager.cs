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

        foreach(FireObject fireObject in fireObjects)
        {
            if(fireObject.isActiveAndEnabled)
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
}
