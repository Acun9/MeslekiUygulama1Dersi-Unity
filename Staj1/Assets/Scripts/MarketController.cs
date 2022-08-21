using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketController : MonoBehaviour
{
    public static MarketController Current;
    public List<MarketItem> items;//satýn alýnmýs veya alýnmamýs tum esyalar lýstesi
    public List<Item> equippedItems;//gýyýlmýs esyalar lýstesý
    public GameObject marketMenu;//edýtordeký market menusune erýsmek ýcýn degýsken (marketý kapat ac fonksyonu ýcýn)

    public void InitializeMarketController()//marketkontrollerý tanýmlama fonks
    {
        Current = this;
        foreach (MarketItem item in items)//items listesinde dön ve teker teker tanýmla
        {
            item.InitializeItem();
        }
    }

    public void ActivateMarketMenu(bool active)//market butonuna basýldýgýnda calýscak fonks
    {
        marketMenu.SetActive(active);
    }
}
