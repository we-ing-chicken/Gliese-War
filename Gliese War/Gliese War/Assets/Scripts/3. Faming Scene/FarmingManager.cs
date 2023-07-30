using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class FarmingManager : MonoBehaviour
{
    private static FarmingManager _instance;

    enum Scene
    {
        LoginScene = 0,
        LobbyScene = 1,
        FarmingScene = 2,
        BattleScene = 3,
        END
    }
    private bool _isEnd = false;
    public bool _isPause = false;
    public bool _isFading = true;
    public bool _isInven = false;

    [Header("Timer")]
    //[SerializeField] private float fadeTime = 2f;
    private float _playTime = 0.0f; // 플레이한 시간
    private float FARMING_TIME = 180f; // 게임 길이
    //private float FARMING_TIME = 30; // 게임 길이
    [SerializeField] private GameObject timerTxt; //타이머 텍스트
    private TextMeshProUGUI _timerTxtComp; // 타이머 텍스트 컴포넌트

    [Header("Light")] [SerializeField] private Light DirectionalLight; // 태양 오브젝트
    private float _timeOfInGame;

    [Header("DayNight")] public bool _isNight;
    [SerializeField] private Image DayNightImage; // 낮밤 이미지 오브젝트
    [SerializeField] private Sprite DayImage; // 낮 이미지
    [SerializeField] private Sprite NightImage; // 밤 이미지
    [SerializeField] private Material dayMaterial;
    [SerializeField] private Material nightMaterial;

    [Header("Canvas")] public GameObject invenCanvas;
    public Canvas pauseCanvas;
    public Canvas fadeCanvas;
    public Canvas hitCanvas;
    public Canvas worldUICanvas;
    private Coroutine GCoroutine;

    public Inventory inventory;
    public CPlayer managed_player;

    public GameObject characterCam;
    public Slider playerCurrentHPBar;

    public GameObject startPostion;

    [SerializeField] private Sprite[] weaponImage;

    [SerializeField] private Image weaponNum1Image;
    [SerializeField] private Image weaponNum2Image;
    
    [SerializeField] private Image weapon1Image;
    [SerializeField] private Image weapon2Image;

    [Header("Debug")][SerializeField] private Playercam playercam;
   [SerializeField] private GameObject CvCam;

    //[SerializeField] private GameObject[] characters;

    public static FarmingManager Instance
    {
        get
        {
            if (!_instance)
            {
                if (_instance == null)
                    return null;

                _instance = FindObjectOfType(typeof(FarmingManager)) as FarmingManager;
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Cursor.visible = false;
        
        GameObject temp = Instantiate(GameManager.Instance.farmingCharacters[GameManager.Instance.charNum], CPlayer.Instance.gameObject.transform);
        //temp.transform.position = new Vector3(116.67f, 19.52f, 185.36f);
        CPlayer.Instance.animator = temp.GetComponent<Animator>();
        CPlayer.Instance.playertransform = temp.transform;
        CPlayer.Instance.handR = temp.GetComponent<WeaponAttack>().handR;
        CPlayer.Instance.back = temp.GetComponent<WeaponAttack>().back;
        CPlayer.Instance.attackEffectPos = temp.GetComponent<WeaponAttack>().attackEffectPos;
        CPlayer.Instance.shoesEffectPos = temp.GetComponent<WeaponAttack>().shoesEffectPos;


        GameObject temp2 = Instantiate(GameManager.Instance.farmingCharacters[GameManager.Instance.charNum], CPlayerUI.instance.gameObject.transform);
        CPlayerUI.instance.GetComponent<CPlayerUI>().animator = temp2.GetComponent<Animator>();
        CPlayerUI.instance.GetComponent<CPlayerUI>().playertransform = temp2.transform;

        SwitchGameObjectActive(characterCam);
        //_timeOfInGame = 8.4f;
        _timeOfInGame = 6f;
        _timerTxtComp = timerTxt.GetComponent<TextMeshProUGUI>();
        _isNight = false;
        fadeCanvas.transform.GetChild(0).GetComponent<Image>().material.color = new Vector4(0f, 0f, 0f);
        playerCurrentHPBar = playerCurrentHPBar.GetComponent<Slider>();
        
        SetTimerText();
        UpdateLighting(_timeOfInGame / 24f);
        
        _isFading = true;
        SwitchCanvasActive(fadeCanvas);
        StartCoroutine(FadeIn(2f));
        SwitchGameObjectActive(characterCam);
    }

    private void Update()
    {
        if (_isFading) return;

        if (!_isEnd && !_isPause)
        {
            TimeCheck();

            // if (_isNight)
            //     _timeOfInGame += 0.1f * 1f * Time.deltaTime;
            // else
            //     _timeOfInGame += 0.1f * 0.8f * Time.deltaTime;
            _timeOfInGame += 0.1f  * 2f * Time.deltaTime;

            _timeOfInGame %= 24;
            UpdateLighting(_timeOfInGame / 24f);
            UpdateDayNightImage();

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!pauseCanvas.gameObject.activeSelf)
                {
                    SwitchCanvasActive(invenCanvas);
                    SwitchGameObjectActive(characterCam);
                }

                if (!invenCanvas.activeSelf)
                {
                    _isInven = false;
                    CvCam.SetActive(true);
                    Cursor.visible = false;
                    
                }
                else
                {
                    _isInven = true;
                    CvCam.SetActive(false);
                    Cursor.visible = true;
                    
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (invenCanvas.gameObject.activeSelf)
            {
                SwitchCanvasActive(invenCanvas);
                SwitchGameObjectActive(characterCam);
                _isInven = false;
                CvCam.SetActive(true);
                Cursor.visible = false;
            }
            else
            {
                SwitchCanvasActive(pauseCanvas);

                if (pauseCanvas.gameObject.activeSelf)
                {
                    _isPause = true;
                    CvCam.SetActive(false);
                    Cursor.visible = true;
                }
                else
                {
                    _isPause = false;
                    CvCam.SetActive(true);
                    Cursor.visible = false;
                }
            }
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha3))
            inventory.AcquireItem(inventory.helmet[1]);
        else if(Input.GetKeyDown(KeyCode.Alpha4))
            inventory.AcquireItem(inventory.armor[1]);
        else if(Input.GetKeyDown(KeyCode.Alpha5))
            inventory.AcquireItem(inventory.shoes[1]);
        else if(Input.GetKeyDown(KeyCode.Alpha6))
            inventory.AcquireItem(inventory.hammer[1]);
        else if(Input.GetKeyDown(KeyCode.Alpha7))
            inventory.AcquireItem(inventory.spear[1]);
        else if(Input.GetKeyDown(KeyCode.Alpha8))
            inventory.AcquireItem(inventory.sword[1]);
        else if (Input.GetKeyDown(KeyCode.F2)) 
            PlayBattlePhase();
        else if(Input.GetKeyDown(KeyCode.F3))
            CPlayer.Instance.Heal(20);

    }

    IEnumerator FadeIn(float fadeTime)
    {
        float alpha = fadeCanvas.transform.GetChild(0).GetComponent<Image>().color.a;
        fadeCanvas.transform.GetChild(0).GetComponent<Image>().material.color = new Vector4(1f, 1f, 1f,1f);
        //float mat_Rgb = fadeCanvas.transform.GetChild(0).GetComponent<Image>().material.color.r;
        //float mat_Blur = fadeCanvas.transform.GetChild(0).GetComponent<Image>().material.GetFloat("Radius");
        //UnityEngine.Debug.Log("mat_Blur : " + mat_Blur);
        
        GameObject loadingText = fadeCanvas.transform.GetChild(0).GetChild(0).gameObject;

        yield return new WaitForSeconds(fadeTime);
        
        loadingText.SetActive(false);
        
        while (true)
        {
            float t = 20f / 255;
            //float blur = 2f / 7;
            //mat_Rgb += t;
            //mat_Blur -= blur;

            alpha -= t;
            fadeCanvas.transform.GetChild(0).GetComponent<Image>().color = new Vector4(0,0,0, alpha);
            //fadeCanvas.transform.GetChild(0).GetComponent<Image>().material.color = new Vector4(mat_Rgb, mat_Rgb, mat_Rgb);
            //fadeCanvas.transform.GetChild(0).GetComponent<Image>().material.SetFloat("Radius", mat_Blur);
            //UnityEngine.Debug.Log(fadeCanvas.transform.GetChild(0).GetComponent<Image>().material.color);
            yield return new WaitForSeconds(0.01f);
            //if (alpha <= 0 || mat_Rgb <= 0)
            if (alpha <= 0)
                break;
        }
        
        _isFading = false;
        SwitchCanvasActive(fadeCanvas);
        
        yield return null;
    }

    IEnumerator FadeOut()
    {
        SwitchCanvasActive(fadeCanvas);
        _isFading = true;
        float alpha = fadeCanvas.transform.GetChild(0).GetComponent<Image>().color.a;
        
        while (true)
        {
            float t = 10f / 255;
            alpha += t;
            fadeCanvas.transform.GetChild(0).GetComponent<Image>().color = new Vector4(0,0,0, alpha);
            yield return new WaitForSeconds(0.01f);
            if (alpha >= 1f)
                break;
        }

        if (_isEnd)
            PlayBattlePhase();
        
        yield return null;
    }

    private void SwitchCanvasActive(Canvas temp)
    {
        if (temp.gameObject.activeSelf)
            temp.gameObject.SetActive(false);
        else
            temp.gameObject.SetActive(true);
    }
    
    private void SwitchCanvasActive(GameObject temp)
    {
        if (temp.gameObject.activeSelf)
            temp.gameObject.SetActive(false);
        else
            temp.gameObject.SetActive(true);
    }
    private void SwitchGameObjectActive(GameObject temp)
    {
        if (temp.gameObject.activeSelf)
            temp.gameObject.SetActive(false);
        else
            temp.gameObject.SetActive(true);
    }

    // 시간에 따른 라이트 변화 함수
    private void UpdateLighting(float timePercent)
    {
        // RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        // RenderSettings.fogColor = Preset.FogoColor.Evaluate(timePercent);
        //
        //DirectionalLight.color = Preset.DirectionColor.Evaluate(timePercent);
        //DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, -170, 0));
        DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 80f, -170, 0));
    }

    // 낮 밤 이미지 교체 함수
    void UpdateDayNightImage()
    {
        if (_timeOfInGame >= 6 && _timeOfInGame <= 18)
        {
            DayNightImage.sprite = DayImage;
            RenderSettings.skybox = dayMaterial;
            _isNight = false;
        }
        else
        {
            DayNightImage.sprite = NightImage;
            RenderSettings.skybox = nightMaterial;
            _isNight = true;
        }
    }

    // 플레이 시간 체크 함수
    void TimeCheck()
    {
        _playTime += Time.deltaTime;
        SetTimerText();

        if (FARMING_TIME <= _playTime)
        {
            //_playTime = FARMING_TIME;
            _isEnd = true;
            _isFading = true;
            StartCoroutine(FadeOut());
        }
    }

    // 타이머 텍스트 출력 함수
    private void SetTimerText()
    {
        var time = FARMING_TIME - _playTime;
        var m = time / 60;
        var s = time % 60;
        _timerTxtComp.text = (int)m + " : " + (s >= 10 ? (int)s : ("0" + (int)s));
    }

    public void Restart()
    {
        SceneManager.LoadScene((int)(Scene.FarmingScene));
    }

    public void Exit()
    {
        SceneManager.LoadScene((int)(Scene.LobbyScene));
    }

    public void Resume()
    {
        _isPause = false;
        Cursor.visible = false;
        SwitchCanvasActive(pauseCanvas);
    }

    public void PlayBattlePhase()
    {
        GameManager.Instance.SaveItems(CPlayer.Instance.helmet, CPlayer.Instance.armor, CPlayer.Instance.shoe, CPlayer.Instance.weapon1, CPlayer.Instance.weapon2);
        GameManager.Instance.SaveStat(CPlayer.Instance.maxHealth, CPlayer.Instance.offensivePower, CPlayer.Instance.defensivePower, CPlayer.Instance.moveSpeed);
        SceneManager.LoadScene((int)(Scene.BattleScene));
    }
    

    public void StartFadeOut()
    {
        StartCoroutine(FadeInOutCoroutine());
    }

    IEnumerator FadeInOutCoroutine()
    {
        StartCoroutine(FadeOut());
        yield return new WaitForSeconds(3f);
        StartCoroutine(FadeIn(0f));
    }

    public void HitScreen()
    {
        StartCoroutine(HitCoroutine());
    }

    IEnumerator HitCoroutine()
    {
        for (int i = 0; i < 6; ++i)
        {
            SwitchCanvasActive(hitCanvas);
            yield return new WaitForSeconds(0.1f);
        }

        yield return null;
    }
    
    public void ActiveG(GameObject obj)
    {
        GCoroutine = StartCoroutine(FollowG(obj));
    }

    public void UnActiveG()
    {
        GameObject GImage = worldUICanvas.gameObject.transform.GetChild(0).gameObject;
        GImage.SetActive(false);
        StopCoroutine(GCoroutine);
    }

    IEnumerator FollowG(GameObject obj)
    {
        GameObject GImage = worldUICanvas.gameObject.transform.GetChild(0).gameObject;
        GImage.SetActive(true);
        while (true)
        {
            GImage.transform.position = Camera.main.WorldToScreenPoint(obj.transform.position + new Vector3(0,4f,0));
            yield return null;
        }
    }

    public void SetEquipWeaponImage()
    {
        if (CPlayer.Instance.weapon1 == null)
        {
            Color color = weapon1Image.color;
            color.a = 0f;
            weapon1Image.color = color;
            
            color = weaponNum1Image.color;
            color.a = 0.5f;
            weaponNum1Image.color = color;
        }
        else
        {
            switch (CPlayer.Instance.weapon1.item.weaponType)
            {
                case Item.WeaponType.Hammer:
                    weapon1Image.sprite = weaponImage[0];
                    break;
                
                case Item.WeaponType.Spear:
                    weapon1Image.sprite = weaponImage[1];
                    break;
                
                case Item.WeaponType.Sword:
                    weapon1Image.sprite = weaponImage[2];
                    break;
            }
            
            if (CPlayer.Instance.weaponNow == 1)
            {
                Color color = weapon1Image.color;
                color.a = 1f;
                weapon1Image.color = color;

                color = weaponNum1Image.color;
                color.a = 1f;
                weaponNum1Image.color = color;
            }
            else
            {
                Color color = weapon1Image.color;
                color.a = 0.5f;
                weapon1Image.color = color;

                color = weaponNum1Image.color;
                color.a = 0.5f;
                weaponNum1Image.color = color;
            }
        }

        if (CPlayer.Instance.weapon2 == null)
        {
            Color color = weapon2Image.color;
            color.a = 0f;
            weapon2Image.color = color;
            
            color = weaponNum2Image.color;
            color.a = 0.5f;
            weaponNum2Image.color = color;
        }
        else
        {
            switch (CPlayer.Instance.weapon2.item.weaponType)
            {
                case Item.WeaponType.Hammer:
                    weapon2Image.sprite = weaponImage[0];
                    break;
                
                case Item.WeaponType.Spear:
                    weapon2Image.sprite = weaponImage[1];
                    break;
                
                case Item.WeaponType.Sword:
                    weapon2Image.sprite = weaponImage[2];
                    break;
            }
            
            if (CPlayer.Instance.weaponNow == 2)
            {
                Color color = weapon2Image.color;
                color.a = 1f;
                weapon2Image.color = color;
            
                color = weaponNum2Image.color;
                color.a = 1f;
                weaponNum2Image.color = color;
            }
            else
            {
                Color color = weapon2Image.color;
                color.a = 0.5f;
                weapon2Image.color = color;
            
                color = weaponNum2Image.color;
                color.a = 0.5f;
                weaponNum2Image.color = color;
            }
        }
        
    }

    public void InvenOff()
    {
        SwitchCanvasActive(invenCanvas);
        SwitchGameObjectActive(characterCam);
        _isInven = false;
    }
}