using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDestroyTimer : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Timer());
    }
    
    IEnumerator Timer()
    {
        float time = 0f;

        while (true)
        {
            time += Time.deltaTime;

            if (time >= 2f)
            {
                Destroy(transform.gameObject);
                break;
            }

            yield return null;
        }
    }
}
