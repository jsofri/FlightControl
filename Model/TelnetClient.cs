using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PlaneController.Model
{
    class TelnetClient
    {
        private Boolean keepRunning;
        private byte[][] messagesBytes;
        private Action<double>[] lambdas;
        private Action<string> errorAction;

        //IPEndPoint endPoint;
        Socket sender;


        public TelnetClient()
        {
            keepRunning = false;
        }

        /*
         * Establish the remote endpoint for the socket.
         * Throws Exception.
         */
        public void Connect(string ip, string port)
        {
            IPAddress ipAddress = IPAddress.Parse(ip);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, Convert.ToInt32(port));

            // Create a TCP/IP  socket.
            this.sender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint.
            sender.Connect(remoteEP);

            this.keepRunning = true;
        }

        /*
         * Loop that send set and get commands to plane.
         * Use messageQueue to get set commands.
         * Get messages are a defined routine given in messageByte array.
         */
        public void Run()
        {
            Thread t;
            byte[] bytes = new byte[1024];
            MessageQueue queue = MessageQueue.GetInstance();
            int i, len = lambdas.Length;
            string message;

            while (this.keepRunning)
            {
                for (i = 0; i < len; i++)
                {
                    t = new Thread(() => StandardGetCommand(i));
                    wait10Sec(t);
                }

                while (i-- > 0)
                {
                    if (queue.Empty())
                    {
                        break;
                    }

                    message = queue.Dequeue();

                    if (message != null)
                    {
                        t = new Thread(() => StandardSetCommand(message));
                        wait10Sec(t);
                    }
                }

            }

            // Release the socket.  
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }// End of run.

        // Disconnect client from server.
        public void Stop()
        {
            this.keepRunning = false;
        }

        public void SetDefaultErrorAction(Action<string> action)
        {
            this.errorAction = action;
        }

        /*
         * Execute a thread, if it takes more than 10 sec,
         * An event of errorAction is called.
         */
        private void wait10Sec(Thread t)
        {
            t.Start();

            if (!t.Join(TimeSpan.FromSeconds(10)))
            {
                t.Abort();
                this.errorAction("More then 10 seconds");
            }
        }

        // Get a string and execute one get command to plane.
        private void StandardSetCommand(string message)
        {
            byte[] bytes = new byte[1024];

            // Convert to bytes and send to server.
            byte[] command = Encoding.ASCII.GetBytes(message);
            this.sender.Send(command);

            // To empty buffer.
            int bytesRec = sender.Receive(bytes);
        }

        // Get an index and execute one get command to plane.
        private void StandardGetCommand(int i)
        {
            byte[] bytes = new byte[1024];
            double answer;

            // Send the data through the socket.  
            this.sender.Send(messagesBytes[i]);

            // Receive the response from the server.
            int bytesRec = sender.Receive(bytes);

            string message = Encoding.ASCII.GetString(bytes, 0, bytesRec);

            // Execute compatible lambda function.
            try
            {
                answer = Convert.ToDouble(message);
                lambdas[i](answer);
            }
            catch (FormatException e)
            {
                if (this.errorAction != null) { this.errorAction("problem in message " + i.ToString()); }
            }
        }

        /*
         * Routine is an array of strings where each string is a command to send server.
         * Another array of lambdas define what to do with the answer from the server.
         * Messages are converted for array of byte for TCP protocol.
         */
        public void SetRoutine(string[] messages, Action<double>[] lambdas)
        {
            int len = messages.Length;

            this.messagesBytes = new byte[len][];

            if (len != lambdas.Length) { RoutineErrorHelper(); }

            for (int i = 0; i < len; i++)
            {
                if (messages[i] != null && lambdas[i] != null)
                {
                    messagesBytes[i] = Encoding.ASCII.GetBytes(messages[i]);
                }
                else
                {
                    this.RoutineErrorHelper();
                }

            }

            this.lambdas = lambdas;
        }

        private void RoutineErrorHelper()
        {
            throw new Exception("illegal string / lambda array");
        }
    }
}
