﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace MyMultithreading
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            Console.WriteLine("***************************************************************************************************");
            for (int i = 0; i < 5; i++)
            {
                string name = string.Format("【同步操作】-- {0}", i);
                DoSomething(name);
            }
            Console.WriteLine("***************************************************************************************************");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }
        /// <summary>
        /// 异步、多线程、并发在C#中具有相同的含义；通过委托来实现异步；
        /// 特点：
        /// 1、各个线程同时进行，不会卡在主线程
        /// 2、各个线程随机顺序
        /// 3、处理速度更快，但是更占资源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAsync_Click(object sender, EventArgs e)
        {
            Console.WriteLine("***************************************************************************************************");
            DoSomethingHandler method = new DoSomethingHandler(DoSomething);
            for (int i = 0; i < 5; i++)
            {
                string name = string.Format("【异步操作】-- {0}", i);
                method.BeginInvoke(name, null, null);
            }
            Console.WriteLine("***************************************************************************************************");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }
        /// <summary>
        /// 异步进阶
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAsyncAdvanced_Click(object sender, EventArgs e)
        {
            Console.WriteLine("***************************************************************************************************");
            //不需要获取异步结果
            //DoSomethingHandler method = new DoSomethingHandler(DoSomething);
            //AsyncCallback callback = new AsyncCallback(CustomCallback);
            //IAsyncResult asyncResult01 = method.BeginInvoke("【异步进阶01】", callback, "墨羽寒鸦");
            //IAsyncResult asyncResult02 = method.BeginInvoke("【异步进阶02】", result => Console.WriteLine("执行callback函数，asyncState={0}", result.AsyncState) , "墨羽寒鸦");
            
            //需要获取异步结果，使用EndInvoke
            DoSomethingLongHandler method = new DoSomethingLongHandler(DoSomethingLong);
            AsyncCallback callback = new AsyncCallback(CustomCallback);     //回调是在子线程中进行的,回调：执行会某些动作后，返回来执行回调函数内容

            IAsyncResult asyncResult01 = method.BeginInvoke("【异步进阶01】", callback, "墨羽寒鸦");
            Console.WriteLine("干点其他事-01");
            Console.WriteLine("干点其他事-02");
            Console.WriteLine("干点其他事-03");
            Console.WriteLine("干点其他事-04");
            Console.WriteLine("干点其他事-05");

            long lResult01 = method.EndInvoke(asyncResult01);     //等待异步结束，卡住当前线程
            Console.WriteLine("EndInvoke得到的结果={0}", lResult01);

            //在回调函数中获取异步结果
            IAsyncResult asyncResult02 = 
            method.BeginInvoke("【异步进阶02】", 
                                result => 
                                {
                                    Console.WriteLine("执行callback函数，asyncState={0}", result.AsyncState);
                                    long lResult02 = method.EndInvoke(result);
                                    Console.WriteLine("EndInvoke得到的结果={0}", lResult02);
                                },
                            "墨羽寒鸦");
            while (!asyncResult02.IsCompleted)
            {
                Console.WriteLine("等待异步完成......");
                Thread.Sleep(100);
            }
            Console.WriteLine("***************************************************************************************************");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTask_Click(object sender, EventArgs e)
        {
            //模拟项目开发过程
            Console.WriteLine("***************************************************************************************************");
            Console.WriteLine("SIMON接了一个项目");
            Console.WriteLine("沟通需求，谈妥价格");
            Console.WriteLine("收取50%费用");
            Console.WriteLine("需求分析，方案设计");

            List<Task> taskList = new List<Task>();
            TaskFactory myTaskFactory = new TaskFactory();
            Action act01 = new Action(() => Coding("小S", "Portal"));
            Action act02 = new Action(() => Coding("小I", "Client"));
            Action act03 = new Action(() => Coding("小M", "WeChat"));
            Action act04 = new Action(() => Coding("小O", "BackOffice"));
            Action act05 = new Action(() => Coding("小N", "Service"));

            taskList.Add(myTaskFactory.StartNew(act01));
            taskList.Add(myTaskFactory.StartNew(act02));
            taskList.Add(myTaskFactory.StartNew(act03));
            taskList.Add(myTaskFactory.StartNew(act04));
            taskList.Add(myTaskFactory.StartNew(act05));

            Action<Task[]> actAll = new Action<Task[]>(t => Console.WriteLine("所有子功能开发完成了，进行联调，当前线程ID={0}", Thread.CurrentThread.ManagedThreadId));
            Task taskAll = myTaskFactory.ContinueWhenAll(taskList.ToArray(), actAll);
            //等待某个子功能完成
            Task.WaitAny(taskList.ToArray());               
            Console.WriteLine("子功能测试");
            Console.WriteLine("收取20%费用");
            //等待所用代码编写完成
            Task.WaitAll(new Task[]{ taskAll });
            //taskList.Add(taskAll);
            //Task.WaitAll(taskList.ToArray());
            Console.WriteLine("验收，当前线程ID={0}", Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("收取剩余20%费用，当前线程ID={0}", Thread.CurrentThread.ManagedThreadId);

            Console.WriteLine("***************************************************************************************************");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        private delegate void DoSomethingHandler(string name);
        private void DoSomething(string name)
        {
            Console.WriteLine("{0} {1}开始执行，当前线程ID={2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), name, Thread.CurrentThread.ManagedThreadId);
            long lResult = 0;
            for (long i = 0; i < 100000000; i++)
            {
                lResult += i;
            }
            Thread.Sleep(2000);
            Console.WriteLine("{0} {1}结束执行，当前线程ID={2}, 计算结果={3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), name, Thread.CurrentThread.ManagedThreadId, lResult);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private delegate long DoSomethingLongHandler(string name);
        private long DoSomethingLong(string name)
        {
            Console.WriteLine("{0} {1}开始执行，当前线程ID={2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), name, Thread.CurrentThread.ManagedThreadId);
            long lResult = 0;
            for (long i = 0; i < 100000000; i++)
            {
                lResult += i;
            }
            Thread.Sleep(2000);
            Console.WriteLine("{0} {1}结束执行，当前线程ID={2}, 计算结果={3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), name, Thread.CurrentThread.ManagedThreadId, lResult);
            return lResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void CustomCallback(IAsyncResult result)
        {
            Console.WriteLine("执行callback函数，asyncState={0}", result.AsyncState);
        }

        private void Coding(string name, string project)
        {
            Console.WriteLine("{0} {1}开始执行{3}，当前线程ID={2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), name, Thread.CurrentThread.ManagedThreadId, project);
            long lResult = 0;
            for (long i = 0; i < 100000000; i++)
            {
                lResult += i;
            }
            Thread.Sleep(new Random().Next(2000,10000));
            Console.WriteLine("{0} {1}结束执行{3}，当前线程ID={2}, 计算结果={4}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), name, Thread.CurrentThread.ManagedThreadId, project, lResult);
        }
    }
}
