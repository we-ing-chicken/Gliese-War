using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowAfterAnim : MonoBehaviour
{
    private GameObject parent;

    private void Start()
    {
        parent = gameObject.transform.parent.gameObject;
    }

    public void AfterAnimation()
    {
        parent.SetActive(false);
    }
}
