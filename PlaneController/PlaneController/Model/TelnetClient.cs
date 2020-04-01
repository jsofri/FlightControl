using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PlaneController.Model
{

    /*
     * Send and recieve messages on a telnet protocol to a specific end point.
     * Use a known set of messages and execute lambda after each message.
     * Use a MessageQueue object to send "set" messages to end point.
     * Make sure a send won't take more than 10 seconds to execute.
     * 
     * author: Jhonny.
     * data: 3.28.20
     */
    class TelnetClient
    {

        // data members are volatile since using multi-threading in class.
        private volatile Boolean _keepRunning;
        private volatile byte[][] _messagesBytes;
        private volatile Action<double>[] _lambdas;
        private volatile Action<string> _errorAction;
        private volatile object _errorActionLocker;

        //IPEndPoint endPoint;
        private volatile Socket _sender;


        public TelnetClient()
        {
            _keepRunning = false;
            _errorActionLocker = new object();
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
            _sender = new Socket(ipAddress.AddressFamily,
                                 SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint.
            _sender.Connect(remoteEP);

            _keepRunning = true;
        }

        /*
         * Loop that send set and get commands to plane.
         * Use messageQueue to get the 'set' commands.
         * Get messages are a defined routine given in messageByte matrix.
         */
        public void Run()
        {
            Thread t;
            byte[] bytes = new byte[1024];
            MessageQueue queue = MessageQueue.GetInstance();
            int i, len = _lambdas.Length;
            string message;

            while (_keepRunning)
            {
                for (i = 0; i < len; i++)
                {
                    t = new Thread(() => StandardGetCommand(i));
                    Wait10Sec(t);
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
                        Wait10Sec(t);
                    }
                }

            }

            // Release socket.  
            _sender.Shutdown(SocketShutdown.Both);
            _sender.Close();
        }// End of run().

        // Disconnect client from server.
        public void Stop()
        {
            _keepRunning = false;
        }

        // Setter for errorAction.
        public void SetDefaultErrorAction(Action<string> action)
        {
            _errorAction = action;
        }

        /*
         * Execute a thread, if it takes more than 10 sec,
         * ErrorAction is called.
         */
        private void Wait10Sec(Thread t)
        {
            t.Start();

            if (!t.Join(TimeSpan.FromSeconds(10)))
            {
                t.Abort();
                NotifyErrorHappened("10 seconds");
            }
        }

        // Get a string and execute one get command to plane.
        private void StandardSetCommand(string message)
        {
            byte[] bytes = new byte[1024];

            try
            {
                // Convert to bytes and send to server.
                byte[] command = Encoding.ASCII.GetBytes(message);
                _sender.Send(command);

                // To empty buffer.
                int bytesRec = _sender.Receive(bytes);
            }
            catch (Exception)
            {
                NotifyErrorHappened("CNC");
            }
        }

        // Get an index and execute one get command to plane.
        private void StandardGetCommand(int i)
        {
            byte[] bytes = new byte[1024];
            double answer;

            try
            {
                // Send the data through the socket.  
                _sender.Send(_messagesBytes[i]);

                // Receive the response from the server.
                int bytesRec = _sender.Receive(bytes);

                // Convert bytes to ASCII string.
                string message = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                // Execute compatible lambda function.
                answer = Convert.ToDouble(message);
                _lambdas[i](answer);
            }
            // Problem in converting to double.
            catch (FormatException)
            {
                NotifyErrorHappened("NaN");
            }
            catch (Exception)
            {
                NotifyErrorHappened("ERR");
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

            _messagesBytes = new byte[len][];

            if (len != lambdas.Length) { RoutineErrorHelper(); }

            for (int i = 0; i < len; i++)
            {
                if (messages[i] != null && lambdas[i] != null)
                {
                    _messagesBytes[i] = Encoding.ASCII.GetBytes(messages[i]);
                }
                else
                {
                    RoutineErrorHelper();
                }

            }

            _lambdas = lambdas;
        }

        // Helper to throw errors when initializing this object.
        private void RoutineErrorHelper()
        {
            throw new Exception("illegal string / lambda array");
        }

        // Invoke errorAction in a multi-Thread safe way.         
        private void NotifyErrorHappened(string str)
        {
            lock (_errorActionLocker)
            {
                if (_errorAction != null)
                {
                    _errorAction(str);
                }
            }
        }
    }
}
