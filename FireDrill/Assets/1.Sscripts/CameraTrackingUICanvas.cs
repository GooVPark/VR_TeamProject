using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrackingUICanvas : MonoBehaviour
{
    private Transform cameraTransform;
    public Transform pivot;
    public Vector3 offset;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }


    private void Update()
    {
        Vector2 start = new Vector2(pivot.position.x, pivot.position.z);
        Vector2 end = new Vector2(cameraTransform.position.x, cameraTransform.position.z);
        Vector2 v2 = end - start;

        transform.rotation = Quaternion.Euler(0, -Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg - 90, 0);
        transform.position = pivot.position + offset;
    }
}
