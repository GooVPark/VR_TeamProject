using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public DataObject userDB;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
        }
    }


    public bool FindUserData(string email, string password, out UserData userData)
    {
        return userDB.IsContain(email, password, out userData);
    }
}
