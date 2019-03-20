using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTables
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestingHashingAlgorithms();
            int size = 10000;
            int[] arr = new int[size];
            Dictionary<int, int> hashTable = new Dictionary<int, int>();

            for (int i = 0; i < size; i++)
            {
                arr[i] = i;
                hashTable.Add(i, i);
            }

            // Search for int.max 
            int key = int.MaxValue;
            int asset = 0; 

            // Inefficient: O(n)
            var watch = Stopwatch.StartNew();
            for (int i = 0; i < size; i++)
            {
                asset = arr.FirstOrDefault(x => x == key);
                asset = i * asset;
            }          
            watch.Stop();
            long ineffTime = watch.ElapsedMilliseconds;

            // Linq: O(lg n)
            watch = Stopwatch.StartNew();           
            for (int i = 0; i < size; i++)
            {
                asset = Array.BinarySearch(arr, key);
                asset = i * asset;
            }
            watch.Stop();
            long ineffLinqTime = watch.ElapsedMilliseconds;

            // Efficient: O(1)
            watch = Stopwatch.StartNew();           
            for (int i = 0; i < size; i++)
            {
                asset = hashTable.ContainsKey(key) ? hashTable[key] : -1;
                asset = i * asset;
            }
            watch.Stop();
            long effTime = watch.ElapsedMilliseconds;

            Console.WriteLine($"Array Size: {size}");
            Console.WriteLine($"Algorithm Type  | Algorithm Run Time ");
            Console.WriteLine($"  Linq Search   |  {ineffTime}ms ");
            Console.WriteLine($" Binary Search  |  {ineffLinqTime}ms ");
            Console.WriteLine($"   Dictionary   |  {effTime}ms ");
            Console.ReadLine();
        }

        private static void TestingHashingAlgorithms()
        {
            string input = string.Empty;

            while (!input.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("> ");
                input = Console.ReadLine();

                Console.WriteLine("Additive: {0}", AdditiveHash(input));
                Console.WriteLine("DJB2: {0}", Djb2(input));
            }
        }

        /// <summary>
        /// Sums the characters in the string. Terrible hashing function
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static int AdditiveHash(string input)
        {
            int currentHashValue = 0;

            foreach (char c in input)
            {
                unchecked
                {
                    currentHashValue += (int)c;
                }
            }

            return currentHashValue;
        }

        private static object Djb2(string input)
        {
            int hash = 5381;

            foreach (int c in input.ToCharArray())
            {
                unchecked
                {
                    hash = ((hash << 5) + hash) + c;
                }
            }

            return hash;
        }
    }
}
