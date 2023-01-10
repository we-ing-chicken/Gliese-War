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

    private bool _isFadeOut = false;
    private bool _isEnd = false;
    private bool _isPause = false;

    [Header("Timer")] private float _playTime = 0.0f; // 플레이한 시간
    private float FARMING_TIME = 360; // 게임 길이
    [SerializeField] private GameObject timerTxt; //타이머 텍스트
    private TextMeshProUGUI _timerTxtComp; // 타이머 텍스트 컴포넌트

    [Header("Light")]
    [SerializeField] private Light DirectionalLight; // 태양 오브젝트
    private float _timeOfInGame;

    [Header("DayNight")] private bool _isNight;
    [SerializeField] private Image DayNightImage; // 낮밤 이미지 오브젝트
    [SerializeField] private Sprite DayImage; // 낮 이미지
    [SerializeField] private Sprite NightImage; // 밤 이미지

    [Header("Canvas")] public Canvas invenCanvas;
    public Canvas pauseCanvas;

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

        SetTimerText();
    }

    private void Update()
    {
        if (!_isEnd && !_isPause)
        {
            TimeCheck();
            
            if(_isNight)
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
    }
    
    private void SwitchCanvasActive(Canvas temp)
    {
        if(temp.gameObject.activeSelf)
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

    private IEnumerator FadeOut()
    {
        yield return null;
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
}