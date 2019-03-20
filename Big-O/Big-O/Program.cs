using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Big_O
{
    enum AlgorithmType
    {
        Constant = 0,
        LogN = 1, 
        Linear = 2, 
        NLogN = 3, 
        NSquared = 4,
        Exponential = 5, 
        Factorial = 6
    }

    class Program
    {
        static void Main(string[] args)
        {
            // runBigOAlgorithms();
            int countRuns = 10000, countOperators = 10000, countAssignments = 10000; 
            ExampleAlgorithm exampleAlgorithm = new ExampleAlgorithm(countRuns, countOperators, countAssignments);

            var watch = Stopwatch.StartNew();
            List<OperatorAsset> assest = exampleAlgorithm.InefficientAlgorithm();
            watch.Stop();
            long ineffTime = watch.ElapsedMilliseconds;

            // Linq
            watch = Stopwatch.StartNew();
            assest = exampleAlgorithm.IneffiecientLinqAlgorithm();
            watch.Stop();
            long ineffLinqTime = watch.ElapsedMilliseconds;


            watch = Stopwatch.StartNew();
            assest = exampleAlgorithm.MoreEfficientAlgorithm();
            watch.Stop();
            long effTime = watch.ElapsedMilliseconds;

            Console.WriteLine($"Operator Count:{countOperators}, Assignment Count: {countAssignments}");
            Console.WriteLine($"Algorithm Type  | Algorithm Run Time ");
            Console.WriteLine($"  Inefficient   |  {ineffTime}ms ");
            Console.WriteLine($" InefficientLinq|  {ineffLinqTime}ms ");
            Console.WriteLine($"   Efficient    |  {effTime}ms ");
            Console.ReadLine();

        }

        /// <summary>
        /// 
        /// </summary>
        static void runBigOAlgorithms()
        {
            runBigOAlgorithms();

            int n = 100000;

            // Initialize list of n integers
            List<int> nList = new List<int>();
            for (int i = 0; i < n; i++) { nList.Add(i); }

            long constantTime = calculateAlgorithmTime(AlgorithmType.Constant, nList);
            long logNTime = calculateAlgorithmTime(AlgorithmType.LogN, nList);
            long linearTime = calculateAlgorithmTime(AlgorithmType.Linear, nList);
            long nlognTime = calculateAlgorithmTime(AlgorithmType.NLogN, nList);
            long nSquaredTime = calculateAlgorithmTime(AlgorithmType.NSquared, nList);
            long exponentialTime = calculateAlgorithmTime(AlgorithmType.Exponential, nList);
            long factorialTime = calculateAlgorithmTime(AlgorithmType.Exponential, nList);

            Console.WriteLine($"Input Size:{n}");
            Console.WriteLine($"Algorithm Type  | Algorithm Run Time ");
            Console.WriteLine($"     O(1)       |  {constantTime}ms ");
            Console.WriteLine($"     O(logn)    |  {logNTime}ms ");
            Console.WriteLine($"     O(n)       |  {linearTime}ms ");
            Console.WriteLine($"     O(nlogn)   |  {nlognTime}ms ");
            Console.WriteLine($"     O(n^2)     |  {nSquaredTime}ms ");
            Console.WriteLine($"     O(2^n)     |  {exponentialTime}ms ");
            Console.WriteLine($"     O(n!)      |  {factorialTime}ms ");
            Console.ReadLine();
        }

        static long calculateAlgorithmTime(AlgorithmType type, List<int> nList)
        {
            int k = 0;
            var watch = Stopwatch.StartNew();
            // something to time
            switch (type)
            {
                case AlgorithmType.Constant:
                    k = ConstantTimeAlgorithm(nList);
                    break;
                case AlgorithmType.Linear:
                    k = LinearTimeAlgorithm(nList);
                    break;
                case AlgorithmType.LogN:
                    k = LogNTimeAlgorithm(nList);
                    break;
                case AlgorithmType.NLogN:
                    k = NLogNAlgorithm(nList);
                    break;
                case AlgorithmType.NSquared:
                    if (nList.Count >= 100001)
                        return long.MaxValue;
                    else 
                        k = NSquaredAlgo(nList);
                    break;
                case AlgorithmType.Exponential:
                    if (nList.Count >= 14)
                        return long.MaxValue;
                    else
                        ExponentialAlgo(nList);
                    break;
                case AlgorithmType.Factorial:
                    if (nList.Count >= 14)
                        return long.MaxValue;
                    else
                        FactorialAlgo(nList);
                    break;
                default:
                    break;
            }

            watch.Stop();
            return watch.ElapsedMilliseconds;
        }

        static int ConstantTimeAlgorithm(List<int> nList)
        {
            return nList.FirstOrDefault();
        }

        static int LogNTimeAlgorithm(List<int> nList)
        {
            int key = nList.Count+1;
            return nList.BinarySearch(key);
        }

        static int LinearTimeAlgorithm(List<int> nList)
        {
            int index = -1;
            for (int i = 0; i < nList.Count; i++)
            {
                if (nList[i] == nList.Count-1)
                {
                    index += nList[i];
                }
            }

            return index;
        }

        static int NLogNAlgorithm(List<int> nList)
        {
            MergeSortHelper helper = new MergeSortHelper();
            helper.MergeSort(nList.ToArray(), 0, nList.Count - 1);

            return -1; 
        }

        static int NSquaredAlgo(List<int> nList)
        {
            int k = 0; 
            foreach (int i in nList)
            {
                foreach (int i1 in nList)
                {
                    k = i + i1;
                }
            }
            return k;
        }

        /// <summary>
        /// O(2^n)
        /// </summary>
        /// <param name="nList"></param>
        static void ExponentialAlgo(List<int> nList)
        {
            fib(nList.Count);
        }

        /// <summary>
        /// Standard 2^n algorithm
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        static int fib(int n)
        {
            if (n <= 1)
            {
                return 1;
            }
            return fib(n-1) + fib(n-2);
        }

        static void FactorialAlgo(List<int> nList)
        {
            factorial(nList.Count);
        }

        static void factorial(int n)
        {
            for (int i = 0; i < n; i++)
            {
                int k = i;
                factorial(n - 1);
            }
        }


    }
}
