using UnityEngine;
using System;
using System.Collections;
using GameServer;
using Server_Unity;
using GlieseWarGameServer;

public class CNetworkManager : MonoBehaviour {

    CServerUnityService gameserver;
	string received_msg;

	public MonoBehaviour message_receiver;
	[SerializeField] BattleManager battleManager;

    void Awake()
	{
		received_msg = "";

        // 네트워크 통신을 위해 CServerUnityService객체를 추가합니다.
        gameserver = gameObject.AddComponent<CServerUnityService>();

		// 상태 변화(접속, 끊김등)를 통보 받을 델리게이트 설정.
		gameserver.appcallback_on_status_changed += on_status_changed;

		// 패킷 수신 델리게이트 설정.
		gameserver.appcallback_on_message += on_message;
	}


	public void connect()
	{
		gameserver.connect("127.0.0.1", 7979);
	}

	public bool is_connected()
	{
		return gameserver.is_connected();
	}

	/// <summary>
	/// 네트워크 상태 변경시 호출될 콜백 매소드.
	/// </summary>
	/// <param name="server_token"></param>
	void on_status_changed(NETWORK_EVENT status)
	{
		switch (status)
		{
				// 접속 성공.
			case NETWORK_EVENT.connected:
				{
					CLogManager.log("on connected");
					received_msg += "on connected\n";

                    battleManager.on_connected();
				}
				break;

				// 연결 끊김.
			case NETWORK_EVENT.disconnected:
				CLogManager.log("disconnected");
				received_msg += "disconnected\n";
				break;
		}
	}

	void on_message(CPacket msg)
	{
		message_receiver.SendMessage("on_recv", msg);
	}

	public void send(CPacket msg)
	{
		gameserver.send(msg);
	}
}
