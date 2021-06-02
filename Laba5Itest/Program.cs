using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Laba5Itest
{
  class Program
  {

    public static readonly int nMemory = 16000;
    public static readonly int sizeInt = sizeof(int);
    public static int[] memory;
    public static SortedList<int, int> freeMemory;
    public static SortedList<int, int> occupiedMemory;
    public static Queue<TaskInfo> queueTasksToWait;
    public static Queue<TaskInfo> queueTasksToFree;
    public static List<Thread> listTaskDoing;
    public static bool isExit;

    static TaskInfo LookingForFree(string file, int n)
    {
      if (freeMemory.Count > 0)
      {
        for (int i = 0; i < freeMemory.Count; i++)
        {
          if (freeMemory.Values[i] - freeMemory.Keys[i] > n)
          {
            TaskInfo something = new TaskInfo(file, i, n);
            Func.SetMemory(something);
            return something;
          }
        }
      }
      return null;
    }

    static void FreeMemory(TaskInfo info)
    {
      occupiedMemory.Remove(info.MemBegin);
      freeMemory.Add(info.MemBegin, info.MemEnd - 1);
      Func.UniteMemory(occupiedMemory);
      Func.UniteMemory(freeMemory);
      Func.PrintMemory();
      CheckQueue();
    }

    static void ThreadDoSomething(object obj)
    {
      TaskInfo info = (TaskInfo)obj;
      int sum = 0;
      for (int i = info.MemBegin; i < info.MemEnd; i++)
      {
        sum += memory[i];
      }
      listTaskDoing.Remove(Thread.CurrentThread);
      queueTasksToFree.Enqueue(info);
    }

    static void ThreadFreeMemory()
    {
      while (!isExit)
      {
        if (queueTasksToFree.Count > 0)
        {
          FreeMemory(queueTasksToFree.Dequeue());
        }
      }
    }

    static void PrintThread()
    {
      Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++");
      Console.WriteLine("Задач на выполнении: " + listTaskDoing.Count);
      Console.WriteLine("Задач в очереди: " + queueTasksToWait.Count);
      foreach (var item in queueTasksToWait)
      {
        Console.WriteLine("Задача: " + item.File + ". Размер: " + item.SizeOfTask);
      }

      Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++");
    }

    static void StartTask(string file, TaskInfo something)
    {
      Thread thread = new Thread(new ParameterizedThreadStart(ThreadDoSomething));
      listTaskDoing.Add(thread);
      thread.Start(something);
    }

    static void CheckQueue()
    {
      if (queueTasksToWait.Count != 0 && queueTasksToFree.Count == 0)
      {
        var task = queueTasksToWait.Peek();
        var res = LookingForFree(task.File, task.SizeOfTask);
        if (res != null && queueTasksToWait.Count != 0)
        {
          StartTask(queueTasksToWait.Dequeue().File, res);
        }
      }
      PrintThread();
    }

    static void ReadFile(string file)
    {
      using(StreamWriter sw = new StreamWriter(file))
      {
        Random random = new Random();
        sw.WriteLine(Convert.ToString(random.Next(65535)), true, System.Text.Encoding.Default);
      }
      using (StreamReader sr = new StreamReader(file))
      {
        int n = int.Parse(sr.ReadLine());
        var res = LookingForFree(file, n);
        if (res != null)
        {
          StartTask(file, res);
        }
        else
        {
          TaskInfo something = new TaskInfo(file, n);
          queueTasksToWait.Enqueue(something);
        }
      }
    }

    static void PrintMenu()
    {
      Console.WriteLine("Введите номер пункта");
      Console.WriteLine("1. Вывести состояние памяти");
      Console.WriteLine("2. Проверить возможность выполнения задачи из очереди");
      Console.WriteLine("3. Добавить файл на выполнение");
      Console.WriteLine("4. Выйти из приложения");
    }

    static void Main(string[] args)
    {
      isExit = false;
      PrintMenu();
      memory = new int[nMemory];
      freeMemory = new SortedList<int, int>();
      occupiedMemory = new SortedList<int, int>();
      listTaskDoing = new List<Thread>();
      queueTasksToWait = new Queue<TaskInfo>();
      queueTasksToFree = new Queue<TaskInfo>();

      freeMemory.Add(0, nMemory);
      Func.PrintMemory();

      Thread thread = new Thread(new ThreadStart(ThreadFreeMemory));
      thread.Start();

      for (int i = 1; i <= 5; i++)
      {
        ReadFile(i + ".txt");
      }

      int key;
      do
      {
        key = int.Parse(Console.ReadLine());
        switch (key)
        {
          case 1:
            {
              Func.PrintMemory();
              break;
            }
          case 2:
            {
              CheckQueue();
              break;
            }
          case 3:
            {
              int keySubMenu;
              do
              {
                Console.WriteLine("Введите номер файла на выполнение (число от 1 до 5):");
                keySubMenu = int.Parse(Console.ReadLine());
              } while (keySubMenu > 6 || keySubMenu < 1);
              ReadFile(keySubMenu + ".txt");
              break;
            }
        }
        if (key != 4)
        {
          PrintMenu();
        }
      } while (key != 4);
      isExit = true;
    }
  }
}