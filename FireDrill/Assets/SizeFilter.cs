using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SizeFilter : MonoBehaviour
{
    private RectTransform rectTransform;
    private RectTransform myRect;
    // Start is called before the first frame update
    void Start()
    {
        myRect = GetComponent<RectTransform>();
        rectTransform = transform.GetChild(0).GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        myRect.sizeDelta = rectTransform.sizeDelta;
    }
}
