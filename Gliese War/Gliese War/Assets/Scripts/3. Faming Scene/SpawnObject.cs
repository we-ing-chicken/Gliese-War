using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
//using Random = System.Random;

public class SpawnObject : MonoBehaviour
{
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private int count;
    [SerializeField] private float circleRange = 10f;

    void Start()
    {
        for (var i = 0; i < count; ++i)
        {
            var temp = GameObject.Instantiate(objectPrefab);
            var pos = gameObject.transform.position;

            Vector3 randPos = (Vector3)Random.insideUnitSphere * circleRange;
            temp.gameObject.transform.position = new Vector3(randPos.x, 0, randPos.y) + transform.position;
        }
    }
}