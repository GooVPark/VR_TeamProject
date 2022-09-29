using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [SerializeField] private GameObject npcCharacterPrefab;

    public void SpawnNPC(User userData)
    {
        GameObject npcObject = Instantiate(npcCharacterPrefab);
        NPCController npcControlelr = npcObject.GetComponent<NPCController>();
        npcControlelr.Initialize(userData);
    }
}
