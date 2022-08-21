using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketItem : MonoBehaviour
{
    public int itemId, wearId;//�tem�d:esyan�n kay�tlardaki son durumu (sat�n al�nm�s m� giyili mi vs), wear�d: hangi kategori
                              // 0 : Daha sat�n al�nmam��
                              // 1 : Sat�n al�nm�� ama giyilmemi�
                              // 2 : Hem sat�n al�nm�� hem de giyilmi�
    public int price;

    public Button buyButton, equipButton, unequipButton;
    public Text priceText;

    public GameObject itemPrefab;//g�y�len esyay� sahnede yaratab�lmek �c�n esyaya ulasacag�m�z deg�sken

    public bool HasItem()//sat�n al�nm�s m�
    {
        bool hasItem = PlayerPrefs.GetInt("item" + itemId.ToString()) != 0;
        return hasItem;
    }

    public bool IsEquipped()
    {
        bool equippedItem = PlayerPrefs.GetInt("item" + itemId.ToString()) == 2;
        return equippedItem;
    }

    public void InitializeItem()//esyalar� tan�mla (al�nm�s m� giyilmis mi vs)
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
                PlayerPrefs.SetInt("item" + itemId.ToString(), 1);//esyan�n durumunu sat�n al�nm�s ama g�y�lmem�s yap
                buyButton.gameObject.SetActive(false);
                equipButton.gameObject.SetActive(true);
            }
        }
    }

    public void EquipItem()
    {
        UnequipItem();
        MarketController.Current.equippedItems[wearId] = Instantiate(itemPrefab, PlayerController.Current.wearSpots[wearId].transform).GetComponent<Item>();//�nstantiate: 1. parametre yarat�lcak obje 2. parametre k�m�n ch�ld� olcag�
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
            PlayerPrefs.SetInt("item" + marketItem.itemId, 1);//al�nm�s ama g�y�lmem�s
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
