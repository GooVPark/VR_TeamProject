using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;

public class TrackingPlayerPosition : MonoBehaviour
{
    [SerializeField] private Transform player;

    private void Awake()
    {
        player = FindObjectOfType<XROrigin>().transform;
    }

    private void Update()
    {
        transform.LookAt(player.position);
    }
}
