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
    public sealed class FunctionInfoValue
    {
        /// <summary>
        /// The name of the library
        /// <para>库名称</para>
        /// </summary>
        public string LibraryName { get; private set; }

        /// <summary>
        /// The engine of the library
        /// <para>库的执行引擎</para>
        /// </summary>
        public string Engine { get; private set; }

        /// <summary>
        /// The list of functions in the library
        /// <para>库里面的函数列表</para>
        /// </summary>
        public FunctionItem[] Functions { get; private set; }

        /// <summary>
        /// The library's source code (when given the WITHCODE modifier)
        /// <para>库的源码. 只有设置WITHCODE时才返回</para>
        /// </summary>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? LibraryCode { get; private set; }
#else
        public string LibraryCode { get; private set; }
#endif

        internal FunctionInfoValue(object data, Encoding encoding)
        {
            if (data is Dictionary<string, object> dic)
            {
                if (dic.TryGetValue("library_name", out var library_name))
                {
                    this.LibraryName = ConvertExtensions.To<string>(library_name, ResultType.String, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                !
#endif
                                ;
                }

                if (dic.TryGetValue("engine", out var engine))
                {
                    this.Engine = ConvertExtensions.To<string>(engine, ResultType.String, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                !
#endif
                                ;
                }

                if (dic.TryGetValue("library_code", out var library_code))
                {
                    this.LibraryCode = ConvertExtensions.To<string>(library_code, ResultType.String, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                !
#endif
                                ;
                }

                if (dic.TryGetValue("functions", out var _functions))
                {
                    if (_functions is object[] functions)
                    {
                        this.Functions = new FunctionItem[functions.Length];
                        for (uint z = 0; z < functions.Length; z++)
                        {
                            this.Functions[z] = new FunctionItem(functions[z], encoding);
                        }
                    }
                }
            }
            else
            {
                if (!(data is object[] array) || array.Length % 2 != 0)
                {
                    throw new FormatException($"The data is not a valid FunctionInfoValue, The actual type is {data.GetType().FullName}");
                }

                for (uint i = 0; i < array.Length; i += 2)
                {
                    var key = ConvertExtensions.To<string>(array[i], ResultType.String, encoding);
                    switch (key)
                    {
                        case "library_name":
                            this.LibraryName = ConvertExtensions.To<string>(array[i + 1], ResultType.String, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                !
#endif
                                ;
                            continue;
                        case "engine":
                            this.Engine = ConvertExtensions.To<string>(array[i + 1], ResultType.String, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                !
#endif
                                ;
                            continue;
                        case "library_code":
                            this.LibraryCode = ConvertExtensions.To<string>(array[i + 1], ResultType.String, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                !
#endif
                                ;
                            continue;
                        case "functions":
                            if (array[i + 1] is object[] functions)
                            {
                                this.Functions = new FunctionItem[functions.Length];
                                for (uint z = 0; z < functions.Length; z++)
                                {
                                    this.Functions[z] = new FunctionItem(functions[z], encoding);
                                }
                            }
                            continue;
                        default: continue;
                    }
                }
            }

            if (this.LibraryName is null) throw new FormatException("Unrecognized library_name format");
            if (this.Engine is null) throw new FormatException("Unrecognized engine format");
            if (this.Functions is null) throw new FormatException("Unrecognized functions format");
        }

        public sealed class FunctionItem
        {
            /// <summary>
            /// The name of the function
            /// <para>函数名称</para>
            /// </summary>
            public string Name { get; private set; }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            /// <summary>
            /// The function's description
            /// <para>函数说明</para>
            /// </summary>
            public string? Description { get; private set; }

            /// <summary>
            /// An array of function flags
            /// <para>函数标记数组</para>
            /// </summary>
            public string[]? Flags { get; private set; }
#else
            /// <summary>
            /// The function's description
            /// <para>函数说明</para>
            /// </summary>
            public string Description { get; private set; }

            /// <summary>
            /// An array of function flags
            /// <para>函数标记数组</para>
            /// </summary>
            public string[] Flags { get; private set; }
#endif

            internal FunctionItem(object data, Encoding encoding)
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

                    if (dic.TryGetValue("description", out var description))
                    {
                        this.Description = ConvertExtensions.To<string>(description, ResultType.String, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                    !
#endif
                                    ;
                    }

                    if (dic.TryGetValue("flags", out var flags))
                    {
                        this.Flags = ConvertExtensions.To<string[]>(flags, ResultType.Array | ResultType.String, encoding);
                    }
                }
                else
                {
                    if (!(data is object[] array) || array.Length % 2 != 0)
                    {
                        throw new FormatException($"The data is not a valid FunctionItem, The actual type is {data.GetType().FullName}");
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
                            case "description":
                                this.Description = ConvertExtensions.To<string>(array[i + 1], ResultType.String, encoding);
                                continue;
                            case "flags":
                                this.Flags = ConvertExtensions.To<string[]>(array[i + 1], ResultType.Array | ResultType.String, encoding);
                                continue;
                            default: continue;
                        }
                    }
                }

                if (this.Name is null) throw new FormatException("Unrecognized name format");
            }
        }
    }
}
