using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static SoundManager instance = null;     //Static instance of SoundManager which allows it to be accessed by any other script.

    public AudioSource gameMusicSource;             //Reference to an AudioSource that will play the game music.
    public AudioSource menuMusicSource;             //Reference to an AudioSource that will play the menu music.
    public AudioSource soundEffectsSource;          //Reference to an AudioSource that will play sound effects.

    public bool muted;                              //Is the game muted.

    public bool isMenuMusicPlaying;                 //Tracks if the menu music is already playing to prevent it from being unnecesarily restarted.

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this, this enforces our singleton pattern, meaning there can only ever be one instance of a SoundManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene.
        DontDestroyOnLoad(gameObject);
    }

    // Either mutes or un-mutes the game, based on the SaveState
    private void InitializeSound()
    {
        // If the game is muted, call Mute().
        if (muted)
            Mute();

        // If the game is not muted, call UnMute();
        else
            UnMute();
    }

    // Mutes the game sounds.
    public void Mute()
    {
        // Mute the musicSource and soundEffectsSource.
        menuMusicSource.mute = true;
        gameMusicSource.mute = true;
        soundEffectsSource.mute = true;

        // Set muted to true.
        muted = true;

        // Call SaveGame() to save mute settings.
        SaveManager.instance.SaveGame();
    }

    // Un-mutes the game sounds.
    public void UnMute()
    {
        // Un-mute the musicSource and soundEffectsSource.
        menuMusicSource.mute = false;
        gameMusicSource.mute = false;
        soundEffectsSource.mute = false;

        // Set muted to false.
        muted = false;

        // Call SaveGame() to save mute settings.
        SaveManager.instance.SaveGame();
    }

    // Used to play single audio clips.
    public void PlaySingleClip(AudioClip clip)
    {
        // Set the clip of our soundEffectsSource to the clip passed in as a parameter.
        soundEffectsSource.clip = clip;

        // Play the clip.
        soundEffectsSource.Play();
    }

    // Play the menu music.
    public void StartMenuMusic()
    {
        gameMusicSource.Stop();

        // Only start the music if it is not already playing, to prevent un-needed restarting.
        if(!menuMusicSource.isPlaying)
            menuMusicSource.Play();
    }

    // Play the game music
    public void StartGameMusic()
    {
        menuMusicSource.Stop();
        gameMusicSource.Play();
    }

    public void SaveStateLoaded(SaveState state)
    {
        muted = state.muted;

        InitializeSound();
    }
}
