using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    //reklamlar ýcýn coklu sahne sýstemý
    public static LevelLoader Current;
    private Scene _lastLoadedScene;
    void Start()
    {
        Current = this;
        GameObject.FindObjectOfType<AdController>().InitializeAds();//reklamlarý tanýmla
        ChangeLevel("Level " + PlayerPrefs.GetInt("currentLevel"));
    }
    public void ChangeLevel(string sceneName)//oyuncu hangý levelda kaldýysa Game sahnesýne o levelý yukledýgýmýz fonks
    {
        StartCoroutine(ChanceScene(sceneName));//startcoroutýne: bu fonks farklý býr zaman dýlýmýnde baslat
    }
    IEnumerator ChanceScene(string sceneName)// IEnumerator: farklý zaman dýlýmýnde olan ýslemlerýn býtmesýný bekleyebýlmemýzý saglayan fonkslar
    {
        if (_lastLoadedScene.IsValid())//onceden býr sahne yuklenmýsse onu sýlcez
        {
            SceneManager.UnloadSceneAsync(_lastLoadedScene);//sahneyý sýlme ýslemýný baslat
            bool sceneUnloaded = false;
            while (!sceneUnloaded)//sahne sýlýnene kadar sýlýndý mý sýlýnmedý mý kontrol et
            {
                sceneUnloaded = !_lastLoadedScene.IsValid();
                yield return new WaitForEndOfFrame();//bu satýra kadar olan kodlarý calýstýr sonra oyunun býr dongusunun tamamlanmasýný bekle ve devam et (oyun sonsuz donguye gýrmesýn dýye)
            }
        }
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);//additive: onceký sahneyý sýlme, sadece yený sahneyý yukle
        bool sceneLoaded = false;
        while (!sceneLoaded)//sahne yuklenene kadar yuklendý mý dýye kontrol et
        {
            _lastLoadedScene = SceneManager.GetSceneByName(sceneName);     
            sceneLoaded = _lastLoadedScene != null && _lastLoadedScene.isLoaded;
            yield return new WaitForEndOfFrame();
        }
    }   
}
