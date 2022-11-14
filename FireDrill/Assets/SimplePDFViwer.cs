using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimplePDFViwer : MonoBehaviour
{
    [SerializeField] private Sprite[] images;
    [SerializeField] private Image image;

    private int currentPage = 0;

    private void Start()
    {
        if(images.Length <= 0)
        {
            return;
        }

        image.sprite = images[0];
    }

    private void OnEnable()
    {
        currentPage = 0;

        image.sprite = images[currentPage];
    }

    public void NextPage()
    {
        if(currentPage == images.Length - 1)
        {
            return;
        }

        currentPage++;

        image.sprite = images[currentPage];
    }

    public void PrevPage()
    {
        if(currentPage == 0)
        {
            return;
        }

        currentPage--;

        image.sprite = images[currentPage];
    }
}
