using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class LoundgeSceneManager : MonoBehaviourPunCallbacks
{
    public static LoundgeSceneManager Instance;

    public Transform playerTransforms;
    private NetworkPlayer player;

    public VoiceChatRequestToast voiceChatRequestToast;

    public Toast voiceChatAcceptToast;
    public Toast voiceChatDeacceptToast;

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

    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        GameObject playerObject = PhotonNetwork.Instantiate("Player", transform.position, transform.rotation);
        player = playerObject.GetComponent<NetworkPlayer>();
    }

    #region Voice Chat

    private int voiceTargetID;
    private Transform voiceChatTarget;

    public void RequsetVoiceChat(int receiverID, int senderID)
    {
        photonView.RPC(nameof(RequestVoiceChatRPC), RpcTarget.All, receiverID, senderID);
    }

    [PunRPC]
    public void RequestVoiceChatRPC(int senderID, int recieverID)
    {
        if (NetworkManager.User.id == recieverID)
        {
            voiceChatRequestToast.gameObject.SetActive(true);
            voiceChatRequestToast.SetToastMessage($"{senderID}의 음성 대화 초대");

            voiceChatRequestToast.accept.onClick.AddListener(() => AcceptVoiceChat(senderID, recieverID));
            voiceChatRequestToast.deaccept.onClick.AddListener(() => DeacceptVoiceChat(senderID, recieverID));
        }
    }

    /// <summary>
    /// 로컬에서 1 : 1 음성채팅을 하는 두 사람을 제외한 모든 사람의 AudioSource르 꺼버림
    /// </summary>
    public void AcceptVoiceChat(int senderID, int recieverID)
    {
        photonView.RPC(nameof(AcceptVoiceChatRPC), RpcTarget.All, senderID, recieverID);
        voiceChatRequestToast.gameObject.SetActive(false);
    }

    [PunRPC]
    public void AcceptVoiceChatRPC(int senderID, int recieverID)
    {
        NetworkPlayer sender = FindTargetPlayer(senderID);
        NetworkPlayer reciever = FindTargetPlayer(recieverID);

        sender.OnVoiceChat = true;
        reciever.OnVoiceChat = true;

        if (NetworkManager.User.id == senderID || NetworkManager.User.id == recieverID)
        {
            voiceChatDistanceCheck = StartCoroutine(VoiceChatDistanceCheck(senderID, recieverID));

            for (int i = 0; i < playerTransforms.childCount; i++)
            {
                NetworkPlayer other = playerTransforms.GetChild(i).GetComponent<NetworkPlayer>();

                if (other.UserID != senderID && other.UserID != recieverID)
                {
                    other.VoiceOff();
                }
            }
        }
        else
        {
            sender.VoiceOff();
            reciever.VoiceOff();
        }

        RemoveVoiceChatButtonEvent(recieverID);
    }

    public void DeacceptVoiceChat(int senderID, int recieverID)
    {
        photonView.RPC(nameof(DeacceptVoiceChatRPC), RpcTarget.All, senderID, recieverID);
        voiceChatRequestToast.gameObject.SetActive(false);
    }

    [PunRPC]
    public void DeacceptVoiceChatRPC(int senderID, int recieverID)
    {
        if(NetworkManager.User.id == senderID)
        {
            voiceChatDeacceptToast.gameObject.SetActive(true);
            voiceChatDeacceptToast.SetToastMessage($"{recieverID}가 음성 대화 거절");
        }
        RemoveVoiceChatButtonEvent(recieverID);
    }

    private void RemoveVoiceChatButtonEvent(int targetID)
    {
        if(NetworkManager.User.id == targetID)
        {
            voiceChatRequestToast.accept.onClick.RemoveAllListeners();
            voiceChatRequestToast.deaccept.onClick.RemoveAllListeners();
        }
    }

    public void DisconnectVoiceChat(int senderID, int recieverID)
    {
        photonView.RPC(nameof(DisconnectVoiceChatRPC), RpcTarget.All, senderID, recieverID);
    }

    [PunRPC]
    public void DisconnectVoiceChatRPC(int senderID, int recieverID)
    {
        NetworkPlayer sender = FindTargetPlayer(senderID);
        NetworkPlayer reciever = FindTargetPlayer(recieverID);

        sender.OnVoiceChat = false;
        reciever.OnVoiceChat = false;

        if (NetworkManager.User.id == senderID || NetworkManager.User.id == recieverID)
        {
            if(voiceChatDistanceCheck != null)
            {
                StopCoroutine(voiceChatDistanceCheck);
                voiceChatDistanceCheck = null;
            }
            for (int i = 0; i < playerTransforms.childCount; i++)
            {
                NetworkPlayer other = playerTransforms.GetChild(i).GetComponent<NetworkPlayer>();

                if (other.UserID != senderID && other.UserID != recieverID)
                {
                    if(!other.OnVoiceChat) other.VoiceOn();
                }
            }
        }
        else
        {
            sender.VoiceOff();
            reciever.VoiceOff();
        }
    }

    Coroutine voiceChatDistanceCheck;
    private IEnumerator VoiceChatDistanceCheck(int senderID, int recieverID)
    {
        WaitForFixedUpdate delay = new WaitForFixedUpdate();
        float distance = 0f;
        do
        {
            distance = Vector3.Distance(player.transform.position, voiceChatTarget.position);
            yield return delay;
        }
        while (distance < 5f);

        DisconnectVoiceChat(senderID, recieverID);
    }
    #endregion

    #region Text Chat

    public void SendTextChat(string message)
    {
        photonView.RPC(nameof(SendTextChatRPC), RpcTarget.All, message);
    }

    [PunRPC]
    public void SendTextChatRPC(string message)
    {
        
    }

    #endregion

    private NetworkPlayer FindTargetPlayer(int id)
    {
        for (int i = 0; i < playerTransforms.childCount; i++)
        {
            NetworkPlayer player = playerTransforms.GetChild(i).GetComponent<NetworkPlayer>();
            
            if(player.UserID == id)
            {
                return player;
            }
        }

        return null;
    }
}
