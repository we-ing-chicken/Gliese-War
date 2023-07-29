using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Team
{
    public class LoginManager : MonoBehaviour
    {
        private static LoginManager _instance = null;
        private MySqlDataReader reader;

        [Header("Window")] public GameObject loginWindow;
        public GameObject joinWindow;
        public GameObject alertWindow;

        [Header("Login")]
        public TMP_InputField idInput;
        public TMP_InputField pwInput;

        [Header("Join")]
        [Space(10f)] public TMP_InputField joinIDInput;
        public TMP_InputField joinNickInput;
        public TMP_InputField joinPwInput;
        private bool idOK = false;
        private bool pwOK = false;
        private bool nickOK = false;
        public Button joinOKButton;

        void Awake()
        {
            if (null == _instance)
            {
                _instance = this;
            }
        }

        public static LoginManager Instance
        {
            get
            {
                if (null == _instance)
                {
                    return null;
                }

                return _instance;
            }
        }

        private void Start()
        {
            reader = MySqlConnector.Instance.doQuery("select count(*) from User");
            reader.Read();
            reader.Close();

            if (PlayerPrefs.HasKey("ID"))
            {
                SceneManager.LoadScene(1);
            }
            
            Cursor.visible = true;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                if (idInput.isFocused)
                    pwInput.Select();
                
                if (pwInput.isFocused)
                    idInput.Select();
                
                if(joinIDInput.isFocused)
                    joinNickInput.Select();
                
                if(joinNickInput.isFocused)
                    joinPwInput.Select();
                
                if(joinPwInput.isFocused)
                    joinIDInput.Select();
            }

            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                if (loginWindow.activeSelf)
                {
                    Login();
                }
                else if (joinWindow.activeSelf)
                {
                    if(joinOKButton.GetComponent<Button>().interactable)
                        JoinOkBtn();
                }
            }
            
        }

        // 로그인 버튼
        public void Login()
        {
            string id = idInput.text;
            string pw = pwInput.text;

            if (id.Length == 0)
            {
                Alert("Input ID");
                return;
            }
            
            if (pw.Length == 0)
            {
                Alert("Input PW");
                return;
            }
            
            reader = MySqlConnector.Instance.doQuery("select count(*) from User where ID = '" + id + "'" + " and PW = '" + pw + "'");
            reader.Read();

            if (int.Parse(reader[0].ToString()) == 1)
            {
                reader.Close();
                PlayerPrefs.SetString("ID", id);
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                MySqlConnector.Instance.doNonQuery("update User set LastLogin = '" + date + "' where id = '" + id +"'");
                SceneManager.LoadScene(1);
            }
            else
            {
                Alert("ID / PW is Wrong.");
            }

            reader.Close();
        }

        // 회원가입 버튼
        public void Join()
        {
            loginWindow.SetActive(false);
            joinWindow.SetActive(true);
        }

        // 회원가입 취소
        public void JoinCancel()
        {
            joinIDInput.text = "";
            joinPwInput.text = "";
            joinNickInput.text = "";
            
            joinWindow.SetActive(false);
            loginWindow.SetActive(true);
        }

        // 아이디 중복 체크
        public void IDCheck()
        {
            string id = joinIDInput.text;
            
            reader = MySqlConnector.Instance.doQuery("select count(*) from User where ID = '" + id + "'");
            reader.Read();

            if (int.Parse(reader[0].ToString()) == 1)
            {
                idOK = false;
                Alert("Same ID");
            }
            else
            {
                idOK = true;
                Alert("You Can Use");
            }

            reader.Close();
            BlankCheck();
        }

        // 닉네임 중복 체크
        public void NickCheck()
        {
            string nick = joinNickInput.text;

            reader = MySqlConnector.Instance.doQuery("select count(*) from User where NickName = '" + nick + "'");
            reader.Read();

            if (int.Parse(reader[0].ToString()) == 1)
            {
                nickOK = false;
                Alert("Same Nick");
            }
            else
            {
                nickOK = true;
                Alert("You Can Use");
            }
            reader.Close();
            BlankCheck();
        }

        // PW칸 체크
        public void PWCheck()
        {
            string pw = joinPwInput.text;

            if (pw.Length == 0)
                pwOK = false;
            else
                pwOK = true;
            
            BlankCheck();
        }

        public void IDCheckReset()
        {
            idOK = false;
            BlankCheck();
        }
        
        public void NickCheckReset()
        {
            nickOK = false;
            BlankCheck();
        }

        // 경고문
        public void Alert(string text)
        {
            alertWindow.SetActive(true);
            GameObject temp = alertWindow.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
            temp.GetComponent<TextMeshProUGUI>().text = text;
        }

        // 경고문 확인
        public void AlertOk()
        {
            alertWindow.SetActive(false);
        }

        
        // 회원가입 시 빈칸이 있는지 체크
        public void BlankCheck()
        {
            if (idOK && nickOK && pwOK )
                joinOKButton.GetComponent<Button>().interactable = true;
            else
                joinOKButton.GetComponent<Button>().interactable = false;
        }
        
        // 회원가입 완료 버튼
        public void JoinOkBtn()
        {
            string id = joinIDInput.text;
            string pw = joinPwInput.text;
            string nick = joinNickInput.text;

            MySqlConnector.Instance.doNonQuery("insert into User values ('" + id + "','" + pw +"','" + nick + "','0','0','2022-08-09')");
            MySqlConnector.Instance.doNonQuery("insert into Career values ('" + id + "', 0, 0, 0, 0, 0, 0, 0, 0");


            JoinCancel();
        }
    }
}