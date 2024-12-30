
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class WeaponPickUp : Interactable
{
    [SerializeField]
    private string[] requiredSequence;

    [SerializeField]
    private float sequenceTimeout = 5f; // time allowed between gestures
    private WeaponPickUpHandTracker handTracker;
    private List<string> currentSequence = new List<string>();
    private float lastGestureTime;

    // Start is called before the first frame update
    void Start()
    {
        if (handTracker == null) handTracker = GetComponent<WeaponPickUpHandTracker>();
        if (handTracker == null) Debug.LogError("missing WeaponPickUpHandTracker component.");
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
        string message = "Pick Up: ";
        for (int i = 0; i < requiredSequence.Length; i++)
        {
            if (i > 0) message += "-";
            if (i < currentSequence.Count)
            {
                message += $"<color=green>{requiredSequence[i]}</color>";
            }
            else if (i == currentSequence.Count)
            {
                if (showProgress && handTracker.GestureToString(handTracker.CurrentGesture) == requiredSequence[i])
                {
                    float progress = handTracker.StabilityProgress;
                    string colorHex = ColorUtility.ToHtmlStringRGB(Color.Lerp(Color.yellow, Color.green, progress));
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
        switch (gesture)
        {
            case 0:
                currentSequence.Add("fist");
                lastGestureTime = Time.time;
                break;
            case 5:
                currentSequence.Add("palm");
                lastGestureTime = Time.time;
                break;
            default:
                return;
        }

        bool isValidSoFar = true;
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
        var number = handTracker.HandleGesture();
        var gesture = handTracker.GestureToString(number);
        return currentSequence.Count < requiredSequence.Length && gesture == requiredSequence[currentSequence.Count];
    }

    protected override void Interact()
    {
        var hoveredWeapon = InteractionManager.Instance.hoveredWeapon;
        if (hoveredWeapon != null)
        {
            Debug.Log("Picked up weapon");
            WeaponManager.Instance.PickupWeapon(hoveredWeapon.gameObject);
            InteractionManager.Instance.hoveredWeapon = null;
        }
    }

    public bool IsReadyForPickup()
    {
        return currentSequence.Count == requiredSequence.Length;
    }

}
