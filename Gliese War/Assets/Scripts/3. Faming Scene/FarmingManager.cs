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

    private bool _isEnd = false;
    public bool _isPause = false;
    public bool _isFading = true;
    public bool _isInven = false;

    [Header("Timer")]
    //[SerializeField] private float fadeTime = 2f;
    private float _playTime = 0.0f; // 플레이한 시간
    private float FARMING_TIME = 360; // 게임 길이
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
    public Player managed_player;

    public GameObject characterCam;
    public Slider playerCurrentHPBar;

    public GameObject startPostion;

    [SerializeField] private Sprite[] weaponImage;

    [SerializeField] private Image weaponNum1Image;
    [SerializeField] private Image weaponNum2Image;
    
    [SerializeField] private Image weapon1Image;
    [SerializeField] private Image weapon2Image;

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

                if (_isInven)
                    _isInven = false;
                else
                    _isInven = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (invenCanvas.gameObject.activeSelf)
            {
                SwitchCanvasActive(invenCanvas);
                SwitchGameObjectActive(characterCam);
            }
            else
            {
                if (_isPause)
                    _isPause = false;
                else
                    _isPause = true;

                SwitchCanvasActive(pauseCanvas);
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
            inventory.AcquireItem(inventory.knife[1]);
        //else 
            //HitScreen();

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
            float t = 20f / 255;
            alpha += t;
            fadeCanvas.transform.GetChild(0).GetComponent<Image>().color = new Vector4(0,0,0, alpha);
            yield return new WaitForSeconds(0.01f);
            if (alpha >= 255)
                break;
        }

        if(_playTime >= FARMING_TIME)
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
            _playTime = FARMING_TIME;
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
        SceneManager.LoadScene(2);
    }

    public void Exit()
    {
        SceneManager.LoadScene(1);
    }

    public void Resume()
    {
        _isPause = false;
        SwitchCanvasActive(pauseCanvas);
    }

    public void PlayBattlePhase()
    {
        SceneManager.LoadScene(3);
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
        if (Player.instance.weapon1 == null)
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
            switch (Player.instance.weapon1.item.weaponType)
            {
                case Item.WeaponType.Hammer:
                    weapon1Image.sprite = weaponImage[0];
                    break;
                
                case Item.WeaponType.Spear:
                    weapon1Image.sprite = weaponImage[1];
                    break;
                
                case Item.WeaponType.Knife:
                    weapon1Image.sprite = weaponImage[2];
                    break;
            }
            
            if (Player.instance.weaponNow == 1)
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

        if (Player.instance.weapon2 == null)
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
            switch (Player.instance.weapon2.item.weaponType)
            {
                case Item.WeaponType.Hammer:
                    weapon2Image.sprite = weaponImage[0];
                    break;
                
                case Item.WeaponType.Spear:
                    weapon2Image.sprite = weaponImage[1];
                    break;
                
                case Item.WeaponType.Knife:
                    weapon2Image.sprite = weaponImage[2];
                    break;
            }
            
            if (Player.instance.weaponNow == 2)
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
}