using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PostContext : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text body;

    public void UpdatePost(Post post)
    {
        title.text = post.title;
        body.text = post.body;
    }
}
