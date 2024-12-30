
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public abstract class HandTracker : MonoBehaviour
{

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

    public string GestureToString(int gesture)
    {
        switch (gesture)
        {
            case 0:
                return "palm";
            case 5:
                return "fist";
            default:
                return "invalid";
        }
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
    public abstract int HandleGesture();
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
