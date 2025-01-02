using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private float health;
    private float lerpTimer;

    [SerializeField]
    public float maxHealth = 100f;

    [SerializeField]
    public float chipSpeed = 2f;

    [SerializeField]
    public Image frontHealthBar;

    [SerializeField]
    public Image backHealthBar;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
    }

    public void UpdateHealthUI()
    {
        float fillFront = frontHealthBar.fillAmount;
        float fillBack = backHealthBar.fillAmount;
        float healthFraction = health / maxHealth;

        if (fillBack > healthFraction)
        {
            frontHealthBar.fillAmount = healthFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;

            float percentComplete = lerpTimer / chipSpeed;
            percentComplete *= percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillBack, healthFraction, percentComplete);
        }

        if (fillFront < healthFraction)
        {
            backHealthBar.fillAmount = healthFraction;
            backHealthBar.color = Color.green;
            lerpTimer += Time.deltaTime;

            float percentComplete = lerpTimer / chipSpeed;
            percentComplete *= percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillFront, backHealthBar.fillAmount, percentComplete);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            print("player dead");
        }
        else
        {
            print("player hit");
        }

        lerpTimer = 0f;
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
            TakeDamage(other.gameObject.GetComponent<XBotHand>().damage);
        }
    }
}
