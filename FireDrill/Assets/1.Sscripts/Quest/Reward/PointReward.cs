using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Reward/Point", fileName = "PointReward")]
public class PointReward : Reward
{
    public override void Give(Quest quest)
    {
        PlayerPrefs.SetInt("bounusScore", Quantity);
        PlayerPrefs.Save();
    }
}
