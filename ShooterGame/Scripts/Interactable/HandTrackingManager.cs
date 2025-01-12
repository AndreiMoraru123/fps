
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using OAKForUnity;

public class HandTrackingManager : MonoBehaviour
{
    [SerializeField]
    private HandTrackingPipeline pipeline;

    [SerializeField]
    protected float requiredStableTime = 0.5f; // how long to hold a gesture
    protected float gestureStableTime = 0f;
    protected int lastRecognizedGesture = -1;

    private static readonly Dictionary<string, int> GestureToNumber = new() {
        {"FIST", 0},
        {"OK", 1},
        {"ONE", 1},
        {"PEACE", 2},
        {"TWO", 2},
        {"THREE", 3},
        {"FOUR", 4},
        {"FIVE", 5},
    };

    public int CountFingers(JSONNode hand)
    {
        if (hand == null || hand["gesture"] == null) return -1;
        return GestureToNumber.TryGetValue(hand["gesture"], out var count) ? count : -1;
    }

    public string GetGestureFromNumber(int number)
    {
        return number switch
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
        if (pipeline != null)
        {
            if (string.IsNullOrEmpty(pipeline.handTrackingResults)) return -1;

            var json = JSON.Parse(pipeline.handTrackingResults);
            var hand0 = json["hand_0"];
            var hand1 = json["hand_1"];

            if (hand0 == null && hand1 == null) return -1;

            var activeHand = hand0 ?? hand1;

            return CountFingers(activeHand);
        }
        else // Throwables
        {
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
