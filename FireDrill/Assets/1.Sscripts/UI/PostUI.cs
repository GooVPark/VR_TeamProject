using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PostUI : MonoBehaviour
{
    [SerializeField] private TMP_Text writerText;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text postNumberText;

    [SerializeField] private Button button;

    public void UpdatePost(Post post)
    {
        //Debug.Log("UpdatePost");

        button.onClick.RemoveAllListeners();

        writerText.text = post.writer;
        titleText.text = post.title;
        postNumberText.text = post.postNumber.ToString();

        button.onClick.AddListener(() => ShowPost(post));
    }

    public void ShowPost(Post post)
    {
        //LoundgeSceneManager.Instance.ShowPost(post);
    }
}
