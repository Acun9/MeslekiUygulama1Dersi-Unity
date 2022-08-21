using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketItem : MonoBehaviour
{
    public int itemId, wearId;//ýtemýd:esyanýn kayýtlardaki son durumu (satýn alýnmýs mý giyili mi vs), wearýd: hangi kategori
                              // 0 : Daha satýn alýnmamýþ
                              // 1 : Satýn alýnmýþ ama giyilmemiþ
                              // 2 : Hem satýn alýnmýþ hem de giyilmiþ
    public int price;

    public Button buyButton, equipButton, unequipButton;
    public Text priceText;

    public GameObject itemPrefab;//gýyýlen esyayý sahnede yaratabýlmek ýcýn esyaya ulasacagýmýz degýsken

    public bool HasItem()//satýn alýnmýs mý
    {
        bool hasItem = PlayerPrefs.GetInt("item" + itemId.ToString()) != 0;
        return hasItem;
    }

    public bool IsEquipped()
    {
        bool equippedItem = PlayerPrefs.GetInt("item" + itemId.ToString()) == 2;
        return equippedItem;
    }

    public void InitializeItem()//esyalarý tanýmla (alýnmýs mý giyilmis mi vs)
    {
        priceText.text = price.ToString();
        if (HasItem())
        {
            buyButton.gameObject.SetActive(false);
            if (IsEquipped())
            {
                EquipItem();
            }
            else
            {
                equipButton.gameObject.SetActive(true);
            }
        }
        else
        {
            buyButton.gameObject.SetActive(true);
        }
    }

    public void BuyItem()
    {
        if (!HasItem())
        {
            int money = PlayerPrefs.GetInt("money");
            if(money >= price)
            {
                PlayerController.Current.itemAudioSource.PlayOneShot(PlayerController.Current.buyAudioClip, 0.1f);
                LevelController.Current.GiveMoneyToPlayer(-price);
                PlayerPrefs.SetInt("item" + itemId.ToString(), 1);//esyanýn durumunu satýn alýnmýs ama gýyýlmemýs yap
                buyButton.gameObject.SetActive(false);
                equipButton.gameObject.SetActive(true);
            }
        }
    }

    public void EquipItem()
    {
        UnequipItem();
        MarketController.Current.equippedItems[wearId] = Instantiate(itemPrefab, PlayerController.Current.wearSpots[wearId].transform).GetComponent<Item>();//ýnstantiate: 1. parametre yaratýlcak obje 2. parametre kýmýn chýldý olcagý
        MarketController.Current.equippedItems[wearId].itemId = itemId;
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(true);
        PlayerPrefs.SetInt("item" + itemId.ToString(), 2);
    }

    public void UnequipItem()
    {
        Item equippedItem = MarketController.Current.equippedItems[wearId];
        if (equippedItem != null)
        {
            MarketItem marketItem = MarketController.Current.items[equippedItem.itemId];
            PlayerPrefs.SetInt("item" + marketItem.itemId, 1);//alýnmýs ama gýyýlmemýs
            marketItem.equipButton.gameObject.SetActive(true);
            marketItem.unequipButton.gameObject.SetActive(false);
            Destroy(equippedItem.gameObject);
        }
    }

    public void EquipItemButton()
    {
        PlayerController.Current.itemAudioSource.PlayOneShot(PlayerController.Current.equipItemAudioClip, 0.1f);
        EquipItem();
    }

    public void UnequipItemButton()
    {
        PlayerController.Current.itemAudioSource.PlayOneShot(PlayerController.Current.unequipItemAudioClip, 0.1f);
        UnequipItem();
    }
}
