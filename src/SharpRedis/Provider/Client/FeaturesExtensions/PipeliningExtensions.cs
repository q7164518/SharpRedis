using SharpRedis.Provider.Standard;

namespace SharpRedis;

public static class PipeliningExtensions
{
    /// <summary>
    /// Begin a command pipeline. Call the EndPipelining method to execute the pipeline and get the return value.
    /// <para>You are advised to release the command pipe immediately after using it.</para>
    /// <para>开启一个命令管道. 调用EndPipelining方法来执行管道并获取返回值</para>
    /// <para>建议使用完命令管道后, 立即进行释放. 使用using语法即可</para>
    /// </summary>
    /// <returns>Command pipeline
    /// <para>命令管道对象</para>
    /// </returns>
    /// <example>
    /// The following code demonstrates how to use pipelines to process commands in batches.
    /// <code>
    /// using (var pipe = redis.StartPipelining())
    /// {
    ///     _ = pipe.String.Incr("num"); //The return value of the method in the pipeline is meaningless and does not need to be processed.
    ///     _ = await pipe.String.GetAsync("key");
    ///     _ = pipe.Hash.HGet("hash", "field");
    ///     var result = pipe.ExecutePipelining();
    ///     // var result = await pipe.ExecutePipeliningAsync();
    /// }
    /// </code>
    /// </example>
    /// <param name="redis">redis</param>
    public static IRedisPipeliningClient StartPipelining(this IRedisPipelining redis)
        => redis.CreatePipeliningRedis();

    /// <summary>
    /// Begin a command pipeline. Call the EndPipelining method to execute the pipeline and get the return value.
    /// <para>You are advised to release the command pipe immediately after using it.</para>
    /// <para>开启一个命令管道. 调用EndPipelining方法来执行管道并获取返回值</para>
    /// <para>建议使用完命令管道后, 立即进行释放. 使用using语法即可</para>
    /// </summary>
    /// <returns>Command pipeline
    /// <para>命令管道对象</para>
    /// </returns>
    /// <example>
    /// The following code demonstrates how to use pipelines to process commands in batches.
    /// <code>
    /// using (var pipe = redis.BeginPipelining())
    /// {
    ///     _ = pipe.String.Incr("num"); //The return value of the method in the pipeline is meaningless and does not need to be processed.
    ///     _ = await pipe.String.GetAsync("key");
    ///     _ = pipe.Hash.HGet("hash", "field");
    ///     var result = pipe.ExecutePipelining();
    ///     // var result = await pipe.ExecutePipeliningAsync();
    /// }
    /// </code>
    /// </example>
    /// <param name="redis">redis</param>
    public static IRedisPipeliningClient BeginPipelining(this IRedisPipelining redis)
        => redis.CreatePipeliningRedis();

    /// <summary>
    /// Begin a command pipeline. Call the EndPipelining method to execute the pipeline and get the return value.
    /// <para>You are advised to release the command pipe immediately after using it.</para>
    /// <para>开启一个命令管道. 调用EndPipelining方法来执行管道并获取返回值</para>
    /// <para>建议使用完命令管道后, 立即进行释放. 使用using语法即可</para>
    /// </summary>
    /// <returns>Command pipeline
    /// <para>命令管道对象</para>
    /// </returns>
    /// <example>
    /// The following code demonstrates how to use pipelines to process commands in batches.
    /// <code>
    /// using (var pipe = redis.CreatePipelining())
    /// {
    ///     _ = pipe.String.Incr("num"); //The return value of the method in the pipeline is meaningless and does not need to be processed.
    ///     _ = await pipe.String.GetAsync("key");
    ///     _ = pipe.Hash.HGet("hash", "field");
    ///     var result = pipe.ExecutePipelining();
    ///     // var result = await pipe.ExecutePipeliningAsync();
    /// }
    /// </code>
    /// </example>
    /// <param name="redis">redis</param>
    public static IRedisPipeliningClient CreatePipelining(this IRedisPipelining redis)
        => redis.CreatePipeliningRedis();

