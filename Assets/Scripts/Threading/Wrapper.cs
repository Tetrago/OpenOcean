using System;
using System.Threading;
using System.Collections;
using UnityEngine;

namespace Threading
{
    public class Wrapper<I>
    {
        public const string PREFIX = "TW#";
        private static uint id_ = 0u;

        private object mutex_;
        private Thread thread_;
        private Action<I> action_;
        private I in_;

        private bool alive_;

        public bool IsAlive
        {
            private set
            {
                lock(mutex_)
                {
                    alive_ = value;
                }
            }

            get
            {
                lock(mutex_)
                {
                    return alive_;
                }
            }
        }

        public Wrapper(Action<I> action, I @in)
        {
            in_ = @in;
            mutex_ = new object();
            thread_ = new Thread(ThreadFunction) { Name = PREFIX + id_++, IsBackground = true };

            lock(mutex_)
            {
                action_ = action;
            }
        }

        public void Start()
        {
            IsAlive = true;
            Events.Instance.StartCoroutine(ThreadState());
            thread_.Start();
        }

        private IEnumerator ThreadState()
        {
            while(IsAlive)
                yield return null;
            
            Clear();
        }

        private void ThreadFunction()
        {
            action_.Invoke(in_);
            IsAlive = false;
        }

        private void Clear()
        {
            if(thread_.IsAlive)
            {
                thread_.Join();
            }
        }
    }
}
