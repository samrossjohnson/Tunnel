using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinShop : MonoBehaviour {

    public Transform shopPanel;             //Reference to the panel containing all the skin buttons
    public Text skinCostText;               //Reference to the text that displays the cost of the selected skin
    public Text currentCoinsText;           //Reference to the text that displays the current coins 
    public Text buyEquipButtonText;         //Reference to the text inside the BuyEquipButton

    public SpriteRenderer previewSprite;    //Reference to the SpriteRenderer of the preview player
    public TrailRenderer previewTrail;      //Reference to the TrailRenderer of the preview player

    private int selectedIndex;              //Index of the shop item that is currently selected

    public Transform adRewardPanel;         //Reference to the panel used for ad rewards.

    private void Awake()
    {
        InitializeShop();
    }

    private void InitializeShop()
    {
        //  Initialize selected index at 0
        selectedIndex = 0;

        // Initialize each UI element 
        int i = 0;
        foreach(Transform t in shopPanel)
        {
            int currentIndex = i;

            //Find the button component and give it onClick functionality
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnSkinSelect(currentIndex));

            //Set the buttons color
            Image img = t.GetComponent<Image>();
            img.color = SkinManager.instance.availableSkins[currentIndex].startColor;

            i++;
        }

        OnSkinSelect(SkinManager.instance.equippedSkinIndex);
        UpdateCoinDisplay();
        ResizeShopItems();
        CloseAdRewardPanel();
    }

    private void OnSkinSelect(int currentIndex)
    {
        // Set selectedIndex to the index of the shop item we are currently viewing
        selectedIndex = currentIndex;

        // Resize the shop items
        ResizeShopItems();

        // If the selected skin has already been purchased
        if (SkinManager.instance.availableSkins[currentIndex].purchased)
        {
            // Display "Purchased" in place of a cost
            skinCostText.text = "Purchased";

            // Set the text inside the BuyEquip button to "Equip"
            buyEquipButtonText.text = "Equip";

            // Display equipped instead of purchased if the selected skin is currently equipped
            if (currentIndex == SkinManager.instance.equippedSkinIndex)
                skinCostText.text = "Equipped";
        }
        // If the selected skin has not been purchased    
        else
        {
            // Display the cost of the skin
            skinCostText.text = "Cost: " + SkinManager.instance.availableSkins[currentIndex].cost.ToString();

            // Set the text inside the BuyEquip button to "Buy"
            buyEquipButtonText.text = "Buy";
        }
        
        // Change the material of the previewTrail and color of the previewSprite
        previewTrail.startColor = SkinManager.instance.availableSkins[currentIndex].startColor;
        previewTrail.endColor = SkinManager.instance.availableSkins[currentIndex].endColor;
        previewSprite.color = SkinManager.instance.availableSkins[currentIndex].startColor;
    }

    public void BuyEquip()
    {
        // If the selected skin is already owned, then equip that skin
        if (SkinManager.instance.availableSkins[selectedIndex].purchased)
        {
            SkinManager.instance.equippedSkin = SkinManager.instance.availableSkins[selectedIndex];
            SkinManager.instance.equippedSkinIndex = selectedIndex;

            // Also save the game, to remember which skin was equipped for next time we play
            SaveManager.instance.SaveGame();
        }
        // If the selected skin is not owned, then attempt to purchase the selected skin
        else
            PurchaseSkin();

        ResizeShopItems();
    }

    // Attempts to purchase the currently selected skin
    private void PurchaseSkin()
    {
        // Check the player can afford the item and purchase it if they can
        if (GameManager.instance.gold >= SkinManager.instance.availableSkins[selectedIndex].cost)
        {
            // Unlock the skin in the SaveState and save the game
            SaveManager.instance.UnlockSkin(selectedIndex);
            SaveManager.instance.SaveGame();

            // Set the purchased bool for the skin to true
            SkinManager.instance.availableSkins[selectedIndex].purchased = true;
            // Remove the coins that were spent on the purchase and update the currentCoinsText
            GameManager.instance.gold -= SkinManager.instance.availableSkins[selectedIndex].cost;
            UpdateCoinDisplay();
            OnSkinSelect(selectedIndex);
        }

        ResizeShopItems();
    }

    // Update the currentCoinsText to the current coins the player has
    private void UpdateCoinDisplay()
    {
        currentCoinsText.text = "Coins: " + GameManager.instance.gold.ToString();
    }

    // Loop through the shop items and resize them according to which are selected/equipped
    private void ResizeShopItems()
    {
        int i = 0;

        foreach (Transform t in shopPanel)
        {
            // If t is the currently selected transform, shrink it slightly
            if (i == selectedIndex)
                t.GetComponent<RectTransform>().localScale = Vector3.one * 0.85f;
            //If t is not the currently selected transform, return it to its normal size
            else
                t.GetComponent<RectTransform>().localScale = Vector3.one;

            // If t is the currently equipped skin, shrink it further
            if (i == SkinManager.instance.equippedSkinIndex)
                t.GetComponent<RectTransform>().localScale = Vector3.one * 0.5f;

            //If this item has been purchased, remove its "Locked" text
            if (t.GetComponentInChildren<Text>() != null && SkinManager.instance.availableSkins[i].purchased)
                t.GetComponentInChildren<Text>().text = "";

            i++;
        }
    }

    // Give the player a reward for watching a rewarded advert.
    public void GetAdReward()
    {
        // Add the coins to the GameManager
        GameManager.instance.AddCoins(25);

        // Call UpdateCoinDisplay()
        UpdateCoinDisplay();

        // Alert the player that they gained their reward sucessfully.
        adRewardPanel.gameObject.SetActive(true);

        // Save the game.
        SaveManager.instance.SaveGame();
    }

    // Close the ad reward panel
    public void CloseAdRewardPanel()
    {
        adRewardPanel.gameObject.SetActive(false);
    }
}
