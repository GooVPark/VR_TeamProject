using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson;

public class NoticeBoard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}



[System.Serializable]
public class Post
{
    public ObjectId _id;
    
    public string uploadTime;
    public string updateTime;

    public int postNumber;

    public string writer;

    public string title;
    public string body;
}

[System.Serializable]
public class Comment
{
    
}
