using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public TestDataManager testDataManager;

    private static UserTable userTable;
    public static UserTable UserTable { get => userTable; }

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

        userTable = testDataManager.GetUserTable();
    }
}
