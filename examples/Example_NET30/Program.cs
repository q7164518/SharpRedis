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
            var redis = Redis.UseStandalone(f =>
            {
                f.Password = "123456";
                f.MaxPoolSize = 300;
            });
            redis.Server.FlushAll();
            redis.String.Set("k1", "k1");
            redis.String.Set("num", "1");
            redis.Hash.HSet("h", "f", "嘿嘿");
            redis.List.RPush("list", new string[] { "123", "456" });


            for (int i = 0; i < 300; i++)
            {
                new Thread(() =>
                {
                    int indexx = i;
                    int iii = indexx % 2;
                    while (true)
                    {
                        var k1 = redis.String.Get("k1");
                        if (k1 != "k1") Console.WriteLine("k1错误");

                        redis.String.Incr("num");

                        var hf = redis.Hash.HGet("h", "f");
                        if (hf != "嘿嘿") Console.WriteLine("hash错误");

                        var list = redis.List.LIndex("list", iii);
                        if (iii == 0 && list != "123") Console.WriteLine("list错误");
                        if (iii == 1 && list != "456") Console.WriteLine("list错误");
                    }
                }).Start();
            }
            Console.WriteLine("启动成功, NET3.0");
            Console.ReadLine();
        }
    }
}
