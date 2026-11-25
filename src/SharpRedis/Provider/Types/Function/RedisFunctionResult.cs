using SharpRedis.Enums;
using System;
using System.Collections.Generic;

namespace SharpRedis;

public sealed partial class RedisFunction
{
    /// <summary>
    /// The <b>FUNCTION LIST</b> command returns a value structure.
    /// <para><b>FUNCTION LIST</b> 命令返回值</para>
    /// </summary>
    public readonly struct ListResult
    {
        private readonly string _libraryName;
        private readonly string _engine;
        private readonly FunctionInfo[] _functions;
#if MODERN_NET
        private readonly string? _libraryCode;
#else
        private readonly string _libraryCode;
#endif

        /// <summary>
        /// Gets the name of the library.
        /// <para>获得函数库的名称</para>
        /// </summary>
        public readonly string LibraryName => this._libraryName;

        /// <summary>
        /// Gets the engine of the library.
        /// <para>获得函数库的脚本引擎名称</para>
        /// </summary>
        public readonly string Engine => this._engine;

        /// <summary>
        /// Gets the list of functions in the library.
        /// <para>获得函数库中的函数列表</para>
        /// </summary>
        public readonly FunctionInfo[] Functions => this._functions;

        /// <summary>
        /// Gets the library's source code (when given the WITHCODE modifier).
        /// <para>获取函数库的源代码, 需要指定WITHCODE参数, 否则返回null</para>
        /// </summary>
#if MODERN_NET
        public readonly string? LibraryCode => this._libraryCode;
#else
        public readonly string LibraryCode => this._libraryCode;
#endif

        internal ListResult(RedisResult value)
        {
#if MODERN_NET
            Dictionary<RedisResult, RedisResult>? resultDic = value;
#else
            Dictionary<RedisResult, RedisResult> resultDic = value;
#endif
            if (resultDic is { Count: > 0 })
            {
                foreach (var item in resultDic)
                {
                    string key = item.Key!;
                    if ("library_name".Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        this._libraryName = item.Value!;
                    }
                    else if ("engine".Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        this._engine = item.Value!;
                    }
                    else if ("library_code".Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        this._libraryCode = item.Value!;
                    }
                    else if ("functions".Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        if (item.Value.Type is RespType.Array && item.Value.RawValue is RedisResult[] funcArray)
                        {
                            this._functions = new FunctionInfo[funcArray.Length];
                            for (int i = 0; i < funcArray.Length; i++)
                            {
                                this._functions[i] = new(funcArray[i]);
                            }
                        }
                    }
                }
            }
            else
            {
                throw new InvalidCastException("Unrecognizable data cannot be converted into LibraryInfo");
            }

            if (string.IsNullOrEmpty(this._libraryName)) throw new InvalidCastException("The library-name is null, so it cannot be converted to ListResult.");
            if (string.IsNullOrEmpty(this._engine)) throw new InvalidCastException("The engine name is null, so it cannot be converted to ListResult.");
            if (this._functions is null) throw new InvalidCastException("The functions is null, so it cannot be converted to ListResult.");
        }

        /// <summary>
        /// Indicates the information of a function
        /// <para>表示一个函数的信息</para>
        /// </summary>
        public readonly struct FunctionInfo
        {
            private readonly string _name;
#if MODERN_NET
            private readonly string? _description;
            private readonly string[]? _flags;
#else
            private readonly string _description;
            private readonly string[] _flags;
#endif

            /// <summary>
            /// Gets the name of the function.
            /// <para>获得函数的名称</para>
            /// </summary>
            public readonly string Name => this._name;

            /// <summary>
            /// Gets the function description.
            /// <para>获得函数的描述说明 (注释说明)</para>
            /// </summary>
#if MODERN_NET
            public readonly string? Description => this._description;
#else
            public readonly string Description => this._description;
#endif

            /// <summary>
            /// Gets the function flags.
            /// <para>获得函数的标记</para>
            /// </summary>
#if MODERN_NET
            public readonly string[]? Flags => this._flags;
#else
            public readonly string[] Flags => this._flags;
#endif

            internal FunctionInfo(RedisResult value)
            {
#if MODERN_NET
                Dictionary<RedisResult, RedisResult>? resultDic = value;
#else
                Dictionary<RedisResult, RedisResult> resultDic = value;
#endif

                if (resultDic is { Count: > 0 })
                {
                    foreach (var item in resultDic)
                    {
                        string key = item.Key!;
                        if ("name".Equals(key, StringComparison.OrdinalIgnoreCase))
                        {
                            this._name = item.Value!;
                        }
                        else if ("description".Equals(key, StringComparison.OrdinalIgnoreCase))
                        {
                            this._description = item.Value!;
                        }
                        else if ("flags".Equals(key, StringComparison.OrdinalIgnoreCase))
                        {
                            if (item.Value.Type is RespType.Array)
                            {
                                this._flags = item.Value;
                            }
                        }
                    }
                }
                else
                {
                    throw new InvalidCastException("Unrecognizable data cannot be converted into FunctionInfo");
                }

                if (string.IsNullOrEmpty(this._name)) throw new InvalidCastException("The function name is null, so it cannot be converted to FunctionInfo.");
            }
        }
    }

