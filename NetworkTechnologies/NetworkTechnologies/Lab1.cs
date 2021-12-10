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

            var test = new List<int>() { 0,1,0,0,0,1,0,0,0,0,1,1,1,1,0,1 };
            // var test = new List<int> { 0,1,0,0,0,1,0,0 };
            // var test = new List<int> { 0,1,0,0 };
            var encodeBits = GetHammingControlBits(test.ToList()).ToList();
            Console.WriteLine($"Debug log: {test.Count}, Result: {encodeBits.Count}");
            Console.WriteLine(BitListToMessage(encodeBits));
            
            Console.WriteLine($"Test Hamming decode");
            var errorMess = new List<int> {1,0,0,1,1,0,0,0,1,1,0,0,0,0,1,0,1,1,1,0,1};
            var decodeResult = DecodeHammingControlBits(errorMess.ToList());
            Console.WriteLine(BitListToMessage(decodeResult));
            Console.WriteLine(errorMess.Count);
        }

        #region MyRegion

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


        #endregion
        
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

            // TODO add decode hamming code
            
            var tryCount = startPackages.Where((t, i) => EqualBitsMessages(GetControlBits(packageBitCount, t), 
                GetControlBits(packageBitCount, resultPackages[i]))).Count();
            var percent = (int)(((float)tryCount / resultPackages.Count) * 100);
            Console.WriteLine($"Percentage of correctly transmitted: {percent}%");
        }

        private static List<int> DecodeHammingControlBits(List<int> package)
        {
            // remove control bits
            var withoutControlBits = package.ToList();
            var powIds = new List<int>();
            for(var i = 0 ; i < 32; i++)
                if ((int) Math.Pow(2, i) <= package.Count)
                    powIds.Add((int) Math.Pow(2, i) - 1);
                else break;
            
            for(var i = powIds.Count - 1; i >= 0; i--)
                withoutControlBits.RemoveAt(powIds[i]);
            // withoutControlBits.Remove(0);
            Console.WriteLine(BitListToMessage(withoutControlBits.ToList()));

            
            // calculate new control bits
            var errorIndexes = new List<int>();
            var packageWithControlBits = GetHammingControlBits(withoutControlBits);
            // Console.WriteLine(BitListToMessage(packageWithControlBits.ToList()));

            for (var i = 0; i < powIds.Count; i++)
            {
                var index = (int) Math.Pow(2, i) - 1;
                if(packageWithControlBits[index] != package[index])
                    errorIndexes.Add(index + 1);
            }
            // Console.WriteLine(BitListToMessage(errorIndexes.ToList()));

            var errorIndex = errorIndexes.Sum();
            // Console.WriteLine($"Line with error: {errorIndex}");
            if (errorIndex != 0)
                package[errorIndex] = package[errorIndex] == 1 ? 0 : 1;
            return package;
        }
        
        private static int[] GetHammingControlBits(List<int> package)
        {
            var result = new List<int>();
            var resultBitsCount = 7;
            if(package.Count > 4) resultBitsCount = package.Count > 8 ? 21 : 12;
            for (var i = 0; i < resultBitsCount; i++) result.Add(-1);

            var controlBitIds = new List<int>();
            var powCount = 0;
            for(var i = 0 ; i < 32; i++) 
                if ((int) Math.Pow(2, i) <= package.Count) powCount++;
                else break;
            for (var i = 0; i < powCount; i++)
                controlBitIds.Add((int)Math.Pow(2, i) - 1);

            while (package.Count > 0)
            {
                var emptyIndex = result.FindIndex(item => item == -1);
                if (controlBitIds.Contains(emptyIndex))
                {
                    result[emptyIndex] = 0;
                    continue;
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

            // Console.WriteLine($"Result: {newItems.Aggregate(0, (current, i) => current ^ i)}");
            return newItems.Aggregate(0, (current, i) => current ^ i);
        }

        #endregion

        #region Berger

        private static void SimulateSendMessageWithBerger(int packageBitCount, List<int> message)
        {
            //TODO add berger simulation
            throw new Exception("TODO add berger");
        }

        private static int[] GetBergerControlBit(List<int> package)
        {
            var result = IntToBinaryCode(package.Sum());
            InvertBits(ref result);
            return result.ToArray();
        }

        #endregion

        #region Common

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

        #endregion
    }
}