using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;//text s�n�f� �c�n

public class LevelController : MonoBehaviour
{
    public static LevelController Current;//diger s�n�flar�n bu s�n�fa erismesi icin static degisken
    public bool gameActive = false;//oyunun aktif olup olmad�g�n� kontrol eden degisken

    public GameObject startMenu, gameMenu, gameOverMenu, finishMenu;//menuler
    public Button rewardedAdButton;
    public Text scoreText, finishScoreText, currentLevelText, nextLevelText, startingMenuMoneyText, gameOverMenuMoneyText, finishGameMenuMoneyText;//kod ile degistircegimiz text objeleri
    public Slider levelProgressBar;
    public float maxDistance;//slider icin karakterin bitis cizgisine olan uzakl�g�n� tuttugumuz degisken
    public GameObject finishLine;
    public AudioSource gameMusicAudioSource;//gameover ve finishgame menulerinde oyunun ana muzigini kapatmak icin degisken
    public AudioClip victoryAudioClip, gameOverAudioClip;
    public DailyReward dailyReward;

    int currentLevel;
    public int score;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] parentsInScene = this.gameObject.scene.GetRootGameObjects();//textler icin sahnedeki tum parentlar� bir listeye at
        foreach (GameObject parent in parentsInScene)
        {
            TextObject[] textObjectsInParent = parent.GetComponentsInChildren<TextObject>(true);//true degeri sahnede inaktif olan text objelerinin de cekilebilmesini sagl�yor
            foreach (TextObject textObject in textObjectsInParent)
            {
                textObject.InitTextObject();//text objelerini tan�mla
            }
        }
        Current = this;
        currentLevel = PlayerPrefs.GetInt("currentLevel");//oyunun haf�zas�ndan �uanki leveli �ek
        PlayerController.Current = GameObject.FindObjectOfType<PlayerController>();// bug olmamas� �c�n burda tan�ml�yoruz
        GameObject.FindObjectOfType<MarketController>().InitializeMarketController();//tan�mlama
        dailyReward.InitializeDailyReward();//gunluk odulu s�n�f�n� tan�mla ve cal�st�r
        currentLevelText.text = (currentLevel + 1).ToString();//level 0 asl�nda 1. b�l�m oldugu icin textte current level + 1
        nextLevelText.text = (currentLevel + 2).ToString();//sonraki level de + 2
        UpdateMoneyTexts();
        //GiveMoneyToPlayer(3000);
        gameMusicAudioSource = Camera.main.GetComponent<AudioSource>();//kameradaki ses kaynag�n� cek
        if (AdController.Current.IsReadyInterstitalAd())
        {
            AdController.Current.interstitial.Show();
        }
    }
    public void ShowRewardedAd()//butona bas�ld�g�nda odul reklam�n� goster fonks
    {
        if (AdController.Current.rewardedAd.IsLoaded())
        {
            AdController.Current.rewardedAd.Show();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameActive)
        {
            PlayerController player = PlayerController.Current;
            float distance = finishLine.transform.position.z - PlayerController.Current.transform.position.z;//karakterin bitis cizgisine uzakl�g�
            levelProgressBar.value = 1 - (distance/maxDistance);//karakter bitise yaklast�kca slider�n 0-1 aras�nda ald�g� deger
        }
    }

    public void StartLevel()
    {
        AdController.Current.bannerView.Hide();//oyun baslay�nca banner reklam� sakla
        maxDistance = finishLine.transform.position.z - PlayerController.Current.transform.position.z;
        PlayerController.Current.ChangeSpeed(PlayerController.Current.runningSpeed);
        startMenu.SetActive(false);//baslang�c menusunu (game canvasta) tap to start butonu icin kapat�p
        gameMenu.SetActive(true);//oyun menusunu aktif et (slider vs)
        PlayerController.Current.animator.SetBool("running", true);//karakterin kosma animasyonunu baslat
        gameActive = true;
    }

    public void RestartLevel()
    {
        LevelLoader.Current.ChangeLevel(this.gameObject.scene.name);
    }
    public void LoadNextLevel()
    {
        LevelLoader.Current.ChangeLevel("Level " + (currentLevel + 1));
    }

    public void GameOver()
    {
        if (AdController.Current.IsReadyInterstitalAd())//eger gec�s reklam� yuklend�yse
        {
            AdController.Current.interstitial.Show();
        }
        AdController.Current.bannerView.Show();
        UpdateMoneyTexts();
        gameMusicAudioSource.Stop();
        gameMusicAudioSource.PlayOneShot(gameOverAudioClip);
        gameMenu.SetActive(false);
        gameOverMenu.SetActive(true);
        gameActive = false;
    }

    public void FinishGame()//bolumu b�t�rd�g�m�zde ac�lan menu (tap for next level butonunun oldugu)
    {
        if (AdController.Current.rewardedAd.IsLoaded())
        {
            rewardedAdButton.gameObject.SetActive(true);
        }
        else
        {
            rewardedAdButton.gameObject.SetActive(false);
        }
        AdController.Current.bannerView.Show();
        GiveMoneyToPlayer(score);
        gameMusicAudioSource.Stop();
        gameMusicAudioSource.PlayOneShot(victoryAudioClip);
        PlayerPrefs.SetInt("currentLevel", currentLevel + 1);
        finishScoreText.text = score.ToString();
        gameMenu.SetActive(false);
        finishMenu.SetActive(true);
        gameActive = false;
    }

    public void ChangeScore(int increment)
    {
        score += increment;
        scoreText.text = score.ToString();
    }

    public void UpdateMoneyTexts()
    {
        int money = PlayerPrefs.GetInt("money");
        startingMenuMoneyText.text = money.ToString();
        gameOverMenuMoneyText.text = money.ToString();
        finishGameMenuMoneyText.text = money.ToString();
    }

    public void GiveMoneyToPlayer(int increment)//finishmenu icin son durumda kazan�lan paray� haf�za birimine kaydeden fonks
    {
        int money = PlayerPrefs.GetInt("money");
        money = Mathf.Max(0, money + increment);//para s�f�r�n alt�na duserse bug olmas�n d�ye mathf.max: iki parametre aras�ndaki en buyuk degeri dondurur
        PlayerPrefs.SetInt("money", money);
        UpdateMoneyTexts();
    }
}
