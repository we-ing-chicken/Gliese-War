using FreeNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CServer;

public class CGameMain : MonoBehaviour
{
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
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        currentScrollPos = GUILayout.BeginScrollView(currentScrollPos, GUILayout.MaxWidth(Screen.width), GUILayout.MinWidth(Screen.width), GUILayout.MaxWidth(Screen.height - 100), GUILayout.MinWidth(Screen.height - 100));
        foreach(string text in recieved_texts)
        {
            GUILayout.BeginHorizontal();
            GUI.skin.label.wordWrap = true;
            GUILayout.Label(text);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        this.input_text = GUILayout.TextField(this.input_text, GUILayout.MaxWidth(Screen.width - 100), GUILayout.MinWidth(Screen.width - 100), GUILayout.MaxHeight(50), GUILayout.MaxHeight(50));
        if (GUILayout.Button("Send", GUILayout.MaxWidth(100), GUILayout.MinWidth(100), GUILayout.MaxHeight(50), GUILayout.MaxHeight(50)))
        {
            CPacket msg = CPacket.create((short)PROTOCOL.CHAT_MSG_REQ);
            msg.push(this.input_text);
            this.networkManager.send(msg);
            this.input_text = string.Empty;
        }
        GUILayout.EndHorizontal();
    }
}
