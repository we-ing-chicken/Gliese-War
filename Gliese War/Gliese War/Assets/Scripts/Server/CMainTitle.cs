using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameServer;
using GlieseWarGameServer;

public class CMainTitle : MonoBehaviour {

	enum USER_STATE
	{
		NOT_CONNECTED,
		CONNECTED,
		WAITING_MATCHING
	}

	CBattleRoom battle_room;

	CNetworkManager network_manager;
	USER_STATE user_state;

	Texture waiting_img;

	// Use this for initialization
	void Start () {
		this.user_state = USER_STATE.NOT_CONNECTED;
		this.battle_room = GameObject.Find("BattleRoom").GetComponent<CBattleRoom>();
		this.battle_room.gameObject.SetActive(false);

		this.network_manager = GameObject.Find("NetworkManager").GetComponent<CNetworkManager>();

		this.waiting_img = Resources.Load("images/waiting") as Texture;

		this.user_state = USER_STATE.NOT_CONNECTED;
		enter();
	}


	public void enter()
	{
		StopCoroutine("after_connected");

		this.network_manager.message_receiver = this;

		if (!this.network_manager.is_connected())
		{
			this.user_state = USER_STATE.CONNECTED;
			this.network_manager.connect();
		}
		else
		{
			on_connected();
		}
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
			if (this.user_state == USER_STATE.CONNECTED)
			{
				if (Input.GetMouseButtonDown(0))
				{
					this.user_state = USER_STATE.WAITING_MATCHING;

					CPacket msg = CPacket.create((short)PROTOCOL.ENTER_GAME_ROOM_REQ);
					this.network_manager.send(msg);

					StopCoroutine("after_connected");
				}
			}

			yield return 0;
		}
	}
	
	void OnGUI()
	{
		switch (this.user_state)
		{
			case USER_STATE.NOT_CONNECTED:
				Debug.Log("NOT_CONNECTED");
				break;

			case USER_STATE.CONNECTED:
                Debug.Log("CONNECTED");
                break;

			case USER_STATE.WAITING_MATCHING:
                Debug.Log("WAITING_MATCHING");
                break;
		}
	}


	/// <summary>
	/// ������ ������ �Ϸ�Ǹ� ȣ���.
	/// </summary>
	public void on_connected()
	{
		this.user_state = USER_STATE.CONNECTED;

		StartCoroutine("after_connected");
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

					this.battle_room.gameObject.SetActive(true);
					this.battle_room.start_loading(player_index);
					gameObject.SetActive(false);
				}
				break;
		}
	}
}
