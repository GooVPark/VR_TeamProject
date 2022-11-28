using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSInfo : MonoBehaviour
{
    public TMP_Text text;
    private NetworkManager networkManager;

    private void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
    }

    private void Update()
    {
        text.text = networkManager.fpsInfo;
    }
}
