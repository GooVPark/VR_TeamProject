using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPosition : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPositions;

    private void Start()
    {
        float angle = 360f / (float)spawnPositions.Length;
        for(int i = 1; i <= spawnPositions.Length; i++)
        {
            spawnPositions[i].Rotate(Vector3.up, angle * i);
        }
    }

    public Vector3 GetPosition(int index)
    {
        return spawnPositions[index].GetChild(0).transform.position;
    }
}
