using NBomber.CSharp;
using SharpRedis;
using System.Diagnostics;

const int Inflight = 300;
const int DurationSeconds = 50;
const int KeyCount = 64;
const int KeyMask = KeyCount - 1;

const string RedisHost = "127.0.0.1";
const int RedisPort = 6379;
const string RedisPassword = "123456";

Console.WriteLine("==========================================");
Console.WriteLine("        SharpRedis Load Benchmark");
Console.WriteLine("==========================================");
Console.WriteLine($"Inflight : {Inflight}");
Console.WriteLine($"Duration : {DurationSeconds}s");
Console.WriteLine($"Redis    : {RedisHost}:{RedisPort}");
Console.WriteLine($"KeyCount : {KeyCount}");
Console.WriteLine();

// 64 必须是 2 的幂，下面使用位运算取模。
if ((KeyCount & (KeyCount - 1)) != 0)
{
    throw new InvalidOperationException("KeyCount 必须是 2 的幂。");
}

//
// Redis Client
//
var redis = Redis.UseStandalone(static f =>
{
    f.Host = RedisHost;
    f.Port = RedisPort;
    f.Password = RedisPassword;
    f.HeartbeatInterval = 1000_000;
    f.ReconnectInterval = 3000;
    f.RespVersion = Resp.RESP3;
});
//var redis = Redis.UseMasterSlave(static opt =>
//{
//    opt.Port = 16379;
//    opt.Slaves.Add(new()
//    {
//        Port = 16380,
//    });
//    opt.Slaves.Add(new()
//    {
//        Port = 16381,
//    });
//    opt.SlaveSelectionMode = SlaveSelectionMode.RoundRobin;
//});

//var redis = Redis.UseCluster(false,
//[
//    "host=127.0.0.1,port=18379,password=sharpredis-cluster-test"
//]);
//var redis = Redis.UseStandalone("host=127.0.0.1,password=123456");
//var redis = Redis.UseStandalone("host=127.0.0.1,port=30004,ssl=true");


//
// 准备测试数据：每个键都有不同的值，且返回值长度不同。
//
var keys = new string[KeyCount];
var expectedValues = new string[KeyCount];

for (int i = 0; i < KeyCount; i++)
{
    keys[i] = $"benchmark:concurrent:{i:D2}";

    int length = 1 + (i * 13 % 256);
    expectedValues[i] =
        $"value-{i:D2}-" +
        new string((char)('A' + i % 26), length);

    await redis.String.SetAsync(keys[i], expectedValues[i])
        .ConfigureAwait(false);
}

//
// GC / Process 初始状态
//
GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();

long allocatedStart = GC.GetTotalAllocatedBytes();

long gen0Start = GC.CollectionCount(0);
long gen1Start = GC.CollectionCount(1);
long gen2Start = GC.CollectionCount(2);

using Process process = Process.GetCurrentProcess();

process.Refresh();
long workingSetStart = process.WorkingSet64;

//
// 并发统计字段
//
int sequence = -1;

long attemptedGetCount = 0;
long mismatchCount = 0;
long commandExceptionCount = 0;

string? firstMismatch = null;
string? firstException = null;

//
// NBomber Scenario
//
var scenario = Scenario.Create(
    "SharpRedis_GET",
    async context =>
    {
        // 全局轮换不同的键，减少固定值导致的漏检。
        int index = Interlocked.Increment(ref sequence) & KeyMask;

        // 与第一个键相隔一半，保证同一轮的两个键不同。
        int secondIndex = (index + KeyCount / 2) & KeyMask;

        try
        {
            //
            // 先发起两个请求，再等待结果。
            // 这样同一轮中的两个 GET 有机会同时处于处理中。
            //
            var task1 = redis.String.GetAsync(keys[index]);
            var task2 = redis.String.GetAsync(keys[secondIndex]);

            Interlocked.Add(ref attemptedGetCount, 2);

            bool hasException = false;

            //
            // 即使第一个请求异常，也继续等待第二个请求，
            // 避免留下未观察的异常任务。
            //
            try
            {
                await task1.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref commandExceptionCount);
                Interlocked.CompareExchange(
                    ref firstException,
                    ex.ToString(),
                    null);

                hasException = true;
            }

            try
            {
                await task2.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref commandExceptionCount);
                Interlocked.CompareExchange(
                    ref firstException,
                    ex.ToString(),
                    null);

                hasException = true;
            }

            if (hasException)
            {
                return Response.Fail();
            }

            //
            // 两个任务都已完成，读取并校验结果。
            //
            var actual1 = task1.Result;
            var actual2 = task2.Result;

            if (actual1 != expectedValues[index] ||
                actual2 != expectedValues[secondIndex])
            {
                Interlocked.Increment(ref mismatchCount);

                // 仅保存第一次不匹配的信息，避免压测时大量输出日志。
                string message =
                    $"Mismatch: " +
                    $"key1={keys[index]}, " +
                    $"expected1={expectedValues[index]}, " +
                    $"actual1={actual1}; " +
                    $"key2={keys[secondIndex]}, " +
                    $"expected2={expectedValues[secondIndex]}, " +
                    $"actual2={actual2}";

                Interlocked.CompareExchange(
                    ref firstMismatch,
                    message,
                    null);

                return Response.Fail();
            }

            return Response.Ok();
        }
        catch (Exception ex)
        {
            // 处理请求创建阶段等意外异常。
            Interlocked.Increment(ref commandExceptionCount);

            Interlocked.CompareExchange(
                ref firstException,
                ex.ToString(),
                null);

            return Response.Fail();
        }
    })
    .WithoutWarmUp()
    .WithRestartIterationOnFail(shouldRestart: false)
    .WithLoadSimulations(
        Simulation.KeepConstant(
            copies: Inflight,
            during: TimeSpan.FromSeconds(DurationSeconds)));

