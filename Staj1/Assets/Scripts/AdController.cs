using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdController : MonoBehaviour
{
    public static AdController Current;
    public BannerView bannerView;
    public InterstitialAd interstitial;
    public RewardedAd rewardedAd;

    private float _interstitialAdTimer = 60;//sureklý reklam cýkmasýn diye 60snlik timer

    // Start is called before the first frame update
     public void InitializeAds()//levelloaderdan reklamlarý tanýmlayacagýmýz fonks
    {
        Current = this;
        MobileAds.Initialize(initStatus => { });
        this.RequestBanner();//google sunucularýna banner reklam istegi
        this.RequestInterstitial();
        this.RequestRewardedAdRequest();
    }

    // Update is called once per frame
    void Update()
    {
        if (_interstitialAdTimer > 0)
        {
            _interstitialAdTimer -= Time.deltaTime;
        }
    }

    private void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);//banner reklamý asagý orta kýsma cek

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        this.bannerView.LoadAd(request);
    }

    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);

        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleOnAdOpening;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    public bool IsReadyInterstitalAd()
    {
        if(_interstitialAdTimer < 0 && interstitial.IsLoaded())
        {
            return true;
        }
        return false;
    }

    private void HandleOnAdClosed(object sender, EventArgs e)//reklam kapandýgý zaman
    {
        _interstitialAdTimer = 60;
        Time.timeScale = 1;//oyunu devam ettir
        Camera.main.GetComponent<AudioListener>().enabled = true;//sesleri ac
        RequestInterstitial();//sonraki gecis reklamýnýn istegi
    }

    private void HandleOnAdOpening(object sender, EventArgs e)//reklam acýldýgý zaman
    {
        Time.timeScale = 0;//oyunu dondur
        Camera.main.GetComponent<AudioListener>().enabled = false;//tum sesleri kapat
    }

    private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)//reklam yuklenmesý basarýsýz olduysa
    {
        RequestInterstitial();
    }

    public void RequestRewardedAdRequest()
    {
        string adUnitId;
#if UNITY_ANDROID
        adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
            adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            adUnitId = "unexpected_platform";
#endif

        this.rewardedAd = new RewardedAd(adUnitId);

        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }

    private void HandleRewardedAdClosed(object sender, EventArgs e)
    {
        Time.timeScale = 1;
        Camera.main.GetComponent<AudioListener>().enabled = true;
        RequestRewardedAdRequest();
    }

    private void HandleUserEarnedReward(object sender, Reward e)
    {
        LevelController.Current.rewardedAdButton.gameObject.SetActive(false);
        LevelController.Current.GiveMoneyToPlayer(LevelController.Current.score);
    }

    private void HandleRewardedAdOpening(object sender, EventArgs e)
    {
        Time.timeScale = 0;
        Camera.main.GetComponent<AudioListener>().enabled = false;
    }

    private void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        RequestRewardedAdRequest();
    }
}
