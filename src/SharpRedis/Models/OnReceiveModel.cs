using System;

namespace SharpRedis.Models
{
    internal sealed class OnReceiveModel<TOnReceive>
        where TOnReceive : Delegate
    {
        private readonly ResultDataType _subscribeDataType;
        private readonly TOnReceive _onReceive;

        internal ResultDataType DataType => this._subscribeDataType;

        internal TOnReceive OnReceive => this._onReceive;

        internal OnReceiveModel(ResultDataType subscribeDataType, TOnReceive onReceive)
        {
            this._subscribeDataType = subscribeDataType;
            this._onReceive = onReceive;
        }
    }
}
