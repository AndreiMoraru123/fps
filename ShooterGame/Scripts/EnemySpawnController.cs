using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    public int waveMultiplier = 1;
    public int initialEnemiesPerWave = 5;
    public int currentEnemiesPerWave;

    public float spawnDelay = 0.5f;

    public int currentWave = 0;
    public float waveCoolDown = 10.0f;

    public bool inCoolDown;
    public float coolDownCounter = 0f;

    public List<Enemy> currentEnemiesAlive;
    public GameObject enemyPrefab;
    public TextMeshProUGUI coolDownCounterUI;

    // Start is called before the first frame update
    void Start()
    {
        currentEnemiesPerWave = initialEnemiesPerWave;
        GlobalReferences.Instance.waveNumber = 0;
        StartNextWave();
    }

    private void StartNextWave()
    {
        currentEnemiesAlive.Clear();
        currentWave++;
        GlobalReferences.Instance.waveNumber = currentWave;
        PlayerUI.Instance.UpdateText("Wave: " + currentWave.ToString());
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        for (var i = 0; i < currentEnemiesPerWave; i++)
        {
            var spawnOffset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            var spawnPosition = transform.position + spawnOffset;

            var enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            var enemyScript = enemy.GetComponent<Enemy>();

            currentEnemiesAlive.Add(enemyScript);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    // Update is called once per frame
    void Update()
    {
        var enemiesToRemove = new List<Enemy>();
        foreach (var enemy in currentEnemiesAlive)
        {
            if (enemy.isDead)
            {
                enemiesToRemove.Add(enemy);
            }
        }

        foreach (var enemy in enemiesToRemove)
        {
            currentEnemiesAlive.Remove(enemy);
        }
        enemiesToRemove.Clear();

        if (currentEnemiesAlive.Count == 0 && !inCoolDown)
        {
            StartCoroutine(WaveCoolDown());
        }

        if (inCoolDown) // run the cool down counter
        {
            coolDownCounter -= Time.deltaTime;
        }
        else // reset the cool down counter
        {
            coolDownCounter = waveCoolDown;
        }
        coolDownCounterUI.text = coolDownCounter.ToString("F0");
    }

    private IEnumerator WaveCoolDown()
    {
        inCoolDown = true;
        PlayerUI.Instance.UpdateText("Wave Over\n Prepare for Next Wave");
        coolDownCounterUI.gameObject.SetActive(true);

        yield return new WaitForSeconds(waveCoolDown);

        inCoolDown = false;
        PlayerUI.Instance.UpdateText(string.Empty);
        coolDownCounterUI.gameObject.SetActive(false);

        currentEnemiesPerWave *= waveMultiplier;
        StartNextWave();
    }
}
