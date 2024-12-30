
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class WeaponPickUp : Interactable
{
    [SerializeField]
    private string requiredGesture;

    [SerializeField]
    private float sequenceTimeout = 3.5f; // time allowed between gestures
    private HandTracker handTracker;
    private string shownGesture;
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
        if (Time.time - lastGestureTime > sequenceTimeout)
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
        var message = $"Pick up: <color=red>{requiredGesture}</color>";
        if (showProgress && handTracker.NumberToString(handTracker.CurrentGesture) == requiredGesture)
        {
            var progress = handTracker.StabilityProgress;
            var colorHex = ColorUtility.ToHtmlStringRGB(Color.Lerp(Color.yellow, Color.green, progress));
            message = $"Pick up: <color=#{colorHex}>{requiredGesture}</color>";
        }
        promptMessage = message;
    }

    public void HandleNewGesture(int gesture)
    {
        switch (gesture)
        {
            case 0:
                shownGesture = "fist";
                lastGestureTime = Time.time;
                break;
            case 5:
                shownGesture = "palm";
                lastGestureTime = Time.time;
                break;
            default:
                return;
        }

        var isValid = true;
        if (shownGesture != requiredGesture)
        {
            isValid = false;
        }

        if (!isValid)
        {
            ResetSequence();
            return;
        }

        UpdatePromptMessage();
        Interact();
        ResetSequence();
    }

    private void ResetSequence()
    {
        shownGesture = null;
        lastGestureTime = 0f;
        UpdatePromptMessage();
    }

    public override bool ValidateInteraction()
    {
        var number = handTracker.CountFingersOnHand();
        var gesture = handTracker.NumberToString(number);
        return gesture == requiredGesture;
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

}
