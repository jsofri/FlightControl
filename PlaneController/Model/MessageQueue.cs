using System.Collections.Generic;

namespace PlaneController.Model
{

    /*
     * A singleton class that stores a queue of strings with a multi-thread control.
     * 
     * author: Jhonny.
     * date: 3.28.20
     */
    class MessageQueue
    {
        private Queue<string> queue;
        private static MessageQueue messageQueue = null;
        private static object classLock = new object();


        private MessageQueue()
        {
            this.queue = new Queue<string>();
        }

        public static MessageQueue GetInstance()
        {
            lock (classLock)
            {
                if (messageQueue == null)
                {
                    messageQueue = new MessageQueue();
                }

                return messageQueue;
            }
        }

        public bool Empty()
        {
            lock (classLock)
            {
                try
                {
                    string s = this.queue.Peek();
                    return false;
                }
                catch (System.Exception)
                {
                    return true;
                }
            }
        }

        public string Dequeue()
        {
            lock (classLock)
            {
                return this.queue.Dequeue();
            }
        }

        public void Enqueue(string str)
        {
            lock (classLock)
            {
                queue.Enqueue(str);
            }
        }
    }
}
