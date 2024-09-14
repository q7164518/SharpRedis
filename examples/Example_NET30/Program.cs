using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SharpRedis;

namespace Example_NET30
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var redis = Redis.UseMasterSlave(2, (m, s) =>
            {
                m.Password = "123456";
                m.Port = 2352;

                s[0].Password = "123456";
                s[0].Port = 2353;

                s[1].Password = "123456789";
                s[1].Port = 2354;
            });

            while (true)
            {
                Console.WriteLine(redis.String.Get("s"));

                Console.ReadLine();
            }
        }
    }
}
