using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class KeypadHandTracker : MonoBehaviour
{
    [SerializeField]
    private OAKForUnity.UBHandTracking handTracking;

    [SerializeField]
    private float requiredStableTime = 0.5f; // how long to hold a gesture
    private float gestureStableTime = 0f;
    private int lastRecognizedGesture = -1;

    public float StabilityProgress
    {
        get
        {
            if (lastRecognizedGesture == -1) return 0f;
            return Mathf.Clamp01((Time.time - gestureStableTime) / requiredStableTime);
        }
    }

    public int CurrentGesture => lastRecognizedGesture;

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

    public int HandleGesture()
    {
        if (string.IsNullOrEmpty(handTracking.ubHandTrackingResults)) return -1;

        var json = JSON.Parse(handTracking.ubHandTrackingResults);
        var hand0 = json["hand_0"];
        var hand1 = json["hand_1"];

        if (hand0 == null && hand1 == null) return -1;

        var activeHand = (hand0 != null) ? hand0 : hand1;

        var numFingers = CountFingers(activeHand);
        return numFingers;

    }

    public bool TryGetStableGesture(out int gesture)
    {
        gesture = HandleGesture();

        if (gesture == lastRecognizedGesture && gesture != -1)
        {
            if (Time.time - gestureStableTime >= requiredStableTime)
            {
                gestureStableTime = Time.time;
                return true;
            }
        }
        else
        {
            lastRecognizedGesture = gesture;
            gestureStableTime = Time.time;
        }

        return false;
    }
}
