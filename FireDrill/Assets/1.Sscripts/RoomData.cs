using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MongoDB.Bson;
using MongoDB.Driver;

public class RoomData 
{
    public ObjectId _id;

    public int roomNumber;
    public string roomName;

    public int progress;
    public int currentPlayerCount;
    public int maxPlayerCount;
    public int requirePlayerCount;

    public string company;
}
