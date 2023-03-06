using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using MySql.Data;
using MySql.Data.MySqlClient;

public class LobbyManager : MonoBehaviour
{
    private MySqlDataReader reader;
    private string id;

    [Header("Lobby")] [SerializeField] private GameObject nickNameText;
    
    [Header("Career")]
    [SerializeField] private Canvas careerCanvas;
    [SerializeField] private GameObject careerListParent;
    private TextMeshProUGUI[] careerList;
    private Animation careerAnimation;
    
    [SerializeField] private Canvas optionCanvas;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("ID"))
            id = PlayerPrefs.GetString("ID");
        else
            id = "hsson";

        reader = MySqlConnector.Instance.doQuery("select * from User where ID = '" + id + "'");
        reader.Read();
        reader.Close();
        
        nickNameText.GetComponent<TextMeshProUGUI>().text = id;

        careerList = careerListParent.GetComponentsInChildren<TextMeshProUGUI>();
        careerAnimation = careerCanvas.transform.GetChild(0).GetComponent<Animation>();
        
    }

    private void Start()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene(2);
    }
    
    public void OpenCareer()
    {
        careerCanvas.gameObject.SetActive(true);

        reader = MySqlConnector.Instance.doQuery("select * from Career where ID = '" + id + "'");
        //reader = MySqlConnector.Instance.doQuery("select count(*) from User");
        reader.Read();
        
        careerAnimation.Play("CareerAnimation");
        
        for (int i = 0; i < careerList.Length; ++i)
        {
            careerList[i].GetComponent<TextMeshProUGUI>().text = reader[i + 1].ToString();
        }

        reader.Close();
    }

    public void CloseCareer()
    {
        careerAnimation.Play("ReturnCareerAnimation");
    }
    
    public void OpenOptionCanvas()
    {
        optionCanvas.gameObject.SetActive(true);
    }

    public void CloseOptionCanvas()
    {
        optionCanvas.gameObject.SetActive(false);
    }
}
