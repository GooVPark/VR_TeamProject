using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

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

[System.Serializable]
public class User
{

    public string email;
    public string password;

    public ObjectId _id;
    public string name;
    public UserType userType;
    public IdleMode idleMode;
    public string company;

    public int characterNumber;

    public bool isOnline;

    public int trainingProgress = 0;
    public bool hasExtingisher;

    public int currentRoom = 999;
    public int[] quizResult = new int[10];
    public int totalScore;

    public string loginPing;
    public string loginFPS;

    public string loundgePing;
    public string loundgeFPS;

    public string roomPing;
    public string roomFPS;

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
        //this._id = id;
    }
}

public class ServerSetting
{
    public ObjectId _id;
    public bool isOnline = false;

    public double announceInterval = 30;
}

[System.Serializable]
public class LoundgeUser
{
    public ObjectId _id;
    public string email;
    public string name;
    public int characterNumber;
    public bool onRequestVoiceChat;
    public bool onVoiceChat;
    public UserType userType;

    public LoundgeUser(User user)
    {
        email = user.email;
        name = user.name;
        characterNumber = user.characterNumber;
        onRequestVoiceChat = false;
        onVoiceChat = false;
        userType = user.userType;
    }
}


[System.Serializable]
public class RoomUser
{
    public ObjectId _id;
    public string email;
    public string name;
    public int characterNumber;

    public RoomUser(User user)
    {
        email = user.email;
        name = user.name;
        characterNumber = user.characterNumber;
    }
}

[System.Serializable]
public class ToastJson
{
    public ObjectId _id;
    public string code;
    public string text;
    public string type;
}

public class DataManager : MonoBehaviour
{
    public bool testBuild = false;
    public bool writeLog = false;
    public static DataManager Instance;
    //public TestDataManager testDataManager;

    MongoClient client;

    IMongoDatabase noticeBoardDatabase;
    IMongoCollection<Post> postCollection;

    IMongoDatabase userAccountDatabase;
    IMongoCollection<User> accountCollection;

    IMongoDatabase roomDatabase;
    IMongoCollection<RoomData> roomCollection;
    IMongoCollection<RoomUser> roomUserCollection;

    IMongoDatabase lobbyDatabase;
    IMongoCollection<LoundgeUser> loundgeUsercollection;

    IMongoDatabase eventLogDatabase;
    IMongoCollection<LogData> eventLogCollection;

    //private static UserTable userTable;
    //public static UserTable UserTable { get => userTable; }

    

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
        
        //userTable = testDataManager.GetUserTable();


