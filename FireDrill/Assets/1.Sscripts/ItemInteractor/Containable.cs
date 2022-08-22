using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Containable : MonoBehaviour
{
    public delegate void OnContain();
    public OnContain onContain;

    [SerializeField] private bool isGrab;
    public bool IsGrab => isGrab;
}
