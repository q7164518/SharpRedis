#pragma warning disable IDE0130
#if NET5_0_OR_GREATER
#pragma warning disable IDE0083
#endif
using SharpRedis.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpRedis
{
    public sealed class FunctionStatsValue
    {
        /// <summary>
        /// Information about the running script. If there's no in-flight function, the server replies with a nil
        /// <para>正在运行的脚本信息, 如果没有正在运行的为null</para>
        /// </summary>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public FunctionStatsItem? RunningScript { get; private set; }
#else
        public FunctionStatsItem RunningScript { get; private set; }
#endif

        /// <summary>
        /// Available engine information
        /// <para>可用的引擎信息</para>
        /// </summary>
        public Dictionary<string, FunctionStatsEngine> Engines { get; private set; }

        internal FunctionStatsValue(object data, Encoding encoding)
        {
#if NET8_0_OR_GREATER
            this.Engines = [];
#else
            this.Engines = new Dictionary<string, FunctionStatsEngine>();
#endif
            if (data is Dictionary<string, object> dic)
            {
                if (dic.TryGetValue("running_script", out var running_script))
                {
                    if (running_script is null || running_script is DBNull)
                    {
                    }
                    else
                    {
                        this.RunningScript = new FunctionStatsItem(running_script, encoding);
                    }
                }

                if (dic.TryGetValue("engines", out var engines))
                {
                    this.SetEngines(engines, encoding);
                }
            }
            else
            {
                if (!(data is object[] array) || array.Length % 2 != 0)
                {
                    throw new FormatException($"The data is not a valid FunctionStatsValue, The actual type is {data.GetType().FullName}");
                }

                for (uint i = 0; i < array.Length; i += 2)
                {
                    var key = ConvertExtensions.To<string>(array[i], ResultType.String, encoding);
                    switch (key)
                    {
                        case "running_script":
                            if (array[i + 1] is null || array[i + 1] is DBNull)
                            {
                            }
                            else
                            {
                                this.RunningScript = new FunctionStatsItem(array[i + 1], encoding);
                            }
                            continue;
                        case "engines":
                            this.SetEngines(array[i + 1], encoding);
                            continue;
                        default: continue;
                    }
                }
            }
        }

        private void SetEngines(object engines, Encoding encoding)
        {
            if (engines is Dictionary<string, object> engDic)
            {
                foreach (var item in engDic)
                {
                    var value = new FunctionStatsEngine(item.Value, encoding);
                    this.Engines.Add(item.Key, value);
                }
            }
            else
            {
                if (!(engines is object[] engArray) || engArray.Length % 2 != 0)
                {
                    throw new FormatException($"The data is not a valid FunctionStatsEngine, The actual type is {engines.GetType().FullName}");
                }

                for (uint i = 0; i < engArray.Length; i += 2)
                {
                    var key = ConvertExtensions.To<string>(engArray[i], ResultType.String, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                        !
#endif
                        ;
                    var value = new FunctionStatsEngine(engArray[i + 1], encoding);
                    this.Engines.Add(key, value);
                }
            }
        }

        public sealed class FunctionStatsItem
        {
            /// <summary>
            /// The name of the function
            /// <para>函数名称</para>
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// The function's runtime duration in milliseconds
            /// <para>函数运行的时间. 单位: 毫秒</para>
            /// </summary>
            public long DurationMs { get; private set; }

            /// <summary>
            /// The command and arguments used for invoking the function
            /// <para>调用函数的命令和参数</para>
            /// </summary>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            public object? Command { get; private set; }
#else
            public object Command { get; private set; }
#endif

            internal FunctionStatsItem(object data, Encoding encoding)
            {
                if (data is Dictionary<string, object> dic)
                {
                    if (dic.TryGetValue("name", out var name))
                    {
                        this.Name = ConvertExtensions.To<string>(name, ResultType.String, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                    !
#endif
                                    ;
                    }

                    if (dic.TryGetValue("duration_ms", out var duration_ms))
                    {
                        this.DurationMs = ConvertExtensions.To<long>(duration_ms, ResultType.Int64, encoding);
                    }

                    if (dic.TryGetValue("command", out var command))
                    {
                        this.Command = command;
                    }
                }
                else
                {
                    if (!(data is object[] array) || array.Length % 2 != 0)
                    {
                        throw new FormatException($"The data is not a valid FunctionStatsItem, The actual type is {data.GetType().FullName}");
                    }

                    for (uint i = 0; i < array.Length; i += 2)
                    {
                        var key = ConvertExtensions.To<string>(array[i], ResultType.String, encoding);
                        switch (key)
                        {
                            case "name":
                                this.Name = ConvertExtensions.To<string>(array[i + 1], ResultType.String, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                !
#endif
                                ;
                                continue;
                            case "duration_ms":
                                this.DurationMs = ConvertExtensions.To<long>(array[i + 1], ResultType.Int64, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                !
#endif
                                ;
                                continue;
                            case "command":
                                this.Command = array[i + 1];
                                continue;
                            default: continue;
                        }
                    }
                }

                if (this.Name is null) throw new FormatException("Unrecognized name format");
            }
        }

        public readonly struct FunctionStatsEngine
        {
            private readonly long _librariesCount;
            /// <summary>
            /// libraries count
            /// <para>库数量</para>
            /// </summary>
            public long LibrariesCount => this._librariesCount;

            private readonly long _functionsCount;
            /// <summary>
            /// functions count
            /// <para>函数数量</para>
            /// </summary>
            public long FunctionsCount => this._functionsCount;

            internal FunctionStatsEngine(object data, Encoding encoding)
            {
                this._librariesCount = 0;
                this._functionsCount = 0;
                if (data is Dictionary<string, object> dic)
                {
                    if (dic.TryGetValue("libraries_count", out var libraries_count))
                    {
                        this._librariesCount = ConvertExtensions.To<long>(libraries_count, ResultType.Int64, encoding);
                    }

                    if (dic.TryGetValue("functions_count", out var functions_count))
                    {
                        this._functionsCount = ConvertExtensions.To<long>(functions_count, ResultType.Int64, encoding);
                    }
                }
                else
                {
                    if (!(data is object[] array) || array.Length % 2 != 0)
                    {
                        throw new FormatException($"The data is not a valid FunctionStatsEngine, The actual type is {data.GetType().FullName}");
                    }

                    for (uint i = 0; i < array.Length; i += 2)
                    {
                        var key = ConvertExtensions.To<string>(array[i], ResultType.String, encoding);
                        switch (key)
                        {
                            case "libraries_count":
                                this._librariesCount = ConvertExtensions.To<long>(array[i + 1], ResultType.Int64, encoding);
                                continue;
                            case "functions_count":
                                this._functionsCount = ConvertExtensions.To<long>(array[i + 1], ResultType.Int64, encoding);
                                continue;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}
