
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class WeaponPickUpHandTracker : HandTracker
{
    [SerializeField]
    private OAKForUnity.UBHandTracking handTracking;

    void Awake()
    {
        requiredStableTime = 2.5f;
    }

    public override int HandleGesture()
    {
        if (string.IsNullOrEmpty(handTracking.ubHandTrackingResults)) return -1;

        var json = JSON.Parse(handTracking.ubHandTrackingResults);
        var hand0 = json["hand_0"];
        var hand1 = json["hand_1"];

        if (hand0 == null && hand1 == null) return -1;

        var activeHand = hand0 ?? hand1;

        var numFingers = CountFingers(activeHand);
        return numFingers;

    }
}
