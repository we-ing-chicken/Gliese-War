using GlieseWarServer;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameServer;
using UnityEngine;
using System;

public class ServeManager : MonoBehaviour
{
    static List<CGameUser> userlist;
    public static CGameServer game_main;
    public Canvas canvas;

    private void Start()
    {
        CPacketBufferManager.initialize(2000);
        game_main = new CGameServer();
        userlist = new List<CGameUser>();
        CNetworkService service = new CNetworkService();

        service.session_created_callback += on_session_created;
        // √ ±‚»≠.
        service.initialize();
        service.listen("0.0.0.0", 7979, 100);

        Debug.Log("Started!");
    }

    static void on_session_created(CUserToken token)
    {
        CGameUser user = new CGameUser(token);
        lock (userlist)
        {
            userlist.Add(user);
        }
    }


}
