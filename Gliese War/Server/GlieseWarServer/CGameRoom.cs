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

        void change_playerstate(CPlayer player, PLAYER_STATE state)
        {
            if (this.player_state.ContainsKey(player.player_index))
            {
                this.player_state[player.player_index] = state;
            }
            else
            {
                this.player_state.Add(player.player_index, state);
            }
        }

        bool allplayers_ready(PLAYER_STATE state)
        {
            foreach (KeyValuePair<byte, PLAYER_STATE> kvp in this.player_state)
            {
                if (kvp.Value != state)
                {
                    return false;
                }
            }

            return true;
        }

        public void enter_gameroom(CGameUser user1, CGameUser user2)
        {
            // 플레이어들을 생성하고 각각 0번, 1번 인덱스를 부여해 준다.
            CPlayer player1 = new CPlayer(user1, 0);        // 1P
            CPlayer player2 = new CPlayer(user2, 1);        // 2P
            this.players.Clear();
            this.players.Add(player1);
            this.players.Add(player2);

            // 플레이어들의 초기 상태를 지정해 준다.
            this.player_state.Clear();
            change_playerstate(player1, PLAYER_STATE.ENTERED_ROOM);
            change_playerstate(player2, PLAYER_STATE.ENTERED_ROOM);

            // 로딩 시작메시지 전송.
            this.players.ForEach(player =>
            {
                CPacket msg = CPacket.create((Int16)PROTOCOL.START_LOADING);
                msg.push(player.player_index);  // 본인의 플레이어 인덱스를 알려준다.
                player.send(msg);
            });

            user1.enter_room(player1, this);
            user2.enter_room(player2, this);
        }

        public void loading_complete(CPlayer player)
        {
            // 해당 플레이어를 로딩완료 상태로 변경한다.
            change_playerstate(player, PLAYER_STATE.LOADING_COMPLETE);

            // 모든 유저가 준비 상태인지 체크한다.
            if (!allplayers_ready(PLAYER_STATE.LOADING_COMPLETE))
            {
                // 아직 준비가 안된 유저가 있다면 대기한다.
                return;
            }

            // 모두 준비 되었다면 게임을 시작한다.
            battle_start();
        }

        void battle_start()
        {
            // 게임을 새로 시작할 때 마다 초기화해줘야 할 것들.
            reset_gamedata();

            // 게임 시작 메시지 전송.
            CPacket msg = CPacket.create((short)PROTOCOL.GAME_START);
            // 플레이어들의 세균 위치 전송.
            msg.push((byte)this.players.Count);
            this.players.ForEach(player =>
            {
                
            });
            broadcast(msg);
        }

        void reset_gamedata()
        {
        }

        CPlayer get_player(byte player_index)
        {
            return this.players.Find(obj => obj.player_index == player_index);
        }

        public void moving_req(CPlayer sender, short begin_pos, short target_pos)
        {
            // 플레이어 이동 처리

            // 최종 결과를 broadcast한다.
            CPacket msg = CPacket.create((short)PROTOCOL.PLAYER_MOVED);
            msg.push(sender.player_index);      // 누가
            msg.push(begin_pos);                // 어디서
            msg.push(target_pos);               // 어디로 이동 했는지
            broadcast(msg);
        }

        void game_over()
        {
            // 우승자 가리기.
            byte win_player_index = byte.MaxValue;



            CPacket msg = CPacket.create((short)PROTOCOL.GAME_OVER);
            msg.push(win_player_index);
            broadcast(msg);

            //방 제거.
            Program.game_main.room_manager.remove_room(this);
        }

        public void destroy()
        {
            CPacket msg = CPacket.create((short)PROTOCOL.ROOM_REMOVED);
            broadcast(msg);

            this.players.Clear();
        }
    }


}
