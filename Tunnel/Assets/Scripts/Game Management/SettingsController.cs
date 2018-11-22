using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour {

    bool muted;                     //Is the game muted?

    public Text muteButtonText;     //Reference to the Text component of the mute button;

    private void Awake()
    {
        // Get muted from the SoundManager
        muted = SoundManager.instance.muted;

        // Set the initial text of the muteButtonText.
        if (muted)
            muteButtonText.text = "Un-mute";
        else
            muteButtonText.text = "Mute";
    }

    public void ResetSaveFile()
    {
        SaveManager.instance.ResetSave();
    }

    public void ToggleMute()
    {
        // If we are currently muted, switch to un-muted.
        if (muted)
        {
            SoundManager.instance.UnMute();
            muted = false;
            muteButtonText.text = "Mute";
        }

        // If we are currently un-muted, switch to muted.
        else
        {
            SoundManager.instance.Mute();
            muted = true;
            muteButtonText.text = "Un-mute";
        }

    }
}
