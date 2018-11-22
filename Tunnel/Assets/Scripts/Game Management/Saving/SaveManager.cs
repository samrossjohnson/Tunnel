using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour {

    public static SaveManager instance = null;
    public SaveState state;

    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a SaveManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        LoadGame();
    }

    //  Save the saveState to the player pref
    public void SaveGame()
    {
        // Set the values of the save state
        state.highScore = GameManager.instance.highScore;
        state.gold = GameManager.instance.gold;
        state.equippedSkinIndex = SkinManager.instance.equippedSkinIndex;
        state.muted = SoundManager.instance.muted;

        PlayerPrefs.SetString("save", Helper.Serialize<SaveState>(state));
    }

    //  Load the previous saved state from the player prefs
    public void LoadGame()
    {
        //Do we already have a save file?
        if(PlayerPrefs.HasKey("save"))
        {
            state = Helper.Deserialize<SaveState>(PlayerPrefs.GetString("save"));
        }
        else
        {
            state = new SaveState();
            SaveGame();
        }

        // Tell the GameManager to update its saveable variables, to those saved in the SaveState.
        GameManager.instance.SaveStateLoaded(state);

        // Tell the SkinManager to update its saveable variables, to those saved in the SaveState.
        SkinManager.instance.SaveStateLoaded(state);

        // Tell the SoundManager to update its saveable variables, to those saved in the SaveState.
        SoundManager.instance.SaveStateLoaded(state);
    }

    //  Reset the save file
    public void ResetSave()
    {
        state = new SaveState();
        PlayerPrefs.SetString("save", Helper.Serialize<SaveState>(state));
        LoadGame();
    }

    // Check if a skin is owned
    public bool IsSkinOwned(int index)
    {
        // Check if the bit is set, if so then the skin is owned
        return (state.purchasedSkins & (1 << index)) != 0;
    }

    // Unlock a skin in the purchasedSkins int
    public void UnlockSkin(int index)
    {
        // Toggle on the bit at the index
        state.purchasedSkins |= 1 << index;
    } 
}
