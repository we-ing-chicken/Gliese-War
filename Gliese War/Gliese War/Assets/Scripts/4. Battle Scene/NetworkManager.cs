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
    public byte userNum = 4;
    //public List<Transform> spawnpoints = new List<Transform>();
    public GameObject[] spawnpoints;
    int i = 0;
    public int p_Num = 0;
    public PhotonView pv;
    public CMvcam cscamera;

    private bool connect = false;

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
    private void Update() => StatusText.text = PhotonNetwork.NetworkClientState.ToString();

    
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
        GameObject temp = PhotonNetwork.Instantiate("player", spawnpoints[p_Num].transform.position, Quaternion.identity);
        //temp.GetComponent<CharacterController>().enabled = false;
        Debug.Log("A");

        switch (temp.GetComponent<PhotonView>().ViewID)
        {
            case 1001:
                temp.GetComponent<BattlePlayer>().myindex = 0;
                break;
            case 2001:
                temp.GetComponent<BattlePlayer>().myindex = 1;
                break;
            case 3001:
                temp.GetComponent<BattlePlayer>().myindex = 2;
                break;
            case 4001:
                temp.GetComponent<BattlePlayer>().myindex = 3;
                break;
        }

        //temp.GetComponent<CharacterController>().center = spawnpoints[temp.GetComponent<BattlePlayer>().myindex].transform.position;
        //StartCoroutine(Teleport(temp.GetComponent<CharacterController>().enabled));

        //temp.GetComponent<CharacterController>().enabled = true;
        Debug.Log("myindex : " + temp.GetComponent<BattlePlayer>().myindex);
        //BattleManager.Instance.player_indexes.Add(temp.GetComponent<PhotonView>().ViewID++);
        StartCoroutine(nc(temp));
        Debug.Log("B");

        //TODO - 시네머신 타겟 go로 변경
        // = go.transform;
        //cscamera.cmvc.GetCinemachineComponent<>

        //Debug.Log(p.GetComponent<playerScript>().isMine());
        //cscamera.ps = p.GetComponent<playerScript>();
    }

    [PunRPC]
    public void numchange()
    {
        p_Num++;
        Debug.Log("p_Num : " + p_Num);
    }

    //IEnumerator Teleport(bool x)
    //{

    //    yield return new WaitForEndOfFrame();
    //    x = true;
    //}
    IEnumerator nc()
    {
        Debug.Log("photonView : " + photonView);

        yield return new WaitUntil(() => photonView.enabled);

        Debug.Log("photonView : " + photonView);
        photonView.RPC("numchange", RpcTarget.All);
       
    }
}