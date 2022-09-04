using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson;

public class Account : MonoBehaviour
{
    
}

[System.Serializable]
public class MemberInfo
{
    public ObjectId objectID;
    
    public string id;
    public string password;

    public string name;
}
