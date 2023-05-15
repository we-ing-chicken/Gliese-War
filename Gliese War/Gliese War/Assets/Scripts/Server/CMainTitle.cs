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
	/// 서버에 접속된 이후에 처리할 루프.
	/// 마우스 입력이 들어오면 ENTER_GAME_ROOM_REQ프로토콜을 요청하고 
	/// 중복 요청을 방지하기 위해서 현재 코루틴을 중지 시킨다.
	/// </summary>
	/// <returns></returns>
	IEnumerator after_connected()
	{
		// CBattleRoom의 게임오버 상태에서 마우스 입력을 통해 메인 화면으로 넘어오도록 되어 있는데,
		// 한 프레임 내에서 이 코루틴이 실행될 경우 아직 마우스 입력이 남아있는것으로 판단되어
		// 메인 화면으로 돌아오자 마자 ENTER_GAME_ROOM_REQ패킷을 보내는 일이 발생한다.
		// 따라서 강제로 한 프레임을 건너뛰어 다음 프레임부터 코루틴의 내용이 수행될 수 있도록 한다.
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
	/// 서버에 접속이 완료되면 호출됨.
	/// </summary>
	public void on_connected()
	{
		this.user_state = USER_STATE.CONNECTED;

		StartCoroutine("after_connected");
	}


	/// <summary>
	/// 패킷을 수신 했을 때 호출됨.
	/// </summary>
	/// <param name="protocol"></param>
	/// <param name="msg"></param>
	public void on_recv(CPacket msg)
	{
		// 제일 먼저 프로토콜 아이디를 꺼내온다.
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
