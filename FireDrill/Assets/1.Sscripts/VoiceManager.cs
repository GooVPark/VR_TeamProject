using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceManager : MonoBehaviour
{
    public static VoiceManager Instance;

    public Transform playersTransform;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance != this)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void SetP2PVoiceMode()
    {
        //���̽����ϴ� ��� ���� ���ÿ��� ���� ���Ұ� ��Ű��
    }
}
