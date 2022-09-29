using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectManager : MonoBehaviour
{
    public delegate void CharacterSelectEvent(int value);
    public static CharacterSelectEvent onCharacterSelect;

    [SerializeField] private GameObject[] models;
    [SerializeField] private int currentIndex = 0;

    private GameObject currentModel;

    private void Start()
    {
        currentModel = models[currentIndex];
    }

    public void OnNextCharacter()
    {
        if (currentIndex == models.Length - 1)
        {
            return;
        }

        currentIndex++;
        ChangeModel(currentIndex);
    }

    public void OnPrevCharacter()
    {
        if(currentIndex == 0)
        {
            return;
        }
        currentIndex--;
        ChangeModel(currentIndex);
    }

    private void ChangeModel(int currentIndex)
    {
        currentModel?.SetActive(false);

        currentModel = models[currentIndex];
        currentModel?.SetActive(true);
    }

    public void OnSelectCharacter()
    {
        onCharacterSelect?.Invoke(currentIndex);
    }
}
