using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlieseWarServer
{
    using GameServer;
    public class CPlayer
    {
        CGameUser owner;

        public byte player_index { get; private set; }

        public CPlayer(CGameUser user, byte player_index)
        {
            this.owner = user;
            this.player_index = player_index;
        }
        public void reset()
        {
        }

        public void send(CPacket msg)
        {
            this.owner.send(msg);
            CPacket.destroy(msg);
        }
    }
}