//
// 开始压测
//
Stopwatch stopwatch = Stopwatch.StartNew();

var result = NBomberRunner
    .RegisterScenarios(scenario)
    .WithTestSuite("SharpRedis")
    .WithTestName("GET")
    .Run();

stopwatch.Stop();

//
// 最终统计
//
process.Refresh();

long allocatedEnd = GC.GetTotalAllocatedBytes();

long gen0End = GC.CollectionCount(0);
long gen1End = GC.CollectionCount(1);
long gen2End = GC.CollectionCount(2);

long workingSetEnd = process.WorkingSet64;

var stats = result.ScenarioStats[0];

long success = stats.Ok.Request.Count;
long failed = stats.Fail.Request.Count;
long totalScenarioRequests = success + failed;

// 使用 NBomber 记录的场景持续时间，不把报告生成等额外时间计入 QPS。
double seconds = stats.Duration.TotalSeconds;

if (seconds <= 0)
{
    seconds = DurationSeconds;
}

double scenarioQps = totalScenarioRequests / seconds;
double successfulScenarioQps = success / seconds;
double getQps = Interlocked.Read(ref attemptedGetCount) / seconds;

//
// 输出
//
Console.WriteLine();
Console.WriteLine("========== SharpRedis Benchmark ==========");

Console.WriteLine($"Inflight:              {Inflight}");
Console.WriteLine($"Target Duration:       {DurationSeconds:F2}s");
Console.WriteLine($"Measured Duration:     {seconds:F2}s");
Console.WriteLine($"Runner Elapsed:        {stopwatch.Elapsed.TotalSeconds:F2}s");

Console.WriteLine();

Console.WriteLine($"Success Scenarios:     {success:N0}");
Console.WriteLine($"Failed Scenarios:      {failed:N0}");
Console.WriteLine($"Total Scenarios:       {totalScenarioRequests:N0}");

Console.WriteLine();

Console.WriteLine($"Scenario QPS:          {scenarioQps:N0}");
Console.WriteLine($"Successful Scenario QPS: {successfulScenarioQps:N0}");
Console.WriteLine($"GET Calls:             {Interlocked.Read(ref attemptedGetCount):N0}");
Console.WriteLine($"GET Command QPS:       {getQps:N0}");

Console.WriteLine();

Console.WriteLine($"Value Mismatch Count:  {Interlocked.Read(ref mismatchCount):N0}");
Console.WriteLine($"Command Exception Count: {Interlocked.Read(ref commandExceptionCount):N0}");

Console.WriteLine();

Console.WriteLine($"P50:                   {stats.Ok.Latency.Percent50} ms");
Console.WriteLine($"P99:                   {stats.Ok.Latency.Percent99} ms");
Console.WriteLine($"Max:                   {stats.Ok.Latency.MaxMs} ms");

if (firstMismatch != null)
{
    Console.WriteLine();
    Console.WriteLine("--------------- First Mismatch ------------------");
    Console.WriteLine(firstMismatch);
}

if (firstException != null)
{
    Console.WriteLine();
    Console.WriteLine("--------------- First Exception ----------------");
    Console.WriteLine(firstException);
}

Console.WriteLine();
Console.WriteLine("--------------- Memory ------------------");

Console.WriteLine(
    $"Working Set:           {ToMB(workingSetStart):N2} MB -> {ToMB(workingSetEnd):N2} MB");

Console.WriteLine(
    $"Working Delta:         {ToMB(workingSetEnd - workingSetStart):N2} MB");

Console.WriteLine(
    $"Allocated:             {ToMB(allocatedEnd - allocatedStart):N2} MB");

Console.WriteLine();

Console.WriteLine("---------------- GC ----------------------");

Console.WriteLine($"Gen0:                  {gen0End - gen0Start:N0}");
Console.WriteLine($"Gen1:                  {gen1End - gen1Start:N0}");
Console.WriteLine($"Gen2:                  {gen2End - gen2Start:N0}");

Console.WriteLine();
Console.WriteLine("==========================================");

Console.ReadLine();

static double ToMB(long bytes)
{
    return bytes / 1024d / 1024d;
}
