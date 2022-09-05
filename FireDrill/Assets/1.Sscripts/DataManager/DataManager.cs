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

    IMongoDatabase roomDatabase;
    IMongoCollection<RoomData> roomCollection;

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

        roomDatabase = client.GetDatabase("RoomDatabase");
        roomCollection = roomDatabase.GetCollection<RoomData>("RoomInfo");
    }


    #region Login

    public void SetOnline(string email)
    {
        if (IsExistID(email))
        {
            var filter = Builders<User>.Filter.Eq("email", email);
            var update = Builders<User>.Update.Set("isOnline", true);

            accountCollection.UpdateOne(filter, update);
        }
    }

    public void SetOffline(string email)
    {
        if (IsExistID(email))
        {
            var filter = Builders<User>.Filter.Eq("email", email);
            var update = Builders<User>.Update.Set("isOnline", false);

            accountCollection.UpdateOne(filter, update);
        }
    }

    public int GetUserCount()
    {
        BsonDocument bson = new BsonDocument { };

        return accountCollection.Find(bson).ToList().Count;
    }


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
        Debug.Log("InsertMember");
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

    #region Training Progress Board

    public List<RoomData> GetRoomData()
    {
        List<RoomData> roomDatas = new List<RoomData>();

        BsonDocument bson = new BsonDocument { };
        roomDatas = roomCollection.Find(bson).ToList();

        return roomDatas;
    }

    public void UpdateRoomPlayerCount(int number, int count)
    {
        Debug.Log("UpdateRoomPlayerCount1");
        var filter = Builders<RoomData>.Filter.Eq("roomNumber", number);
        var update = Builders<RoomData>.Update.Set("currentPlayerCount", count);
        Debug.Log("UpdateRoomPlayerCount2");
        roomCollection.UpdateOne(filter, update);
        Debug.Log("UpdateRoomPlayerCount3");
    }

    public void UpdateRoomProgress(int number, int progress)
    {
        var filter = Builders<RoomData>.Filter.Eq("roomNumber", number);
        var update = Builders<RoomData>.Update.Set("progress", progress);

        roomCollection.UpdateOne(filter, update);
    }

    #endregion
    ///
    /// Mongo DB 예제
    /// 
    /// filter를 만들어서 찾을때 쓸수있을거같다
    /// 
    /// var filter = Builduers<User>.Filter.Eq(x => x.name, "name"); 같은거
    /// var filter = Builduers<User>.Filter.Ne(x => x.name, "name"); 다른거
    /// var filter = Builduers<User>.Filter.Gt(x => x.name, "name"); 초과
    /// var filter = Builduers<User>.Filter.Gte(x => x.name, "name"); 이상
    /// var filter = Builduers<User>.Filter.Lt(x => x.name, "name"); 미만
    /// var filter = Builduers<User>.Filter.Lte(x => x.name, "name"); 이하
    /// 
    /// var result = collection.Find(filter).ToList(); 필터로 가져오기
    ///
}
