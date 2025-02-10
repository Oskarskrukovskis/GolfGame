using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class GameStateManager : MonoBehaviour
{
    //Components
    public CanvasGroup startMenuCanvas;
    public Transform buttonImage;

    //Animation variables
    public float startMenuFadeDuration = 1;
    public float gameOverElementDisappearTime = 0.5f;
    public float timeUntilRestart = 0.5f;

    private static GameStateManager instance;
    public static GameStateManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType(typeof(GameStateManager)) as GameStateManager;

            return instance;
        }
        set
        {
            instance = value;
        }
    }


    public void StartGame()
    {
        StartCoroutine(StartGameCo());
    }

    IEnumerator StartGameCo()
    {
        //Play animation of the main menu going out and disable it after
        startMenuCanvas.DOFade(0, startMenuFadeDuration);
        buttonImage.DOScale(0, startMenuFadeDuration);

        yield return new WaitForSeconds(startMenuFadeDuration);

        startMenuCanvas.gameObject.SetActive(false);
        AudioManager.Instance.generalAudioSource.PlayOneShot(AudioManager.Instance.startGameSound);

        SetupStatesAndValuesForNewGame();
    }

    //Sets up states and values for a new game.
    private static void SetupStatesAndValuesForNewGame()
    {
        TimerManager.Instance.isTimerRunning = true;
        FlashingManager.Instance.ResetTimerAndStart();
        TimerManager.Instance.RestartTimer();
        Ball.Instance.ShowBallIsReady();
    }

    public void EndGame()
    {
        StartCoroutine(EndGameCo());
    }

    IEnumerator EndGameCo()
    {
        
        InitiategameOverEventForgameObjects();
        AudioManager.Instance.generalAudioSource.PlayOneShot(AudioManager.Instance.gameOverSound);

        yield return new WaitForSeconds(gameOverElementDisappearTime);

        //Resets the score
        ScoreManager.Instance.scoreCount = 0;
        ScoreManager.Instance.UpdateScoreUi();

        //Moves player to the center
        Ball.Instance.gameObject.transform.DOMove(Vector3.zero, timeUntilRestart).SetEase(Ease.OutBounce);

        yield return new WaitForSeconds(timeUntilRestart);

        //Spawns new coins and holes.
        ObjectSpawnerManager.Instance.SpawnCoins();
        ObjectSpawnerManager.Instance.SpawnHoles();

        yield return new WaitForSeconds(ObjectSpawnerManager.Instance.spawnanimationTime);

        //Resets the timer
        TimerManager.Instance.RestartTimer();

        //Resets the ball to its default values.
        Ball.Instance.gameOver = false;
        Ball.Instance.currentlyInHole = false;

    }

    //Resets the player and gets rid of the the current coins and holes.
    private void InitiategameOverEventForgameObjects()
    {
        Ball.Instance.ResetBallAfterGameOver();
        foreach (Hole hole in FindObjectsByType<Hole>(FindObjectsSortMode.None))
        {
            hole.InitiategameOverEvent(gameOverElementDisappearTime);
        }
        foreach (Coin coin in FindObjectsByType<Coin>(FindObjectsSortMode.None))
        {
            coin.InitiategameOverEvent(gameOverElementDisappearTime);
        }
    }
}
