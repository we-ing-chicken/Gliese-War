using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace FreeNet
{
    class CListener
    {
        // �񵿱� Accept�� ���� EventArgs.
        SocketAsyncEventArgs accept_args;

        Socket listen_socket;

        // Acceptó���� ������ �����ϱ� ���� �̺�Ʈ ����.
        AutoResetEvent flow_control_event;

        // ���ο� Ŭ���̾�Ʈ�� �������� �� ȣ��Ǵ� �ݹ�.
        public delegate void NewclientHandler(Socket client_socket, object token);
        public NewclientHandler callback_on_newclient;

        public CListener()
        {
            this.callback_on_newclient = null;
        }

        public void start(string host, int port, int backlog)
        {
            this.listen_socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            IPAddress address;
            if (host == "0.0.0.0")
            {
                address = IPAddress.Any;
            }
            else
            {
                address = IPAddress.Parse(host);
            }
            IPEndPoint endpoint = new IPEndPoint(address, port);

            try
            {
                listen_socket.Bind(endpoint);
                listen_socket.Listen(backlog);

                this.accept_args = new SocketAsyncEventArgs();
                this.accept_args.Completed += new EventHandler<SocketAsyncEventArgs>(on_accept_completed);

                Thread listen_thread = new Thread(do_listen);
                listen_thread.Start();
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
            }
        }

		void do_listen()
        {
            this.flow_control_event = new AutoResetEvent(false);

            while (true)
            {
                
                this.accept_args.AcceptSocket = null;

                bool pending = true;
                try
                {
                    pending = listen_socket.AcceptAsync(this.accept_args);
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e.Message);
                    continue;
                }

                if (!pending)
                {
                    on_accept_completed(null, this.accept_args);
                }

                this.flow_control_event.WaitOne();

            }
        }

		void on_accept_completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                Socket client_socket = e.AcceptSocket;

                this.flow_control_event.Set();

                if (this.callback_on_newclient != null)
                {
                    this.callback_on_newclient(client_socket, e.UserToken);
                }

                return;
            }
            else
            {
                Console.WriteLine("Failed to accept client.");
            }
            this.flow_control_event.Set();
        }

        //int connected_count = 0;
        //void on_new_client(Socket client_socket)
        //{
        //    //Interlocked.Increment(ref this.connected_count);

        //    //Console.WriteLine(string.Format("[{0}] A client connected. handle {1},  count {2}", 
        //    //	Thread.CurrentThread.ManagedThreadId, client_socket.Handle,
        //    //	this.connected_count));
        //    //return;

        //    // ������ �����Ǹ� ��Ŷ ��� ���� �غ� �Ѵ�.
        //    //StateObject state = new StateObject(client_socket);
        //    //state.remain_size_to_read = CPacket.HEADER_SIZE;
        //    //client_socket.BeginReceive(state.buffer, 0, CPacket.HEADER_SIZE, 0,
        //    //	new AsyncCallback(on_recv_header), state);
        //}

        //void on_recv_header(IAsyncResult iar)
        //{
        //StateObject state = (StateObject)iar.AsyncState;

        //int read_size = state.workSocket.EndReceive(iar);

        //// ����� ©���� �� ��� ������ ��ŭ �ٽ� �о�´�.
        //if (state.remain_size_to_read > read_size)
        //{
        //	state.remain_size_to_read -= read_size;
        //	state.workSocket.BeginReceive(state.buffer, read_size, state.remain_size_to_read, SocketFlags.None,
        //		new AsyncCallback(on_recv_header), state);
        //	return;
        //}

        ////Console.WriteLine("read size " + remain_size);
        //if (read_size <= 0)
        //{
        //	Console.WriteLine(string.Format("[{0}] [on_disconnect]  client socket {1}",
        //		System.Threading.Thread.CurrentThread.ManagedThreadId,
        //		state.workSocket.Handle));
        //	state.workSocket.Close();
        //	return;
        //}
        //else
        //{
        //	// �ٵ� ������ �Ľ�.
        //	Int16 body_size = BitConverter.ToInt16(state.buffer, 0);

        //	if (body_size <= 0 || body_size > 10240)
        //	{
        //		state.workSocket.Close();
        //		return;
        //	}

        //	// �������� id �Ľ�.
        //	short protocol_id = BitConverter.ToInt16(state.buffer, 2);
        //	state.set_protocol(protocol_id);

        //	state.body_size = body_size;
        //	state.remain_size_to_read = state.body_size;

        //	// �ٵ� �б�.
        //	Array.Clear(state.buffer, 0, state.buffer.Length);
        //	state.workSocket.BeginReceive(state.buffer, 0, body_size, 0,
        //		new AsyncCallback(on_recv), state);
        //}
        //}

        //void on_recv(IAsyncResult iar)
        //{
        //StateObject state = (StateObject)iar.AsyncState;

        //int read_size = state.workSocket.EndReceive(iar);
        //if (state.remain_size_to_read > read_size)
        //{
        //	state.remain_size_to_read -= read_size;
        //	state.workSocket.BeginReceive(state.buffer, read_size, state.remain_size_to_read, SocketFlags.None,
        //		new AsyncCallback(on_recv), state);
        //	return;
        //}

        //if (read_size <= 0)
        //{
        //	Console.WriteLine(string.Format("[{0}] [on_disconnect]  client socket {1}",
        //		System.Threading.Thread.CurrentThread.ManagedThreadId,
        //		state.workSocket.Handle));
        //	state.workSocket.Close();
        //}
        //else
        //{
        //	try
        //	{
        //		Interlocked.Increment(ref recv_body_count);

        //		// �ϳ��� ��Ŷ ���̸�ŭ�� �߶� peer�� ����.
        //		byte[] clone_buffer = new byte[state.body_size];
        //		System.Buffer.BlockCopy(state.buffer, 0, clone_buffer, 0, state.body_size);

        //		// Peer��ü�� �����ϰ� ���� �޽����� ���� �� �ֵ��� �ٽ� ����� ���� �غ� �Ѵ�.
        //		CPacket msg = new CPacket(state.protocol_id, clone_buffer);
        //		state.peer.on_recv(msg);

        //		// �� ���۴� ���� ��Ŷ�� ���� �� ����� ���̱� ������ ����ϰ� Ŭ���� ���ش�.
        //		Array.Clear(state.buffer, 0, state.buffer.Length);

        //		// ��� �б�.
        //		state.remain_size_to_read = CPacket.HEADER_SIZE;
        //		state.workSocket.BeginReceive(state.buffer, 0, CPacket.HEADER_SIZE, 0,
        //			new AsyncCallback(on_recv_header), state);
        //	}
        //	catch (Exception e)
        //	{
        //		Console.WriteLine(e.Message + "\n" + e.StackTrace);

        //		//byte[] clone_buffer = new byte[state.body_size];
        //		//System.Buffer.BlockCopy(state.buffer, 0, clone_buffer, 0, state.body_size);

        //		//CPacket msg = new CPacket(clone_buffer);
        //		//Int32 data = msg.pop_int32();

        //		//Console.WriteLine("int32 " + data + ",   buffer : " + clone_buffer.Length);
        //	}
        //}
        //}
    }
}