    /// <summary>
    /// The <b>FUNCTION STATS</b> command returns a value structure.
    /// <para><b>FUNCTION STATS</b> 命令返回值</para>
    /// </summary>
    public readonly struct StatsResult
    {
#if MODERN_NET
        private readonly string? _name;
        private readonly RedisResult[]? _commands;
#else
        private readonly string _name;
        private readonly RedisResult[] _commands;
#endif
        private readonly int _duration = -1;
        private readonly Dictionary<string, EnginesInfo> _engines;

        /// <summary>
        /// Gets the name of the currently running function. If there is no running function, it will be null.
        /// <para>获得正在运行的函数名称. 没有正在运行的函数则为null</para>
        /// </summary>
#if MODERN_NET
        public readonly string? Name => this._name;
#else
        public readonly string Name => this._name;
#endif

        /// <summary>
        /// Gets the command and arguments used for invoking the function. If there is no running function, it will be null.
        /// <para>获得用于调用函数的命令和参数. 没有正在运行的函数则为null</para>
        /// </summary>
#if MODERN_NET
        public readonly RedisResult[]? Command => this._commands;
#else
        public readonly RedisResult[] Command => this._commands;
#endif

        /// <summary>
        /// Gets the function runtime duration in milliseconds. If there is no running function with a value of -1
        /// <para>获得函数执行耗时, 单位: 毫秒. 如果没有正在运行的函数为-1</para>
        /// </summary>
        public readonly int Duration => this._duration;

        /// <summary>
        /// This is a map of maps. Each entry in the map represent a single engine. Engine map contains statistics about the engine like number of functions and number of libraries.
        /// <para>获得脚本引擎信息, 每一项都是一个引擎, Key为引擎名称, Value为引擎统计信息, 包含函数数量和库数量</para>
        /// </summary>
        public readonly Dictionary<string, EnginesInfo> Engines => this._engines;

        internal StatsResult(RedisResult value)
        {
#if MODERN_NET
            Dictionary<RedisResult, RedisResult>? dic = value;
#else
            Dictionary<RedisResult, RedisResult> dic = value;
#endif
            if (dic is null) throw new InvalidCastException("Unrecognizable data cannot be converted into StatsResult");
            foreach (var item in dic)
            {
                string key = item.Key!;
                if ("engines".Equals(key, StringComparison.OrdinalIgnoreCase))
                {
#if MODERN_NET
                    Dictionary<RedisResult, RedisResult>? engines = item.Value;
#else
                    Dictionary<RedisResult, RedisResult> engines = item.Value;
#endif
                    if(engines is null) throw new InvalidCastException("Unrecognizable data cannot be converted into Dictionary<string, EnginesInfo>");
                    this._engines = new(engines.Count);
                    foreach (var engine in engines)
                    {
                        this._engines[engine.Key!] = new(engine.Value);
                    }
                }
                else if ("running_script".Equals(key, StringComparison.OrdinalIgnoreCase))
                {
#if MODERN_NET
                    Dictionary<RedisResult, RedisResult>? running = item.Value;
#else
                    Dictionary<RedisResult, RedisResult> running = item.Value;
#endif
                    if (running is null) continue;

                    foreach (var run in running)
                    {
                        string runKey = run.Key!;
                        if ("name".Equals(runKey, StringComparison.OrdinalIgnoreCase))
                        {
                            this._name = run.Value!;
                        }
                        else if ("command".Equals(runKey, StringComparison.OrdinalIgnoreCase))
                        {
                            this._commands = run.Value;
                        }
                        else if ("duration_ms".Equals(runKey, StringComparison.OrdinalIgnoreCase))
                        {
                            this._duration = run.Value;
                        }
                    }
                }
            }

            if (this._engines is null) throw new InvalidCastException("The engines is null, so it cannot be converted to StatsResult.");
        }

        /// <summary>
        /// Script Engine Information
        /// <para>脚本引擎信息</para>
        /// </summary>
        public readonly struct EnginesInfo
        {
            private readonly int _librariesCount;
            private readonly int _functionsCount;

            /// <summary>
            /// Gets the number of libraries under the current engine
            /// <para>获得当前引擎下库的数量</para>
            /// </summary>
            public readonly int LibrariesCount => this._librariesCount;

            /// <summary>
            /// Gets the number of functions under the current engine
            /// <para>获得当前引擎下函数的数量</para>
            /// </summary>
            public readonly int FunctionsCount => this._functionsCount;

            internal EnginesInfo(RedisResult value)
            {
#if MODERN_NET
                Dictionary<RedisResult, RedisResult>? dic = value;
#else
                Dictionary<RedisResult, RedisResult> dic = value;
#endif
                if (dic is null) throw new InvalidCastException("Unrecognizable data cannot be converted into EnginesInfo");
                foreach (var item in dic)
                {
                    string key = item.Key!;
                    if ("libraries_count".Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        this._librariesCount = item.Value;
                    }
                    else if ("functions_count".Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        this._functionsCount = item.Value;
                    }
                }
            }
        }
    }
}
