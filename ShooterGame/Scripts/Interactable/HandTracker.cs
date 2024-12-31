
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class HandTracker : MonoBehaviour
{
    [SerializeField]
    private OAKForUnity.UBHandTracking handTracking;

    [SerializeField]
    protected float requiredStableTime = 0.5f; // how long to hold a gesture
    protected float gestureStableTime = 0f;
    protected int lastRecognizedGesture = -1;

    public int CountFingers(JSONNode hand)
    {
        if (hand == null) return -1;
        if (hand["gesture"] == "FIST") return 0;
        if (hand["gesture"] == "OK" || hand["gesture"] == "ONE") return 1;
        if (hand["gesture"] == "PEACE" || hand["gesture"] == "TWO") return 2;
        if (hand["gesture"] == "THREE") return 3;
        if (hand["gesture"] == "FOUR") return 4;
        if (hand["gesture"] == "FIVE") return 5;
        return -1;
    }

    public string NumberToString(int gesture)
    {
        return gesture switch
        {
            0 => "FIST",
            5 => "PALM",
            _ => "INVALID",
        };

    }

    public float StabilityProgress
    {
        get
        {
            if (lastRecognizedGesture == -1) return 0f;
            return Mathf.Clamp01((Time.time - gestureStableTime) / requiredStableTime);
        }
    }

    public int CurrentGesture => lastRecognizedGesture;
    public int CountFingersOnHand()
    {
        try
        {
            if (string.IsNullOrEmpty(handTracking.ubHandTrackingResults)) return -1;

            var json = JSON.Parse(handTracking.ubHandTrackingResults);
            var hand0 = json["hand_0"];
            var hand1 = json["hand_1"];

            if (hand0 == null && hand1 == null) return -1;

            var activeHand = hand0 ?? hand1;

            return CountFingers(activeHand);
        }
        catch // Throwables
        {
            print("Caught!");
            return -1;
        }

    }
    public bool TryGetStableGesture(out int number)
    {
        number = CountFingersOnHand();

        if (number == lastRecognizedGesture && number != -1)
        {
            if (Time.time - gestureStableTime >= requiredStableTime)
            {
                gestureStableTime = Time.time;
                return true;
            }
        }
        else
        {
            lastRecognizedGesture = number;
            gestureStableTime = Time.time;
        }

        return false;
    }

}