    /// <summary>
    /// Begin a command pipeline. Call the EndPipelining method to execute the pipeline and get the return value.
    /// <para>You are advised to release the command pipe immediately after using it.</para>
    /// <para>开启一个命令管道. 调用EndPipelining方法来执行管道并获取返回值</para>
    /// <para>建议使用完命令管道后, 立即进行释放. 使用using语法即可</para>
    /// </summary>
    /// <returns>Command pipeline
    /// <para>命令管道对象</para>
    /// </returns>
    /// <example>
    /// The following code demonstrates how to use pipelines to process commands in batches.
    /// <code>
    /// using (var pipe = redis.NewPipelining())
    /// {
    ///     _ = pipe.String.Incr("num"); //The return value of the method in the pipeline is meaningless and does not need to be processed.
    ///     _ = await pipe.String.GetAsync("key");
    ///     _ = pipe.Hash.HGet("hash", "field");
    ///     var result = pipe.ExecutePipelining();
    ///     // var result = await pipe.ExecutePipeliningAsync();
    /// }
    /// </code>
    /// </example>
    /// <param name="redis">redis</param>
    public static IRedisPipeliningClient NewPipelining(this IRedisPipelining redis)
        => redis.CreatePipeliningRedis();

    /// <summary>
    /// Begin a command pipeline. Call the EndPipelining method to execute the pipeline and get the return value.
    /// <para>You are advised to release the command pipe immediately after using it.</para>
    /// <para>开启一个命令管道. 调用EndPipelining方法来执行管道并获取返回值</para>
    /// <para>建议使用完命令管道后, 立即进行释放. 使用using语法即可</para>
    /// </summary>
    /// <returns>Command pipeline
    /// <para>命令管道对象</para>
    /// </returns>
    /// <example>
    /// The following code demonstrates how to use pipelines to process commands in batches.
    /// <code>
    /// using (var pipe = redis.UsePipelining())
    /// {
    ///     _ = pipe.String.Incr("num"); //The return value of the method in the pipeline is meaningless and does not need to be processed.
    ///     _ = await pipe.String.GetAsync("key");
    ///     _ = pipe.Hash.HGet("hash", "field");
    ///     var result = pipe.ExecutePipelining();
    ///     // var result = await pipe.ExecutePipeliningAsync();
    /// }
    /// </code>
    /// </example>
    /// <param name="redis">redis</param>
    public static IRedisPipeliningClient UsePipelining(this IRedisPipelining redis)
        => redis.CreatePipeliningRedis();

    /// <summary>
    /// Begin a command pipeline. Call the EndPipelining method to execute the pipeline and get the return value.
    /// <para>You are advised to release the command pipe immediately after using it.</para>
    /// <para>开启一个命令管道. 调用EndPipelining方法来执行管道并获取返回值</para>
    /// <para>建议使用完命令管道后, 立即进行释放. 使用using语法即可</para>
    /// </summary>
    /// <returns>Command pipeline
    /// <para>命令管道对象</para>
    /// </returns>
    /// <example>
    /// The following code demonstrates how to use pipelines to process commands in batches.
    /// <code>
    /// using (var pipe = redis.BatchesPipelining())
    /// {
    ///     _ = pipe.String.Incr("num"); //The return value of the method in the pipeline is meaningless and does not need to be processed.
    ///     _ = await pipe.String.GetAsync("key");
    ///     _ = pipe.Hash.HGet("hash", "field");
    ///     var result = pipe.ExecutePipelining();
    ///     // var result = await pipe.ExecutePipeliningAsync();
    /// }
    /// </code>
    /// </example>
    /// <param name="redis">redis</param>
    public static IRedisPipeliningClient BatchesPipelining(this IRedisPipelining redis)
        => redis.CreatePipeliningRedis();
}
