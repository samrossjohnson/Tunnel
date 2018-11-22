using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preloader : MonoBehaviour {

    private CanvasGroup fadeGroup;
    private float loadTime;
    private float minimumLogoTime = 2f;     //Minimum time the scene can last

    public GameObject gameManager;          //GameManager prefab to instantiate.
    public GameObject saveManager;          //SaveManager prefab to instantiate.
    public GameObject skinManager;          //SkinManager prefab to instantiate.
    public GameObject soundManager;         //SoundManager prefab to instantiate.

    public AudioClip logoSoundClip;         //Audio clip to play while the logo fades in.

    private void Start()
    {
        //  Find the (only) CanvasGroup in the scene
        fadeGroup = FindObjectOfType<CanvasGroup>();

        //  Start with alpha at 1
        fadeGroup.alpha = 1;

        // Pre-load the game
        // Check if a GameManager has already been assigned to static variable GameManager.instance or if it's still null.
        if (GameManager.instance == null)
            // Instantiate gameManager prefab.
            Instantiate(gameManager);

        // Check if a SaveManager has already been assigned to static variable GameManager.instance or if it's still null.
        if (SaveManager.instance == null)
            // Instantiate gameManager prefab.
            Instantiate(saveManager);

        // Check if a SkinManager has already been assigned to static variable SkinManager.instance or if it's still null.
        if (SkinManager.instance == null)
            // Instantiate skinManager prefab.
            Instantiate(skinManager);

        // Check if a SoundManager has already been assigned to static variable SoundManager.instance or if it's still null.
        if (SoundManager.instance == null)
            // Instantiate soundManager prefab.
            Instantiate(soundManager);

        // Play our logo sound.
        SoundManager.instance.PlaySingleClip(logoSoundClip);

        if (Time.time < minimumLogoTime)
            loadTime = minimumLogoTime;
        else
            loadTime = Time.time;
    }

    private void Update()
    {
        //  Fade-in
        if(Time.time < minimumLogoTime)
        {
            fadeGroup.alpha = 1 - Time.time;
        }

        //  Fade-out
        if(Time.time > minimumLogoTime && loadTime != 0)
        {
            fadeGroup.alpha = Time.time - minimumLogoTime;
            if (fadeGroup.alpha >= 1)
                //Change to the main menu scene
                GameManager.instance.ChangeScene("main_menu");
        }
    }
}
