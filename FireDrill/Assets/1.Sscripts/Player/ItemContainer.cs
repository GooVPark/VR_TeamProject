using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{

    [SerializeField] private string[] containableItemTags;
    private BoxCollider collider;

    private bool IsContainable(string tag)
    {
        return containableItemTags.Contains(tag);
    }


    private void OnTriggerEnter(Collider other)
    {
        Containable item;
        if(other.TryGetComponent(out item))
        {
            if(IsContainable(item.tag))
            {
                item.onContain?.Invoke();
            }
        }
    }
}
