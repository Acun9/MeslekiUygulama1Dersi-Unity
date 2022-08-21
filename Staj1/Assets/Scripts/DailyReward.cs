using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{
    public bool initialized;//bu sýnýfýn level controllerda cagýrýlýnca tanýmlanmasý ýcýn gereklý degýsken
    public long rewardGivingTimeTicks;//sonraki odulu ne zaman alacagýmýzý tutan degýsken
    public GameObject rewardMenu;//odul verme menusu
    public Text remainingTimeText;//ekrandaki kalan zaman yazýsý

    public void InitializeDailyReward()
    {
        //PlayerPrefs.SetString("lastDailyReward", (System.DateTime.Now.Ticks - 864000000000 + 10 * 10000000).ToString()); // test ýcýn 10 sn kalaya gýt
        if (PlayerPrefs.HasKey("lastDailyReward"))//hafýzada lastdailyreward diye bir degýsken var mý yani gunluk odul daha once alýnmýs mý 
        {
            rewardGivingTimeTicks = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + 864000000000;//sonraki odul alým tarihinin tick cinsi
            long currentTime = System.DateTime.Now.Ticks;//simdiki zamanýn tick cinsi
            if(currentTime >= rewardGivingTimeTicks)
            {
                GiveReward();
            }
        }
        else
        {
            GiveReward();
        }

        initialized = true;//bu fonks level controllerda cagýrýlýnca tanýmlansýn dýye
    }

    public void GiveReward()
    {
        LevelController.Current.GiveMoneyToPlayer(100);//gunluk odul 100
        rewardMenu.SetActive(true);
        PlayerPrefs.SetString("lastDailyReward", System.DateTime.Now.Ticks.ToString());//son odul alým tarihini guncelle
        rewardGivingTimeTicks = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + 864000000000;//býr sonraki odul alým tarihini guncelle
    }
    void Update()
    {
        if (initialized)//odul alma tanýmlandýysa
        {
            if (LevelController.Current.startMenu.activeInHierarchy)//oyuncu baslangýc menusunde ise
            {
                long currentTime = System.DateTime.Now.Ticks;
                long remainingTime = rewardGivingTimeTicks - currentTime;//kalan zaman hesabý
                if(remainingTime <= 0)
                {
                    GiveReward();
                }
                else
                {
                    System.TimeSpan timeSpan = System.TimeSpan.FromTicks(remainingTime);//timespan: kalan zamaný istedigimiz cinse cevirmeye yarýyor
                    remainingTimeText.text = string.Format("{0}:{1}:{2}", timeSpan.Hours.ToString("D2"), timeSpan.Minutes.ToString("D2"), timeSpan.Seconds.ToString("D2"));//D2: deger iki basamaktan kucukse basýnda sýfýrla yaz
                }
            }
        }
    }

    public void TapToReturnButton()//odul menusunu kapat
    {
        rewardMenu.SetActive(false);
    }
}
