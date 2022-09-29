using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineHandler : MonoBehaviour
{
    [SerializeField] private GameObject outlineHoverObject;
    [SerializeField] private GameObject outlineSelectObject;
    

    public void OnHoverEntered()
    {    
        outlineHoverObject?.SetActive(true);
    }

    public void OnHoverExited()
    {
        outlineHoverObject?.SetActive(false);
    }

    public void OnSelectEntered()
    {
        outlineSelectObject?.SetActive(true);
    }

    public void OnSelectExit()
    {
        outlineSelectObject.SetActive(false);
    }
}
