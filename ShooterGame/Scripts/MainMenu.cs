using TMPro;
using SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class MainMenu : MonoBehaviour
{
    public TMP_Text highScoreUI;
    public TMP_Text deviceUI;
    string newGameScene = "ShooterOAK";
    string devicesJson = "";
    string devices = "";

    void Start()
    {
        var highScore = SaveLoadManager.Instance.LoadHighScore();
        highScoreUI.text = $"Top Wave Survived: {highScore}";
        deviceUI.text = "Device ID: 0\nDevice State: Unknown\nDevice Name: Unknown\n";
    }

    public void CheckDevices()
    {
        devices = deviceUI.text;
        devicesJson = Marshal.PtrToStringAnsi(DeviceManager.GetAllDevices());

        if (string.IsNullOrEmpty(devicesJson) || devicesJson == "null")
        {
            devices = "No devices found.\n";
        }
        else
        {
            var obj = JSON.Parse(devicesJson);
            devices = "";
            foreach (JSONNode arr in obj)
            {
                string deviceId = arr["deviceId"];
                string deviceName = arr["deviceName"];
                string deviceState = arr["deviceState"];

                // build line with device information
                devices += $"Device ID: {deviceId}\nDevice State: {deviceState}\nDevice Name: {deviceName}\n\n";
            }
        }
        deviceUI.text = devices;
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene(newGameScene);
    }

    public void ExitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}