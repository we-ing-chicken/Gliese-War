using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyManager : MonoBehaviour
{
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
            id = "test1234";

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
        careerAnimation.Play("CareerAnimation");
        
        for (int i = 0; i < careerList.Length; ++i)
        {
            careerList[i].GetComponent<TextMeshProUGUI>().text = "2";
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
}
