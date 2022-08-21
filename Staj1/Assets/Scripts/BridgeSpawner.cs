using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeSpawner : MonoBehaviour
{

    public GameObject startReference, endReference;//baþlangýç ve bitiþ referans objelerini tutmak için deðiþkenler
    public BoxCollider hiddenPlatform;//box colliderý konumlandýrmak ve boyutlandýrmak icin degisken
    // Start is called before the first frame update
    void Start()
    {
        Vector3 direction = endReference.transform.position - startReference.transform.position; //baslangýc ve bitis arasýndaki yön vektörü
        float distance = direction.magnitude;// magnitude: yön vektörünün aðýrlýðý (baslangýc-bitis noktalarý arasýndaki mesafe)
        direction = direction.normalized;// normalized : yön vektörünü islemlerde kullanabilmek icin uzunlugu 1 olan birim vektöre dönüstür
        hiddenPlatform.transform.forward = direction;// baslangýc-bitis noktalarý arasý yön degisirse görünmez platformun yönü de degissin
        hiddenPlatform.size = new Vector3(hiddenPlatform.size.x, hiddenPlatform.size.y, distance);// iki platform arasý mesafeye göre görünmez platform da büyüyüp kücülsün(x,y sabit z degissin)

        hiddenPlatform.transform.position = startReference.transform.position + (direction * distance / 2) + (new Vector3(0, -direction.z, direction.y) * hiddenPlatform.size.y / 2);
        //              görünmez platformu baslangýc-bitis noktalarýnýn ortasýna getir                     + gorünmez platformun üst cizgisi baslangýc-bitis noktalarýnýn üst cizgisine denk gelsin
    }

}
