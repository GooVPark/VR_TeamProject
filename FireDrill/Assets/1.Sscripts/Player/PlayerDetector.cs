using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] private SphereCollider detectingCollider;
    [SerializeField] private string detectableTag;
    [SerializeField] private Transform detectingAreaMarker;

    private void Start()
    {
        detectingCollider = GetComponent<SphereCollider>();
        Vector3 origin = detectingAreaMarker.localScale;
        float scaler = detectingCollider.radius * 2f;
        detectingAreaMarker.localScale = new Vector3(origin.x * scaler, origin.y, origin.z * scaler);

    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag(detectableTag))
        {
            other.GetComponentInParent<NetworkPlayer>()?.ShowSpeachBubble();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(detectableTag))
        {
            other.GetComponentInParent<NetworkPlayer>()?.HideSpeachBubble();
        }
    }
}
