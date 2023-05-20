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

        // ��Ʈ��ũ ����� ���� CServerUnityService��ü�� �߰��մϴ�.
        gameserver = gameObject.AddComponent<CServerUnityService>();

		// ���� ��ȭ(����, �����)�� �뺸 ���� ��������Ʈ ����.
		gameserver.appcallback_on_status_changed += on_status_changed;

		// ��Ŷ ���� ��������Ʈ ����.
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
	/// ��Ʈ��ũ ���� ����� ȣ��� �ݹ� �żҵ�.
	/// </summary>
	/// <param name="server_token"></param>
	void on_status_changed(NETWORK_EVENT status)
	{
		switch (status)
		{
				// ���� ����.
			case NETWORK_EVENT.connected:
				{
					CLogManager.log("on connected");
					received_msg += "on connected\n";

                    battleManager.on_connected();
				}
				break;

				// ���� ����.
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