        DebugManager.instance.enableRuntimeUI = false;
    }

    private void Start()
    {
        client = new MongoClient("mongodb+srv://firedrillMember:member11@cluster0.pt8thqp.mongodb.net/?retryWrites=true&w=majority");

        userAccountDatabase = client.GetDatabase("UserAccount");

        if (testBuild)
        {
            accountCollection = userAccountDatabase.GetCollection<User>("UserAccountsTest");
        }
        else
        {
            accountCollection = userAccountDatabase.GetCollection<User>("UserAccounts");
        }

        roomDatabase = client.GetDatabase("RoomDatabase");
        if (testBuild)
        {
            roomCollection = roomDatabase.GetCollection<RoomData>("RoomInfoTest");
        }
        else
        {
            roomCollection = roomDatabase.GetCollection<RoomData>("RoomInfo");
            roomUserCollection = roomDatabase.GetCollection<RoomUser>("Room1");
        }

        lobbyDatabase = client.GetDatabase("LobbyData");
        if (testBuild)
        {
            loundgeUsercollection = lobbyDatabase.GetCollection<LoundgeUser>("LoundgeTest");
        }
        else
        {
            loundgeUsercollection = lobbyDatabase.GetCollection<LoundgeUser>("Loundge");
        }

        eventLogDatabase = client.GetDatabase("EventData");
        eventLogCollection = eventLogDatabase.GetCollection<LogData>("EventLog");
    }

    #region Log

    public void WriteLog(string log)
    {
        if (writeLog)
        {


            LogData logData = new LogData(System.DateTime.Now.ToString(), NetworkManager.User.email, log);

            eventLogCollection.InsertOne(logData);
        }
    }

    #endregion

    #region Login
    public int GetUserCount()
    {
        BsonDocument bson = new BsonDocument { };

        return accountCollection.Find(bson).ToList().Count;
    }

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

    public bool FindRoomUser(string email)
    {
        var filter = Builders<RoomUser>.Filter.Eq("email", email);
        var roomUsers = roomUserCollection.Find(filter).ToList();

        if (roomUsers.Count > 0)
        {
            return true;
        }

        return false;
    }

    public void InsertRoomUser(User user)
    {
        if (FindRoomUser(user.email))
        {
            return;
        }
        RoomUser roomUser = new RoomUser(user);
        //NetworkManager.Instance.SetLoundgeUser(roomUser);
        roomUserCollection.InsertOne(roomUser);
    }

    public void DeleteRoomUser(User user)
    {
        var filter = Builders<RoomUser>.Filter.Eq("email", user.email);

        roomUserCollection.DeleteOne(filter);
    }

    public int GetRoomUserCount(int roomNumber)
    {
        int userCounts = 0;
        switch (roomNumber)
        {
            case 0:
                var filter = Builders<RoomUser>.Filter.Empty;
                List<RoomUser> roomUsers = roomUserCollection.Find(filter).ToList();
                userCounts = roomUsers.Count;
                break;
        }

        Debug.Log($"Room Number: {roomNumber}\nUser Counts: {userCounts}");
        return userCounts;
    }
    #endregion

    #region Quiz

    //public Dictionary<int, QuizJson> quizsByCode = new Dictionary<int, QuizJson>();
    //public void GetQuizDatabase()
    //{
    //    BsonDocument bson = new BsonDocument { };
    //    List<QuizJson> quizJsons = new List<QuizJson>();
    //    quizJsons = selectionQuizCollection.Find(bson).ToList();

    //    foreach (QuizJson quiz in quizJsons)
    //    {
    //        quizsByCode.Add(quiz.code, quiz);
    //    }
    //}

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

    #endregion

    #region User

    /// <summary>
    /// 로컬 유저가 현재 속해있는 방의 번호를 업데이트 합니다.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="number"></param>
    public void UpdateCurrentRoom(string email, int number)
    {
        var filter = Builders<User>.Filter.Eq("email", email);
        var update = Builders<User>.Update.Set("currentRoom", number);

        accountCollection.UpdateOne(filter, update);
    }
    /// <summary>
    /// 룸의 유저 목록을 가져옵니다.
    /// 유저 데이터베이스에서 지정된 방번호와 일치하는 유저의 리스트를 불러옵니다.
    /// 라운지의 룸 넘버는 999입니다. 이후 룸 1번은 0번부터 시작합니다.
    /// </summary>
    /// <param name="roomNumber"></param>
    /// <returns></returns>
    public List<User> GetUsersListInRoom(int roomNumber)
    {
        var filter = Builders<User>.Filter.Eq("currentRoom", roomNumber);

        return accountCollection.Find(filter).ToList();
    }

    public void UpdateUserCharacter(string email, int value)
    {
        var filter = Builders<User>.Filter.Eq("email", email);
        var update = Builders<User>.Update.Set("characterNumber", value);

        accountCollection.UpdateOne(filter, update);

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

    public void UpdateLoginFPS(string fps, string ping, string email)
    {
        var filter = Builders<User>.Filter.Eq("email", email);

        var updatePing = Builders<User>.Update.Set("loginPing", ping);
        var updateFps = Builders<User>.Update.Set("loginFPS", fps);

        accountCollection.UpdateOne(filter, updatePing);
        accountCollection.UpdateOne(filter, updateFps);
    }
    public void UpdateLoundgeFPS(string fps, string ping, string email)
    {
        var filter = Builders<User>.Filter.Eq("email", email);

        var updatePing = Builders<User>.Update.Set("loundgePing", ping);
        var updateFps = Builders<User>.Update.Set("loundgeFPS", fps);

        accountCollection.UpdateOne(filter, updatePing);
        accountCollection.UpdateOne(filter, updateFps);
    }
    public void UpdateRoomFPS(string fps, string ping, string email)
    {
        var filter = Builders<User>.Filter.Eq("email", email);

        var updatePing = Builders<User>.Update.Set("roomPing", ping);
        var updateFps = Builders<User>.Update.Set("roomFPS", fps);

        accountCollection.UpdateOne(filter, updatePing);
        accountCollection.UpdateOne(filter, updateFps);
    }
    #endregion

    #region Lobby

    public bool FindLoundgeUser(string email)
    {
        var filter = Builders<LoundgeUser>.Filter.Eq("email", email);
        var loundgeUsers = loundgeUsercollection.Find(filter).ToList();

        if(loundgeUsers.Count > 0)
        {
            return true;
        }

        return false;
    }

    public void InsertLobbyUser(User user)
    {
        if(FindLoundgeUser(user.email))
        {
            return;
        }
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

    public int GetLoundgeUserCount()
    {
        return (int)loundgeUsercollection.CountDocuments(new BsonDocument());
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
