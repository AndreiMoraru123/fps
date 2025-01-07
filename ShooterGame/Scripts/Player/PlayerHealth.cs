using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private float health;
    private float lerpTimer;
    public bool isDead;
    public float maxHealth = 100f;
    public float chipSpeed = 2f;
    public Image frontHealthBar;
    public Image backHealthBar;
    public GameObject bloodyScreen;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthBarUI();
    }

    public void UpdateHealthBarUI()
    {
        var fillFront = frontHealthBar.fillAmount;
        var fillBack = backHealthBar.fillAmount;
        var healthFraction = health / maxHealth;

        if (fillBack > healthFraction)
        {
            frontHealthBar.fillAmount = healthFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;

            var percentComplete = lerpTimer / chipSpeed;
            percentComplete *= percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillBack, healthFraction, percentComplete);
        }

        if (fillFront < healthFraction)
        {
            backHealthBar.fillAmount = healthFraction;
            backHealthBar.color = Color.green;
            lerpTimer += Time.deltaTime;

            var percentComplete = lerpTimer / chipSpeed;
            percentComplete *= percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillFront, backHealthBar.fillAmount, percentComplete);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            PlayerDead();
            isDead = true;
        }
        else
        {
            StartCoroutine(BloodyScreenEffect());
        }

        lerpTimer = 0f;
    }

    private void PlayerDead()
    {
        GetComponent<InputManager>().enabled = false;
        GetComponentInChildren<Animator>().enabled = true;
        StartCoroutine(ShowGameOverUI());
        GetComponent<ScreenBlackout>().StartFade();
    }

    private IEnumerator ShowGameOverUI()
    {
        PlayerUI.Instance.UpdateText("GAME OVER");
        yield return new WaitForSeconds(0.5f);

        var waveSurvived = GlobalReferences.Instance.waveNumber - 1;
        if (waveSurvived > SaveLoadManager.Instance.LoadHighScore())
        {

            SaveLoadManager.Instance.SaveHighScore(waveSurvived);
        }

        StartCoroutine(ReturnToMainMenu());
    }

    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("MainMenu");
    }


    private IEnumerator BloodyScreenEffect()
    {
        if (bloodyScreen.activeInHierarchy == false)
        {
            bloodyScreen.SetActive(true);
        }

        yield return StartCoroutine(BloodyFadeEffect());

        if (bloodyScreen.activeInHierarchy == true)
        {
            bloodyScreen.SetActive(false);
        }
    }

    private IEnumerator BloodyFadeEffect(float startingAlpha = 0.5f)
    {
        var image = bloodyScreen.GetComponentInChildren<Image>();
        var startColor = image.color;
        startColor.a = startingAlpha;
        image.color = startColor;

        var duration = 2f;
        var elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            var alpha = Mathf.Lerp(startingAlpha, 0f, elapsedTime / duration);

            var newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;

            elapsedTime += Time.deltaTime;

            yield return null; // wait for next frame
        }
    }


    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("XBotAttackHand"))
        {
            if (isDead == false)
            {
                TakeDamage(other.gameObject.GetComponent<XBotHand>().damage);
            }
        }
    }
}
