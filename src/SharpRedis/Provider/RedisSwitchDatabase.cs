#if !NET30
#pragma warning disable IDE0130
#endif
using SharpRedis.Provider.Calls;
using SharpRedis.Provider.Standard;

namespace SharpRedis
{
    /// <summary>
    /// Switches to the specified database client
    /// <para>切换到指定的数据库</para>
    /// </summary>
    public sealed partial class RedisSwitchDatabase : FeatureRedis<RedisSwitchDatabase>
    {
        private SwitchDatabaseCall _switchCall;

        internal SwitchDatabaseCall SwitchCall => this._switchCall;

        private protected sealed override string DisposedException => "The currently selected database connection has been released";

        internal RedisSwitchDatabase(SwitchDatabaseCall call) : base(call)
        {
            this._switchCall = call;
        }

        protected sealed override void Dispose(bool disposing)
        {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#pragma warning disable CS8625
#endif
            base.Dispose(disposing);
            this._switchCall = null;
        }

        ~RedisSwitchDatabase()
        {
            this.Dispose(true);
        }
    }
}
