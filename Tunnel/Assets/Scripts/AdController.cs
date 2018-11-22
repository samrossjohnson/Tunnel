using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdController : MonoBehaviour {

    public SkinShop skinShop;   //Reference to the SkinShop component.

    public void ShowAd()
    {
        // Check that the advertisement is ready to be shown
        if (Advertisement.IsReady())
            // Show an ad
            Advertisement.Show("rewardedVideo", new ShowOptions() {resultCallback = HandleAdResult});
    }

    private void HandleAdResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Failed:
                break;
            case ShowResult.Skipped:
                break;
            case ShowResult.Finished:
                AdReward();
                break;
            default:
                break;
        }
    }

    private void AdReward()
    {
        skinShop.GetAdReward();
    }
}
