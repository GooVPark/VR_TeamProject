using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using MongoDB.Bson;
using MongoDB.Driver;

public class LogData
{
    public string date;
    public string user;
    public string log;

    public LogData(string date, string user, string log)
    {
        this.date = date;
        this.user = user;
        this.log = log;
    }
}

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
    
    IMongoDatabase textDatabase;
    IMongoCollection<ToastJson> toastCollection;

    IMongoDatabase quizDatabase;
    IMongoCollection<QuizJson> selectionQuizCollection;
    IMongoCollection<QuizJson> quizTypeACollection;
    IMongoCollection<QuizJson> quizTypeBCollection;
    IMongoCollection<QuizJson> quizTypeCCollection;

    IMongoDatabase lobbyDatabase;
    IMongoCollection<LoundgeUser> loundgeUsercollection;

    IMongoDatabase eventLogDatabase;
    IMongoCollection<LogData> eventLogCollection;

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
        
        textDatabase = client.GetDatabase("TextDatabase");
        toastCollection = textDatabase.GetCollection<ToastJson>("Toasts");

        quizDatabase = client.GetDatabase("QuizDatabase");
        selectionQuizCollection = quizDatabase.GetCollection<QuizJson>("Selection");

        lobbyDatabase = client.GetDatabase("LobbyData");
        loundgeUsercollection = lobbyDatabase.GetCollection<LoundgeUser>("MainLoundge");

        eventLogDatabase = client.GetDatabase("EventData");
        eventLogCollection = eventLogDatabase.GetCollection<LogData>("EventLog");
        
        GetAllToast();
        GetQuizDatabase();
    }

    #region Log

    public void WriteLog(string log)
    {
        LogData logData = new LogData(System.DateTime.Now.ToString(), NetworkManager.User.email, log);

        eventLogCollection.InsertOne(logData);
    }

    #endregion

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
        var filter = Builders<RoomData>.Filter.Eq("roomNumber", number);
        var update = Builders<RoomData>.Update.Set("currentPlayerCount", count);
        roomCollection.UpdateOne(filter, update);

    }

    public void UpdateRoomProgress(int number, int progress)
    {
        var filter = Builders<RoomData>.Filter.Eq("roomNumber", number);
        var update = Builders<RoomData>.Update.Set("progress", progress);

        roomCollection.UpdateOne(filter, update);
    }

    #endregion

    #region Toast Text

    public Dictionary<string, ToastJson> toastsByCode = new Dictionary<string, ToastJson>();

    public void GetAllToast()
    {
        BsonDocument bson = new BsonDocument { };
        List<ToastJson> toastJsons = new List<ToastJson>();
        toastJsons = toastCollection.Find(bson).ToList();

        foreach(ToastJson toast in toastJsons)
        {
            toastsByCode.Add(toast.code, toast);
        }
    }


    #endregion

    #region Room

    public void UpdateRoomState(int roomNumber, bool value)
    {
        var filter = Builders<RoomData>.Filter.Eq("roomNumber", roomNumber);
        var update = Builders<RoomData>.Update.Set("isStarted", value);

        roomCollection.UpdateOne(filter, update);
    }

    public bool GetRoomProgressState(int roomNumber)
    {
        List<RoomData> roomDatas = new List<RoomData>();

        BsonDocument bson = new BsonDocument { { "roomNumber", roomNumber } };
        roomDatas = roomCollection.Find(bson).ToList();

        if (roomDatas.Count > 0)
        {
            return roomDatas[0].isStarted;
        }
        else
        {
            return false;
        }
    }

    public RoomData GetRoomData(int roomNumber)
    {
        List<RoomData> roomDatas = new List<RoomData>();

        var filter = Builders<RoomData>.Filter.Eq("roomNumber", roomNumber);

        roomDatas = roomCollection.Find(filter).ToList();
        
        return roomDatas[0];
    }

    #endregion

    #region Quiz

    public Dictionary<int, QuizJson> quizsByCode = new Dictionary<int, QuizJson>();

    public void GetQuizDatabase()
    {
        BsonDocument bson = new BsonDocument { };
        List<QuizJson> quizJsons = new List<QuizJson>();
        quizJsons = selectionQuizCollection.Find(bson).ToList();

        foreach (QuizJson quiz in quizJsons)
        {
            quizsByCode.Add(quiz.code, quiz);
        }
    }

    public void InitializeQuizScore(string email)
    {
        var filter = Builders<User>.Filter.Eq("email", email);
        var update = Builders<User>.Update.Set("quizResult", new int[10]);

        accountCollection.UpdateOne(filter, update);
    }

    public void SetQuizResult(string email, int result, int code)
    {
        var filter = Builders<User>.Filter.Eq("email", email);
        var update = Builders<User>.Update.Set(x => x.quizResult[code], result);

        accountCollection.UpdateOne(filter, update);

        int total = GetTotalScore(email);

        update = Builders<User>.Update.Set("totalScore", total);

        accountCollection.UpdateOne(filter, update);
    }

    public int GetTotalScore(string email)
    {
        var filter = Builders<User>.Filter.Eq("email", email);

        var users = accountCollection.Find(filter).ToList();
        var user = users[0];

        int total = 0;

        for(int i = 0; i < user.quizResult.Length; i++)
        {
            if(user.quizResult[i] == 1)
            {
                total += 10;
            }
        }

        return total;
    }

    public void GetScoreBoard()
    {

    }

    public List<QuizJson> GetQuizListByType(string type)
    {
        IMongoCollection<QuizJson> quizCollection = quizDatabase.GetCollection<QuizJson>(type);
        BsonDocument bson = new BsonDocument { };
        List<QuizJson> quizJsons = new List<QuizJson>();

        return quizCollection.Find(bson).ToList();
    }

    #endregion

    #region User

    public User FindUserByEmail(string email)
    {
        if (!IsExistID(email)) return null;

        List<User> users = accountCollection.Find(x => x.email == email).ToList();

        return users[0];
    }

    public void UpdateCurrentRoom(string email, int number)
    {
        var filter = Builders<User>.Filter.Eq("email", email);
        var update = Builders<User>.Update.Set("currentRoom", number);

        accountCollection.UpdateOne(filter, update);
    }

    public List<User> GetUsersListInRoom(int roomNumber)
    {
        var filter = Builders<User>.Filter.Eq("currentRoom", roomNumber);

        return accountCollection.Find(filter).ToList();
    }

    public void UpdateExtingusher(string email, bool value)
    {
        var filter = Builders<User>.Filter.Eq("email", email);
        var update = Builders<User>.Update.Set("hasExtingisher", value);

        accountCollection.UpdateOne(filter, update);
    }

    public void UpdateUserCharacter(string email, int value)
    {
        var filter = Builders<User>.Filter.Eq("email", email);
        var update = Builders<User>.Update.Set("characterNumber", value);
    }

    public void UpdateUserData(string filterField, object filterValue, string updateField, object updateValue)
    {
        var filter = Builders<User>.Filter.Eq(filterField, filterValue);
        var update = Builders<User>.Update.Set(updateField, updateValue);

        accountCollection.UpdateOne(filter, update);
    }

    public List<User> GetUsersInRoom(int roomNumber)
    {
        var filter = Builders<User>.Filter.Eq("currentRoom", roomNumber);

        List<User> users = accountCollection.Find(filter).ToList();

        return users;
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

    #region Lobby

    public void InsertLobbyUser(User user)
    {
        LoundgeUser loundgeUser = new LoundgeUser(user);
        NetworkManager.Instance.SetLoundgeUser(loundgeUser);
        loundgeUsercollection.InsertOne(loundgeUser);
    }

    public void DeleteLobbyUser(User user)
    {
        var filter = Builders<LoundgeUser>.Filter.Eq("email", user.email);

        loundgeUsercollection.DeleteOne(filter);
    }

    public void DeleteLobbyUser(string email)
    {
        var filter = Builders<LoundgeUser>.Filter.Eq("email", email);

        loundgeUsercollection.DeleteOne(filter);
    }

    public bool FindLobbyUser(User user)
    {
        var filter = Builders<LoundgeUser>.Filter.Eq("email", user.email);
        List<LoundgeUser> loundgeUsers = loundgeUsercollection.Find(filter).ToList();
        
        if(loundgeUsers.Count > 0)
        {
            return true;
        }
        return false;
    }

    public List<LoundgeUser> GetLoundgeUsers()
    {
        var filter = Builders<LoundgeUser>.Filter.Empty;
        List<LoundgeUser> loundgeUsers = loundgeUsercollection.Find(filter).ToList();
        return loundgeUsers;
    }

    public void UpdateLobbyUser(LoundgeUser user)
    {
        var filter = Builders<LoundgeUser>.Filter.Eq("email", user.email);
        var update = Builders<LoundgeUser>.Update.Set("onVoiceChat", user.onVoiceChat);
        loundgeUsercollection.UpdateOne(filter, update);
    }

    public LoundgeUser GetUser(string email)
    {
        var filter = Builders<LoundgeUser>.Filter.Eq("email", email);

        return loundgeUsercollection.Find(filter).ToList()[0];
    }

    public void SetUserOnRequest(string email, bool state)
    {
        var filter = Builders<LoundgeUser>.Filter.Eq("email", email);
        var update = Builders<LoundgeUser>.Update.Set("onRequestVoiceChat", state);

        loundgeUsercollection.UpdateOne(filter, update);
    }

    #endregion
}

public class UserInRoom
{
    public string email;
}
