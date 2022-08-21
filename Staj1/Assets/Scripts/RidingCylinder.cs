using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidingCylinder : MonoBehaviour
{
    private bool _filled; //silindir maks hacminde mi kontrol degiskeni
    private float _value; //silindirin sayýsal olarak maks hacmi

    public void IncrementCylinderVolume(float value) // silindirin boyunu arttýran azaltan fonks
    {
        _value += value;
        if (_value > 1) //silindir en büyük haline geldiyse
        {


            float leftValue = _value - 1; //1 silindir hacminden kalan deger
            int cylinderCount = PlayerController.Current.cylinders.Count;//karakterimizin altýndaki silindir sayýsý
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylinderCount - 1) - 0.25f, transform.localPosition.z);// local pozisyon: bir objenin parentýna baglý pozisyonu (silindir karakterin ayagýna girmesin diye)
            transform.localScale = new Vector3(0.5f, transform.localScale.y, 0.5f);// Silindirin boyutunu tam olarak 1 yap
            PlayerController.Current.CreateCylinder(leftValue); // 1'den ne kadar büyükse o büyüklükte yeni bir silindir yarat

        }
        else if (_value < 0) // altýmýzdaki silindir yok olduysa
        {
            PlayerController.Current.DestroyCylinder(this);
            // Karakterimize bu silindiri yok etmesini söylüyoruz
        }
        else
        {
            // Silindir tam þiþmediyse, karakterle arasýndaki mesafeyi ayarlamak için boyutunun güncellenmesi
            int cylinderCount = PlayerController.Current.cylinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylinderCount - 1) - 0.25f * _value, transform.localPosition.z);
            transform.localScale = new Vector3(0.5f * _value, transform.localScale.y, 0.5f * _value);
        }
    }
}
