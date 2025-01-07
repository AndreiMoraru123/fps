using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMP_Text highScoreUI;
    string newGameScene = "ShooterOAK";

    void Start()
    {
        var highScore = SaveLoadManager.Instance.LoadHighScore();
        highScoreUI.text = $"Top Wave Survived: {highScore}";
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