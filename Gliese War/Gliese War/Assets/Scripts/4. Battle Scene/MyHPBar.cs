using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyHPBar : MonoBehaviour
{
    static public MyHPBar Instance;
    
    void Start()
    {
        Instance = this;
    }

    public void SetHPBar(int max, int now)
    {
        GetComponent<Slider>().value = (float)now / max;
    }
}
