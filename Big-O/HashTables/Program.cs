using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTables
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = string.Empty;

            while (!input.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("> ");
                input = Console.ReadLine();

                Console.WriteLine("Additive: {0}", AdditiveHash(input));
                Console.WriteLine("DJB2: {0}", Djb2(input));
            }
            {

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
