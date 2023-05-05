using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlieseWarServer
{
    using GameServer;
    public class CGameRoom
    {
        enum PLAYER_STATE : byte
        {
            // 방에 막 입장한 상태.
            ENTERED_ROOM,

            // 로딩을 완료한 상태.
            LOADING_COMPLETE,

            // 턴 진행 준비 상태.
            READY_TO_TURN,

            // 턴 연출을 모두 완료한 상태.
            CLIENT_TURN_FINISHED
        }
        List<CPlayer> players;

        Dictionary<byte, PLAYER_STATE> player_state;

        public CGameRoom()
        {
            this.players = new List<CPlayer>();
            this.player_state = new Dictionary<byte, PLAYER_STATE>();
        }

        void broadcast(CPacket msg)
        {
            this.players.ForEach(player => player.send_for_broadcast(msg));
            CPacket.destroy(msg);
        }






    }


}
