using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageLoopLibrary
{
    public sealed class SingleThreadWorker
    {
        private Thread _thread;
        private AutoResetEvent _resetEvent;
        public bool IsRunning { get; private set; }

        private ConcurrentQueue<(object target, Delegate method, object[] args, Action<object> callback)> _delegates;

        public SingleThreadWorker()
        {
            IsRunning = true;
            _delegates = new ConcurrentQueue<(object target, Delegate method, object[] args, Action<object> callback)>();
            _resetEvent = new AutoResetEvent(false);
            _thread = new Thread(RunMessageLoop);
            _thread.IsBackground = true;
            _thread.Start();
        }


        public void Call(object target, Delegate method, object[] args, Action<object> callback)
        {
            _delegates.Enqueue((target, method, args, callback));
            _resetEvent.Set();
        }

        private void Stop()
        {
            IsRunning = false;
            _resetEvent?.Set();
        }

        private void RunMessageLoop()
        {
            while (IsRunning)
            {
                while (_delegates.TryDequeue(out (object target, Delegate method, object[] args, Action<object> callback) item))
                {
                    object result = item.method.Method.Invoke(item.target, item.args);
                    item.callback?.Invoke(result);
                }

                if (_delegates.Count == 0 && IsRunning)
                {
                    _resetEvent.WaitOne();
                }
            }
        }

        ~SingleThreadWorker()
        {
            Stop();
        }

    }
}
