using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;

public class PlayerManager : MonoBehaviour
{
    public Transform originTransform;

    private void Update()
    {
        UpdatePlayerDistance();
    }

    public void UpdatePlayerDistance()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            NetworkPlayer player = transform.GetChild(i).GetComponent<NetworkPlayer>();
            float distance = Vector3.Distance(originTransform.position, player.transform.position);
            player.UpdateDistanceUI(distance);
        }
    }
}
