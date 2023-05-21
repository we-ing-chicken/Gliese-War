using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameServer;
using GlieseWarGameServer;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour {

    private static BattleManager _instance;


    enum USER_STATE
	{
		NOT_CONNECTED,
		CONNECTED,
		WAITING_MATCHING
	}
	[SerializeField]
	USER_STATE user_state;

	public Button match_Button;
    public TMP_Text waiting_txt;

    [Header("Canvas")] public Canvas BattleCanvas;

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

    // Use this for initialization
    void Start () {
		user_state = USER_STATE.NOT_CONNECTED;
        CBattleRoom.Instance.gameObject.SetActive(false);

        waiting_txt.text = "Start";

        user_state = USER_STATE.NOT_CONNECTED;
		enter();
	}


	public void enter()
	{
		StopCoroutine("after_connected");

        CNetworkManager.Instance.message_receiver = this;

		if (!CNetworkManager.Instance.is_connected())
		{
			user_state = USER_STATE.CONNECTED;
            CNetworkManager.Instance.connect();
		}
		else
		{
			on_connected();
		}
	}
	
	void OnGUI()
	{
		switch (user_state)
		{
			case USER_STATE.NOT_CONNECTED:
				waiting_txt.text = "NOT_CONNECTED";
                break;

			case USER_STATE.CONNECTED:
                waiting_txt.text = "CONNECTED";
                break;

			case USER_STATE.WAITING_MATCHING:
                waiting_txt.text = "WAITING_MATCHING";
                break;
		}
	}


	/// <summary>
	/// ������ ������ �Ϸ�Ǹ� ȣ���.
	/// </summary>
	public void on_connected()
	{
		user_state = USER_STATE.CONNECTED;

		StartCoroutine("after_connected");
	}

    /// <summary>
    /// ������ ���ӵ� ���Ŀ� ó���� ����.
    /// ���콺 �Է��� ������ ENTER_GAME_ROOM_REQ���������� ��û�ϰ� 
    /// �ߺ� ��û�� �����ϱ� ���ؼ� ���� �ڷ�ƾ�� ���� ��Ų��.
    /// </summary>
    /// <returns></returns>
    IEnumerator after_connected()
    {
        // CBattleRoom�� ���ӿ��� ���¿��� ���콺 �Է��� ���� ���� ȭ������ �Ѿ������ �Ǿ� �ִµ�,
        // �� ������ ������ �� �ڷ�ƾ�� ����� ��� ���� ���콺 �Է��� �����ִ°����� �ǴܵǾ�
        // ���� ȭ������ ���ƿ��� ���� ENTER_GAME_ROOM_REQ��Ŷ�� ������ ���� �߻��Ѵ�.
        // ���� ������ �� �������� �ǳʶپ� ���� �����Ӻ��� �ڷ�ƾ�� ������ ����� �� �ֵ��� �Ѵ�.
        yield return new WaitForEndOfFrame();

        while (true)
        {
            Debug.Log("after_connected");
            yield return 0;
        }
    }

    public void start_match()
    {
        if (user_state == USER_STATE.CONNECTED)
        {
                user_state = USER_STATE.WAITING_MATCHING;

                CPacket msg = CPacket.create((short)PROTOCOL.ENTER_GAME_ROOM_REQ);
            CNetworkManager.Instance.send(msg);

                StopCoroutine("after_connected");
        }
    }

    /// <summary>
    /// ��Ŷ�� ���� ���� �� ȣ���.
    /// </summary>
    /// <param name="protocol"></param>
    /// <param name="msg"></param>
    public void on_recv(CPacket msg)
	{
		// ���� ���� �������� ���̵� �����´�.
		PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();

		switch (protocol_id)
		{
			case PROTOCOL.START_LOADING:
				{
					byte player_index = msg.pop_byte();

                    CBattleRoom.Instance.gameObject.SetActive(true);
                    CBattleRoom.Instance.start_loading(player_index);
					gameObject.SetActive(false);
				}
				break;
		}
	}
}
