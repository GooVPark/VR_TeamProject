using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LoundgeSceneManager : GameManager
{
    public delegate void Eventmessage(string message);
    public static Eventmessage eventMesage;

    public static LoundgeSceneManager Instance;

    public VoiceManager voiceManager;
    public EventSyncronizer eventSyncronizer;
    public Transform NPCTransforms;
    public SpawnPosition spawnPosition;

    public Dictionary<string, LoundgeUser> spawnedNPC = new Dictionary<string, LoundgeUser>();
    public Dictionary<string, GameObject> spawnedNPCObject = new Dictionary<string, GameObject>();

    [SerializeField] private GameObject npcPrefab;

    private bool isOnline = false;
    public bool isEventServerConnected = false;
    private bool onVoiceChat = false;

    [SerializeField] private GameObject voiceChatErrorToast;
    [SerializeField] private GameObject megaphoneErrorToast;
    [SerializeField] private GameObject scoreboardErrorToast;

    [SerializeField] private List<RoomEnterance> roomEnterances;

    [SerializeField] private Announcement announcement;

    private int currentUserCount;
    private bool onSpawn;

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

    private void Start()
    {
        Debug.Log("Start Loundge Scene Manager");
        PhotonNetwork.JoinLobby();
    }

    private void Update()
    {
        if(PhotonNetwork.CurrentRoom != null)
        {
            Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        }
    }


    private Coroutine checkPlayerCount;
    private IEnumerator CheckPlayerCount()
    {
        WaitForSeconds delay = new WaitForSeconds(1f);
        while(true)
        {
            yield return delay;

            if(currentUserCount != DataManager.Instance.GetLoundgeUsers().Count)
            {
                if(!onSpawn)
                {
                    if(spawnNPC != null)
                    {
                        StopCoroutine(spawnNPC);
                    }

                    spawnNPC = null;
                    spawnNPC = StartCoroutine(SpawnProcess());
                }
            }
        }
    }

    private Coroutine initializer;
    private IEnumerator Initializer()
    {
        Debug.Log("Lounge Initialize");
        WaitForSeconds waitForSeconds = new WaitForSeconds(1f);
        while (!isOnline || !isEventServerConnected)
        {
            isOnline = DataManager.Instance.FindLobbyUser(NetworkManager.User);
            yield return waitForSeconds;
        }

        Debug.Log("Set UI Data");

        DataManager.Instance.UpdateCurrentRoom(NetworkManager.User.email, roomNumber);

        NetworkManager.Instance.SetRoomNumber(roomNumber);
        NetworkManager.Instance.roomType = RoomType.Loundge;
        NetworkManager.Instance.megaphoneDisabled = true;
        NetworkManager.Instance.voiceChatDisabled = true;
        NetworkManager.Instance.onVoiceChat = false;
        NetworkManager.Instance.hasExtingusher = false;
        NetworkManager.User.hasExtingisher = false;

        string message = $"{EventMessageType.SPAWN}_{NetworkManager.User.email}";
        eventMesage?.Invoke(message);

        //for (int i = 0; i < playerCountsText.Length; i++)
        //{
        //    UpdateRoomPlayerCount(i);
        //}

        for (int i = 0; i < roomEnterances.Count; i++)
        {
            UpdateRoomEnterence(i);
        }

        DataManager.Instance.UpdateLoundgeFPS(NetworkManager.Instance.fps, PhotonNetwork.GetPing().ToString(), NetworkManager.User.email);

        if (initializer != null)
        {
            StopCoroutine(initializer);
            initializer = null;
        }
    }

    public void SpawnNPC()
    {
        List<LoundgeUser> loundgeUsers = DataManager.Instance.GetLoundgeUsers();

        foreach (LoundgeUser loundgeUser in loundgeUsers)
        {
            if (!spawnedNPC.ContainsKey(loundgeUser.email))
            {
                spawnedNPC.Add(loundgeUser.email, loundgeUser);

                Vector2 randomValue = Random.insideUnitCircle * 3f;
                Vector3 spawnPosition = this.spawnPosition.transform.position + new Vector3(randomValue.x, 0, randomValue.y);

                GameObject npcObject = Instantiate(npcPrefab, spawnPosition, Quaternion.identity, NPCTransforms);
                spawnedNPCObject.Add(loundgeUser.email, npcObject);
                NPCController npc = npcObject.GetComponent<NPCController>();
                npc.Initialize(loundgeUser);
                npc.eventMessage += eventSyncronizer.OnSendMessage;

                if (NetworkManager.Instance.onVoiceChat)
                {
                    npc.senderIsOnVoiceChat = true;
                }

                if (loundgeUser.email.Equals(NetworkManager.User.email))
                {
                    npcObject.SetActive(false);
                }
            }
        }
    }


    private Coroutine spawnNPC;
    public IEnumerator SpawnProcess()
    {
        onSpawn = true;
        List<LoundgeUser> loundgeUsers = DataManager.Instance.GetLoundgeUsers();

        foreach (LoundgeUser loundgeUser in loundgeUsers)
        {
            if (!spawnedNPC.ContainsKey(loundgeUser.email))
            {
                spawnedNPC.Add(loundgeUser.email, loundgeUser);

                Vector2 randomValue = Random.insideUnitCircle * 3f;
                Vector3 spawnPosition = this.spawnPosition.transform.position + new Vector3(randomValue.x, 0, randomValue.y);

                GameObject npcObject = Instantiate(npcPrefab, spawnPosition, Quaternion.identity, NPCTransforms);
                spawnedNPCObject.Add(loundgeUser.email, npcObject);
                NPCController npc = npcObject.GetComponent<NPCController>();
                npc.Initialize(loundgeUser);
                npc.eventMessage += eventSyncronizer.OnSendMessage;

                if (NetworkManager.Instance.onVoiceChat)
                {
                    npc.senderIsOnVoiceChat = true;
                }

                if (loundgeUser.email.Equals(NetworkManager.User.email))
                {
                    npcObject.SetActive(false);
                }
            }

            yield return null;
        }
        onSpawn = false;
    }

    public void RemoveTargetNPC(string targetKey)
    {
        GameObject target = spawnedNPCObject[targetKey];
        spawnedNPCObject.Remove(targetKey);
        Destroy(target);
        spawnedNPC.Remove(targetKey);
    }

    public void JoinVoiceChatRoom(string userID)
    {
        if(!NetworkManager.Instance.onVoiceChat)
        {
            return;
        }
       
        string roomName = $"VoiceChatRoom_{userID}";

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 2;
        //roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();

        NetworkManager.Instance.roomType = RoomType.VoiceRoom;
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);
    }
    public void VoiceChatToRoomErrorToast()
    {
        voiceChatErrorToast.gameObject.SetActive(true);
    }

    public void LeaveVoiceChatAndEnterRoom()
    {
        voiceChatErrorToast.SetActive(false);
        voiceManager.DisconnectVoiceChat();
    }

    public void KeepVoiceChat()
    {
        voiceChatErrorToast.SetActive(false);
    }


    public void RemoveNPCObject(string email)
    {
        GameObject npc = spawnedNPCObject[email];
        spawnedNPC.Remove(email);
        spawnedNPCObject.Remove(email);
        Destroy(npc);

        DataManager.Instance.DeleteLobbyUser(email);
    }

    public void LeaveVoiceChatRoom()
    {
        PhotonNetwork.LeaveRoom();   
    }

    public LoundgeUser GetLoundgeUser(string email)
    {
        if(spawnedNPC.ContainsKey(email))
        {
            return spawnedNPC[email];
        }
        else
        {
            return null;
        }
    }

    public void MegaphoneSelected()
    {
        megaphoneErrorToast.SetActive(true);
    }

    public void ScoreBoardSelected()
    {
        scoreboardErrorToast.SetActive(true);
    }

    private void CloseToast()
    {
        megaphoneErrorToast.SetActive(false);
    }

    public void UpdateRoomEnterence(int roomNumber)
    {
        bool isStarted = DataManager.Instance.GetRoomProgressState(roomNumber);
        roomEnterances[roomNumber].isStarted = isStarted;
    }

    [SerializeField] float responeInterval;
    [SerializeField] float timeoutInterval;
    private bool isResponsed;
    private Coroutine voiceChatTimer;
    private IEnumerator VoiceChatTimer()
    {
        WaitForFixedUpdate wait = new WaitForFixedUpdate();
        float responseInterval = 3f;
        float responseElapsed = 0f;

        float timeOutInterval = 10;
        float timeOutElapsed = 0;

        while(timeOutInterval > timeOutElapsed)
        {
            responseElapsed += Time.fixedDeltaTime;
            timeOutElapsed += Time.fixedDeltaTime;

            if (responseElapsed > responseInterval)
            {
                if(!photonView.IsMine)
                {
                    photonView.RPC(nameof(CheckOther), RpcTarget.All);
                }

                responseElapsed = 0f;
            }
            if(isResponsed)
            {
                timeOutElapsed = 0f;
                isResponsed = false;
            }

            this.responeInterval = responseElapsed;
            this.timeoutInterval = timeOutElapsed;

            yield return wait;
        }

        voiceManager.DisconnectVoiceChat();
    }

    [PunRPC]
    private void CheckOther()
    {
        isResponsed = true;
    }

    #region Database

    private void InsertUserData()
    {
        DataManager.Instance.InsertLobbyUser(NetworkManager.User);
    }
    

    private Coroutine updateUserCountInVoiceChat;
    private IEnumerator UpdateRoomUserCountInVoiceChat()
    {
        WaitForSeconds wait = new WaitForSeconds(1f);
        while(true)
        {
            UpdateRoomPlayerCount(DataManager.Instance.GetRoomUserCount(0));
            yield return wait;
        }
    }

    #endregion

    #region Training Progress Boards

    [Header("Progress Board")]
    [SerializeField] private List<RoomData> roomDatas;
    [SerializeField] private List<ProgressUI> progresses;
    [SerializeField] private TMP_Text playerCountText;

    [SerializeField] private Image[] progressUIs;
    [SerializeField] private Sprite[] progressImages;

    [SerializeField] private TMP_Text[] playerCountsText;

    private float progressBoardElapsedTime = 0f;
    private float progressBoardUpdateInterval = 1f;

    //라운지 전광판의 진행도를 업데이트 함
    public void UpdateProgressBoard()
    {
        roomDatas = DataManager.Instance.GetRoomData();

        for(int i = 0; i < roomDatas.Count; i++)
        {
            progressUIs[i].sprite = progressImages[roomDatas[i].progress];
        }
    }

    //라운지 전광판의 라운지 유저 수를 업데이트 함
    public void UpdateLobbyPlayerCount(int count)
    {
        playerCountText.text = count.ToString();
    }

    //라운지 전광판의 룸 유저 수를 업데이트함
    public void UpdateRoomPlayerCount(int count)
    {
        playerCountsText[0].text = $"{count}/16";
    }
    #endregion

    #region Room State Lamp

    public Lamp[] lamps;

    public void UpdateRoomStateLamp()
    {
        roomDatas = DataManager.Instance.GetRoomData();

        for (int i = 0; i < roomDatas.Count; i++)
        {
            lamps[i].UpdateLampState(roomDatas[i]);
        }
    }

    #endregion

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Initialize();

        NetworkManager.Instance.inFireControl = false;
        NetworkManager.Instance.voiceChatDisabled = true;
        NetworkManager.Instance.scoreBoardDisabled = true;
        NetworkManager.Instance.onTextChat = false;
        DisableTextChat();
        NetworkManager.Instance.roomType = RoomType.Loundge;
        NetworkManager.Instance.SetRoomNumber(roomNumber);

        

        SetIdleMode(IdleMode.STAND);

        InsertUserData();

        UpdateProgressBoard();


        initializer = StartCoroutine(Initializer());
    }

    public int roomNumber;
    private string roomName;
    private RoomOptions roomOptions;
    public void JoinRoom(int roomNumber)
    {
        string message = $"{EventMessageType.DISCONNECT}_{NetworkManager.User.email}";
        eventMesage?.Invoke(message);

        DataManager.Instance.DeleteLobbyUser(NetworkManager.User);
        NetworkManager.Instance.SetRoomNumber(roomNumber);
        roomName = roomNumber.ToString();

        roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 0;

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        eventSyncronizer.Disconnect();
        LoadingSceneController.LoadScene("Room (BG3)");
    }

    //플레이어가 룸, 1:1음성채팅에 들어갈때 호출됨, 포톤 기본함수임 -> 현재 라운지 매니저에서 호출되는 OnJoinedRoom의 경우 1:1 음성채팅에 진입 할 경우에만 호출됨.
    public override void OnJoinedRoom()
    {
        //플레이어가 진입하는 룸의 종류에 따라 다른 처리
        switch (NetworkManager.Instance.roomType)
        {
            case RoomType.Room: //플레이어가 룸으로 이동 했을 경우 - 지금은 특별한 동작 없음.
                break;
            case RoomType.VoiceRoom: //라운지에서 1:1 음성채팅에 진입하는 경우
                spawnedNPCObject[voiceManager.sender.email].SetActive(false);
                spawnedNPCObject[voiceManager.reciever.email].SetActive(false);

                voiceChatButton.button.OnClick.AddListener(() => voiceManager.DisconnectVoiceChat());
                NetworkManager.Instance.voiceChatDisabled = false;
                NetworkManager.Instance.onVoiceChat = true;
                
                FindObjectOfType<Photon.Voice.Unity.Recorder>().TransmitEnabled = true;

                NetworkManager.Instance.onTextChat = false;
                DisableTextChat();

                announcement.StopAudio();

                foreach(string key in spawnedNPCObject.Keys)
                {
                    spawnedNPCObject[key].GetComponent<NPCController>().SetVoiceChatState(true);
                }

                break;
            case RoomType.Loundge: //라운지로 진입하는 경우
                break;
        }

        PhotonNetwork.Instantiate("Player", Vector3.one, Quaternion.identity);
    }

    //플레이어가 룸, 1:1 음성채팅에서 나올때 호출됨, 포톤 기본함수임
    public override void OnLeftRoom()
    {
        //플레이어가 있던 룸의 종류에 따라 다른 처리
        switch (NetworkManager.Instance.roomType)
        {
            case RoomType.Room:
                break;
            case RoomType.VoiceRoom: //라운지에서 1:1 음성채팅 중 종료 했을 경우 호출됨

                if(voiceManager.sender.email == NetworkManager.User.email)
                {
                    GameObject npcObject = null;
                    if (spawnedNPCObject.TryGetValue(voiceManager.reciever.email, out npcObject))
                    {
                        npcObject.SetActive(true);
                    }
                }
                if(voiceManager.reciever.email == NetworkManager.User.email)
                {
                    GameObject npcObject = null;
                    if (spawnedNPCObject.TryGetValue(voiceManager.sender.email, out npcObject))
                    {
                        npcObject.SetActive(true);
                    }
                }
                NetworkManager.Instance.roomType = RoomType.Loundge;
                NetworkManager.Instance.onVoiceChat = false;
                NetworkManager.Instance.voiceChatDisabled = true;
                FindObjectOfType<Photon.Voice.Unity.Recorder>().TransmitEnabled = false;
                NetworkManager.Instance.onTextChat = false;
                DisableTextChat();
                announcement.PlayAudio();
                voiceChatButton.button.OnClick.RemoveAllListeners();

                foreach (string key in spawnedNPCObject.Keys)
                {
                    spawnedNPCObject[key].GetComponent<NPCController>().SetVoiceChatState(false);
                }
                break;
            case RoomType.Loundge:
                break;
        }
    }
    
    //룸에 다른 플레이어가 접속하면 호출된다.
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (newPlayer != PhotonNetwork.LocalPlayer)
        {
            ((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetComponent<NetworkPlayer>().InvokeProperties();
        }
    }
    #endregion

    private void OnApplicationQuit()
    {
        if (NetworkManager.Instance.roomType == RoomType.VoiceRoom)
        {
            voiceManager.DisconnectVoiceChat();
        }

        string message = $"{EventMessageType.DISCONNECT}_{NetworkManager.User.email}";
        eventMesage?.Invoke(message);
        eventSyncronizer.Disconnect();

        DataManager.Instance.SetOffline(NetworkManager.User.email);
        DataManager.Instance.UpdateCurrentRoom(NetworkManager.User.email, roomNumber);
        DataManager.Instance.DeleteLobbyUser(NetworkManager.User);
    }
    private void OnApplicationPause()
    {
        //string message = $"{EventMessageType.DISCONNECT}_{NetworkManager.User.email}";
        //eventMesage?.Invoke(message);
        //eventSyncronizer.Disconnect();

        //DataManager.Instance.SetOffline(NetworkManager.User.email);
        //DataManager.Instance.UpdateCurrentRoom(NetworkManager.User.email, roomNumber);
        //DataManager.Instance.DeleteLobbyUser(NetworkManager.User);

        if(NetworkManager.Instance.roomType == RoomType.VoiceRoom)
        {
            voiceManager.DisconnectVoiceChat();
        }
    }
}
