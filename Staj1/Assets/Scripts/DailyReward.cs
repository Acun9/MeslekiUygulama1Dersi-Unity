using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{
    public bool initialized;//bu s�n�f�n level controllerda cag�r�l�nca tan�mlanmas� �c�n gerekl� deg�sken
    public long rewardGivingTimeTicks;//sonraki odulu ne zaman alacag�m�z� tutan deg�sken
    public GameObject rewardMenu;//odul verme menusu
    public Text remainingTimeText;//ekrandaki kalan zaman yaz�s�

    public void InitializeDailyReward()
    {
        //PlayerPrefs.SetString("lastDailyReward", (System.DateTime.Now.Ticks - 864000000000 + 10 * 10000000).ToString()); // test �c�n 10 sn kalaya g�t
        if (PlayerPrefs.HasKey("lastDailyReward"))//haf�zada lastdailyreward diye bir deg�sken var m� yani gunluk odul daha once al�nm�s m� 
        {
            rewardGivingTimeTicks = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + 864000000000;//sonraki odul al�m tarihinin tick cinsi
            long currentTime = System.DateTime.Now.Ticks;//simdiki zaman�n tick cinsi
            if(currentTime >= rewardGivingTimeTicks)
            {
                GiveReward();
            }
        }
        else
        {
            GiveReward();
        }

        initialized = true;//bu fonks level controllerda cag�r�l�nca tan�mlans�n d�ye
    }

    public void GiveReward()
    {
        LevelController.Current.GiveMoneyToPlayer(100);//gunluk odul 100
        rewardMenu.SetActive(true);
        PlayerPrefs.SetString("lastDailyReward", System.DateTime.Now.Ticks.ToString());//son odul al�m tarihini guncelle
        rewardGivingTimeTicks = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + 864000000000;//b�r sonraki odul al�m tarihini guncelle
    }
    void Update()
    {
        if (initialized)//odul alma tan�mland�ysa
        {
            if (LevelController.Current.startMenu.activeInHierarchy)//oyuncu baslang�c menusunde ise
            {
                long currentTime = System.DateTime.Now.Ticks;
                long remainingTime = rewardGivingTimeTicks - currentTime;//kalan zaman hesab�
                if(remainingTime <= 0)
                {
                    GiveReward();
                }
                else
                {
                    System.TimeSpan timeSpan = System.TimeSpan.FromTicks(remainingTime);//timespan: kalan zaman� istedigimiz cinse cevirmeye yar�yor
                    remainingTimeText.text = string.Format("{0}:{1}:{2}", timeSpan.Hours.ToString("D2"), timeSpan.Minutes.ToString("D2"), timeSpan.Seconds.ToString("D2"));//D2: deger iki basamaktan kucukse bas�nda s�f�rla yaz
                }
            }
        }
    }

    public void TapToReturnButton()//odul menusunu kapat
    {
        rewardMenu.SetActive(false);
    }
}
