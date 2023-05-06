using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class Destruct : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(Erase());
    }

    IEnumerator Erase()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}