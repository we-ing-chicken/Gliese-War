using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace FreeNet
{
    public class CNetworkService
    {
        int connected_count;
        CListener client_listener;
        SocketAsyncEventArgsPool receive_event_args_pool;
        SocketAsyncEventArgsPool send_event_args_pool;
        BufferManager buffer_manager;

        public delegate void SessionHandler(CUserToken token);
        public SessionHandler session_created_callback { get; set; }

        // configs.
        int max_connections;
        int buffer_size;
        readonly int pre_alloc_count = 2;       // read, write

        public CNetworkService()
        {
            this.connected_count = 0;
            this.session_created_callback = null;
        }

        // Initializes the server by preallocating reusable buffers and 
        // context objects.  These objects do not need to be preallocated 
        // or reused, but it is done this way to illustrate how the API can 
        // easily be used to create reusable objects to increase server performance.
        //
        public void initialize()
        {
            this.max_connections = 10000;
            this.buffer_size = 1024;

            this.buffer_manager = new BufferManager(this.max_connections * this.buffer_size * this.pre_alloc_count, this.buffer_size);
            this.receive_event_args_pool = new SocketAsyncEventArgsPool(this.max_connections);
            this.send_event_args_pool = new SocketAsyncEventArgsPool(this.max_connections);

            this.buffer_manager.InitBuffer();

            SocketAsyncEventArgs arg;

            for (int i = 0; i < this.max_connections; i++)
            {
                // ������ ���Ͽ� ��� send, receive�� �ϹǷ�
                // user token�� ���Ǻ��� �ϳ����� ����� ���� 
                // receive, send EventArgs���� ������ token�� �����ϵ��� �����Ѵ�.
                CUserToken token = new CUserToken();

                // receive pool
                {
                    arg = new SocketAsyncEventArgs();
                    arg.Completed += new EventHandler<SocketAsyncEventArgs>(receive_completed);
                    arg.UserToken = token;

                    this.buffer_manager.SetBuffer(arg);

                    this.receive_event_args_pool.Push(arg);
                }


                // send pool
                {
                    arg = new SocketAsyncEventArgs();
                    arg.Completed += new EventHandler<SocketAsyncEventArgs>(send_completed);
                    arg.UserToken = token;

                    this.buffer_manager.SetBuffer(arg);

                    this.send_event_args_pool.Push(arg);
                }
            }
        }

        public void listen(string host, int port, int backlog)
        {
            this.client_listener = new CListener();
            this.client_listener.callback_on_newclient += on_new_client;
            this.client_listener.start(host, port, backlog);
        }

        public void on_connect_completed(Socket socket, CUserToken token)
        {
            // SocketAsyncEventArgsPool���� ������ �ʰ� �׶� �׶� �Ҵ��ؼ� ����Ѵ�.
            // Ǯ�� �������� Ŭ���̾�Ʈ���� ��ſ����θ� ������ ������̱� �����̴�.
            // Ŭ���̾�Ʈ ���忡�� ������ ����� �� ���� ������ ������ �ΰ��� EventArgs�� ������ �Ǳ� ������ �׳� new�ؼ� ����.
            // Ǯ��ó���� �Ϸ��� c->s�� ���� ������ Ǯ�� ���� ��� �Ѵ�.
            SocketAsyncEventArgs receive_event_arg = new SocketAsyncEventArgs();
            receive_event_arg.Completed += new EventHandler<SocketAsyncEventArgs>(receive_completed);
            receive_event_arg.UserToken = token;
            receive_event_arg.SetBuffer(new byte[1024], 0, 1024);

            SocketAsyncEventArgs send_event_arg = new SocketAsyncEventArgs();
            send_event_arg.Completed += new EventHandler<SocketAsyncEventArgs>(send_completed);
            send_event_arg.UserToken = token;
            send_event_arg.SetBuffer(new byte[1024], 0, 1024);

            begin_receive(socket, receive_event_arg, send_event_arg);
        }

		void on_new_client(Socket client_socket, object token)
        {
            //todo:
            // peer listó��.

            Interlocked.Increment(ref this.connected_count);

            Console.WriteLine(string.Format("[{0}] A client connected. handle {1},  count {2}",
                Thread.CurrentThread.ManagedThreadId, client_socket.Handle,
                this.connected_count));

            // �ÿ��� �ϳ� ������ ����Ѵ�.
            SocketAsyncEventArgs receive_args = this.receive_event_args_pool.Pop();
            SocketAsyncEventArgs send_args = this.send_event_args_pool.Pop();

            CUserToken user_token = null;
            if (this.session_created_callback != null)
            {
                user_token = receive_args.UserToken as CUserToken;
                this.session_created_callback(user_token);
            }

            begin_receive(client_socket, receive_args, send_args);
            //user_token.start_keepalive();
        }

        void begin_receive(Socket socket, SocketAsyncEventArgs receive_args, SocketAsyncEventArgs send_args)
        {
            // receive_args, send_args �ƹ��������� �����͵� �ȴ�. �Ѵ� ������ CUserToken�� ���� �ִ�.
            CUserToken token = receive_args.UserToken as CUserToken;
            token.set_event_args(receive_args, send_args);
            token.socket = socket;

            bool pending = socket.ReceiveAsync(receive_args);
            if (!pending)
            {
                process_receive(receive_args);
            }
        }

        void receive_completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                process_receive(e);
                return;
            }

            throw new ArgumentException("The last operation completed on the socket was not a receive.");
        }

        void send_completed(object sender, SocketAsyncEventArgs e)
        {
            CUserToken token = e.UserToken as CUserToken;
            token.process_send(e);
        }

        private void process_receive(SocketAsyncEventArgs e)
        {
            CUserToken token = e.UserToken as CUserToken;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                token.on_receive(e.Buffer, e.Offset, e.BytesTransferred);

                bool pending = token.socket.ReceiveAsync(e);
                if (!pending)
                {
                    process_receive(e);
                }
            }
            else
            {
                Console.WriteLine(string.Format("error {0},  transferred {1}", e.SocketError, e.BytesTransferred));
                close_clientsocket(token);
            }
        }

        public void close_clientsocket(CUserToken token)
        {
            token.on_removed();

            if (this.receive_event_args_pool != null)
            {
                this.receive_event_args_pool.Push(token.receive_event_args);
            }

            if (this.send_event_args_pool != null)
            {
                this.send_event_args_pool.Push(token.send_event_args);
            }
        }
    }
}
