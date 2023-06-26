using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; //����
using Photon.Realtime; //����
using UnityEngine.UI; //����
using TMPro;
using UnityEngine.UIElements;

public class NetworkManager : MonoBehaviourPunCallbacks //Ŭ���� ���
{
    private static NetworkManager _instance;
    public TMP_Text StatusText;
    public TMP_InputField NickNameInput;
    public TMP_InputField roomNameInput;
    public GameObject uiPanel;
    public byte userNum = 4;
    //public List<Transform> spawnpoints = new List<Transform>();
    public GameObject[] spawnpoints;

    public CServercamTest cscamera;

    private bool connect = false;

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
    private void Update() => StatusText.text = PhotonNetwork.NetworkClientState.ToString();

    
    public void Connect() => PhotonNetwork.ConnectUsingSettings();
    public override void OnConnectedToMaster()
    {
        Debug.Log("�������ӿϷ�");
        string nickName = PhotonNetwork.LocalPlayer.NickName;
        nickName = NickNameInput.text;
        Debug.Log("����� �̸��� " + nickName + " �Դϴ�.");
        connect = true;
    }

    public void Disconnect() => PhotonNetwork.Disconnect();
    public override void OnDisconnected(DisconnectCause cause) => Debug.Log("�������" + cause);
    public void JoinRoom()
    {
        if(connect)
        {
            PhotonNetwork.JoinRandomRoom();
            uiPanel.SetActive(false);
            Debug.Log(roomNameInput.text + "�濡 �����Ͽ����ϴ�.");
        }
    }
    public override void OnJoinRandomFailed(short returnCode, string message) =>
    PhotonNetwork.CreateRoom(roomNameInput.text, new RoomOptions { MaxPlayers = userNum });

    //TODO - �÷��̾� �ε��� �˻��ؼ� spawnpoints �ٸ��� �Ҵ�?
    public override void OnJoinedRoom()
    {
        //cscamera.gameObject.SetActive(true);
        GameObject p = new GameObject();
        p = PhotonNetwork.Instantiate("player", spawnpoints[0].transform.position, Quaternion.identity);
        //Debug.Log(p.GetComponent<playerScript>().isMine());
        //cscamera.ps = p.GetComponent<playerScript>();
    }
}