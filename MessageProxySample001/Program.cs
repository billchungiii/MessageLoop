using MessageProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageProxySample001
{
    class Program
    {
        static void Main(string[] args)
        {
            var proxy001 = new MessageProxy<MyClass>(new MyClass()).GetTransparentProxy() as MyClass;
            Call(proxy001, nameof(proxy001));
            var proxy002 = new MessageProxy<MyClass>(new MyClass()).GetTransparentProxy() as MyClass;
            Call(proxy002, nameof(proxy002));

            Console.ReadLine();
        }


        private static void Call(MyClass proxy, string proxyName)
        {
            proxy.Method001(proxyName);
            for (int i = 0; i < 3; i++)
            {
                CallMethod002(proxy, proxyName);
            }
        }
        private static void CallMethod002(MyClass proxied, string proxyName)
        {
            var result = proxied.Method002(proxyName);
            Console.WriteLine($"Count is {result}");
        }
    }

    class MyClass  : MarshalByRefObject
    {
        private int _count = 0;
        public void Method001(string caller)
        {
            Console.WriteLine($"Method 001 running in Thread Id {Thread.CurrentThread.ManagedThreadId} by {caller}");
        }

        public int Method002(string caller)
        {
            _count++;
            Console.WriteLine($"Method 002 running in Thread Id {Thread.CurrentThread.ManagedThreadId}  by {caller}");
            return _count;
        }
    }
}
