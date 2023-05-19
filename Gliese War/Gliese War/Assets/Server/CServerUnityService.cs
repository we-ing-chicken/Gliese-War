using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using GameServer;

namespace Server_Unity
{
	public class CServerUnityService : MonoBehaviour
	{
		CServerEventManager event_manager;

		// ����� ���� ���� ��ü.
		IPeer gameserver;

		// TCP����� ���� ���� ��ü.
		CNetworkService service;

		// ���� �Ϸ�� ȣ��Ǵ� ��������Ʈ. ���ø����̼ǿ��� �ݹ� �żҵ带 �����Ͽ� ����Ѵ�.
		public delegate void StatusChangedHandler(NETWORK_EVENT status);
		public StatusChangedHandler appcallback_on_status_changed;

		// ��Ʈ��ũ �޽��� ���Ž� ȣ��Ǵ� ��������Ʈ. ���ø����̼ǿ��� �ݹ� �żҵ带 �����Ͽ� ����Ѵ�.
		public delegate void MessageHandler(CPacket msg);
		public MessageHandler appcallback_on_message;

		void Awake()
		{
			CPacketBufferManager.initialize(10);
			event_manager = new CServerEventManager();
		}

		public void connect(string host, int port)
		{
			if (service != null)
			{
				Debug.LogError("Already connected.");
				return;
			}

			// CNetworkService��ü�� �޽����� �񵿱� ��,���� ó���� �����Ѵ�.
			service = new CNetworkService();

			// endpoint������ �����ִ� Connector����. ������ NetworkService��ü�� �־��ش�.
			CConnector connector = new CConnector(service);
			// ���� ������ ȣ��� �ݹ� �żҵ� ����.
			connector.connected_callback += on_connected_gameserver;
			IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(host), port);
			connector.connect(endpoint);
		}


		public bool is_connected()
		{
			return gameserver != null;
		}


		/// <summary>
		/// ���� ������ ȣ��� �ݹ� �żҵ�.
		/// </summary>
		/// <param name="server_token"></param>
		void on_connected_gameserver(CUserToken server_token)
		{
			gameserver = new CRemoteServerPeer(server_token);
			((CRemoteServerPeer)gameserver).set_eventmanager(event_manager);

			// ����Ƽ ���ø����̼����� �̺�Ʈ�� �Ѱ��ֱ� ���ؼ� �Ŵ����� ť�� ���� �ش�.
			event_manager.enqueue_network_event(NETWORK_EVENT.connected);
		}

		/// <summary>
		/// ��Ʈ��ũ���� �߻��ϴ� ��� �̺�Ʈ�� Ŭ���̾�Ʈ���� �˷��ִ� ������ Update���� �����Ѵ�.
		/// FreeNet������ �޽��� �ۼ��� ó���� ��Ŀ�����忡�� ��������� ����Ƽ�� ���� ó���� ���� �����忡�� ����ǹǷ�
		/// ť��ó���� ���Ͽ� ���� �����忡�� ��� ���� ó���� �̷�������� �����Ͽ���.
		/// </summary>
		void Update()
		{
			// ���ŵ� �޽����� ���� �ݹ�.
			if (event_manager.has_message())
			{
				CPacket msg = event_manager.dequeue_network_message();
				if (appcallback_on_message != null)
				{
					appcallback_on_message(msg);
				}
			}

			// ��Ʈ��ũ �߻� �̺�Ʈ�� ���� �ݹ�.
			if (event_manager.has_event())
			{
				NETWORK_EVENT status = event_manager.dequeue_network_event();
				if (appcallback_on_status_changed != null)
				{
					appcallback_on_status_changed(status);
				}
			}
		}

		public void send(CPacket msg)
		{
			try
			{
				gameserver.send(msg);
				CPacket.destroy(msg);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}
		}

		/// <summary>
		/// �������� ����ÿ��� OnApplicationQuit�żҵ忡�� disconnect�� ȣ���� ��� ����Ƽ�� hang���� �ʴ´�.
		/// </summary>
		void OnApplicationQuit()
		{
			if (gameserver != null)
			{
				((CRemoteServerPeer)gameserver).token.disconnect();
			}
		}
	}

}
