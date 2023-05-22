using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameServer;
using GlieseWarGameServer;
using TMPro;
using System;

public class CBattleRoom : MonoBehaviour 
{
	enum GAME_STATE
	{
		READY = 0,
		STARTED
	}

    // ���� ���� �������� �÷��̾� �ε���.
    byte current_player_index;
	public byte my_player_index;

    List<CPlayer> players;

	public GameObject prefab;

	public GameObject spawn;

    // ��Ʈ��ũ ������ ��,������ ���� ��Ʈ��ũ �Ŵ��� ���۷���.
    [SerializeField]
    CNetworkManager network_manager;
    [SerializeField]
     TMP_Text txt;

    // ���� ���¿� ���� ���� �ٸ� GUI����� �����ϱ� ���� �ʿ��� ���� ����.
    GAME_STATE game_state;

	// �¸��� �÷��̾� �ε���.
	// ���º��϶��� byte.MaxValue�� ����.
	byte win_player_index;


	// ������ ����Ǿ������� ��Ÿ���� �÷���.
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
	/// ���ӹ濡 ������ �� ȣ��ȴ�. ���ҽ� �ε��� �����Ѵ�.
	/// </summary>
	public void start_loading(byte player_index)
	{
		clear();
		my_player_index = player_index;

        network_manager.message_receiver = this;

        StartCoroutine(Loading());
        CPacket msg = CPacket.create((short)PROTOCOL.LOADING_COMPLETED);
		GameObject s = spawn.transform.GetChild(my_player_index+1).gameObject;
		Debug.Log("LOADING_COMPLETED : " + s.transform.position);
		msg.push(s.transform.position.x);
        msg.push(s.transform.position.y);
        msg.push(s.transform.position.z);

        network_manager.send(msg);
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

			case PROTOCOL.PLAYER_ROTATE:
				on_player_rotate(msg);
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
		BattleManager.Instance.gameObject.SetActive(true);
        BattleManager.Instance.enter();

		gameObject.SetActive(false);
	}


	void on_game_over(CPacket msg)
	{
		is_game_finished = true;
		win_player_index = msg.pop_byte();
		// ���� ��� ���
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
		txt.text = game_state.ToString();
    }

	void on_game_start(CPacket msg)
	{
		players = new List<CPlayer>();

		byte count = msg.pop_byte();
		for (byte i = 0; i < count; ++i)
		{
			byte player_index = msg.pop_byte();
			float player_x = msg.pop_float();
            float player_y = msg.pop_float();
            float player_z = msg.pop_float();

            //GameObject obj = new GameObject(string.Format("player{0}", i));
            GameObject obj1 = Instantiate(prefab);
			obj1.transform.position = new Vector3(player_x, player_y, player_z);
			Debug.Log("obj1 : " + obj1.transform.position);

            CPlayer player = obj1.AddComponent<CPlayer>();
			player.initialize(player_index, this);
			player.clear();

			players.Add(player);
		}

		reset();

		game_state = GAME_STATE.STARTED;
	}


	void on_player_moved(CPacket msg)
	{
		byte player_index = msg.pop_byte();
		float x = msg.pop_int32();
		float y = msg.pop_int32();
		float z = msg.pop_int32();

		//�÷��̾� �̵� ó��
	}

        void on_player_rotate(CPacket msg)
        {
            byte player_index = msg.pop_byte();
            float x = msg.pop_int32();
            float y = msg.pop_int32();
            float z = msg.pop_int32();

            //�÷��̾� �̵� ó��
        }

        /// <summary>
        /// ���� ���� ȭ�� �׸���.
        /// </summary>
        void on_playing()
	{
		if (game_state != GAME_STATE.STARTED)
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

		CPlayer current_player = players[current_player_index];
		CPlayer other_player = players.Find(obj => obj.player_index != current_player_index);

		//yield return new WaitForSeconds(0.5f);

		yield return new WaitForSeconds(0.5f);
	}
}
