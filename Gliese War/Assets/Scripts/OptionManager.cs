using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("퀄리티 : "+QualitySettings.GetQualityLevel());
    }

    public void SetFullScreen(bool isOn)
    {
        if (isOn)
        {
            int setWidth = 1920; // 화면 너비
            int setHeight = 1080; // 화면 높이

            //해상도를 설정값에 따라 변경
            //3번째 파라미터는 풀스크린 모드를 설정 > true : 풀스크린, false : 창모드
            //Screen.SetResolution(setWidth, setHeight, true);
            Debug.Log("풀스크린");
        }
    }

    public void SetWindowScreen(bool isOn)
    {
        if (isOn)
        {
            int setWidth = 1920; // 화면 너비
            int setHeight = 1080; // 화면 높이

            //해상도를 설정값에 따라 변경
            //3번째 파라미터는 풀스크린 모드를 설정 > true : 풀스크린, false : 창모드
            //Screen.SetResolution(setWidth, setHeight, false);
            Debug.Log("윈도우");
        }
    }

    public void SetVSyncOn(bool isOn)
    {
        if (isOn)
        {
            QualitySettings.vSyncCount = 1;
            Debug.Log("v싱크 : " + QualitySettings.vSyncCount);
        }
    }

    public void SetVSyncOff(bool isOn)
    {
        if (isOn)
        {
            QualitySettings.vSyncCount = 0;
            Debug.Log("v싱크 : " + QualitySettings.vSyncCount);
        }
    }

    public void SetAntiAliasing0(bool isOn)
    {
        if (isOn)
        {
            QualitySettings.antiAliasing = 0;
            Debug.Log("안티 앨리어싱 : " + QualitySettings.antiAliasing);
        }
    }

    public void SetAntiAliasing2(bool isOn)
    {
        if (isOn)
        {
            QualitySettings.antiAliasing = 2;
            Debug.Log("안티 앨리어싱 : " + QualitySettings.antiAliasing);
        }
    }

    public void SetAntiAliasing4(bool isOn)
    {
        if (isOn)
        {
            QualitySettings.antiAliasing = 4;
            Debug.Log("안티 앨리어싱 : " + QualitySettings.antiAliasing);
        }
    }

    public void SetSoundVolume(float size)
    {
        AudioListener.volume = size;
        Debug.Log("사운드 : " + size);
    }

    public void SetQuality0(bool isOn)
    {
        if (isOn)
        {
            QualitySettings.SetQualityLevel(0);
        }
    }
    
    public void SetQuality1(bool isOn)
    {
        if (isOn)
        {
            QualitySettings.SetQualityLevel(1);
        }
    }
    
    public void SetQuality2(bool isOn)
    {
        if (isOn)
        {
            QualitySettings.SetQualityLevel(2);
        }
    }
    
    public void SetQuality3(bool isOn)
    {
        if (isOn)
        {
            QualitySettings.SetQualityLevel(3);
        }
    }
    
    public void SetQuality4(bool isOn)
    {
        if (isOn)
        {
            QualitySettings.SetQualityLevel(4);
        }
    }
    
    public void SetQuality5(bool isOn)
    {
        if (isOn)
        {
            QualitySettings.SetQualityLevel(5);
        }
    }
}