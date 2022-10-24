using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitArea : MonoBehaviour
{
    public RoomState_GoToLoundge roomState;

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("NetworkPlayerRoom"))
        {
            NetworkPlayer player =other.GetComponentInParent<NetworkPlayer>();

            if(player.UserID.Equals(NetworkManager.User.email))
            {
                roomState.LeaveRoom();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
