using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Current; // yaratýlan her silindirin playercontroller sýnýfýna erisebilmesi icin static degisken (current: suanki playerýmýz)

    public float limitX; //karakterin platformdan cýkmamasý icin (sað, sol)

    public float runningSpeed; // karakterin maks hýzýný tutacak
    public float xSpeed;//karakterin saða sola kayma hýzý
    private float _currentRunningSpeed;

    public GameObject ridingCylinderPrefab;
    public List<RidingCylinder> cylinders; //altýmýzdaki silindirleri tuttugumuz liste

    private bool _spawningBridge;// koprü olusturma kontrol degiskeni
    public GameObject bridgePiecePrefab;//koprü olusturmak icin yaratýlan kopru parcalarý degiskeni
    private BridgeSpawner _bridgeSpawner;//kopru olustururken baslangýc-bitis noktalarýný alacagýmýz degisken
    private float _createBridgeTimer;// update fonks icerisinde koprü parcasý yarattýgýmýz süreyi tutan degisken (sürekli parca olusturulmasýn diye)

    private bool _finished;//karakterin bitis cizgisine gelip gelmedigini tutan degisken

    private float _scoreTimer = 0;//bitis cizgisinden sonra ilerlerken skor kazanmak icin degisken

    public Animator animator;//playerýn animatorü (oyun basladýgýnda nefes alýp verirken baslaya basýldýgýnda kosmasý, tuzaklarda ölmesi vs)

    private float _lastTouchedX;//oyuncunun ekrana dokundugu son yatay pozisyon
    private float _dropSoundTimer;//tuzakla carpýsýlýrken ya da kopru yaratýlýrken silindir kuculme sesi sürekli calacagý icin bir timer degiskeni

    public AudioSource cylinderAudioSource, triggerAudioSource, itemAudioSource;//unitydeki objelerin ses componentlerine (audio source) eriþmek için deðiþkenler
    public AudioClip gatherAudioclip, dropAudioClip, coinAudioClip, buyAudioClip, equipItemAudioClip, unequipItemAudioClip;//sesleri audio sourcelere kod ile atamak icin degiskenler (ornegýn silindir icin 2 ses var buyurken ve kuculurken)

    public List<GameObject> wearSpots;//sapka ve gozlugun sahnede dogru yerde olusturulmasý ýcýn unityde tanýmladýgýmýz yerleri tutan degýsken (kafa 0, surat 1)

    // Update is called once per frame
    void Update()
    {
        if (LevelController.Current == null || !LevelController.Current.gameActive)//eðer level controller yoksa veya level controllerda oyun aktif degilse update fonks bitir
        {
            return;
        }
        float newX = 0;
        float touchXDelta = 0;
        if (Input.touchCount > 0) //dokunmatik ekran icin
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)//ekrana ilk dokunuþ ise
            {
                _lastTouchedX = Input.GetTouch(0).position.x;//last'ý ilk dokunulan pozisyona eþitle
            }else if (Input.GetTouch(0).phase == TouchPhase.Moved)//ilk dokunuþ deðilse, parmak hareket ediyorsa
            {
                touchXDelta = 5 * (Input.GetTouch(0).position.x - _lastTouchedX) / Screen.width;//parmagý kaydýrma farký kadar ilerlemesi icin (5: dokunus hassasiyeti icin carpan)
                _lastTouchedX = Input.GetTouch(0).position.x;
            }
            
        }else if(Input.GetMouseButton(0)) //fare icin
        {
            touchXDelta = Input.GetAxis("Mouse X");
        }

        newX = transform.position.x + xSpeed * touchXDelta * Time.deltaTime;
        newX = Mathf.Clamp(newX, -limitX, limitX);//x i sað ve solda sýnýrlandýrýyoruz (min -limitx kadar max limitx kadar hareket etsin)

        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z + _currentRunningSpeed * Time.deltaTime); // karakterin ilerlemesi
        transform.position = newPosition;

        if (_spawningBridge)//suan kopru yaratýlýyor mu
        {
            PlayDropSound();
            _createBridgeTimer -= Time.deltaTime;// bridge timerdan geriye saniye sayýyormus gibi
            if(_createBridgeTimer < 0)// kopru parcasý olusturma süresi bittiyse yeni kopru parcasýnýn olusturuldugu ve zamanlayýcýnýn guncellendigi kýsým
            {
                _createBridgeTimer = 0.01f;
                IncrementCylinderVolume(-0.01f);// karakterin altýndaki silindiri kücült
                GameObject createdBridgePiece = Instantiate(bridgePiecePrefab, this.transform);//kopru parcasýný yarat
                createdBridgePiece.transform.SetParent(null);//koprunun diger bolume gecmemesý ýcýn parentýný sil

                //konumunu ve yönünü ayarla
                Vector3 direction = _bridgeSpawner.endReference.transform.position - _bridgeSpawner.startReference.transform.position;
                float distance = direction.magnitude;
                direction = direction.normalized;
                createdBridgePiece.transform.forward = direction;

                float characterDistance = transform.position.z - _bridgeSpawner.startReference.transform.position.z;//karakterin baslangýc noktasýndan ne kadar uzakta oldugunu hesapla
                characterDistance = Mathf.Clamp(characterDistance, 0, distance);// clamp fonks: sýnýrlandýrma ( 0 ile baslangýc-bitis arasýndaki maks uzaklýk arasýnda)
                Vector3 newPiecePosition = _bridgeSpawner.startReference.transform.position + direction * characterDistance;//kopru parcasýnýn yeni pozisyonu karakterin altýnda olcak sekilde
                newPiecePosition.x = transform.position.x;//parcanýn karakterle beraber saga soal gitmesi
                createdBridgePiece.transform.position = newPiecePosition;//parcanýn pozisyonunu guncelle

                if (_finished)//kopruyu bitis cizgisinden sonra olusturuyorsan skor ekle
                {
                    _scoreTimer -= Time.deltaTime;
                    if (_scoreTimer < 0)
                    {
                        _scoreTimer = 0.3f;// her 1/3 sn'de +1 skor kazansýn
                        LevelController.Current.ChangeScore(1);
                    }
                }
            }
        }
    }

    public void ChangeSpeed(float value)//oyunu yani karakterin kosmasýný level controllardan baslatmak icin (butona basýp baslatmak)
    {
        _currentRunningSpeed = value;
    }

    private void OnTriggerEnter(Collider other) //karakter bir objeyle çarpýþtýðýnda bir kere çalýþýr
    {
        if(other.tag == "AddCylinder") // carpýstýgýmýz obje addcylýnder ise
        {
            cylinderAudioSource.PlayOneShot(gatherAudioclip, 0.1f);//silindir toplama sesini 0.1f yuksekliginde 1 defa çal
            IncrementCylinderVolume(0.1f);
            Destroy(other.gameObject);
        }else if(other.tag == "SpawnBridge")// carpýstýgýmýz obje köprü yaratma objesi ise
        {
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }else if(other.tag == "StopSpawnBridge")
        {
            stopSpawningBridge();
            if (_finished)// bitis cizgisinden sonraki stopspawnbridge'ye ulastýysak yani karakterin maks ilerleyebilecegi uzaklýga geldiysek
            {
                LevelController.Current.FinishGame();
            }
        }else if (other.tag == "Finish")// caprýstýgýmýz obje bitis cizgisi ise
        {
            _finished = true;
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
        else if (other.tag == "Coin")// carpýstýgýmýz obje coin ise
        {
            triggerAudioSource.PlayOneShot(coinAudioClip, 0.1f);
            other.tag = "Untagged";//birden fazla silindir elmasa carptýgýnda 10dan fazla para kazanmamak ýcýn ýlk carpýsmadan sonra elmasýn tagýný degýstýrýyoruz
            LevelController.Current.ChangeScore(10);
            Destroy(other.gameObject);// carpýsmadan sonra elmasý yok et
        }
    }

    private void OnTriggerStay(Collider other) //karakter bir objeyle çarpýþtýðý her an çalýþýr (tuzaklarda kaldýðý durum)
    {
        if (LevelController.Current.gameActive)//karakter tuzagýn ustunde oluyken ses calmaya devam etmesýn dýye
        {
            if (other.tag == "Trap") //çarpýlan obje trap ise
            {
                PlayDropSound();
                IncrementCylinderVolume(-Time.fixedDeltaTime);//silindirleri azalt
            }
        }
    }

    public void IncrementCylinderVolume(float value) //silindir hacmini arttýr-azalt fonks
    {
        if (cylinders.Count == 0)//listenin eleman sayýsý 0 ise yani altýmýzda hic silindir yoksa
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
                else //bitis cizgisinden önce silindirler bittiyse
                {
                    Die();
                }
            }
        }
        else // altýmýzda silindir varsa en aþaðýdaki silindirin boyutunu arttýr 
        {
            cylinders[cylinders.Count - 1].IncrementCylinderVolume(value); 
        }
    }
    
    public void Die()
    {
        animator.SetBool("dead", true);//ölüm animasyonunun unitydeki degiskenini true yap
        gameObject.layer = 6;// karakter platformlar arasýnda ölürse asagý düssün diye katmanýný degistir (unityde de edit/projec settings/physics kýsmýndan bu iki katmanýn etkilesimini kaldýrmýs olmak gerek)
        Camera.main.transform.SetParent(null);//kamera karakterle beraber asagý dusmesin diye parentini null yap
        LevelController.Current.GameOver();
    }

    public void CreateCylinder(float value)
    {
        RidingCylinder createdCylinder = Instantiate(ridingCylinderPrefab, transform).GetComponent<RidingCylinder>(); //instantiate fonks 1. parametredeki objeden bir tane daha yaratýyor 2. parametre ise oranýn childý olmasý ýcýn
        cylinders.Add(createdCylinder); //yarattýgýmýz objeyi silindirler listesine ekliyor
        createdCylinder.IncrementCylinderVolume(value); //yeni silindirin boyutunu ayarlýyor
    }
    public void DestroyCylinder(RidingCylinder cylinder) //silindir yok et
    {
        cylinders.Remove(cylinder); //listeden bir silindir çýkar
        Destroy(cylinder.gameObject); //silindir objesini sahneden çýkar
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

    public void PlayDropSound()//tuzakla carpýsýlýrken ya da kopru yaratýlýrken silindir kuculme sesi icin fonks
    {
        _dropSoundTimer -= Time.deltaTime;
        if (_dropSoundTimer < 0)
        {
            _dropSoundTimer = 0.15f;//sesi calma sýklýgý icin kucuk bir deger
            cylinderAudioSource.PlayOneShot(dropAudioClip, 0.1f);
        }
    }
}
