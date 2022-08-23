using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActorType { Lecture, Student }

[System.Serializable]
public class Table<T>
{
    public List<T> elements;
    public Table()
    {
        elements = new List<T>();
    }
}
[System.Serializable]
public class User
{
    public int userID;
    public string name;
    public string email;
    public string password;

    public ActorType actorType;
    
    public Time loginTime;
    public Time logoutTime;
    public Time activateTime;

    public int companyID;
}

[System.Serializable]
public class Room
{
    public int roomID;
    public int userCount;

    public Time startTime;
    public Time endTime;
    public Time currentTime;

    public string roomDescription;
    public int companyID;
}

[System.Serializable]
public class Company
{
    public int companyID;
    public int roomID;

    public string companyName;
}