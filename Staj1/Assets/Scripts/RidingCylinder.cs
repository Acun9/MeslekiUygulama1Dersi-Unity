using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidingCylinder : MonoBehaviour
{
    private bool _filled; //silindir maks hacminde mi kontrol degiskeni
    private float _value; //silindirin say�sal olarak maks hacmi

    public void IncrementCylinderVolume(float value) // silindirin boyunu artt�ran azaltan fonks
    {
        _value += value;
        if (_value > 1) //silindir en b�y�k haline geldiyse
        {


            float leftValue = _value - 1; //1 silindir hacminden kalan deger
            int cylinderCount = PlayerController.Current.cylinders.Count;//karakterimizin alt�ndaki silindir say�s�
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylinderCount - 1) - 0.25f, transform.localPosition.z);// local pozisyon: bir objenin parent�na bagl� pozisyonu (silindir karakterin ayag�na girmesin diye)
            transform.localScale = new Vector3(0.5f, transform.localScale.y, 0.5f);// Silindirin boyutunu tam olarak 1 yap
            PlayerController.Current.CreateCylinder(leftValue); // 1'den ne kadar b�y�kse o b�y�kl�kte yeni bir silindir yarat

        }
        else if (_value < 0) // alt�m�zdaki silindir yok olduysa
        {
            PlayerController.Current.DestroyCylinder(this);
            // Karakterimize bu silindiri yok etmesini s�yl�yoruz
        }
        else
        {
            // Silindir tam �i�mediyse, karakterle aras�ndaki mesafeyi ayarlamak i�in boyutunun g�ncellenmesi
            int cylinderCount = PlayerController.Current.cylinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylinderCount - 1) - 0.25f * _value, transform.localPosition.z);
            transform.localScale = new Vector3(0.5f * _value, transform.localScale.y, 0.5f * _value);
        }
    }
}
