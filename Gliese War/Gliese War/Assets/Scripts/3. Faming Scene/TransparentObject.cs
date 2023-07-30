using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TransparentObject : MonoBehaviour
{
    Renderer ObstacleRenderer;
    private Coroutine cor;

    public void StartTransparent()
    {
        cor = StartCoroutine(TransParent());
    }

    IEnumerator TransParent()
    {
        while(true)
        {
            float distance = Vector3.Distance(Camera.main.transform.position, CPlayer.Instance.transform.position);
            Vector3 direction = (CPlayer.Instance.transform.position - Camera.main.transform.position).normalized;
            RaycastHit[] hit;

            hit = (Physics.RaycastAll(Camera.main.transform.position, direction, distance));
            bool flag = false;
        
            for (int i = 0; i < hit.Length; ++i)
            {
                if (hit[i].transform.gameObject == gameObject)
                {
                    ObstacleRenderer = transform.gameObject.GetComponentInChildren<Renderer>();
                
                    if (ObstacleRenderer != null)
                    {
                        // Material material = ObstacleRenderer.material;
                        // Color color = material.color;
                        // color.a = 0.1f;
                        // material.color = color;
                        gameObject.layer = 7;
                        flag = true;
                        break;
                    }
                }

            }

            if (!flag)
            {
                ObstacleRenderer = transform.gameObject.GetComponentInChildren<Renderer>();

                // Material material = ObstacleRenderer.material;
                // Color color = material.color;
                // color.a = 1f;
                //material.color = color;
                gameObject.layer = 0;
                
                if(cor != null)
                    StopCoroutine(cor);
            }

            yield return null;
        }
    }
}