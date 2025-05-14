using CustomSkipList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ExperimentsConsoleApp
{
    /// <summary>
    /// Класс, реализующий консольное приложение для тестирования производительности Skip List
    /// в сравнении с SortedList из .NET.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Точка входа в приложение. Выполняет тестирование операций добавления, поиска
        /// и удаления для SkipList и SortedList на 10 000 элементов.
        /// </summary>
        /// <param name="args">Аргументы командной строки (не используются).</param>
        static void Main(string[] args)
        {
            const int n = 10000; // Количество элементов для теста
            var random = new Random(); // Генератор случайных чисел
            var uniqueNumbers = new HashSet<int>(); // Множество для уникальных чисел

            // Генерация 10 000 уникальных случайных чисел
            while (uniqueNumbers.Count < n)
            {
                int number = random.Next(1, 100000);
                uniqueNumbers.Add(number);
            }
            var numbers = uniqueNumbers.ToArray(); // Преобразование в массив

            // Тестирование SkipList
            var skipList = new SkipList<int, int>(); // Создание SkipList
            var stopwatch = new Stopwatch(); // Таймер для замера времени

            // Замер общего времени для SkipList
            stopwatch.Start();

            // Добавление элементов в SkipList
            for (int i = 0; i < n; i++)
            {
                skipList.Add(numbers[i], i * 100);
            }

            // Поиск каждого элемента в SkipList
            for (int i = 0; i < n; i++)
            {
                skipList.Contains(numbers[i]);
            }

            // Удаление элементов (от n/2 до 3n/4)
            int start = n / 2;
            int end = (3 * n) / 4;
            for (int i = start; i < end; i++)
            {
                skipList.Remove(numbers[i]);
            }

            stopwatch.Stop();
            long skipListTotalTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"Общее время работы SkipList: {skipListTotalTime} мс");

            // Тестирование SortedList
            var sortedList = new SortedList<int, int>(); // Создание SortedList
            stopwatch.Restart();

            // Добавление элементов в SortedList
            for (int i = 0; i < n; i++)
            {
                sortedList[numbers[i]] = numbers[i];
            }

            // Поиск каждого элемента в SortedList
            for (int i = 0; i < n; i++)
            {
                sortedList.ContainsKey(numbers[i]);
            }

            // Удаление элементов (от n/2 до 3n/4)
            for (int i = start; i < end; i++)
            {
                sortedList.Remove(numbers[i]);
            }

            stopwatch.Stop();
            long sortedListTotalTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"Общее время работы SortedList: {sortedListTotalTime} мс");

            // Вычисление и вывод отношения времени
            double timeRatio = (double)sortedListTotalTime / skipListTotalTime;
            Console.WriteLine("{0:F2}", timeRatio);
        }
    }
}