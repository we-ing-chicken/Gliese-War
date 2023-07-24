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

    [Header("Character")]
    public int charNum;
    private bool isChanging;
    [SerializeField] private GameObject[] characters;
    [SerializeField] private GameObject originPosition;
    [SerializeField] private GameObject leftPosition;
    [SerializeField] private GameObject rightPosition;

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
        charNum = 0;
        GameManager.Instance.charNum = charNum;
        
        isChanging = false;
        Cursor.visible = true;
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

    public void CharacterChangeLeftButton()
    {
        if (isChanging) return;
        isChanging = true;
        
        int originNum = charNum;
        
        --charNum;
        if (charNum < 0)
            charNum = characters.Length-1;

        GameManager.Instance.charNum = charNum;
        
        characters[charNum].gameObject.SetActive(true);
        characters[charNum].transform.position = rightPosition.transform.position;
        
        StartCoroutine(MoveCharacter(characters[originNum], characters[charNum], leftPosition));
    }
    
    public void CharacterChangeRightButton()
    {
        if (isChanging) return;
        isChanging = true;
        
        int originNum = charNum;
        
        ++charNum;
        if (charNum >= characters.Length)
            charNum = 0;
        
        GameManager.Instance.charNum = charNum;

        characters[charNum].gameObject.SetActive(true);
        characters[charNum].transform.position = leftPosition.transform.position;
        
        StartCoroutine(MoveCharacter(characters[originNum], characters[charNum], rightPosition));
    }

    IEnumerator MoveCharacter(GameObject origin, GameObject newChar, GameObject to)
    {
        Vector3 velo = Vector3.zero;
        float offset = 0.2f;
        
        while (true)
        {
            origin.transform.position = Vector3.SmoothDamp(origin.transform.position, to.transform.position, ref velo, offset);
            newChar.transform.position = Vector3.SmoothDamp(newChar.transform.position, originPosition.transform.position, ref velo, offset);
    
            //target.y + offset <= this.transform.position.y
            
            if ((newChar.transform.position == originPosition.transform.position) || (origin.transform.position.x <= -11) || (origin.transform.position.x >= 11))
            {
                origin.SetActive(false);
                newChar.transform.position = new Vector3(0f, -2.703704f, 80f);
                isChanging = false;
                break;
            }

            yield return null;
        }
        
        yield return null;
    }
}
