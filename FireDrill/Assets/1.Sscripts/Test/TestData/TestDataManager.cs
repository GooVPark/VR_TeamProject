using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using MongoDB.Bson;
using MongoDB.Driver;

[CreateAssetMenu(menuName = "Test Database")]
public class TestDataManager : ScriptableObject
{
    [SerializeField] private UserTable userTable;
    public string fileName;

    public UserTable GetUserTable()
    {
        return userTable;
    }

    [ContextMenu("Rest User Table")]
    public void ResetUserTable()
    {
        for (int i = 0; i < 100; i++)
        {
            userTable.AddUser(i.ToString(), i.ToString(), i.ToString(), UserType.Student, "com", i);
        }

        for (int i = 100; i < 105; i++)
        {
            userTable.AddUser(i.ToString(), i.ToString(), i.ToString(), UserType.Lecture, "com", i);
        }
    }

    [ContextMenu("Save User Table")]
    public void SaveUserTable()
    {
        DataBbaseSystem.Save(userTable, fileName);
    }

    [ContextMenu("Load User Table")]
    public void LoadUserTable()
    {
        DataBbaseSystem.Load(fileName);
    }
}


public enum UserType { Lecture, Student }

[System.Serializable]
public class TempDataBase
{
    public UserTable userTable;
}

[System.Serializable]
public class UserTable
{
    public List<int> key;
    public List<User> value;

    public UserTable()
    {
        key = new List<int>();
        value = new List<User>();
    }

    public void AddUser(string email, string password, string name, UserType userType, string companty, int index)
    {
        int id = ((int)userType + 1) * 100000 + index;

        key.Add(id);
        value.Add(new User(email, password, name, userType, companty, id));
    }

    public bool GetUser(string email, string password, out User user)
    {
        foreach (User u in value)
        {
            if (u.email.Equals(email) && u.password.Equals(password))
            {
                user = u;
                return true;
            }
        }

        //if (value.All(x => x.email.Equals(email)) && value.All(x => x.password.Equals(password)))
        //{
        //    Debug.Log("Found");
        //    user = value.Find(x => x.email.Equals(email));
        //    return true;
        //}

        Debug.Log("Not Found");
        user = null;
        return false;
    }

    public User GetUser(int id)
    {
        if (key.Contains(id))
        {
            int index = key.IndexOf(id);
            return value[index];
        }

        return null;
    }
}



[System.Serializable]
public class User
{

    public string email;
    public string password;

    public int id;
    public string name;
    public UserType userType;
    public string company;

    public int characterNumber;

    public bool isOnline;

    public int trainingProgress = 0;
    public bool hasExtingisher;

    public int currentRoom = 999;
    public int[] quizResult = new int[10];
    public int totalScore;

    public User()
    {

    }

    public User(string email, string password, string name, UserType userType, string company, int id)
    {
        this.email = email;
        this.password = password;
        this.name = name;
        this.userType = userType;
        this.company = company;
        this.id = id;
    }
}

[System.Serializable]
public class LoundgeUser
{
    public ObjectId id;
    public string email;
    public string name;
    public int characterNumber;
    public bool onVoiceChat;
    public UserType userType;

    public LoundgeUser(User user)
    {
        email = user.email;
        name = user.name;
        characterNumber = user.characterNumber;
        onVoiceChat = false;
        userType = user.userType;
    }
}

public static class DataBbaseSystem
{
    private static string SAVE_PATH => Application.persistentDataPath + "/datas/";

    public static void Save(UserTable userTable, string fileName)
    {
        if (!Directory.Exists(SAVE_PATH))
        {
            Directory.CreateDirectory(SAVE_PATH);
        }

        string saveJson = JsonUtility.ToJson(userTable);
        string filePath = SAVE_PATH + fileName + ".json";

        File.WriteAllText(filePath, saveJson);
    }

    public static UserTable Load(string fileName)
    {
        string filePath = SAVE_PATH + fileName + ".json";

        if (!File.Exists(filePath))
        {
            Debug.LogError("File not exist.");
            return null;
        }

        string file = File.ReadAllText(filePath);
        UserTable userTable = JsonUtility.FromJson<UserTable>(file);
        return userTable;
    }
}