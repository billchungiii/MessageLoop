using MessageLoopLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageLoopSample001
{
    class Program
    {
        static void Main(string[] args)
        {
            var worker = new SingleThreadWorker();
            worker.Call(null,new Action<string> (Method001), new object[] { nameof(worker) }, null);
            worker.Call(null, new Func<string,int>(Method002), new object[] { nameof(worker) }, (x) =>  Console.WriteLine($" count is  {x}"));
            worker.Call(null, new Func<string,int>(Method002), new object[] { nameof(worker) }, (x) => Console.WriteLine($" count is  {x}"));

           

            var anotherWroker = new SingleThreadWorker();
            anotherWroker.Call(null, new Action<string>(Method001), new object[] { nameof(anotherWroker) }, null);
            anotherWroker.Call(null, new Func<string, int>(Method002), new object[] { nameof(anotherWroker) }, (x) => Console.WriteLine($" count is  {x}"));
            anotherWroker.Call(null, new Func<string, int>(Method002), new object[] { nameof(anotherWroker) }, (x) => Console.WriteLine($" count is  {x}"));
            Console.ReadLine();
        }

        static int _count = 0;
        static void Method001(string caller)
        {
            Console.WriteLine($"Method 001 running in Thread Id {Thread.CurrentThread.ManagedThreadId} by {caller}");
        }

        static int Method002(string caller)
        {
            _count++;
            Console.WriteLine($"Method 002 running in Thread Id {Thread.CurrentThread.ManagedThreadId}  by {caller}");
            return _count;
        }

    }
}
