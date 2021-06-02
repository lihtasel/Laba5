using System;
using System.Collections.Generic;
using System.IO;

namespace Laba5Itest
{
  public class Func
  {
    public static void UniteMemory(SortedList<int, int> pairs)
    {
      int tempBegin = -1, tempEnd = -1;
      for (int i = 0; i < pairs.Count - 1; i++)
      {
        for (int j = i + 1; j < pairs.Count; j++)
        {
          if (pairs.Values[i] == (pairs.Keys[j] - 1))
          {
            tempBegin = pairs.Keys[i];
            tempEnd = pairs.Values[j];
            pairs.RemoveAt(j);
            pairs.RemoveAt(i);
            pairs.Add(tempBegin, tempEnd);
            if (i != 0)
            {
              i--;
              j = i;
            }
            else
            {
              j--;
            }
          }
        }
      }
    }
    public static void PrintMemory()
    {
      Console.WriteLine("———————————————————————————————");
      Console.WriteLine("Память перераспределена!");
      Console.WriteLine("Всего памяти " + Program.memory.Length * Program.sizeInt + " байт");
      if (Program.freeMemory.Count > 0)
      {
        Console.WriteLine("Свободные области памяти:");
        for (int i = 0; i < Program.freeMemory.Count; i++)
        {
          Console.WriteLine("[" + Program.freeMemory.Keys[i] * Program.sizeInt + "; " + (Program.freeMemory.Values[i] * Program.sizeInt) + "]");
          
        }
      }
      else
      {
        Console.WriteLine("Нет свободных областей памяти");
      }

      if (Program.occupiedMemory.Count > 0)
      {
        Console.WriteLine("Занятые области памяти:");
        for (int i = 0; i < Program.occupiedMemory.Count; i++)
        {
          Console.WriteLine("[" + Program.occupiedMemory.Keys[i] * Program.sizeInt + "; " + ((Program.occupiedMemory.Values[i] + 1) * Program.sizeInt) + "]");
          
        }
      }
      else
      {
        Console.WriteLine("Нет занятых областей памяти");
      }

      Console.WriteLine("———————————————————————————————");
    }
    public static TaskInfo SetMemory(TaskInfo info)
    {
      int n;
      using (StreamReader sr = new StreamReader(info.File))
      {
        n = int.Parse(sr.ReadLine());

        info.MemBegin = Program.freeMemory.Keys[info.IndexFreeMemory];
        info.MemEnd = Program.freeMemory.Keys[info.IndexFreeMemory] + n;

        Program.occupiedMemory.Add(info.MemBegin, info.MemEnd - 1);

        int maxMemory = Program.freeMemory.Values[info.IndexFreeMemory];
        Program.freeMemory.RemoveAt(info.IndexFreeMemory);
        if (info.MemEnd < maxMemory)
        {
          Program.freeMemory.Add(info.MemEnd, maxMemory);
        }
        for (int i = info.MemBegin; i < info.MemEnd; i++)
        {
          Program.memory[i] = Convert.ToInt32((sr.ReadLine()));
        }
      }
      UniteMemory(Program.occupiedMemory);
      UniteMemory(Program.freeMemory);
      PrintMemory();
      return info;
    }
  }
}
