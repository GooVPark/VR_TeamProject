using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Alert : MonoBehaviour
{
    public Image alertImage;
    
    public void OnAlert()
    {
        alertImage.gameObject.SetActive(true);
    }
}
