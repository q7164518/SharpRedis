using System;

namespace SharpRedis;

/// <summary>
/// Behaviour markers associated with a given command
/// <para>命令执行行为的一些标记, 要设置多个按位或</para>
/// </summary>
[Flags]
public enum CommandFlags : short
{
    /// <summary>
    /// Default behaviour
    /// <para>默认的, 什么都不设置</para>
    /// </summary>
    None = 0,

    /// <summary>
    /// Set the return value of the command to byte[].
    /// <para>设置命令的返回值为byte[]</para>
    /// </summary>
    ReturnBytes = 1,

    /// <summary>
    /// Force the execution of the current command using the master node.
    /// <para>If it is in the single-node mode, this setting is meaningless.</para>
    /// <para>强制使用master节点执行当前命令. 如果是单机模式, 此设置无意义</para>
    /// </summary>
    ForceMaster = 1 << 1,

    /// <summary>
    /// Force the execution of the current command using the slave node.
    /// <para>If the command involves write operations, an exception will be raised.</para>
    /// <para>强制使用slave节点执行当前命令. 如果命令包含写操作, 会引发异常</para>
    /// </summary>
    ForceSlave = 1 << 2,
}
