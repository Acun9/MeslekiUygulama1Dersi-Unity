using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeSpawner : MonoBehaviour
{

    public GameObject startReference, endReference;//ba�lang�� ve biti� referans objelerini tutmak i�in de�i�kenler
    public BoxCollider hiddenPlatform;//box collider� konumland�rmak ve boyutland�rmak icin degisken
    // Start is called before the first frame update
    void Start()
    {
        Vector3 direction = endReference.transform.position - startReference.transform.position; //baslang�c ve bitis aras�ndaki y�n vekt�r�
        float distance = direction.magnitude;// magnitude: y�n vekt�r�n�n a��rl��� (baslang�c-bitis noktalar� aras�ndaki mesafe)
        direction = direction.normalized;// normalized : y�n vekt�r�n� islemlerde kullanabilmek icin uzunlugu 1 olan birim vekt�re d�n�st�r
        hiddenPlatform.transform.forward = direction;// baslang�c-bitis noktalar� aras� y�n degisirse g�r�nmez platformun y�n� de degissin
        hiddenPlatform.size = new Vector3(hiddenPlatform.size.x, hiddenPlatform.size.y, distance);// iki platform aras� mesafeye g�re g�r�nmez platform da b�y�y�p k�c�ls�n(x,y sabit z degissin)

        hiddenPlatform.transform.position = startReference.transform.position + (direction * distance / 2) + (new Vector3(0, -direction.z, direction.y) * hiddenPlatform.size.y / 2);
        //              g�r�nmez platformu baslang�c-bitis noktalar�n�n ortas�na getir                     + gor�nmez platformun �st cizgisi baslang�c-bitis noktalar�n�n �st cizgisine denk gelsin
    }

}
