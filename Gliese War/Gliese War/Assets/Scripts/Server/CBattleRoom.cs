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

    // ���� ���� �������� �÷��̾� �ε���.
    byte current_player_index;

    List<CPlayer> players;

	// ���� ���� �� �������� ���ư� �� ����ϱ� ���� MainTitle��ü�� ���۷���.
	CMainTitle main_title;

	// ��Ʈ��ũ ������ ��,������ ���� ��Ʈ��ũ �Ŵ��� ���۷���.
	CNetworkManager network_manager;

	// ���� ���¿� ���� ���� �ٸ� GUI����� �����ϱ� ���� �ʿ��� ���� ����.
	GAME_STATE game_state;

	// �¸��� �÷��̾� �ε���.
	// ���º��϶��� byte.MaxValue�� ����.
	byte win_player_index;


	// ������ ����Ǿ������� ��Ÿ���� �÷���.
	bool is_game_finished;

	void Awake()
	{

		this.network_manager = GameObject.Find("NetworkManager").GetComponent<CNetworkManager>();

		this.game_state = GAME_STATE.READY;

		this.main_title = GameObject.Find("MainTitle").GetComponent<CMainTitle>();

		this.win_player_index = byte.MaxValue;
	}
	
	void reset()
	{
	}


	void clear()
	{
		this.is_game_finished = false;
	}

	/// <summary>
	/// ���ӹ濡 ������ �� ȣ��ȴ�. ���ҽ� �ε��� �����Ѵ�.
	/// </summary>
	public void start_loading(byte player_me_index)
	{
		clear();

		this.network_manager.message_receiver = this;

        StartCoroutine(Loading());
        CPacket msg = CPacket.create((short)PROTOCOL.LOADING_COMPLETED);

		this.network_manager.send(msg);
	}

	IEnumerator Loading()
	{

		yield return null;
	}


	/// <summary>
	/// ��Ŷ�� ���� ���� �� ȣ���.
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
		this.main_title.gameObject.SetActive(true);
		this.main_title.enter();

		gameObject.SetActive(false);
	}


	void on_game_over(CPacket msg)
	{
		this.is_game_finished = true;
		this.win_player_index = msg.pop_byte();
		// ���� ��� ���
	}


	void Update()
	{
		if (this.is_game_finished)
		{
			if (Input.GetMouseButtonDown(0))
			{
				back_to_main();
			}
		}
	}

	void on_game_start(CPacket msg)
	{
		this.players = new List<CPlayer>();

		byte count = msg.pop_byte();
		for (byte i = 0; i < count; ++i)
		{
			byte player_index = msg.pop_byte();

			GameObject obj = new GameObject(string.Format("player{0}", i));
			CPlayer player = obj.AddComponent<CPlayer>();
			player.initialize(player_index);
			player.clear();

			this.players.Add(player);
		}

		reset();

		this.game_state = GAME_STATE.STARTED;
	}


	void on_player_moved(CPacket msg)
	{
		byte player_index = msg.pop_byte();
		short from = msg.pop_int16();
		short to = msg.pop_int16();

		//�÷��̾� �̵� ó��
	}





	float ratio = 1.0f;


	/// <summary>
	/// ���� ���� ȭ�� �׸���.
	/// </summary>
	void on_playing()
	{
		if (this.game_state != GAME_STATE.STARTED)
		{
			return;
		}
	}


	/// <summary>
	/// ��� ȭ�� �׸���.
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
		// ���� - �ٸ� �÷��̾��� ���տ��� ���� �÷��̾��� �������� �ű��.

		CPlayer current_player = this.players[this.current_player_index];
		CPlayer other_player = this.players.Find(obj => obj.player_index != this.current_player_index);

		//yield return new WaitForSeconds(0.5f);

		yield return new WaitForSeconds(0.5f);
	}
}
