﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlieseWarServer
{
    using GameServer;
    using System.Diagnostics;

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
            players = new List<CPlayer>();
            player_state = new Dictionary<byte, PLAYER_STATE>();
        }


        /// <summary>
        /// 모든 유저들에게 메시지를 전송한다.
        /// </summary>
        /// <param name="msg"></param>
        void broadcast(CPacket msg)
        {
            players.ForEach(player => player.send_for_broadcast(msg));
            CPacket.destroy(msg);
        }


        /// <summary>
        /// 플레이어의 상태를 변경한다.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="state"></param>
        void change_playerstate(CPlayer player, PLAYER_STATE state)
        {
            if (player_state.ContainsKey(player.player_index))
            {
                player_state[player.player_index] = state;
            }
            else
            {
                player_state.Add(player.player_index, state);
            }
        }

        void change_playerstate(CPlayer player, PLAYER_STATE state, position p)
        {
            if (player_state.ContainsKey(player.player_index))
            {
                player_state[player.player_index] = state;
                CPlayer cp = get_player(player.player_index);
                cp.player_position = p;
                players.Remove(get_player(player.player_index));
                players.Add(cp);
            }
            else
            {
                player_state.Add(player.player_index, state);
            }
        }


        /// <summary>
        /// 모든 플레이어가 특정 상태가 되었는지를 판단한다.
        /// 모든 플레이어가 같은 상태에 있다면 true, 한명이라도 다른 상태에 있다면 false를 리턴한다.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        bool allplayers_ready(PLAYER_STATE state)
        {
            foreach (KeyValuePair<byte, PLAYER_STATE> kvp in player_state)
            {
                if (kvp.Value != state)
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// 매칭이 성사된 플레이어들이 게임에 입장한다.
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        public void enter_gameroom(CGameUser user1, CGameUser user2)
        {
            // 플레이어들을 생성하고 각각 0번, 1번 인덱스를 부여해 준다.
            CPlayer player1 = new CPlayer(user1, 0);        // 1P
            CPlayer player2 = new CPlayer(user2, 1);        // 2P
            players.Clear();
            players.Add(player1);
            players.Add(player2);

            // 플레이어들의 초기 상태를 지정해 준다.
            player_state.Clear();
            change_playerstate(player1, PLAYER_STATE.ENTERED_ROOM);
            change_playerstate(player2, PLAYER_STATE.ENTERED_ROOM);

            // 로딩 시작메시지 전송.
            players.ForEach(player =>
            {
                CPacket msg = CPacket.create((Int16)PROTOCOL.START_LOADING);
                msg.push(player.player_index);  // 본인의 플레이어 인덱스를 알려준다.
                player.send(msg);
            });

            user1.enter_room(player1, this);
            user2.enter_room(player2, this);
        }

        public void enter_gameroom(CGameUser user1)
        {
            // 플레이어들을 생성하고 각각 0번, 1번 인덱스를 부여해 준다.
            CPlayer player1 = new CPlayer(user1, 0);        // 1P
            players.Clear();
            players.Add(player1);

            // 플레이어들의 초기 상태를 지정해 준다.
            player_state.Clear();
            change_playerstate(player1, PLAYER_STATE.ENTERED_ROOM);

            // 로딩 시작메시지 전송.
            players.ForEach(player =>
            {
                CPacket msg = CPacket.create((Int16)PROTOCOL.START_LOADING);
                msg.push(player.player_index);  // 본인의 플레이어 인덱스를 알려준다.
                player.send(msg);
            });

            user1.enter_room(player1, this);
        }

        public void enter_gameroom(List<CGameUser> matching_waiting_users)
        {
            players.Clear();
            player_state.Clear();
            List<CGameUser> mwu = matching_waiting_users;
            byte i = 0;
            mwu.ForEach(cgameuser =>
            {
                CPlayer player = new CPlayer(cgameuser, i);
                players.Add(player);
                change_playerstate(player, PLAYER_STATE.ENTERED_ROOM);

                CPacket msg = CPacket.create((Int16)PROTOCOL.START_LOADING);
                msg.push(player.player_index);  // 본인의 플레이어 인덱스를 알려준다.
                player.send(msg);
                cgameuser.enter_room(player, this);
                i++;
            });            
        }


        /// <summary>
        /// 클라이언트에서 로딩을 완료한 후 요청함.
        /// 이 요청이 들어오면 게임을 시작해도 좋다는 뜻이다.
        /// </summary>
        /// <param name="sender">요청한 유저</param>
        /// LOADING_COMPLETED
        public void loading_complete(CPlayer player)
        {
            // 해당 플레이어를 로딩완료 상태로 변경한다.
            change_playerstate(player, PLAYER_STATE.LOADING_COMPLETE);

            // 모든 유저가 준비 상태인지 체크한다.
            if (!allplayers_ready(PLAYER_STATE.LOADING_COMPLETE))
            {
                Console.WriteLine("!allplayers_ready");
                // 아직 준비가 안된 유저가 있다면 대기한다.
                return;
            }

            // 모두 준비 되었다면 게임을 시작한다.
            battle_start();
        }
        public void loading_complete(CPlayer player, position p)
        {
            // 해당 플레이어를 로딩완료 상태로 변경한다.
            change_playerstate(player, PLAYER_STATE.LOADING_COMPLETE, p);


            // 모든 유저가 준비 상태인지 체크한다.
            if (!allplayers_ready(PLAYER_STATE.LOADING_COMPLETE))
            {
                Console.WriteLine("!allplayers_ready");
                // 아직 준비가 안된 유저가 있다면 대기한다.
                return;
            }

            // 모두 준비 되었다면 게임을 시작한다.
            battle_start();
        }


        /// <summary>
        /// 게임을 시작한다.
        /// </summary>
        void battle_start()
        {
            // 게임을 새로 시작할 때 마다 초기화해줘야 할 것들.
            reset_gamedata();
            // 게임 시작 메시지 전송.
            CPacket msg = CPacket.create((short)PROTOCOL.GAME_START);
            // 플레이어들의 위치 전송.
            msg.push((byte)players.Count);
            players.ForEach(player =>
            {
                msg.push(player.player_index);      // 누구인지 구분하기 위한 플레이어 인덱스.
                msg.push(player.player_position.x);
                msg.push(player.player_position.y);
                msg.push(player.player_position.z);
            });
            broadcast(msg);
        }


        /// <summary>
        /// 게임 데이터를 초기화 한다.
        /// 게임을 새로 시작할 때 마다 초기화 해줘야 할 것들을 넣는다.
        /// </summary>
        void reset_gamedata()
        {
        }

        CPlayer get_player(byte player_index)
        {
            return players.Find(obj => obj.player_index == player_index);
        }


        /// <summary>
        /// 클라이언트의 이동 요청.
        /// </summary>
        /// <param name="sender">요청한 유저</param>
        /// <param name="begin_pos">시작 위치</param>
        /// <param name="target_pos">이동하고자 하는 위치</param>
        public void moving_req(CPlayer sender, position p)
        {
            Console.WriteLine("sender : " + sender.player_index + ", position : " + p.x + ", " + p.y + ", " + p.z);
            // 플레이어 이동 처리
            players.Find(player =>
            {
                return player.player_index == sender.player_index;
            }).SetPosition(p);
            // 최종 결과를 broadcast한다.
            CPacket msg = CPacket.create((short)PROTOCOL.PLAYER_MOVED);
            msg.push(sender.player_index);      // 누가
            msg.push(sender.player_position.x);
            msg.push(sender.player_position.y);
            msg.push(sender.player_position.z);
            //msg.push(sender.transform.position.x);
            //msg.push(sender.transform.position.y); 
            //msg.push(sender.transform.position.z);
            broadcast(msg);
        }

        public void moving_req(CPlayer sender, float moveLR, float moveFB)
        {
            Console.WriteLine("sender : " + sender.player_index + ", moveLR : " + moveLR + ",moveFB : " + moveFB);
            // 플레이어 이동 처리
            players.Find(player =>
            {
                return player.player_index == sender.player_index;
            }).SetPosition(moveLR, moveFB);
            // 최종 결과를 broadcast한다.
            CPacket msg = CPacket.create((short)PROTOCOL.PLAYER_MOVED);
            msg.push(sender.player_index);      // 누가
            msg.push(sender.moveLR);
            msg.push(sender.moveFB);
            //msg.push(sender.transform.position.x);
            //msg.push(sender.transform.position.y); 
            //msg.push(sender.transform.position.z);
            broadcast(msg);
        }

        public void rotate_req(CPlayer sender, float mX)
        {
            Console.WriteLine("sender : " + sender.player_index + ", MouseX : " + mX);
            // 플레이어 이동 처리
            players.Find(player =>
            {
                return player.player_index == sender.player_index;
            }).SetRotation(mX);
            // 최종 결과를 broadcast한다.
            CPacket msg = CPacket.create((short)PROTOCOL.PLAYER_ROTATE);
            msg.push(sender.player_index);      // 누가
            msg.push(sender.MouseX);
            broadcast(msg);
        }


        void game_over()
        {
            // 우승자 가리기.
            byte win_player_index = whostheWinner(players);

            CPacket msg = CPacket.create((short)PROTOCOL.GAME_OVER);
            msg.push(win_player_index);
            broadcast(msg);

            //방 제거.
            Program.game_main.room_manager.remove_room(this);
        }
        public byte whostheWinner(List<CPlayer> players)
        {
            byte result = new byte();
            return result;
        }

        public void destroy()
        {
            CPacket msg = CPacket.create((short)PROTOCOL.ROOM_REMOVED);
            broadcast(msg);

            players.Clear();
        }
    }


}
