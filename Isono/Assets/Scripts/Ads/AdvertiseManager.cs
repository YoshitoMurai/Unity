using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdvertiseManager : MonoBehaviour
{
    public string gameID = "3379055";
    public string placementId = "Banner";
    #region Singleton

    private static AdvertiseManager instance;

    public static AdvertiseManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (AdvertiseManager)FindObjectOfType(typeof(AdvertiseManager));

                if (instance == null)
                {
                    Debug.LogError(typeof(AdvertiseManager) + "is nothing");
                }
            }

            return instance;
        }
    }

    #endregion Singleton
    public void Awake()
    {
        if (this != Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        if (Advertisement.isSupported) { // If runtime platform is supported...
            Advertisement.Initialize(gameID, true); // ...initialize.
        }

        ShowBannerWhenReady();
    }
    public void ShowMovieAds()
    {
        // Wait until Unity Ads is initialized,
        //  and the default ad placement is ready.
        if (Advertisement.IsReady())
        {
            Advertisement.Show();
        }
    }
    void ShowBannerWhenReady()
    {
        if (!Advertisement.IsReady(placementId))
        {
            var bannerHeight = Camera.main.ScreenToWorldPoint(new Vector3(1, 50 * Screen.dpi / 160, 1)).y - Camera.main.ScreenToWorldPoint(new Vector3(1, 0, 1)).y;
            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
            Advertisement.Banner.Show(placementId);
            Debug.Log("Show Banner");
        }
    }
}