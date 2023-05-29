using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
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

    [Header("Help")]
    [SerializeField] private Canvas helpCanvas;
    [SerializeField] private Sprite[] helpImages;
    private Animation helpAnimation;
    
    
    [Header("Option")]
    [SerializeField] private Canvas optionCanvas;
    

    private void Awake()
    {
        id = PlayerPrefs.GetString("ID");

        if (PlayerPrefs.HasKey("ID"))
        {
            reader = MySqlConnector.Instance.doQuery("select * from User where ID = '" + id + "'");
            reader.Read();
            string nick = reader[2].ToString();
            nickNameText.GetComponent<TextMeshProUGUI>().text = nick;
        }
        else
        {
            //nickNameText.GetComponent<TextMeshProUGUI>().text = id;
            nickNameText.GetComponent<TextMeshProUGUI>().text = "1234";
        }

        careerList = careerListParent.GetComponentsInChildren<TextMeshProUGUI>();
        careerAnimation = careerCanvas.transform.GetChild(0).GetComponent<Animation>();
        helpAnimation = helpCanvas.transform.GetChild(0).GetComponent<Animation>();
        
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
        careerAnimation.Play("CareerAnimation");
        
        //reader = MySqlConnector.Instance.doQuery("select * from Career where ID = '" + id + "'");
        //reader = MySqlConnector.Instance.doQuery("select * from Career where ID = 'hsson'");
        //reader.Read();

        for (int i = 0; i < careerList.Length; ++i)
        {
            //careerList[i].GetComponent<TextMeshProUGUI>().text = reader[i + 1].ToString();
        }
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
    
    public void OpenHelp()
    {
        helpCanvas.gameObject.SetActive(true);
        helpAnimation.Play("HelpAnimation");

        HelpButton(0);
    }

    public void HelpButton(int n)
    {
        helpCanvas.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = helpImages[n];
    }

    public void CloseHelp()
    {
        helpAnimation.Play("ReturnHelpAnimation");
    }
}
