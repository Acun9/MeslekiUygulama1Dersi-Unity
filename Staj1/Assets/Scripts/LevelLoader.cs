using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    //reklamlar �c�n coklu sahne s�stem�
    public static LevelLoader Current;
    private Scene _lastLoadedScene;
    void Start()
    {
        Current = this;
        GameObject.FindObjectOfType<AdController>().InitializeAds();//reklamlar� tan�mla
        ChangeLevel("Level " + PlayerPrefs.GetInt("currentLevel"));
    }
    public void ChangeLevel(string sceneName)//oyuncu hang� levelda kald�ysa Game sahnes�ne o level� yukled�g�m�z fonks
    {
        StartCoroutine(ChanceScene(sceneName));//startcorout�ne: bu fonks farkl� b�r zaman d�l�m�nde baslat
    }
    IEnumerator ChanceScene(string sceneName)// IEnumerator: farkl� zaman d�l�m�nde olan �slemler�n b�tmes�n� bekleyeb�lmem�z� saglayan fonkslar
    {
        if (_lastLoadedScene.IsValid())//onceden b�r sahne yuklenm�sse onu s�lcez
        {
            SceneManager.UnloadSceneAsync(_lastLoadedScene);//sahney� s�lme �slem�n� baslat
            bool sceneUnloaded = false;
            while (!sceneUnloaded)//sahne s�l�nene kadar s�l�nd� m� s�l�nmed� m� kontrol et
            {
                sceneUnloaded = !_lastLoadedScene.IsValid();
                yield return new WaitForEndOfFrame();//bu sat�ra kadar olan kodlar� cal�st�r sonra oyunun b�r dongusunun tamamlanmas�n� bekle ve devam et (oyun sonsuz donguye g�rmes�n d�ye)
            }
        }
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);//additive: oncek� sahney� s�lme, sadece yen� sahney� yukle
        bool sceneLoaded = false;
        while (!sceneLoaded)//sahne yuklenene kadar yuklend� m� d�ye kontrol et
        {
            _lastLoadedScene = SceneManager.GetSceneByName(sceneName);     
            sceneLoaded = _lastLoadedScene != null && _lastLoadedScene.isLoaded;
            yield return new WaitForEndOfFrame();
        }
    }   
}
