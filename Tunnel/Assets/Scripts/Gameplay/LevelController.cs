using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {

    public Text inGameScoreText;                    //Reference to the Text component that displays the in-game score.

    public CanvasGroup postGameScorePanel;          //Reference to the CanvasGroup component of the panel that displays post-game info.
    public Text finalScoreText;                     //Reference to the Text component that displays the final score.
    public Text coinsEarnedText;                    //Reference to the Text component that displays the coins earned.
    public Text newHighscoreText;                   //Reference to the Text component that displays if the player has set a new highscore.

    public AudioClip deathSoundEffect;              //Rererence to the death sound effect AudioClip

    private void Start()
    {
        // Make the post-game infro panel invisible at the start of the game
        postGameScorePanel.alpha = 0f;

        // Also set it to inactive, as buttons can still be pressed accidentally otherwise
        postGameScorePanel.gameObject.SetActive(false);

        // Play the game music when the game starts
        SoundManager.instance.StartGameMusic();
    }

    // Run when the player loses the game
    public void GameOver(int score, float finalMovementSpeed)
    {
        // Play the death sound effect
        SoundManager.instance.PlaySingleClip(deathSoundEffect);

        // Play the menu music
        SoundManager.instance.StartMenuMusic();

        // Hide the ingameScoreText as we no longer need to display it
        inGameScoreText.text = "";

        // Display the final score on the finalScoreText.
        finalScoreText.text = score.ToString();

        // Display if the player beat their highscore.
        if (score > GameManager.instance.highScore)
            newHighscoreText.text = "NEW HIGH SCORE!";
        else
            newHighscoreText.text = "";

        // Send the players final score to the GameManager
        GameManager.instance.CheckScore(score);

        // Calculate how many coins the player has earned from their score
        int coinsEarned = Mathf.RoundToInt(score / 10f);

        // Display the amount of coins earned
        coinsEarnedText.text = "Coins: +" + coinsEarned.ToString();

        // Add the newly earned coins.
        GameManager.instance.AddCoins(coinsEarned);

        //Finally, save the game.
        SaveManager.instance.SaveGame();

        DisplayPostGamePanel();
    }

    private void DisplayPostGamePanel()
    {
        // Return the post-game info panels alpha to 1
        postGameScorePanel.alpha = 1f;

        // Return the post-game score panel to being active
        postGameScorePanel.gameObject.SetActive(true);
    }
}
