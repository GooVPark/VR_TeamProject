using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrackingUICanvas : MonoBehaviour
{
    // the target to follow
    [SerializeField] private Transform followTarget;
    // local offset to e.g. place the camera behind the target object etc
    [SerializeField] private Vector3 positionOffset;
    // how smooth the camera position is updated, smaller value -> slower
    [SerializeField] private float interpolation = 5f;

    private void Update()
    {
        // target position taking the targets rotation and the offset into account
        var targetPosition = followTarget.position + followTarget.forward * positionOffset.z + followTarget.right * positionOffset.x + followTarget.up * positionOffset.y;

        // move smooth towards this target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, interpolation * Time.deltaTime);

        // rotate to look at the target without rotating in Z
        transform.LookAt(followTarget);
    }
}
