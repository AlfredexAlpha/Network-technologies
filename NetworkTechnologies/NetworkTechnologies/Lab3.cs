using System;
using System.Collections.Generic;
using System.Text;
using static NetworkTechnologies.BitExtensions;

namespace NetworkTechnologies
{
    public class Lab3
    {
        public static void Start()
        {
            var defaultMessage = "Zheleznyi Aleksandr 14.02.2021";
            var message = StringToListBits(defaultMessage);
            // _   _
            //| \ / \
            // * * *  * *   *  *  
            // -----  -----  -- --------
            // 5ю
            Console.WriteLine(BitListToMessage(Scramble(new List<int> {1,1,0,1,1,0,0,0,0,0})));
            // 1. представить первые 10 бит в трез из предложенных методов кодирования, вывести графиком в консоль
            // 2. рассчитать характеристики
            // 3. преобразовать сообщение кодом 4B/5B и расс хар
            // 4. преобразовать скремблером(+) и расс хар
            // - - - - - - -
            // - - - - - - -
            // * * * * * - *
        }

        private static void DrawRZ(List<int> signal)
        {
            var topLine = new StringBuilder();
            topLine.Append("Strob: ")
            foreach (var VARIABLE in COLLECTION)
            {
                // **   ***
                //   *  *
                //   ****
            }
        }
        

        private static List<int> Scramble(List<int> message)
        {
            var result = new List<int>();
            for (var i = 0; i < message.Count; i++)
            {
                if (i < 3)
                {
                    result.Add(message[i]);
                    continue;
                }

                if (i < 5)
                {
                    result.Add(result[i - 3] ^ message[i]);
                    continue;
                }
                result.Add(message[i] ^ result[i - 3] ^ result[i - 5]);
            }
            return result;
        }
    }
}