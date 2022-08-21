using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketController : MonoBehaviour
{
    public static MarketController Current;
    public List<MarketItem> items;//sat�n al�nm�s veya al�nmam�s tum esyalar l�stesi
    public List<Item> equippedItems;//g�y�lm�s esyalar l�stes�
    public GameObject marketMenu;//ed�tordek� market menusune er�smek �c�n deg�sken (market� kapat ac fonksyonu �c�n)

    public void InitializeMarketController()//marketkontroller� tan�mlama fonks
    {
        Current = this;
        foreach (MarketItem item in items)//items listesinde d�n ve teker teker tan�mla
        {
            item.InitializeItem();
        }
    }

    public void ActivateMarketMenu(bool active)//market butonuna bas�ld�g�nda cal�scak fonks
    {
        marketMenu.SetActive(active);
    }
}
