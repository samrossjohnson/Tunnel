using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.

    public int highScore;
    public int gold;

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }
    //Takes a scene name and changes to that scene
    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);  
    }

    // Checks if the players score broke their highscore
    public void CheckScore(int score)
    {
        if (score > highScore)
            highScore = score;
    }

    public void AddCoins(int coins)
    {
        gold += coins;
    }

    public void SaveStateLoaded(SaveState save)
    {
        highScore = save.highScore;
        gold = save.gold;
    }
}
