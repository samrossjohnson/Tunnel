using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour {

    public static SkinManager instance = null;

    public Skin[] availableSkins;
    public Skin equippedSkin;
    public int equippedSkinIndex;

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a SkinManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    // Update the skins data from the SaveState
    public void SaveStateLoaded(SaveState state)
    {
        // Set equippedSkinIndex to the index of the skin that was equipped when we saved
        equippedSkinIndex = SaveManager.instance.state.equippedSkinIndex;

        // availableSkins[0] is the default skin, so is always purchased
        availableSkins[0].purchased = true;

        for (int i = 1; i < availableSkins.Length; i++)
            availableSkins[i].purchased = SaveManager.instance.IsSkinOwned(i);

        // Once we have loaded which skin was last equipped, equip it
        equippedSkin = availableSkins[equippedSkinIndex];
    }

    [System.Serializable]
    public struct Skin
    {
        public string name;
        public Color startColor;
        public Color endColor;
        public int cost;
        public bool purchased;
    }
}
