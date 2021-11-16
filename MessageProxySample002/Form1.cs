using MessageProxyLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MessageProxySample002
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        MyClass proxy001 = new MessageProxy<MyClass>(new MyClass()).GetTransparentProxy() as MyClass;

        async private void button1_Click(object sender, EventArgs e)
        {            
            await Task.Run(() => proxy001.Method001(nameof(proxy001)));
        }

        async private void button2_Click(object sender, EventArgs e)
        {
            int count = await Task.Run(() => proxy001.Method002(nameof(proxy001)));
            MessageBox.Show($"count is {count}");
        }
    }

    class MyClass : MarshalByRefObject
    {
        private int _count = 0;
        public void Method001(string caller)
        {

            Debug.WriteLine($"Method 001 running in Thread Id {Thread.CurrentThread.ManagedThreadId} by {caller}");
            Thread.Sleep(10000);
            Debug.WriteLine($"Method 001 running in Thread Id {Thread.CurrentThread.ManagedThreadId} end");
        }

        public int Method002(string caller)
        {
            _count++;
            Debug.WriteLine($"Method 002 running in Thread Id {Thread.CurrentThread.ManagedThreadId}  by {caller}");
            return _count;
        }
    }
}
