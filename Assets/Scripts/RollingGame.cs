using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RollingGame : MonoBehaviour {

    [Tooltip("The next level to be loaded")]
    [SerializeField]
    private string nextScene;

    [Tooltip("This scene to be loaded on loss")]
    [SerializeField]
    private string thisScene;

    [Tooltip("The number of items to pick up to win game.")]
    [SerializeField]
    private int numPickups = 5;

    [Tooltip("The player ball")]
    [SerializeField]
    private GameObject player;

    [Tooltip("The canvas giving the player feedback")]
    [SerializeField]
    private FeedbackCanvas feedbackCanvas;

    // The number of pickups collected so far in the game.
    private int numPickupsCollected = 0;

    // An enum denoting whether the game is starting, in progress, or over
    public enum GameState { PRE_GAME, GAME, POST_GAME }

    // The current state of the game
    private GameState curGameState;

    // The amount of time that has passed since the beginnning of the level
    private float timePassed = 0f;

    // The duration of the game, and the time left in the current game
    private float timeLeft;

    // The current score of the player in the game
    private float gameScore = 0f;

    // Number of errors that the user made
    private int numBadPickupsCollected = 0;
    private int numFalls = 0;

    // Use this for initialization
    void Start () {
        curGameState = GameState.GAME;
        feedbackCanvas.UpdateScoreText(gameScore);

        timeLeft = GlobalControl.Instance.timeLimit;
		
	}
	
	// Update is called once per frame
	void Update () {

        timePassed += Time.deltaTime;

        if (curGameState == GameState.GAME)
        {
            timeLeft -= Time.deltaTime;
            feedbackCanvas.UpdateTimeText(timeLeft);

            if (numPickupsCollected >= numPickups)
            {
                // Game won
                GameEnded(true);
            }
            if (timeLeft < 0)
            {
                // Game lost
                GameEnded(false);
            }
        }
        else
        {
            // This level is over
        }
	}

    // You got a pickup, increase score!
    public void PickupCollected()
    {
        numPickupsCollected++;

        // If this is the first 10 seconds of the game, award bonus points
        if (timePassed < 10f)
        {
            gameScore += 200f;
        }
        else
        {
            gameScore += 100f;
        }

        feedbackCanvas.UpdateScoreText(gameScore);
        GetComponent<SoundEffectPlayer>().PlayHappySound();
    }

    // You got a BAD pickup, decrease score.
    public void BadPickupCollected()
    {
        numBadPickupsCollected++;
        gameScore = gameScore - 10f;
        feedbackCanvas.UpdateScoreText(gameScore);
        GetComponent<SoundEffectPlayer>().PlayBadSound();
    }

    // The ball fell out of bounds. Decrease score.
    public void BallOutOfBounds()
    {
        numFalls++;
        gameScore = gameScore - 10f;
        feedbackCanvas.UpdateScoreText(gameScore);
        GetComponent<SoundEffectPlayer>().PlayResetSound();
    }

    // The game just ended. won = Did the player win?
    public void GameEnded(bool won)
    {
        player.GetComponent<Player>().FreezePlayer();
        curGameState = GameState.POST_GAME;

        if (won)
        {
            Invoke("NextLevel", 2);
            feedbackCanvas.DisplayWinText();
            GetComponent<SoundEffectPlayer>().PlayHappySound();
        }
        else
        {
            Invoke("SameLevel", 2);
            feedbackCanvas.DisplayLossText();
            GetComponent<SoundEffectPlayer>().PlayBadSound();
        }
    }

    private void NextLevel()
    {
        SceneManager.LoadScene(nextScene);
    }

    private void SameLevel()
    {
        SceneManager.LoadScene(thisScene);
    }
}
