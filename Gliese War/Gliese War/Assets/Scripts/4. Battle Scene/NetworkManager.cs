using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; //����
using Photon.Realtime; //����
using UnityEngine.UI; //����
using TMPro;
using UnityEngine.UIElements;
using Cinemachine;
using System;

public class NetworkManager : MonoBehaviourPunCallbacks //Ŭ���� ���
{
    private static NetworkManager _instance;
    public TMP_Text StatusText;
    public TMP_InputField NickNameInput;
    public TMP_InputField roomNameInput;
    public GameObject uiPanel;
    public byte userNum = 1;
    //public List<Transform> spawnpoints = new List<Transform>();
    public GameObject[] spawnpoints;
    int i = 0;
    public int p_Num = 0;
    public PhotonView pv;
    public CMvcam cscamera;

    public bool connect = false;
    public bool sendOK;

    public GameObject go;

    public static NetworkManager Instance
    {
        get
        {
            if (!_instance)
            {
                if (_instance == null)
                    return null;

                _instance = FindObjectOfType(typeof(NetworkManager)) as NetworkManager;
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

    private void Start()
    {
        Connect();
    }

    //���� ���� ǥ�� 
    private void Update() => StatusText.text = "Loading...";
        
    public void Connect() => PhotonNetwork.ConnectUsingSettings();
    public override void OnConnectedToMaster()
    {
        Debug.Log("접속 완료");
        string nickName = PhotonNetwork.LocalPlayer.NickName;
        nickName = NickNameInput.text;
        Debug.Log("당신의 닉네임은 " + nickName + " 입니다.");
        connect = true;
    }

    public void Disconnect() => PhotonNetwork.Disconnect();
    public override void OnDisconnected(DisconnectCause cause) => Debug.Log("접속 종료" + cause);
    public void JoinRoom()
    {
        if(connect)
        {
            PhotonNetwork.JoinRandomRoom();
            uiPanel.SetActive(false);
            Debug.Log(roomNameInput.text + " 방으로 입장합니다.");
        }
    }
    public override void OnJoinRandomFailed(short returnCode, string message) =>
    PhotonNetwork.CreateRoom(roomNameInput.text, new RoomOptions { MaxPlayers = userNum });

    //TODO - �÷��̾� �ε��� �˻��ؼ� spawnpoints �ٸ��� �Ҵ�?
    public override void OnJoinedRoom()
    {
        //cscamera.gameObject.SetActive(true);
        //GameObject temp = PhotonNetwork.Instantiate("player", new Vector3(0,0, 0), Quaternion.identity);
        GameObject temp;
        if(GameManager.Instance != null)
            temp = PhotonNetwork.Instantiate(GameManager.Instance.battleCharacters[GameManager.Instance.charNum].name, spawnpoints[PhotonNetwork.CurrentRoom.Players.Count-1].transform.position, Quaternion.identity);
        else
            temp = PhotonNetwork.Instantiate("Sugar", spawnpoints[PhotonNetwork.CurrentRoom.Players.Count-1].transform.position, Quaternion.identity);
        
        //pv.RPC("numchange", RpcTarget.All);
        temp.GetComponent<BattlePlayer>().myindex = PhotonNetwork.CurrentRoom.Players.Count-1;
        
        //photonView.RPC("numchange", RpcTarget.All);
        sendOK = true;
    }

    [PunRPC]
    public void numchange()
    {
        p_Num++;

    }
}