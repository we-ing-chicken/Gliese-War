using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameServer;
using GlieseWarGameServer;
using TMPro;
using System;

public class CBattleRoom : MonoBehaviour 
{
    private static CBattleRoom _instance;
    enum GAME_STATE
	{
		READY = 0,
		STARTED
	}

    public static CBattleRoom Instance
    {
        get
        {
            if (!_instance)
            {
                if (_instance == null)
                    return null;

                _instance = FindObjectOfType(typeof(CBattleRoom)) as CBattleRoom;
            }

            return _instance;
        }
    }

    // 현재 턴을 진행중인 플레이어 인덱스.
    byte current_player_index;
	public byte my_player_index;

    public List<CPlayer> players;

	public GameObject prefab;

	public GameObject spawn;
    [SerializeField]
    TMP_Text txt;

    // 게임 상태에 따라 각각 다른 GUI모습을 구현하기 위해 필요한 상태 변수.
    GAME_STATE game_state;

	// 승리한 플레이어 인덱스.
	// 무승부일때는 byte.MaxValue가 들어간다.
	byte win_player_index;


	// 게임이 종료되었는지를 나타내는 플래그.
	bool is_game_finished;

	void Awake()
	{
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

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
	public void start_loading(byte player_index)
	{
		clear();
		my_player_index = player_index;

        CNetworkManager.Instance.message_receiver = this;

        StartCoroutine(Loading());
        CPacket msg = CPacket.create((short)PROTOCOL.LOADING_COMPLETED);
		GameObject s = spawn.transform.GetChild(my_player_index).gameObject;
		msg.push(s.transform.position.x);
        msg.push(s.transform.position.y);
        msg.push(s.transform.position.z);

        CNetworkManager.Instance.send(msg);
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
		BattleManager.Instance.gameObject.SetActive(true);
        BattleManager.Instance.enter();

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
		txt.text = game_state.ToString();
    }

	void on_game_start(CPacket msg)
	{
        SwitchCanvasActive(BattleManager.Instance.BattleCanvas);
		players = new List<CPlayer>();
		Debug.Log(my_player_index);
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
			if (my_player_index == player_index)
			{
				obj1.GetComponent<CPlayer>().isMine = true;
				//obj1.transform.GetChild(1).GetComponent<CServercam>().enabled = true;
				//obj1.transform.GetComponentInChildren<CServercam>().enabled = true;
			}
            CPlayer player = obj1.GetComponent<CPlayer>();
			//obj1.GetComponent<CharacterController>().enabled = true;
			player.initialize(player_index);
			
			player.clear();

			players.Add(player);
		}

		reset();

		game_state = GAME_STATE.STARTED;
	}

    private void SwitchCanvasActive(Canvas temp)
    {
        if (temp.gameObject.activeSelf)
            temp.gameObject.SetActive(false);
        else
            temp.gameObject.SetActive(true);
    }


    void on_player_moved(CPacket msg)
	{
		byte player_index = msg.pop_byte();
		float x = msg.pop_float();
        float y = msg.pop_float();
        float z = msg.pop_float();

		//Debug.Log((int)player_index);
        //Debug.Log(new Vector3(x,y,z));
		if (my_player_index == player_index)
			return;

        //플레이어 이동 처리
        players.Find(player =>
		{
			return player.player_index == player_index;
		}).transform.position = new Vector3(x, y, z);
        Debug.Log("===========================");

		//players.ForEach(player =>
		//{
		//	Debug.Log(player.player_index + ", " + player.transform.position);
		//});

    }

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
