using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinOrigin : MonoBehaviour
{
    [SerializeField] Transform pinTransform;

    private Vector3 originPosition;

    private void Start()
    {
        originPosition = pinTransform.position;
    }


}
