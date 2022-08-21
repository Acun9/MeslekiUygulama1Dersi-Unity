using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Current; // yarat�lan her silindirin playercontroller s�n�f�na erisebilmesi icin static degisken (current: suanki player�m�z)

    public float limitX; //karakterin platformdan c�kmamas� icin (sa�, sol)

    public float runningSpeed; // karakterin maks h�z�n� tutacak
    public float xSpeed;//karakterin sa�a sola kayma h�z�
    private float _currentRunningSpeed;

    public GameObject ridingCylinderPrefab;
    public List<RidingCylinder> cylinders; //alt�m�zdaki silindirleri tuttugumuz liste

    private bool _spawningBridge;// kopr� olusturma kontrol degiskeni
    public GameObject bridgePiecePrefab;//kopr� olusturmak icin yarat�lan kopru parcalar� degiskeni
    private BridgeSpawner _bridgeSpawner;//kopru olustururken baslang�c-bitis noktalar�n� alacag�m�z degisken
    private float _createBridgeTimer;// update fonks icerisinde kopr� parcas� yaratt�g�m�z s�reyi tutan degisken (s�rekli parca olusturulmas�n diye)

    private bool _finished;//karakterin bitis cizgisine gelip gelmedigini tutan degisken

    private float _scoreTimer = 0;//bitis cizgisinden sonra ilerlerken skor kazanmak icin degisken

    public Animator animator;//player�n animator� (oyun baslad�g�nda nefes al�p verirken baslaya bas�ld�g�nda kosmas�, tuzaklarda �lmesi vs)

    private float _lastTouchedX;//oyuncunun ekrana dokundugu son yatay pozisyon
    private float _dropSoundTimer;//tuzakla carp�s�l�rken ya da kopru yarat�l�rken silindir kuculme sesi s�rekli calacag� icin bir timer degiskeni

    public AudioSource cylinderAudioSource, triggerAudioSource, itemAudioSource;//unitydeki objelerin ses componentlerine (audio source) eri�mek i�in de�i�kenler
    public AudioClip gatherAudioclip, dropAudioClip, coinAudioClip, buyAudioClip, equipItemAudioClip, unequipItemAudioClip;//sesleri audio sourcelere kod ile atamak icin degiskenler (orneg�n silindir icin 2 ses var buyurken ve kuculurken)

    public List<GameObject> wearSpots;//sapka ve gozlugun sahnede dogru yerde olusturulmas� �c�n unityde tan�mlad�g�m�z yerleri tutan deg�sken (kafa 0, surat 1)

    // Update is called once per frame
    void Update()
    {
        if (LevelController.Current == null || !LevelController.Current.gameActive)//e�er level controller yoksa veya level controllerda oyun aktif degilse update fonks bitir
        {
            return;
        }
        float newX = 0;
        float touchXDelta = 0;
        if (Input.touchCount > 0) //dokunmatik ekran icin
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)//ekrana ilk dokunu� ise
            {
                _lastTouchedX = Input.GetTouch(0).position.x;//last'� ilk dokunulan pozisyona e�itle
            }else if (Input.GetTouch(0).phase == TouchPhase.Moved)//ilk dokunu� de�ilse, parmak hareket ediyorsa
            {
                touchXDelta = 5 * (Input.GetTouch(0).position.x - _lastTouchedX) / Screen.width;//parmag� kayd�rma fark� kadar ilerlemesi icin (5: dokunus hassasiyeti icin carpan)
                _lastTouchedX = Input.GetTouch(0).position.x;
            }
            
        }else if(Input.GetMouseButton(0)) //fare icin
        {
            touchXDelta = Input.GetAxis("Mouse X");
        }

        newX = transform.position.x + xSpeed * touchXDelta * Time.deltaTime;
        newX = Mathf.Clamp(newX, -limitX, limitX);//x i sa� ve solda s�n�rland�r�yoruz (min -limitx kadar max limitx kadar hareket etsin)

        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z + _currentRunningSpeed * Time.deltaTime); // karakterin ilerlemesi
        transform.position = newPosition;

        if (_spawningBridge)//suan kopru yarat�l�yor mu
        {
            PlayDropSound();
            _createBridgeTimer -= Time.deltaTime;// bridge timerdan geriye saniye say�yormus gibi
            if(_createBridgeTimer < 0)// kopru parcas� olusturma s�resi bittiyse yeni kopru parcas�n�n olusturuldugu ve zamanlay�c�n�n guncellendigi k�s�m
            {
                _createBridgeTimer = 0.01f;
                IncrementCylinderVolume(-0.01f);// karakterin alt�ndaki silindiri k�c�lt
                GameObject createdBridgePiece = Instantiate(bridgePiecePrefab, this.transform);//kopru parcas�n� yarat
                createdBridgePiece.transform.SetParent(null);//koprunun diger bolume gecmemes� �c�n parent�n� sil

                //konumunu ve y�n�n� ayarla
                Vector3 direction = _bridgeSpawner.endReference.transform.position - _bridgeSpawner.startReference.transform.position;
                float distance = direction.magnitude;
                direction = direction.normalized;
                createdBridgePiece.transform.forward = direction;

                float characterDistance = transform.position.z - _bridgeSpawner.startReference.transform.position.z;//karakterin baslang�c noktas�ndan ne kadar uzakta oldugunu hesapla
                characterDistance = Mathf.Clamp(characterDistance, 0, distance);// clamp fonks: s�n�rland�rma ( 0 ile baslang�c-bitis aras�ndaki maks uzakl�k aras�nda)
                Vector3 newPiecePosition = _bridgeSpawner.startReference.transform.position + direction * characterDistance;//kopru parcas�n�n yeni pozisyonu karakterin alt�nda olcak sekilde
                newPiecePosition.x = transform.position.x;//parcan�n karakterle beraber saga soal gitmesi
                createdBridgePiece.transform.position = newPiecePosition;//parcan�n pozisyonunu guncelle

                if (_finished)//kopruyu bitis cizgisinden sonra olusturuyorsan skor ekle
                {
                    _scoreTimer -= Time.deltaTime;
                    if (_scoreTimer < 0)
                    {
                        _scoreTimer = 0.3f;// her 1/3 sn'de +1 skor kazans�n
                        LevelController.Current.ChangeScore(1);
                    }
                }
            }
        }
    }

    public void ChangeSpeed(float value)//oyunu yani karakterin kosmas�n� level controllardan baslatmak icin (butona bas�p baslatmak)
    {
        _currentRunningSpeed = value;
    }

    private void OnTriggerEnter(Collider other) //karakter bir objeyle �arp��t���nda bir kere �al���r
    {
        if(other.tag == "AddCylinder") // carp�st�g�m�z obje addcyl�nder ise
        {
            cylinderAudioSource.PlayOneShot(gatherAudioclip, 0.1f);//silindir toplama sesini 0.1f yuksekliginde 1 defa �al
            IncrementCylinderVolume(0.1f);
            Destroy(other.gameObject);
        }else if(other.tag == "SpawnBridge")// carp�st�g�m�z obje k�pr� yaratma objesi ise
        {
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }else if(other.tag == "StopSpawnBridge")
        {
            stopSpawningBridge();
            if (_finished)// bitis cizgisinden sonraki stopspawnbridge'ye ulast�ysak yani karakterin maks ilerleyebilecegi uzakl�ga geldiysek
            {
                LevelController.Current.FinishGame();
            }
        }else if (other.tag == "Finish")// capr�st�g�m�z obje bitis cizgisi ise
        {
            _finished = true;
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
        else if (other.tag == "Coin")// carp�st�g�m�z obje coin ise
        {
            triggerAudioSource.PlayOneShot(coinAudioClip, 0.1f);
            other.tag = "Untagged";//birden fazla silindir elmasa carpt�g�nda 10dan fazla para kazanmamak �c�n �lk carp�smadan sonra elmas�n tag�n� deg�st�r�yoruz
            LevelController.Current.ChangeScore(10);
            Destroy(other.gameObject);// carp�smadan sonra elmas� yok et
        }
    }

    private void OnTriggerStay(Collider other) //karakter bir objeyle �arp��t��� her an �al���r (tuzaklarda kald��� durum)
    {
        if (LevelController.Current.gameActive)//karakter tuzag�n ustunde oluyken ses calmaya devam etmes�n d�ye
        {
            if (other.tag == "Trap") //�arp�lan obje trap ise
            {
                PlayDropSound();
                IncrementCylinderVolume(-Time.fixedDeltaTime);//silindirleri azalt
            }
        }
    }

    public void IncrementCylinderVolume(float value) //silindir hacmini artt�r-azalt fonks
    {
        if (cylinders.Count == 0)//listenin eleman say�s� 0 ise yani alt�m�zda hic silindir yoksa
        {
            if (value > 0)
            {
                CreateCylinder(value); //silindir ekle
            }
            else //game over
            {
                if (_finished)//eger karakter bitis cizgisini gectikten sonra silindirler bittiyse
                {
                    LevelController.Current.FinishGame();
                }
                else //bitis cizgisinden �nce silindirler bittiyse
                {
                    Die();
                }
            }
        }
        else // alt�m�zda silindir varsa en a�a��daki silindirin boyutunu artt�r 
        {
            cylinders[cylinders.Count - 1].IncrementCylinderVolume(value); 
        }
    }
    
    public void Die()
    {
        animator.SetBool("dead", true);//�l�m animasyonunun unitydeki degiskenini true yap
        gameObject.layer = 6;// karakter platformlar aras�nda �l�rse asag� d�ss�n diye katman�n� degistir (unityde de edit/projec settings/physics k�sm�ndan bu iki katman�n etkilesimini kald�rm�s olmak gerek)
        Camera.main.transform.SetParent(null);//kamera karakterle beraber asag� dusmesin diye parentini null yap
        LevelController.Current.GameOver();
    }

    public void CreateCylinder(float value)
    {
        RidingCylinder createdCylinder = Instantiate(ridingCylinderPrefab, transform).GetComponent<RidingCylinder>(); //instantiate fonks 1. parametredeki objeden bir tane daha yarat�yor 2. parametre ise oran�n child� olmas� �c�n
        cylinders.Add(createdCylinder); //yaratt�g�m�z objeyi silindirler listesine ekliyor
        createdCylinder.IncrementCylinderVolume(value); //yeni silindirin boyutunu ayarl�yor
    }
    public void DestroyCylinder(RidingCylinder cylinder) //silindir yok et
    {
        cylinders.Remove(cylinder); //listeden bir silindir ��kar
        Destroy(cylinder.gameObject); //silindir objesini sahneden ��kar
    }

    public void StartSpawningBridge(BridgeSpawner spawner)
    {
        _bridgeSpawner = spawner;
        _spawningBridge = true;
    }

    public void stopSpawningBridge()
    {
        _spawningBridge = false;
    }

    public void PlayDropSound()//tuzakla carp�s�l�rken ya da kopru yarat�l�rken silindir kuculme sesi icin fonks
    {
        _dropSoundTimer -= Time.deltaTime;
        if (_dropSoundTimer < 0)
        {
            _dropSoundTimer = 0.15f;//sesi calma s�kl�g� icin kucuk bir deger
            cylinderAudioSource.PlayOneShot(dropAudioClip, 0.1f);
        }
    }
}
