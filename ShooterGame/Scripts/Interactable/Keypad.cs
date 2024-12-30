using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class Keypad : Interactable
{

    [SerializeField]
    private GameObject door;

    [SerializeField]
    private int[] requiredSequence;

    [SerializeField]
    private float sequenceTimeout = 5f; // time allowed between gestures
    private HandTracker handTracker;
    private bool doorOpen;
    private List<int> currentSequence = new List<int>();
    private float lastGestureTime;

    // Start is called before the first frame update
    void Start()
    {
        if (handTracker == null) handTracker = GetComponent<HandTracker>();
        if (handTracker == null) Debug.LogError("missing HandTracker component.");
        UpdatePromptMessage();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentSequence.Count > 0 && Time.time - lastGestureTime > sequenceTimeout)
        {
            ResetSequence();
        }

        if (handTracker.CurrentGesture != -1)
        {
            UpdatePromptMessage(true);
        }

        if (handTracker.TryGetStableGesture(out int gesture))
        {
            HandleNewGesture(gesture);
        }
    }

    private void UpdatePromptMessage(bool showProgress = false)
    {
        var message = "Code: ";
        for (int i = 0; i < requiredSequence.Length; i++)
        {
            if (i > 0) message += "-";
            if (i < currentSequence.Count)
            {
                message += $"<color=green>{requiredSequence[i]}</color>";
            }
            else if (i == currentSequence.Count)
            {
                if (showProgress && handTracker.CurrentGesture == requiredSequence[i])
                {
                    var progress = handTracker.StabilityProgress;
                    var colorHex = ColorUtility.ToHtmlStringRGB(Color.Lerp(Color.yellow, Color.green, progress));
                    message += $"<color=#{colorHex}>{requiredSequence[i]}</color>";
                }
                else
                {
                    message += $"<color=red>{requiredSequence[i]}</color>";
                }
            }
            else
            {
                message += requiredSequence[i].ToString();
            }
        }
        promptMessage = message;
    }

    public void HandleNewGesture(int gesture)
    {
        if (gesture >= 0)
        {
            currentSequence.Add(gesture);
            lastGestureTime = Time.time;
        }

        var isValidSoFar = true;
        for (int i = 0; i < currentSequence.Count && i < requiredSequence.Length; i++)
        {
            if (currentSequence[i] != requiredSequence[i])
            {
                isValidSoFar = false;
                break;
            }
        }

        if (!isValidSoFar)
        {
            ResetSequence();
            return;
        }

        UpdatePromptMessage();

        if (currentSequence.Count == requiredSequence.Length)
        {
            Interact();
            ResetSequence();
        }
    }

    private void ResetSequence()
    {
        currentSequence.Clear();
        lastGestureTime = 0f;
        UpdatePromptMessage();
    }

    public override bool ValidateInteraction()
    {
        var number = handTracker.CountFingersOnHand();
        return currentSequence.Count < requiredSequence.Length && number == requiredSequence[currentSequence.Count];
    }

    protected override void Interact()
    {
        doorOpen = !doorOpen;
        door.GetComponent<Animator>().SetBool("IsOpen", doorOpen);
    }

}
