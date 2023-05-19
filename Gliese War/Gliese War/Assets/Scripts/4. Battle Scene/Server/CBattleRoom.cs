using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameServer;
using GlieseWarGameServer;

public class CBattleRoom : MonoBehaviour {

	enum GAME_STATE
	{
		READY = 0,
		STARTED
	}

    // 현재 턴을 진행중인 플레이어 인덱스.
    byte current_player_index;

    List<CPlayer> players;

    // 게임 종료 후 메인으로 돌아갈 때 사용하기 위한 MainTitle객체의 레퍼런스.
    [SerializeField]
	BattleManager main_title;

    // 네트워크 데이터 송,수신을 위한 네트워크 매니저 레퍼런스.
    [SerializeField]
    CNetworkManager network_manager;

	// 게임 상태에 따라 각각 다른 GUI모습을 구현하기 위해 필요한 상태 변수.
	GAME_STATE game_state;

	// 승리한 플레이어 인덱스.
	// 무승부일때는 byte.MaxValue가 들어간다.
	byte win_player_index;


	// 게임이 종료되었는지를 나타내는 플래그.
	bool is_game_finished;

	void Awake()
	{
		game_state = GAME_STATE.READY;

		win_player_index = byte.MaxValue;
	}
	
	void reset()
	{
	}


	void clear()
	{
		is_game_finished = false;
	}

	/// <summary>
	/// 게임방에 입장할 때 호출된다. 리소스 로딩을 시작한다.
	/// </summary>
	public void start_loading(byte player_me_index)
	{
		clear();

		network_manager.message_receiver = this;

        StartCoroutine(Loading());
        CPacket msg = CPacket.create((short)PROTOCOL.LOADING_COMPLETED);

		network_manager.send(msg);
	}

	IEnumerator Loading()
	{

		yield return null;
	}


	/// <summary>
	/// 패킷을 수신 했을 때 호출됨.
	/// </summary>
	/// <param name="protocol"></param>
	/// <param name="msg"></param>
	void on_recv(CPacket msg)
	{
		PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();

		switch (protocol_id)
		{
			case PROTOCOL.GAME_START:
				on_game_start(msg);
				break;

			case PROTOCOL.PLAYER_MOVED:
				on_player_moved(msg);
				break;

			case PROTOCOL.ROOM_REMOVED:
				on_room_removed();
				break;

			case PROTOCOL.GAME_OVER:
				on_game_over(msg);
				break;
		}
	}


	void on_room_removed()
	{
		if (!is_game_finished)
		{
			back_to_main();
		}
	}


	void back_to_main()
	{
		main_title.gameObject.SetActive(true);
		main_title.enter();

		gameObject.SetActive(false);
	}


	void on_game_over(CPacket msg)
	{
		is_game_finished = true;
		win_player_index = msg.pop_byte();
		// 게임 결과 출력
	}


	void Update()
	{
		if (is_game_finished)
		{
			if (Input.GetMouseButtonDown(0))
			{
				back_to_main();
			}
		}
	}

	void on_game_start(CPacket msg)
	{
		players = new List<CPlayer>();

		byte count = msg.pop_byte();
		for (byte i = 0; i < count; ++i)
		{
			byte player_index = msg.pop_byte();

			GameObject obj = new GameObject(string.Format("player{0}", i));
			CPlayer player = obj.AddComponent<CPlayer>();
			player.initialize(player_index);
			player.clear();

			players.Add(player);
		}

		reset();

		game_state = GAME_STATE.STARTED;
	}


	void on_player_moved(CPacket msg)
	{
		byte player_index = msg.pop_byte();
		short from = msg.pop_int16();
		short to = msg.pop_int16();

		//플레이어 이동 처리
	}





	float ratio = 1.0f;


	/// <summary>
	/// 게임 진행 화면 그리기.
	/// </summary>
	void on_playing()
	{
		if (game_state != GAME_STATE.STARTED)
		{
			return;
		}
	}


	/// <summary>
	/// 결과 화면 그리기.
	/// </summary>
	void on_game_result()
	{

	}

	
	void on_click(short cell)
	{

	}

	
	//IEnumerator moving()
	//{
	//}
	
	IEnumerator reproduce(short cell)
	{
		// 기존 - 다른 플레이어의 세균에서 현재 플레이어의 세균으로 옮기기.

		CPlayer current_player = players[current_player_index];
		CPlayer other_player = players.Find(obj => obj.player_index != current_player_index);

		//yield return new WaitForSeconds(0.5f);

		yield return new WaitForSeconds(0.5f);
	}
}
