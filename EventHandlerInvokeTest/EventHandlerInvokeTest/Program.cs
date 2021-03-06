﻿using System;
using System.Threading;

namespace EventHandlerInvokeTest
{
    class Program
    {
        private static EventInvoker invoker;
        static void Main(string[] args)
        {
            invoker = new EventInvoker();
            invoker.TrialValueChanged += (sender, b) =>
            {
                Console.WriteLine("TrialValueChanged callback enter");

                Thread.Sleep(5*1000);

                Console.WriteLine("TrialValueChanged callback exit");
            };

            Console.WriteLine("Main sync start");

            invoker.TrialValueSync = true;

            Console.WriteLine("Main sync end");

            Console.WriteLine("Main async start");

            invoker.TrialValueAsync = true;

            Console.WriteLine("Main async end");


            Console.ReadKey();
        }
    }
}
