using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera cam;

    [SerializeField]
    private float distance = 3f;

    [SerializeField]
    private LayerMask mask;
    private InputManager inputManager;
    private string lastPromptMessage = string.Empty;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<PlayerLook>().cam;
        inputManager = GetComponent<InputManager>();
        mask = LayerMask.GetMask("Interactable", "Weapon");
    }

    // Update is called once per frame
    void Update()
    {
        var ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance);
        RaycastHit hitInfo;

        // Only update the UI if I need to
        if (Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                var interactable = hitInfo.collider.GetComponent<Interactable>();

                if (lastPromptMessage != interactable.promptMessage)
                {
                    PlayerUI.Instance.UpdateText(interactable.promptMessage);
                    lastPromptMessage = interactable.promptMessage;
                }

                if (inputManager.onFoot.Interact.triggered)
                {
                    interactable.BaseInteract();
                }
            }
        }
        else if (lastPromptMessage != string.Empty)
        {
            PlayerUI.Instance.UpdateText(string.Empty);
            lastPromptMessage = string.Empty;
        }
    }
}
