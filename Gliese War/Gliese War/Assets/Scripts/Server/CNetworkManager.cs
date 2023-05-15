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

	void Awake()
	{
		this.received_msg = "";

        // ��Ʈ��ũ ����� ���� CServerUnityService��ü�� �߰��մϴ�.
        this.gameserver = gameObject.AddComponent<CServerUnityService>();

		// ���� ��ȭ(����, �����)�� �뺸 ���� ��������Ʈ ����.
		this.gameserver.appcallback_on_status_changed += on_status_changed;

		// ��Ŷ ���� ��������Ʈ ����.
		this.gameserver.appcallback_on_message += on_message;
	}


	public void connect()
	{
		this.gameserver.connect("127.0.0.1", 7979);
	}

	public bool is_connected()
	{
		return this.gameserver.is_connected();
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
					this.received_msg += "on connected\n";

					GameObject.Find("MainTitle").GetComponent<CMainTitle>().on_connected();
				}
				break;

				// ���� ����.
			case NETWORK_EVENT.disconnected:
				CLogManager.log("disconnected");
				this.received_msg += "disconnected\n";
				break;
		}
	}

	void on_message(CPacket msg)
	{
		this.message_receiver.SendMessage("on_recv", msg);
	}

	public void send(CPacket msg)
	{
		this.gameserver.send(msg);
	}
}
