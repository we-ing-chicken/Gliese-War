using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using GameServer;

namespace GlieseWarServer
{
    public class CGameUser : IPeer
    {
        CUserToken token;
        public CGameRoom battle_room { get; private set; }

        CPlayer player;
        public CGameUser(CUserToken cutoken)
        {
            token = cutoken;
            token.set_peer(this);
        }
        void IPeer.on_message(Const<byte[]> buffer)
        {
            // ex)
            byte[] clone = new byte[1024];
            Array.Copy(buffer.Value, clone, buffer.Value.Length);
            CPacket msg = new CPacket(clone, this);
            Program.game_main.enqueue_packet(msg, this);
        }

        void IPeer.on_removed()
        {
            Console.WriteLine("The client disconnected.");

            Program.remove_user(this);
        }

        public void send(CPacket msg)
        {
           token.send(msg);
        }

        void IPeer.disconnect()
        {
            token.socket.Disconnect(false);
        }
        // 리시브 처리.
        void IPeer.process_user_operation(CPacket msg)
        {
            PROTOCOL protocol = (PROTOCOL)msg.pop_protocol_id();
            Console.WriteLine("protocol id " + protocol);
            switch (protocol)
            {
                case PROTOCOL.ENTER_GAME_ROOM_REQ:
                    Program.game_main.matching_req(this);
                    break;

                case PROTOCOL.LOADING_COMPLETED:
                    position p = new position();
                    p.x = msg.pop_float();
                    p.y = msg.pop_float();
                    p.z = msg.pop_float();
                    battle_room.loading_complete(player, p);
                    break;

                case PROTOCOL.MOVING_REQ:
                    {
                        float moveLR = msg.pop_float();
                        float moveFB = msg.pop_float();

                        //position po = new position();
                        //po.x = msg.pop_float();
                        //po.y = msg.pop_float();
                        //po.z = msg.pop_float();

                        battle_room.moving_req(this.player, moveLR, moveFB);
                    }
                    break;

                case PROTOCOL.ROTATE_REQ:
                    {
                        float mouseX = msg.pop_float();

                        battle_room.rotate_req(this.player, mouseX);
                    }
                    break;
            }
        }

        public void enter_room(CPlayer cplayer, CGameRoom room)
        {
            player = cplayer;
            battle_room = room;
        }
    }
}
