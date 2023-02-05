using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FarmingManager : MonoBehaviour
{
    private static FarmingManager _instance;

    private bool _isEnd = false;
    private bool _isPause = false;
    private bool _isFading = true;
    
    [Header("Timer")]
    [SerializeField] private float fadeTime = 2f;
    private float _playTime = 0.0f; // 플레이한 시간
    private float FARMING_TIME = 360; // 게임 길이
    [SerializeField] private GameObject timerTxt; //타이머 텍스트
    private TextMeshProUGUI _timerTxtComp; // 타이머 텍스트 컴포넌트

    [Header("Light")] [SerializeField] private Light DirectionalLight; // 태양 오브젝트
    private float _timeOfInGame;

    [Header("DayNight")] private bool _isNight;
    [SerializeField] private Image DayNightImage; // 낮밤 이미지 오브젝트
    [SerializeField] private Sprite DayImage; // 낮 이미지
    [SerializeField] private Sprite NightImage; // 밤 이미지

    [Header("Canvas")] public Canvas invenCanvas;
    public Canvas pauseCanvas;
    public Canvas fadeCanvas;

    [SerializeField] private Inventory inventory;

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
        _timeOfInGame = 8.4f;
        _timerTxtComp = timerTxt.GetComponent<TextMeshProUGUI>();
        _isNight = false;
        fadeCanvas.transform.GetChild(0).GetComponent<Image>().material.color = new Vector4(0f, 0f, 0f);

        SetTimerText();
        UpdateLighting(_timeOfInGame / 24f);
        
        SwitchCanvasActive(fadeCanvas);
        StartCoroutine(FadeIn());
    }

    private void Update()
    {
        if (_isFading) return;

        if (!_isEnd && !_isPause)
        {
            TimeCheck();

            if (_isNight)
                _timeOfInGame += 0.1f * 1f * Time.deltaTime;
            else
                _timeOfInGame += 0.1f * 0.8f * Time.deltaTime;

            _timeOfInGame %= 24;
            UpdateLighting(_timeOfInGame / 24f);
            UpdateDayNightImage();

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!pauseCanvas.gameObject.activeSelf)
                    SwitchCanvasActive(invenCanvas);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (invenCanvas.gameObject.activeSelf)
                SwitchCanvasActive(invenCanvas);
            else
            {
                if (_isPause)
                    _isPause = false;
                else
                    _isPause = true;

                SwitchCanvasActive(pauseCanvas);
            }
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha1))
            inventory.AcquireItem(inventory.knife[0]);
        else if(Input.GetKeyDown(KeyCode.Alpha2))
            inventory.AcquireItem(inventory.knife[1]);
        else if(Input.GetKeyDown(KeyCode.Alpha3))
            inventory.AcquireItem(inventory.knife[2]);
        else if(Input.GetKeyDown(KeyCode.Alpha4))
            inventory.AcquireItem(inventory.knife[3]);
        else if(Input.GetKeyDown(KeyCode.Alpha5))
            inventory.AcquireItem(inventory.knife[4]);
        else if(Input.GetKeyDown(KeyCode.Alpha6))
            inventory.AcquireItem(inventory.hammer[0]);
        else if(Input.GetKeyDown(KeyCode.Alpha7))
            inventory.AcquireItem(inventory.hammer[1]);
        else if(Input.GetKeyDown(KeyCode.Alpha8))
            inventory.AcquireItem(inventory.hammer[2]);
        else if(Input.GetKeyDown(KeyCode.Alpha9))
            inventory.AcquireItem(inventory.hammer[3]);
        else if(Input.GetKeyDown(KeyCode.Alpha0))
            inventory.AcquireItem(inventory.hammer[4]);
        else if(Input.GetKeyDown(KeyCode.Y))
            inventory.AcquireItem(inventory.spear[0]);
        else if(Input.GetKeyDown(KeyCode.U))
            inventory.AcquireItem(inventory.spear[1]);
        else if(Input.GetKeyDown(KeyCode.I))
            inventory.AcquireItem(inventory.spear[2]);
        else if(Input.GetKeyDown(KeyCode.O))
            inventory.AcquireItem(inventory.spear[3]);
        else if(Input.GetKeyDown(KeyCode.P))
            inventory.AcquireItem(inventory.spear[4]);
        
    }

    IEnumerator FadeIn()
    {
        float alpha = fadeCanvas.transform.GetChild(0).GetComponent<Image>().color.a;
        float mat_Rgb = fadeCanvas.transform.GetChild(0).GetComponent<Image>().material.color.r;
        //float mat_Blur = fadeCanvas.transform.GetChild(0).GetComponent<Image>().material.GetFloat("Radius");
        //UnityEngine.Debug.Log("mat_Blur : " + mat_Blur);


        GameObject loadingText = fadeCanvas.transform.GetChild(0).GetChild(0).gameObject;

        yield return new WaitForSeconds(fadeTime);
        
        loadingText.SetActive(false);
        
        while (true)
        {
            float t = 2f / 255;
            //float blur = 2f / 7;
            mat_Rgb += t;
            //mat_Blur -= blur;

            UnityEngine.Debug.Log(mat_Rgb);
            alpha -= t;
            fadeCanvas.transform.GetChild(0).GetComponent<Image>().color = new Vector4(0,0,0, alpha);
            fadeCanvas.transform.GetChild(0).GetComponent<Image>().material.color = new Vector4(mat_Rgb, mat_Rgb, mat_Rgb);
            //fadeCanvas.transform.GetChild(0).GetComponent<Image>().material.SetFloat("Radius", mat_Blur);
            UnityEngine.Debug.Log(fadeCanvas.transform.GetChild(0).GetComponent<Image>().material.color);
            yield return new WaitForSeconds(0.01f);
            if (alpha <= 0 || mat_Rgb <= 0)
                break;
        }
        
        _isFading = false;
        SwitchCanvasActive(fadeCanvas);
        yield return null;
    }

    IEnumerator FadeOut()
    {
        SwitchCanvasActive(fadeCanvas);
        
        float alpha = fadeCanvas.transform.GetChild(0).GetComponent<Image>().color.a;
        
        while (true)
        {
            float t = 2f / 255;
            alpha += t;
            fadeCanvas.transform.GetChild(0).GetComponent<Image>().color = new Vector4(0,0,0, alpha);
            yield return new WaitForSeconds(0.01f);
            if (alpha >= 255)
                break;
        }

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

    // 시간에 따른 라이트 변화 함수
    private void UpdateLighting(float timePercent)
    {
        // RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        // RenderSettings.fogColor = Preset.FogoColor.Evaluate(timePercent);
        //
        //DirectionalLight.color = Preset.DirectionColor.Evaluate(timePercent);
        DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, -170, 0));
    }

    // 낮 밤 이미지 교체 함수
    void UpdateDayNightImage()
    {
        if (_timeOfInGame >= 6 && _timeOfInGame <= 18)
        {
            DayNightImage.sprite = DayImage;
            _isNight = false;
        }
        else
        {
            DayNightImage.sprite = NightImage;
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
}