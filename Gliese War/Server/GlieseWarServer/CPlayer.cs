using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace GlieseWarServer
{
    using GameServer;

    public struct position
    {
        public float x, y, z;
    }

    public class CPlayer
    {
        CGameUser owner;

        public byte player_index { get; private set; }

        public position player_position;
        public float moveLR;
        public float moveFB;
        public float MouseX;

        public CPlayer(CGameUser user, byte my_player_index)
        {
            owner = user;
            player_index = my_player_index;
            Clear();
        }
        public void reset()
        {
        }

        public void send(CPacket msg)
        {
            owner.send(msg);
            CPacket.destroy(msg);
        }

        public void send_for_broadcast(CPacket msg)
        {
            owner.send(msg);
        }

        public void Clear()
        {
            player_position.x = 0;
            player_position.y = 0;
            player_position.z = 0;

        }
        public void SetPosition(position p)
        {
            //transform.position = new Vector3(p.x, p.y, p.z);
            player_position.x = p.x;
            player_position.y = p.y;
            player_position.z = p.z;
        }
        public void SetPosition(float mymoveLR, float mymoveFB)
        {
            moveLR = mymoveLR;
            moveFB = mymoveFB;
        }

        public void SetRotation(float myMouseX)
        {
            MouseX = myMouseX;
        }
    }
}
