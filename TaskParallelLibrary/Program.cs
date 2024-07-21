using System.Collections.Concurrent;
using System.Diagnostics;

namespace TaskParallelLibrary
{
    class Program
    {
        static void Main(string[] args)
        {
            //TaskExample();

            //ThreadExample();

            //RunTasksInParrallel();

            //RunTasksInParrallelWithExceptions();

            //RunTasksInParrallelWithReturnData();

            RunTasksInParallelWithImprovedPerformance().Wait();
        }

        static void TaskExample()
        {
            // Writing a simple task

            var task = new Task(() =>
            {
                Console.WriteLine("Task started");

                Thread.Sleep(2000); // Simulate some work

                Console.WriteLine("Task completed");
            });

            task.Start();

            Console.WriteLine("Task state: {0}", task.Status);

            while (!task.IsCompleted)
            {
                Console.WriteLine("Task is not completed yet");
                Thread.Sleep(500);
            }

            Console.WriteLine("Task state: {0}", task.Status);
        }

        static void ThreadExample()
        {
            // Writing a simple thread

            var thread = new Thread(() =>
            {
                Console.WriteLine("Thread started");

                Thread.Sleep(2000); // Simulate some work

                Console.WriteLine("Thread completed");
            });

            thread.Start();

            Console.WriteLine("Thread state: {0}", thread.ThreadState);

            while (thread.ThreadState != System.Threading.ThreadState.Stopped)
            {
                Console.WriteLine("Thread is not completed yet");
                Thread.Sleep(500);
            }

            Console.WriteLine("Thread state: {0}", thread.ThreadState);
        }

        static void RunTasksInParrallel()
        {
            Console.WriteLine("Starting running tasks in parallel");
            // Running 1000 tasks in parallel

            Parallel.For(0, 10, i =>
            {
                Console.WriteLine("Task started");

                Thread.Sleep(2000); // Simulate some work

                Console.WriteLine("Task completed");
            });

            Console.WriteLine("Completed running tasks in parallel");
        }

        static void RunTasksInParrallelWithExceptions()
        {
            Console.WriteLine("Starting running tasks in parallel");
            // Running 1000 tasks in parallel

            try
            {
                Parallel.For(0, 10, i =>
                {
                    Console.WriteLine("Task started");

                    Thread.Sleep(2000); // Simulate some work

                    Console.WriteLine("Task completed");

                    if (i % 3 == 0)
                    {
                        // throw exception with data
                        var ex = new Exception("Exception in task")
                        {
                            Data = { { "TaskId", i }, { "Message", "sdlfjsdflk" } }
                        };

                        throw ex;
                    }
                });
            }
            catch (AggregateException ex)
            {
                ex.Handle(e =>
                {
                    Console.WriteLine("Exception in task: {0}", e.Message);

                    Console.WriteLine("Exception data: {0}", e.Data.Contains("TaskId") ? e.Data["TaskId"] : "No data");

                    return true;
                });

                //Console.WriteLine("Exception in task: {0}", ex.Message);
            }

            Console.WriteLine("Completed running tasks in parallel");
        }

        static void RunTasksInParrallelWithReturnData()
        {
            Console.WriteLine("Starting running tasks in parallel");
            // Run 1000 tasks in parallel and collect result data

            var taskData = new ConcurrentBag<List<TaskData>>();
            var sw = new Stopwatch();
            sw.Start();
            Parallel.For(0, 60000, i =>
            {
                Thread.Sleep(1);
                taskData.Add(
                [
                    new() {
                        TaskId = i,
                        Message = "Task completed successfully"
                    }
                ]);
            });

            sw.Stop();

            Console.WriteLine($"Completed running {60000}ms tasks in parallel in {sw.ElapsedMilliseconds} ms.");
        }

        static async Task RunTasksInParallelWithImprovedPerformance()
        {
            Console.WriteLine("Starting running tasks in parallel");

            // Run 1,000,000 tasks in parallel and collect result data
            var numberOfTasks = 1000000;
            var taskData = new ConcurrentBag<TaskData>();
            var sw = Stopwatch.StartNew();

            await Task.WhenAll(Enumerable.Range(0, numberOfTasks).Select(async i =>
             {
                 var internalSw = Stopwatch.StartNew();
                 await Task.Delay(1); // Simulate some work
                 taskData.Add(new TaskData
                 {
                     TaskId = i,
                     Message = "Task completed successfully",
                     ElapsedMilliseconds = internalSw.ElapsedMilliseconds
                 });
             }));

            Console.WriteLine($"Completed running {numberOfTasks:N0} tasks in parallel in {sw.ElapsedMilliseconds} ms.");
            Console.WriteLine($"Parallelism: {(taskData.Count / sw.Elapsed.TotalMilliseconds).ToString("N2")} tasks/ms.");
            Console.WriteLine($"Average Task: {taskData.Average(x => x.ElapsedMilliseconds):N2} ms.");
        }
    }

    public class TaskData
    {
        public int TaskId { get; set; }

        public string Message { get; set; } = string.Empty;

        public long ElapsedMilliseconds { get; set; }
    }
}