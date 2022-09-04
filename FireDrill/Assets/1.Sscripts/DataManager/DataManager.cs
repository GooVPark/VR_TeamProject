using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


using MongoDB.Bson;
using MongoDB.Driver;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public TestDataManager testDataManager;

    MongoClient client;
    IMongoDatabase noticeBoardDatabase;
    IMongoCollection<Post> postCollection;

    IMongoDatabase userAccountDatabase;
    IMongoCollection<User> accountCollection;

    private static UserTable userTable;
    public static UserTable UserTable { get => userTable; }

    

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(Instance.gameObject);
        }

        userTable = testDataManager.GetUserTable();
    }

    private void Start()
    {
        client = new MongoClient("mongodb+srv://firedrillMember:member11@cluster0.pt8thqp.mongodb.net/?retryWrites=true&w=majority");

        userAccountDatabase = client.GetDatabase("UserAccount");
        accountCollection = userAccountDatabase.GetCollection<User>("Accounts");

        noticeBoardDatabase = client.GetDatabase("NoticeBoard");
        postCollection = noticeBoardDatabase.GetCollection<Post>("Posts");
    }


    #region Login

    public bool IsExistID(string value)
    {
        BsonDocument bson = new BsonDocument { { "email", value } };

        List<User> accounts = accountCollection.Find(bson).ToList();

        return accounts.Count > 0;
    }

    public bool IsExistAccount(string email, string password, out User user)
    {
        BsonDocument bson = new BsonDocument { { "email", email }, { "password", password} };

        List<User> accounts = accountCollection.Find(bson).ToList();

        foreach(User u in accounts)
        {
            Debug.Log(u.name);
        }

        if(accounts.Count > 0)
        {
            user = accounts[0];
            return true;
        }
        else
        {
            user = null;
            return false;
        }
    }

    public void InsertMember(User member)
    {
        accountCollection.InsertOne(member);
    }

    #endregion


    #region Notice Board

    public int GetLastPostNumber()
    {
        BsonDocument bson = new BsonDocument { };

        return postCollection.Find(bson).ToList().Count;
    }

    public List<Post> GetPostInCurrentPage(int currentPage, int postCountPerPage)
    {
        List<Post> posts = new List<Post>();

        int start = currentPage * postCountPerPage;
        int end = (currentPage + 1) * postCountPerPage;
        
        for(int i = start; i < end; i++)
        {
            Post post = FindPostByNumber(i);
            if (post == null)
            {
                break;
            }

            posts.Add(post);
        }

        return posts;
    }

    public Post FindPostByNumber(int number)
    {
        BsonDocument bson = new BsonDocument { { "postNumber", number } };

        List<Post> post = postCollection.Find(bson).ToList();
        if (post.Count > 0) return post[0];
        else return null;
    }
    public void UploadPost(Post post)
    {
        postCollection.InsertOne(post);
    }

    #endregion
}
