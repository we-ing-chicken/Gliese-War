using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicArea : MonoBehaviour
{
    static public MagicArea Instance;
    
    void Start()
    {
        Instance = this;
    }
}
