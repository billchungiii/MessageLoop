using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageProxyLibrary
{
    public class MessageProxy<T> : RealProxy where T : class
    {
        private readonly T _target;
        private readonly SingleThreadWorker _worker;

        public MessageProxy(T target) : base(typeof(T))
        {
            _target = target;
            _worker = new SingleThreadWorker();
        }

        public override IMessage Invoke(IMessage message)
        {
            var call = message as IMethodCallMessage;
            var result = _worker.Call(_target, call);
            return new ReturnMessage(result, null, 0, call.LogicalCallContext, call);
        }


        private sealed class SingleThreadWorker
        {
            private Thread _thread;
            private AutoResetEvent _resetEvent;
            public bool IsRunning { get; private set; }

            private ConcurrentQueue<WorkData> _delegates;

            public SingleThreadWorker()
            {
                IsRunning = true;
                _delegates = new ConcurrentQueue<WorkData>();
                _resetEvent = new AutoResetEvent(false);
                _thread = new Thread(RunMessageLoop);
                _thread.IsBackground = true;
                _thread.Start();
            }


            public object Call(object target, IMethodCallMessage call)
            {                
                var spin = new SpinWait();
                var data = new WorkData(target, call);
                _delegates.Enqueue(data);
                _resetEvent.Set();
                while (!data.IsCompleted)
                {
                    spin.SpinOnce();
                }
                return data.Result;
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
                    while (_delegates.TryDequeue(out WorkData data))
                    { 
                        data.Result = (data.Call.MethodBase as MethodInfo).Invoke(data.Target, data.Call.InArgs);
                        data.IsCompleted = true;
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


        private class WorkData
        {
            public object Target { get; }

            public IMethodCallMessage Call { get; }

            public bool IsCompleted { get; set; }
           
            public object Result { get; set; }

            public WorkData(object target, IMethodCallMessage call)
            {
                IsCompleted = false;
                Target = target;
                Call = call;
            }
        }
    }
}
