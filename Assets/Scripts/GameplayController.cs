using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public GameObject playerOne, playerTwo;

    [SerializeField] private int startingCommonPlayerHP;
    private int commonPlayerHP;

    private UIController ui;
    private AudioController audioController;

    [SerializeField] private GameObject line;

    public float playerinvulnerabilityTime = 3;

    [SerializeField] private GameObject[] enemies;

    [SerializeField] private Vector3[] spawnPoints;

    // Make this available in inspector only as a testing tool
    [SerializeField] private bool spawnEnemies = true;

    [SerializeField] private float spawnEnemyTimer = 5;
    
    public float arenaHeight = 9.85f, arenaWidth = 18.25f;

    [SerializeField] private float playerLineDistance = 10;
    [SerializeField] private Vector3[] playerSpawnPoints = new Vector3[2];
    [SerializeField] private Vector3[] cutscenePlayerSpawnPoints = new Vector3[2];

    [HideInInspector] public int score = 0;
    [HideInInspector] public bool isPaused = true, canPlayersMove = true;

    [Header("Settings for intro movie display")]
    [SerializeField] private float smoothMove = 1.25f;
    public bool playIntroMovie = true;

    [HideInInspector] public bool playersInvulnerable = false;
    private bool playingIntro;

    private Vector3 savedVelocity, savedVelocity1;
    private float savedAngularVelocity, savedAngularVelocity1;
    private GameObject introEnemy;
    private int currentWave = 0;

    public int CommonPlayerHp => commonPlayerHP;

    private void Awake()
    {
        ui = GetComponent<UIController>();
        audioController = GetComponent<AudioController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playingIntro && Input.GetKey(KeyCode.Space))
            SkipIntro(introEnemy);
        DrawLine();
        if (spawnEnemies && !isPaused) 
            SpawnRandomEnemy();
    }

    #region StartAndIntro
    public void StartGame()
    {
        audioController.PlayMusic();
        score = 0;
        ui.SetScore(score);

        spawnEnemies = false;
        commonPlayerHP = startingCommonPlayerHP;
        ui.commonPlayerHPText.text = "HP x" + commonPlayerHP;

        var enems = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enems)
        {
            Destroy(enemy);
        }
        if (playIntroMovie)
        {
            playingIntro = true;
            playersInvulnerable = true;
            canPlayersMove = false;
            ui.SetCinematicBars(true);
            //spawn players on bottom, move them to the starting positions
            playerOne.transform.position = cutscenePlayerSpawnPoints[0];
            playerTwo.transform.position = cutscenePlayerSpawnPoints[1];
            StartCoroutine(TransferObjectToPosition(playerOne, playerSpawnPoints[0]));
            StartCoroutine(TransferObjectToPosition(playerTwo, playerSpawnPoints[1]));

            introEnemy = Instantiate(enemies[2], spawnPoints[3], Quaternion.identity);
            StartCoroutine(MakeEnemyWait(introEnemy, spawnPoints[3], new Vector3(-3, 0, 0)));
        }
        else
        {
            canPlayersMove = true;
            playerOne.transform.position = playerSpawnPoints[0];
            playerTwo.transform.position = playerSpawnPoints[1];
            SetEnemySpawn(true);
        }
        ResumeGame();
        isPaused = false;
    }

    private void SkipIntro(GameObject enemyObject)
    {
        playingIntro = false;
        Destroy(enemyObject.gameObject);
        ui.SetWave(1);
        playerOne.transform.position = playerSpawnPoints[0];
        playerTwo.transform.position = playerSpawnPoints[1];
        canPlayersMove = true;
        playersInvulnerable = false;
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
        while (currentTime <= 1.5f && gameObject!= null)
        {
            currentTime += Time.deltaTime;
            gameObject.transform.position = waitPosition;
            yield return null;
        }
        StartCoroutine(TransferObjectToPosition(gameObject, targetPosition));
        yield return new WaitForSeconds(1.5f);
        ui.SetWave(1);
        canPlayersMove = true;
        playingIntro = false;
        playersInvulnerable = false;
        yield return null;
    }
    #endregion

    public void PauseGame()
    {
        isPaused = true;
        //save player movement
        var rigidbody = playerOne.gameObject.GetComponent<Rigidbody2D>();
        savedVelocity = rigidbody.velocity;
        savedAngularVelocity = rigidbody.angularVelocity;
        rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;

        var rigidbody1 = playerTwo.gameObject.GetComponent<Rigidbody2D>();
        savedVelocity1 = rigidbody1.velocity;
        savedAngularVelocity1 = rigidbody1.angularVelocity;
        rigidbody1.constraints = RigidbodyConstraints2D.FreezeAll;

        canPlayersMove = false;
    }

    public void ResumeGame()
    {
        isPaused = false;

        var rigidbody = playerOne.gameObject.GetComponent<Rigidbody2D>();
        rigidbody.constraints = RigidbodyConstraints2D.None;
        rigidbody.AddForce(savedVelocity, ForceMode2D.Force);
        rigidbody.AddTorque(savedAngularVelocity, ForceMode2D.Force);

        var rigidbody1 = playerTwo.gameObject.GetComponent<Rigidbody2D>();
        rigidbody1.constraints = RigidbodyConstraints2D.None;
        rigidbody1.AddForce(savedVelocity1, ForceMode2D.Force);
        rigidbody1.AddTorque(savedAngularVelocity1, ForceMode2D.Force);

        canPlayersMove = true;
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
        score += (enemyScore);
        ui.SetScore(score);
        if (score / 5000 > currentWave)
        {
            currentWave++;
            ui.SetWave(currentWave + 1);
        }
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
