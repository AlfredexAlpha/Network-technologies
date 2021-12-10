using System;
using System.Collections.Generic;
using System.Linq;
using static NetworkTechnologies.BitExtensions;

namespace NetworkTechnologies
{
    public class Lab1
    {
        
        public static void Start()
        {
            /*
            WriteStepText(1);
            Console.WriteLine("Enter message: ");
            var bitMessage = StringToListBits(Console.ReadLine());
            var controlBit = GetSummationControlBit(bitMessage);
            var startMessage = BitListToMessage(bitMessage) + $" {controlBit}";
            Console.WriteLine(startMessage);
            var noErrorsMess = $"0 error: {startMessage}";

            WriteStepText(2);
            Console.WriteLine("Enter probability  of noise on bit");
            var probability = float.Parse(Console.ReadLine());
            var oneError = AddNoiseToMessage(probability, 1, bitMessage);
            var oneErrorContBit = GetSummationControlBit(oneError);
            var twoError = AddNoiseToMessage(probability, 2, bitMessage);
            var twoErrorContBit = GetSummationControlBit(twoError);

            Console.WriteLine(noErrorsMess);
            Console.WriteLine("1 error: " + BitListToMessage(oneError) + $" {oneErrorContBit}");
            Console.WriteLine("2 error: " + BitListToMessage(twoError) + $" {twoErrorContBit}");
            WriteStepText(3);
            Console.WriteLine("Enter send message:");
            var longMessage = StringToListBits(Console.ReadLine());
            Console.WriteLine("---------------------------------------------");
            SimulateSendMessage(16, longMessage.ToList());
            SimulateSendMessage(8, longMessage.ToList());
            SimulateSendMessage(4, longMessage.ToList());
            
            SimulateSendMessageWithHamming(16,  longMessage.ToList());
            SimulateSendMessageWithHamming(8,  longMessage.ToList());
            SimulateSendMessageWithHamming(4,  longMessage.ToList());
            */
            
            // запустить симмуляцию с помощью хемминга
            // запустить симуляцию с помощью берна

            // var test = new List<int>() { 0,1,0,0,0,1,0,0,0,0,1,1,1,1,0,1 };
            var test = new List<int> { 0,1,0,0,0,1,0,0 };
            // var test = new List<int> { 0,1,0,0 };
            var result = GetHammingControlBits(test.ToList()).ToList();
            Console.WriteLine($"Debug log: {test.Count}, Result: {result.Count}");
            Console.WriteLine(BitListToMessage(result));
        }
        
        private static List<int> AddBergerControlBit(List<int> package)
        {
            var result = new List<int>();
            /*            
            result.Add((package[0] + package[1]) % 2 == 0 ? 1 : 0);
            result.Add((package[0] + package[1] + package[2] + package[3]) % 2 == 0 ? 0 : 1);
            result.Add(package[0]);
            result.Add(package[1]);
            result.Add((package[1] + package[2] + package[3]) % 2 == 1 ? 1 : 0);
            result.Add(package[2]);
            result.Add(package[3]);

            if (package.Count == 8)
            {
                result.Add((package[4] + package[5]+ package[6]+ package[7]) % 2 == 0 ? 0 : 1);
                result.Add(package[4]);
                result.Add(package[5]);
                result.Add(package[6]);
                result.Add(package[7]);
            }

            if (package.Count == 16)
            {
                result.Add((package[11] + package[12] + package[13] + package[15]) % 2 == 0 ? 0 : 1);
                result.Add(package[11]);
                result.Add(package[12]);
                result.Add(package[13]);
                result.Add(package[14]);
                result.Add(package[15]);
            }

            return result;
            */ 

            // 1, 2, 4, 8, 16
            // var controlBits = new List<int>();
            // for (var i = 1; i < package.Count; i += i)
            // {
            //     var bit = 0;
            //     for (var j = 0; j < i; j++)
            //         bit ^= package[i - 1 + j];
            //     controlBits.Add(bit);
            // }
            //
            // var result = new List<int>();
            // for (var i = 0; i < package.Count; i += i)
            // {
            //     
            // }
            var controlBitIndex = 1;
            for (var i = 1; i < package.Count; i += i)
            {
                Console.WriteLine(i.ToString());
                var localCounter = 0;
                for (var j = 0; j < package.Count; j++)
                {
                                        
                }
                
                while (localCounter < i)
                    result.Add(package[i]);
            }

            return result;
        }

        private static void SimulateSendMessage(int packageBitCount, List<int> message)
        {
            Console.WriteLine($"\nStart SUMM simulate: {packageBitCount}, {BitListToMessage(message)}");

            var startPackages = GetPackages(packageBitCount, message);
            var resultPackages=  AddNoiseToPackages(startPackages);

            CalculateControlBitForPackages(ref startPackages);
            CalculateControlBitForPackages(ref resultPackages);
            
            Console.WriteLine("Send packages:");
            WriteInLogPackages(startPackages, true, packageBitCount);
            Console.WriteLine("Accepted packages:");
            WriteInLogPackages(resultPackages, true, packageBitCount);

            var tryCount = startPackages.Where((t, i) => t[t.Count - 1] == resultPackages[i][resultPackages[i].Count - 1]).Count();
            var percent = (int)(((float)tryCount / resultPackages.Count) * 100);
            Console.WriteLine($"Percentage of correctly transmitted: {percent}%");
        }

