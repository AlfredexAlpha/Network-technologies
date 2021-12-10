using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace NetworkTechnologies
{
    public enum ControlBitsType
    {
        Summ,
        Hamming,
        Berger
    }
    
    public static class BitExtensions
    {
        #region Common

        public static List<int> GetControlBits(int backLength, List<int> package)
        {
            if (backLength >= package.Count) return new List<int>();
            for (var i = 0; i < backLength; i++)
                package.RemoveAt(0);
            return package;
        }

        public static bool EqualBitsMessages(List<int> A, List<int> B)
        {
            if (A.Count != B.Count) return false;
            return A.Where((t, i) => t == B[i]).Count() == A.Count;
        }

        public static List<List<int>> GetPackages(int packageBitCount, List<int> message)
        {
            var startPackages = new List<List<int>>();
            while (message.Count > 0)
            {
                var package = new List<int>();
                var addTher = 0;
                for (var i = 0; i < packageBitCount - message.Count; i++)
                {
                    package.Add(0);
                    addTher++;
                }
                package.AddRange(GetPackage(packageBitCount, ref message));
                startPackages.Add(package);
            }

            return startPackages;
        }

        public static List<List<int>> AddNoiseToPackages(List<List<int>> packages)
        {
            return (from package in packages let rnd = new Random() let probability = .5f 
                select AddNoiseToMessage(probability, rnd.Next(0, 2), package.ToList())).ToList();
        }
        
        public static List<int> GetPackage(int count, ref List<int> message)
        {
            var result = new List<int>();
            for (var i = 0; i < count; i++)
            {
                if (message.Count == 0) break;
                result.Add(message[0]);
                message.RemoveAt(0);
            }
            return result;
        }


        public static List<int> IntToBinaryCode(int value)
        {
            var binary = Convert.ToString(value, 2);
            return binary.Select(ch => int.Parse(ch.ToString())).ToList();
        }

        public static void InvertBits(ref List<int> package)
        {
            for (var i = 0; i < package.Count; i++)
                package[i] = package[i] == 1 ? 1 : 0;
        }

        public static List<int> AddNoiseToMessage(float probabilityOnBit, int errorCount, List<int> message)
        {
            if (errorCount == 0) return message;
            var errorCounter = 0;
            var result = message.ToList();
            var rnd = new Random();
            var rndBit = new Random();
            while (errorCounter < errorCount)
            {
               var index = rnd.Next(0, message.Count);
               var change = rndBit.Next(0, 100) / 100 < probabilityOnBit;
               if (!change) continue;
               result[index] = message[index] == 1 ? 0 : 1;
               errorCounter++;
            }

            return result;
        }
        
        public static List<int> StringToListBits(string message)
        {
            var bitArray = new BitArray(Encoding.ASCII.GetBytes(message));
            var result = new List<int>();
            for (int i = 0; i < bitArray.Count; i++)
                result.Add(bitArray[i] ? 1 : 0);
            return result;
        }

        public static string BitListToMessage(List<int> message, bool useControlBit = false, int messageLenght = 1)
        {
            var result = new StringBuilder();
            for (var i = 0; i < message.Count; i++)
            {
                if (useControlBit && messageLenght == i)
                    result.Append(" ");
                result.Append(message[i]);
            }
            return result.ToString();
        }

        #endregion

        #region Log Messages

        public static void WriteStepText(int step) => Console.WriteLine($"---Step {step} ---");

        public static void WriteInLogPackages(List<List<int>> packages, bool useControlBit = false, int messageLenght = 1)
        {
            var message = new StringBuilder();
            foreach (var package in packages)
                message.AppendLine(BitListToMessage(package, useControlBit, messageLenght));
            message.AppendLine("-------------------------");
            Console.WriteLine(message.ToString());
        }

        #endregion
    }
}