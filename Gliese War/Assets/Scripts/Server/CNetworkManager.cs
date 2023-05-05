using UnityEngine;
using System;
using System.Collections;
using GameServer;
using System.Net.Sockets;

namespace Client_Unity
{
    public class CNetworkManager : MonoBehaviour
    {
        public enum PROTOCOL : short
        {
            CHAT_MSG_REQ = 1,
            CHAT_MSG_ACK = 2
        }
        CServer gameserver;
        string received_msg;

        public MonoBehaviour message_receiver;

        void Awake()
        {
            //this.received_msg = "";

            // 네트워크 통신을 위해 CServer객체를 추가합니다.
            this.gameserver = gameObject.AddComponent<CServer>();

            // 상태 변화(접속, 끊김등)를 통보 받을 델리게이트 설정.
            this.gameserver.appcallback_on_status_changed += on_status_changed;

            // 패킷 수신 델리게이트 설정.
            this.gameserver.appcallback_on_message += on_message;
        }

        private void Start()
        {
            connect();
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
                        Debug.Log("on connected");
                        CPacket msg = CPacket.create((short)PROTOCOL.CHAT_MSG_REQ);
                        msg.push("Hello!");
                        this.gameserver.send(msg);

                        //GameObject.Find("MainTitle").GetComponent<CMainTitle>().on_connected();
                    }
                    break;

                // 연결 끊김.
                case NETWORK_EVENT.disconnected:
                    Debug.Log("disconnected");
                    //this.received_msg += "disconnected\n";
                    break;
            }
        }

        void on_message(CPacket msg)
        {
            PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();

            switch (protocol_id)
            {
                case PROTOCOL.CHAT_MSG_ACK:
                    {
                        string text = msg.pop_string();
                        GameObject.Find("GameMain").GetComponent<CGameMain>().on_recieve_chat_msg(text);
                    }
                    break;
            }
        }

        public void send(CPacket msg)
        {
            this.gameserver.send(msg);
        }
    }
}