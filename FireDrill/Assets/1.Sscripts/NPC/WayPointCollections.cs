using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointCollections : MonoBehaviour
{
    public static List<WayPoint> list;

    private void Awake()
    {
        list = new List<WayPoint>();

        foreach (Transform item in transform)
        {
            list.Add(item.GetComponent<WayPoint>());
        }

    }
}
