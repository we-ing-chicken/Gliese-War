using GameServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client_Unity
{
    public class CGameMain : MonoBehaviour
    {
        public enum PROTOCOL : short
        {
            CHAT_MSG_REQ = 1,
            CHAT_MSG_ACK = 2
        }
        string input_text;
        List<string> recieved_texts;
        CNetworkManager networkManager;

        Vector2 currentScrollPos = new Vector2();

        private void Awake()
        {
            this.input_text = string.Empty;
            this.recieved_texts = new List<string>();
            this.networkManager = GameObject.Find("NetworkManager").GetComponent<CNetworkManager>();
        }

        public void on_recieve_chat_msg(string text)
        {
            this.recieved_texts.Add(text);
            this.currentScrollPos.y = float.PositiveInfinity;
        }
        
    }
}
