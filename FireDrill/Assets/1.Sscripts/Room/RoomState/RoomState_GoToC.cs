using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomState_GoToC : RoomState
{
    public override void OnStateEnter()
    {
        base.OnStateEnter();
        Debug.Log("End");
    }
}
