using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static NetworkTechnologies.BitExtensions;

namespace NetworkTechnologies
{
    public class Lab2
    {
        private static bool COMPLETED;
        private static bool RE_SYNC;
        private static int LINE;
        private static int NOISE_COUNTER;
        
        public static void Start()
        {
            var listener = new Thread(LISTENER);
            listener.Start();
            Thread.Sleep(315);
            var code1 = new Thread(CODE1);
            code1.Start();
            var code2 = new Thread(CODE2);
            code2.Start();
            var code3 = new Thread(CODE3);
            code3.Start();
            var noise = new Thread(NOISE);
            noise.Start();
        }

        private static void NOISE()
        {
            var rnd = new Random();
            while (true)
            {
                Thread.Sleep(rnd.Next(500, 1000));
                LINE = rnd.Next(0, 2);
                NOISE_COUNTER++;
            }            
        }
        
        private static void CODE1()
        {
            var str = "Aleksandr Zheleznyi 14.02.2000 New test text New test text New test text";
            var message = MessageAndBits(str);
            
            for (var i = 0; i < message.Count; i++)
            {
                Console.WriteLine($"Send: {message[i]}");
                LINE = message[i] + 1;
                Thread.Sleep(100 / 2);
            }

            if (NOISE_COUNTER > 0)
                Console.WriteLine($"Failed get message: {str}");

            LINE = 0;
        }

        private static void CODE2()
        {
            var str = "test message 1";
            var message = MessageAndBits(str);

            for (var i = 0; i < message.Count; i++)
            {
                Console.WriteLine($"Send: {message[i]}");
                LINE = message[i] + 1;
                Thread.Sleep(200 / 2);
            }

            if (NOISE_COUNTER > 0)
                Console.WriteLine($"Failed get message: {str}");
            LINE = 0;
        }

        private static void CODE3()
        {
            var str = "test message 2";
            var message = MessageAndBits(str);

            for (var i = 0; i < message.Count; i++)
            {
                Console.WriteLine($"Send: {message[i]}");
                LINE = message[i] + 1;
                Thread.Sleep(250 / 2);
            }
            if (NOISE_COUNTER > 0)
                Console.WriteLine($"Failed get message: {str}");
            LINE = 0;
        }

        private static List<int> MessageAndBits(string message)
        {
            var result = StringToListBits(message);
            var log = new StringBuilder($"Message Text: {message}");
            log.AppendLine($"\nMessage Bits: {BitListToMessage(result)}");
            Console.WriteLine(log.ToString());
            return result;
        }
        
        private static void DECODE()
        {
            while (!RE_SYNC)
            {
                Thread.Sleep(10); if (RE_SYNC) break;
                if(LINE == 2) Console.WriteLine($"GotBit: 1");
                if(LINE == 1) Console.WriteLine($"GotBit: 0");
                for (int i = 0; i < 8; i++) Thread.Sleep(10 / 2);
                var rnd = new Random();
                Thread.Sleep(rnd.Next(0, 9));
            }
        }

        private static void LISTENER()
        {
            var buf = 1;
            while (!COMPLETED)
            {
                buf = LINE;
                Thread.Sleep(20);
                if (Math.Abs(buf - LINE) > 0)
                {
                    RE_SYNC = true;
                    Thread.Sleep(10);
                    RE_SYNC = false;
                    var thread = new Thread(DECODE);
                    thread.Start();
                }
            }
        }
    }
}