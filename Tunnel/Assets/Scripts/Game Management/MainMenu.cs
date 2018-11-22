using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public Text highScoreText;          //Reference to the highscoreText component;
    public AudioClip menuMusic;         //Reference to the menuMusic AudioClip

    private void Start()
    {
        // Display the highscore.
        highScoreText.text = "HIGH SCORE: " + GameManager.instance.highScore;

        // Start the menu music.
        SoundManager.instance.StartMenuMusic();
    }
}
