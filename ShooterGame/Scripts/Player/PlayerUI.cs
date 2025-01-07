using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    // Singleton pattern
    public static PlayerUI Instance { get; set; }

    [SerializeField]
    private TextMeshProUGUI promptText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /* EnemySpawnController.StartNextWave will set this instead
    void Start()
    {
        if (promptText != null)
        {
            promptText.text = string.Empty;
        }
    }
    */

    public void UpdateText(string promptMessage)
    {
        promptText.text = promptMessage;
    }
}
