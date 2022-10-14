using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChatServerProtocol
{
    public void GetUser(string userEmail);
}
