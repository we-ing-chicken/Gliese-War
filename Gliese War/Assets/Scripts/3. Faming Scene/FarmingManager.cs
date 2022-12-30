using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FarmingManager : MonoBehaviour
{
    [SerializeField] private GameObject timerTxt;
    private TextMeshProUGUI _timerTxtComp;
    
    private float FARMING_TIME = 360;
    private float _playTime = 0.0f;
    private bool _isFadeOut = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _timerTxtComp = timerTxt.GetComponent<TextMeshProUGUI>();
        SetTimerText();
        StartCoroutine(TimeCheck());
    }

    private IEnumerator FadeOut()
    {
        yield return null;
    }

    private IEnumerator TimeCheck()
    {
        //yield return new WaitUntil(() => _isFadeOut);
        while (FARMING_TIME >= _playTime)
        {
            Debug.Log("12321312321");
            _playTime += Time.deltaTime;
            SetTimerText();

            if (FARMING_TIME <= _playTime)
                _playTime = FARMING_TIME;
            
            yield return null;
        }
        
        if (FARMING_TIME <= _playTime)
            _playTime = FARMING_TIME;
        
    }

    private void SetTimerText()
    {
        float time = FARMING_TIME - _playTime;
        float m = time / 60;
        float s = time % 60;
        _timerTxtComp.text = (int)m + " : " + (s >= 10 ? (int)s : ("0" + (int)s ));
    }
}
