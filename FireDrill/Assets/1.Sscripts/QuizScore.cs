using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson;
using MongoDB.Driver;

public class QuizScore : MonoBehaviour
{
    
    //string quiz = new string(NetworkManager.User.email);
    //NetworkManager.User.email = new string()
    // Start is called before the first frame update
    void Start()
    {
        string quiz = DataManager.Instance.temp;

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
