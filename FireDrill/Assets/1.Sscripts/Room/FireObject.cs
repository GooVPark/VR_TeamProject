using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class FireObject : MonoBehaviourPun
{
    public delegate void FireObjectDelegate(int fireObjectIndex, int flameIndex, bool state);
    public FireObjectDelegate onFireObjectTriggerd;
    public GameObject[] flames;

    public int fireObjectIndex = 0;
    [SerializeField] private float extinguishThrashold = 0f;
    [SerializeField] private float reviveThrashold = 0f;
    [SerializeField] private int flameIndex = 0;
    public float totalDuration = 0;
    public float currentDuration = 0f;
    private float killTime;
    private float reviveTime;
    [SerializeField] private bool isExtinguishing = false;

    private void Start()
    {
        totalDuration = flames.Length;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (flameIndex < 0)
            {
                return;
            }
            if (!isExtinguishing)
            {
                reviveTime += Time.deltaTime;
                if (reviveThrashold < reviveTime)
                {
                    //onFireObjectTriggerd(fireObjectIndex, flameIndex, true);
                    photonView.RPC(nameof(ControlFire), RpcTarget.All, flameIndex, true);
                    flameIndex--;
                    flameIndex = Mathf.Clamp(flameIndex, 0, flames.Length);

                    reviveTime = 0f;
                }
            }

            currentDuration = totalDuration - flameIndex;
        }
    }

    private Coroutine extinguishTrigger;
    private IEnumerator ExtinguishTrigger()
    {
        float elapsedTime = 0f;
        while(elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isExtinguishing = false;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (photonView.IsMine)
        {
            isExtinguishing = true;
            killTime += Time.deltaTime;
            if (extinguishThrashold < killTime)
            {
                //onFireObjectTriggerd(fireObjectIndex, flameIndex, false);
                photonView.RPC(nameof(ControlFire), RpcTarget.All, flameIndex, false);
                flameIndex++;
                killTime = 0f;

                if (flameIndex >= flames.Length)
                {
                    StopCoroutine(extinguishTrigger);
                    gameObject.SetActive(false);
                }
            }

            if (extinguishTrigger != null)
            {
                StopCoroutine(extinguishTrigger);
                extinguishTrigger = null;
            }
            if (gameObject.activeSelf)
            {
                extinguishTrigger = StartCoroutine(ExtinguishTrigger());
            }
        }
    }


    [PunRPC]
    public void ControlFire(int index, bool value)
    {
        flames[index].SetActive(value);
    }
}
