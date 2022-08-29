using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUIPivot : MonoBehaviour
{
    public Transform uiObject;
    public Vector3[] points;

    private float elapsedTime;

    private void Update()
    {
        for(int i = 0; i < points.Length - 1; i++)
        {
            points[i] = points[i + 1];
        }
        points[0] = transform.position;
    }
}
