using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ThreadPoolApp
{
    #region Helper class
    public class Printer
    {
        private object lockToken = new object();

        public void PrintNumbers()
        {
            lock (lockToken)
            {
                // Display Thread info.
                Console.WriteLine("Потік {0} виконує PrintNumbers()",
                  Thread.CurrentThread.ManagedThreadId);

                // Print out numbers.
                Console.Write("Ваші номери: ");
                for (int i = 0; i < 10; i++)
                {
                    Console.Write("{0}, ", i);
                    Thread.Sleep(200);
                }
                Console.WriteLine();
            }
        }
    }
    #endregion

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;
            Console.WriteLine("Основний потік запущено. ThreadID = {0}",
              Thread.CurrentThread.ManagedThreadId);

            Printer p = new Printer();

            WaitCallback workItem = new WaitCallback(PrintTheNumbers);

            // Черга
            for (int i = 0; i < 10; i++)
            {
                ThreadPool.QueueUserWorkItem(workItem, p);
            }

            Console.WriteLine("Усі завдання в черзі");
            Console.ReadLine();
        }

        static void PrintTheNumbers(object state)
        {
            Printer task = (Printer)state;
            task.PrintNumbers();
        }
    }
}