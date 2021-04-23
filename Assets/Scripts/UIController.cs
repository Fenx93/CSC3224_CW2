using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text commonPlayerHPText;

    [SerializeField] private Text scoreText, deathScoreText, waveText;

    [SerializeField] private GameObject startScreen, defeatScreen, menuScreen, helpScreen;
    
    [SerializeField] private GameObject topBar, bottomBar;

    [SerializeField] private GameObject goBackButton, continueButton, retryButton;

    private GameplayController gameController;
    private AudioController audioController;

    //private bool isInStartMenu;

    private void Awake()
    {
        gameController = GetComponent<GameplayController>();
        audioController = GetComponent<AudioController>();
    }

    void Start()
    {
        menuScreen.SetActive(false);
        defeatScreen.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKey("escape"))
        {
            OpenMenu();
        }
    }

    public void SetScore(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void SetWave(int wave)
    {
        waveText.enabled = true;
        waveText.text = "Wave " + wave;
        StartCoroutine(FadeTextToZeroAlpha( 1.5f, waveText));
        gameController.SetEnemySpawn(true);
        SetCinematicBars(false);
    }

    public void SetCinematicBars(bool enabled)
    {
        topBar.SetActive(enabled);
        bottomBar.SetActive(enabled);
    }

    private IEnumerator FadeTextToZeroAlpha(float time, Text text)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / time));
            yield return null;
        }
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        text.enabled = false;
    }

    public void Defeat()
    {
        scoreText.enabled = false;
        gameController.PauseGame();
        deathScoreText.text = "Your Score is: "+gameController.score;
        commonPlayerHPText.enabled = false;
        defeatScreen.SetActive(true);
    }

    public void Restart()
    {
        scoreText.enabled = true;
        commonPlayerHPText.enabled = true;
        defeatScreen.SetActive(false);
        menuScreen.SetActive(false);
        gameController.playIntroMovie = false;
        gameController.StartGame();
    }

    public void SetPlayerInvulnerability(bool invulnerable)
    {
        gameController.playersInvulnerable = invulnerable;
        commonPlayerHPText.text = invulnerable ? 
            "HP ∞" : "HP x" + gameController.GetCommonPlayerHp();
    }

    #region StartGameMenu
    public void OpenStartMenu()
    {
        audioController.PlayMenuMusic();
        startScreen.SetActive(true);
        menuScreen.SetActive(false);
        defeatScreen.SetActive(false);
    }

    public void OpenHelpMenu(bool open)
    {
        helpScreen.SetActive(open);
    }

    public void OpenStartOptionsMenu(bool open)
    {
        startScreen.SetActive(!open);
        menuScreen.SetActive(open);
        goBackButton.SetActive(open);
        continueButton.SetActive(!open);
        retryButton.SetActive(!open);
    }

    public void StartGame()
    {
        startScreen.SetActive(false);
        gameController.StartGame();
    }
    #endregion

    private void OpenMenu()
    {
        menuScreen.SetActive(true);
        gameController.PauseGame();
        continueButton.SetActive(true);
    }

    public void Continue()
    {
        menuScreen.SetActive(false);
        continueButton.SetActive(false);
        gameController.OnResumeGame();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
