using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public GameObject playerOne, playerTwo;

    [SerializeField]
    private int commonPlayerHP;

    private UIController ui;
    private AudioController audioController;

    [SerializeField]
    private GameObject line;

    public float playerinvulnerabilityTime = 3;

    [SerializeField]
    private GameObject[] enemies;

    [SerializeField]
    private Vector3[] spawnPoints;

    // Make this available in inspector only as a testing tool
    [SerializeField]
    private bool spawnEnemies = true;

    [SerializeField]
    private float spawnEnemyTimer = 5;
    
    public float arenaHeight = 9.85f, arenaWidth = 18.25f;

    [SerializeField]
    private float playerLineDistance = 10;
    [SerializeField]
    private Vector3[] playerSpawnPoints = new Vector3[2];
    [SerializeField]
    private Vector3[] cutscenePlayerSpawnPoints = new Vector3[2];

    [HideInInspector]
    public int score = 0;

    [HideInInspector]
    public bool isPaused = true, canPlayersMove = true;

    [Header("Settings for intro movie display")]
    [SerializeField]
    private float smoothMove = 1.25f;
    public bool playIntroMovie = true;

    [HideInInspector]
    public bool playersInvulnerable = false;

    private void Awake()
    {
        ui = GetComponent<UIController>();
        audioController = GetComponent<AudioController>();
    }

    // Update is called once per frame
    void Update()
    {
        DrawLine();
        if (spawnEnemies) { SpawnRandomEnemy(); }
    }

    private IEnumerator TransferObjectToPosition(GameObject gameObject, Vector3 targetPosition)
    {
        float currentTime = 0.0f;
        var posStart = gameObject.transform.position;
        var posEnd = targetPosition;//playerSpawnPoints[0];

        while (gameObject != null && gameObject.transform.position != posEnd)
        {
            float lerpValue = Mathf.InverseLerp(0, smoothMove, currentTime);
            currentTime += Time.deltaTime;
            gameObject.transform.position = Vector3.Lerp(posStart, posEnd, lerpValue);
            yield return null;
        }
        yield return null;
    }

    private IEnumerator MakeEnemyWait(GameObject gameObject, Vector3 waitPosition, Vector3 targetPosition)
    {
        float currentTime = 0.0f;
        while (currentTime <= 1.5f)
        {
            currentTime += Time.deltaTime;
            gameObject.transform.position = waitPosition;
            yield return null;
        }
        StartCoroutine(TransferObjectToPosition(gameObject, targetPosition));
        yield return new WaitForSeconds(1.5f);
        ui.SetWave(1);
        canPlayersMove = true;
        yield return null;
    }

    //private IEnumerator MovePlayer2ToStartingPositions()
    //{
    //    float currentTime = 0.0f;
    //    float lerpValue = 0.0f;
    //    var posStart = playerTwo.transform.position;
    //    var posEnd = playerSpawnPoints[1];

    //    while (playerTwo.transform.position != posEnd)
    //    {
    //        lerpValue = Mathf.InverseLerp(0, smoothMove, currentTime);
    //        currentTime += Time.deltaTime;
    //        playerTwo.transform.position = Vector3.Lerp(posStart, posEnd, lerpValue);
    //        yield return null;
    //    }
    //    yield return null;
    //}

    //private IEnumerator MoveEnemyToStartingPositions(GameObject enemy, Vector3 targetPosition)
    //{
    //    float currentTime = 0.0f;
    //    float lerpValue = 0.0f;
    //    var posStart = enemy.transform.position;
    //    var posEnd = targetPosition;

    //    while (enemy != null && enemy.transform.position != posEnd)
    //    {
    //        lerpValue = Mathf.InverseLerp(0, smoothMove, currentTime);
    //        currentTime += Time.deltaTime;
    //        enemy.transform.position = Vector3.Lerp(posStart, posEnd, lerpValue);
    //        yield return null;
    //    }
    //    yield return null;
    //}

    public void StartGame()
    {
        audioController.PlayMusic();
        score = 0;
        ui.SetScore(score);

        spawnEnemies = false;
        commonPlayerHP = 10;
        var enems = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enems)
        {
            Destroy(enemy);
        }
        if (playIntroMovie)
        {
            ui.SetCinematicBars(true);
            canPlayersMove = false;
            //spawn players on bottom, move them to the starting positions
            playerOne.transform.position = cutscenePlayerSpawnPoints[0];
            playerTwo.transform.position = cutscenePlayerSpawnPoints[1];
            StartCoroutine(TransferObjectToPosition(playerOne, playerSpawnPoints[0]));
            StartCoroutine(TransferObjectToPosition(playerTwo, playerSpawnPoints[1]));

            var introEnemy = Instantiate(enemies[2], spawnPoints[3], Quaternion.identity);
            StartCoroutine(MakeEnemyWait(introEnemy, spawnPoints[3], new Vector3(-3, 0, 0)));

        }
        else
        {
            playerOne.transform.position = playerSpawnPoints[0];
            playerTwo.transform.position = playerSpawnPoints[1];
            SetEnemySpawn(true);
        }
        isPaused = false;
    }

    public int GetCommonPlayerHp()
    {
        return commonPlayerHP;
    }

    public void SetEnemySpawn(bool spawn)
    {
        StartCoroutine(WaitAndStartSpawningEnemies(spawn));
    }

    private IEnumerator WaitAndStartSpawningEnemies(bool spawn)
    {
        yield return new WaitForSeconds(1.5f);
        spawnEnemies = spawn;
    }

    public void AddScore(int enemyScore)
    {
        score += enemyScore;
        ui.SetScore(score);
    }

    private void SpawnRandomEnemy()
    {
        int enemyId = Random.Range(0, enemies.Length);
        var enemy = enemies[enemyId];

        int positionId = Random.Range(0, spawnPoints.Length);
        Vector3 randomSpawnPosition = spawnPoints[positionId];

        Instantiate(enemy, randomSpawnPosition, Quaternion.identity);
        spawnEnemies = false;
        StartCoroutine(SpawnWait());
    }

    // TO-DO: probably want to pass dealt damage as a parameter
    public void PlayerTakesDamage()
    {
        commonPlayerHP--;
        if (commonPlayerHP == 0)
        {
            ui.Defeat();
        }
        else
        {
            ui.commonPlayerHPText.text = "HP x" + commonPlayerHP;
        }
    } 

    private void DrawLine()
    {
        Vector2 player1Pos = playerOne.gameObject.transform.position;
        Vector2 player2Pos = playerTwo.gameObject.transform.position;

        Vector2 centre = new Vector2((player1Pos.x + player2Pos.x) / 2, (player1Pos.y + player2Pos.y) / 2);
        float distance = Vector2.Distance(player1Pos, player2Pos);
        float angle = Mathf.Atan2(player1Pos.y - player2Pos.y, player1Pos.x - player2Pos.x) * Mathf.Rad2Deg;

        if (distance <= playerLineDistance)
        {
            line.SetActive(true);
            // set position, scale of y and rotation of z
            line.transform.position = centre;
            line.transform.localScale = new Vector3(line.transform.localScale.x, distance, line.transform.localScale.z);
            line.transform.rotation = Quaternion.Euler(0, 0, angle + 90);
        }
        else
        {
            line.SetActive(false);
        }

    }

    private IEnumerator SpawnWait()
    {
        float currentTime = 0.0f;
        do
        {
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= spawnEnemyTimer);
        spawnEnemies = true;
    }
}
