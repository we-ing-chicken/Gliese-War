using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CMvcam : MonoBehaviour
{
    private static CMvcam _instance;

    public static CMvcam Instance
    {
        get
        {
            if (!_instance)
            {
                if (_instance == null)
                    return null;

                _instance = FindObjectOfType(typeof(CMvcam )) as CMvcam;
            }

            return _instance;
        }
    }

    void Awake()
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

    public float ymove = 0;
    public float ymove_max = 20.0f;
    public float ymove_min = 10.0f;

    public CinemachineVirtualCamera cmvc;
    
    // Start is called before the first frame update
    void Start()
    {
        cmvc = GetComponent<CinemachineVirtualCamera>();
        
    }

    // Update is called once per frame
    void Update()
    {
        ymove -= Input.GetAxis("Mouse Y");
        ymove = Mathf.Clamp(ymove, ymove_min, ymove_max);

        Vector3 pre = cmvc.GetCinemachineComponent<Cinemachine3rdPersonFollow>().ShoulderOffset;
        pre.y = ymove;
        cmvc.GetCinemachineComponent<Cinemachine3rdPersonFollow>().ShoulderOffset = pre;
    }
}