        private static int GetSummationControlBit(List<int> package)
        {
            return package.Aggregate(0, (current, bit) => current ^ bit);
        }
        
        #region Hamming

        private static void SimulateSendMessageWithHamming(int packageBitCount, List<int> message)
        {
            Console.WriteLine($"\nStart HAMMING simulate: {packageBitCount}, {BitListToMessage(message)}");

            var startPackages = GetPackages(packageBitCount, message);
            var resultPackages = AddNoiseToPackages(startPackages.ToList());

            CalculateControlBitForPackages(ref startPackages, ControlBitsType.Hamming);
            CalculateControlBitForPackages(ref resultPackages, ControlBitsType.Hamming);
            
            Console.WriteLine("Send packages:");
            WriteInLogPackages(startPackages, true, packageBitCount);
            Console.WriteLine("Accepted packages:");
            WriteInLogPackages(resultPackages, true, packageBitCount);

            var tryCount = startPackages.Where((t, i) => EqualBitsMessages(GetControlBits(packageBitCount, t), 
                GetControlBits(packageBitCount, resultPackages[i]))).Count();
            var percent = (int)(((float)tryCount / resultPackages.Count) * 100);
            Console.WriteLine($"Percentage of correctly transmitted: {percent}%");
        }

        private static int[] DecodeHammingControlBits(List<int> package)
        {
            throw new Exception();
        }
        
        private static int[] GetHammingControlBits(List<int> package)
        {
            var result = new List<int>();
            var resultBitsCount = 7;
            resultBitsCount = package.Count > 8 ? 21 : 12;
            for (var i = 0; i < resultBitsCount; i++)
                result.Add(-1);

            var controlBitIds = new List<int>();
            var powCount = resultBitsCount > 12 ? 4 : 3;
            powCount = resultBitsCount > 16 ? 5 : powCount;
            for (var i = 0; i < powCount; i++)
            {
                controlBitIds.Add((int)Math.Pow(2, i) - 1);
                // result[index] = GetHammingControlBit(index, result.ToList());
                // controllBits.Add(index, GetHammingControlBit(index, result.ToList()));
            }

            while (package.Count > 0)
            {
                var emptyIndex = result.FindIndex(item => item == -1);
                if (controlBitIds.Contains(emptyIndex))
                {
                    result[emptyIndex] = 0;
                }

                result[emptyIndex] = package[0];
                package.RemoveAt(0);
            }

            foreach (var index in controlBitIds)
                result[index] = GetHammingControlBit(index, result.ToList());

            return result.ToArray();
        }

        private static int GetHammingControlBit(int nextCount, List<int> items)
        {
            // Console.WriteLine($"Start index: {nextCount }, Items: {items.Count}");
            var newItems = new List<int>();

            for (var i = nextCount; i < items.Count; )
            {
                for (var j = 0; j < nextCount + 1; j++)
                {
                    var addIndex = i + j;
                    if (items.Count <= addIndex) break;
                    // Console.WriteLine($"Add: {addIndex + 1}");
                    newItems.Add(items[addIndex]);
                }

                if (nextCount == 3) 
                    i += nextCount * 2 + (nextCount + 2) / 2;
                else if(nextCount < 3)
                    i += nextCount + 2 + (nextCount + 1) / 2;
                else 
                    i += nextCount + 2 + nextCount;
            }

            //Console.WriteLine($"Result: {newItems.Aggregate(0, (current, i) => current ^ i)}");
            return newItems.Aggregate(0, (current, i) => current ^ i);
        }

        #endregion

        #region Berger

        private static void SimulateSendMessageWithBerger(int packageBitCount, List<int> message)
        {
            
        }

        private static int[] GetBergerControlBit(List<int> package)
        {
            // 1. считаем кол-во едениц переводим в двоичное представление
            // 2. ивертируем число и добавляем его к исзодному слову
            var result = IntToBinaryCode(package.Sum());
            InvertBits(ref result);
            return result.ToArray();
        }

        #endregion

        private static void CalculateControlBitForPackages(ref List<List<int>> packages, ControlBitsType bitsType = ControlBitsType.Summ)
        {
            foreach (var package in packages)
            {
                switch (bitsType)
                {
                    case ControlBitsType.Summ:
                        package.Add(GetSummationControlBit(package.ToList()));
                        break;
                    case ControlBitsType.Hamming:
                        package.AddRange(GetHammingControlBits(package.ToList()));
                        break;
                    case ControlBitsType.Berger:
                        package.AddRange(GetBergerControlBit(package.ToList()));
                        break;
                }
            }
        }
    }
}