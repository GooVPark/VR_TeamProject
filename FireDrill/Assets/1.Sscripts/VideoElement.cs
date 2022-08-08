using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoElement : MonoBehaviour
{
    public Image thumbnail;
    public Button selectButton;
    public Button infoButton;

    public Image progressImage;
    [Range(0f, 1f)]
    public float progressAmount;
}
