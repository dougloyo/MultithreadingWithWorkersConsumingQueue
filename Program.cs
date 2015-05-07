using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultithreadedWorkers
{
    class Program
    {
        private static BlockingCollection<TestWork> _workQueue;

        static void Main(string[] args)
        {
            int workerCount = 3;

            QueueWork();

            // Setting default threading settings
            ThreadPool.SetMinThreads(workerCount, workerCount);
            System.Net.ServicePointManager.DefaultConnectionLimit = workerCount;

            // Create the workers
            var simultaneousActions = new List<Action>();
            for (var i = 0; i < workerCount; i++)
            {
                simultaneousActions.Add(() =>
                {
                    TestWork w;
                    while (_workQueue.TryTake(out w))
                        w.DoWork();
                });
            }

            var stopWatch = Stopwatch.StartNew();
            Console.WriteLine("*** Starting Work with {0} workers ***", workerCount);
            Parallel.Invoke(simultaneousActions.ToArray());
            stopWatch.Stop();
            Console.WriteLine("*** Finished All Work in {0} milliseconds with {1} workers***", stopWatch.ElapsedMilliseconds, workerCount);
            Console.WriteLine("Hit any key to close.");
            Console.ReadKey();
        }

        public static void QueueWork()
        {
            // Queue Work the Work
            _workQueue = new BlockingCollection<TestWork>
                {
                    new TestWork(1, 1000),
                    new TestWork(2, 3000),
                    new TestWork(3, 2000),
                    new TestWork(4, 5000),
                    new TestWork(5, 1000),
                    new TestWork(6, 2000),
                    new TestWork(7, 3000),
                    new TestWork(8, 2000),
                    new TestWork(9, 2000),
                    new TestWork(10, 4000),
                    new TestWork(11, 5000),
                    new TestWork(12, 2000),
                };

            _workQueue.CompleteAdding();
        }

        public class TestWork
        {
            private int _sleepTime = 1000;

            public int Id { get; set; }

            public TestWork(int id, int sleepTime)
            {
                Id = id;
                _sleepTime = sleepTime;
            }

            public void DoWork()
            {
                Console.WriteLine("Task {0} started.", Id);
                var stopWatch = Stopwatch.StartNew();
                Thread.Sleep(_sleepTime);
                stopWatch.Stop();
                Console.WriteLine("Task {0} finished in {1} ms.", Id, stopWatch.ElapsedMilliseconds);
            }
        }
    }
}
