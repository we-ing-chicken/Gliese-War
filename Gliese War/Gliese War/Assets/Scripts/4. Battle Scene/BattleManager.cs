using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private static BattleManager _instance;
    public List<int> charNums = new List<int>();

    public static BattleManager Instance
    {
        get
        {
            if (!_instance)
            {
                if (_instance == null)
                    return null;

                _instance = FindObjectOfType(typeof(BattleManager)) as BattleManager;
            }

            return _instance;
        }
    }

    private void Awake()
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

    public void BM_RemoveList(int removeNum)
    {
        for (int i = charNums.Count - 1; i >= 0; i--)
        {
            if (charNums[i] == removeNum)
            {
                Debug.Log("removelist" + charNums[i]);
                charNums.Remove(charNums[i]);
            }
                
        }

    }
}
