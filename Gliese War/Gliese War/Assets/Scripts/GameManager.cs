using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public MySqlDataReader reader;

    public string id;
    public string nickName;

    public GameObject[] farmingCharacters;
    public GameObject[] battleCharacters;
    public int charNum;

    public Stat stat;
    public RealItem helmet;
    public RealItem armor;
    public RealItem shoe;
    public RealItem weapon1;
    public RealItem weapon2;
    
    public static GameManager Instance
    {
        get
        {
            if (!_instance)
            {
                if (_instance == null)
                    return null;

                _instance = FindObjectOfType(typeof(GameManager)) as GameManager;
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
        
        DontDestroyOnLoad(gameObject);
    }

    public void SaveItems(RealItem hm, RealItem am, RealItem sh, RealItem w1, RealItem w2)
    {
        helmet = hm;
        armor = am;
        shoe = sh;
        weapon1 = w1;
        weapon2 = w2;
    }

    public void SaveStat(int maxHP, int aP, int dP, int mV)
    {
        stat.health = maxHP;
        stat.attackPower = aP;
        stat.defensePower = dP;
        stat.moveSpeed = mV;
    }


}
