using System;

namespace SharpRedis.Models
{
    internal readonly struct ChannelMode : IEquatable<ChannelMode>
    {
        internal ChannelModeEnum Mode { get; }
        internal string Channel { get; }

        internal ChannelMode(string channel, in ChannelModeEnum mode)
        {
            this.Mode = mode;
            this.Channel = channel;
        }

        public override int GetHashCode()
        {
            return this.Channel.GetHashCode() ^ this.Mode.GetHashCode();
        }

        public bool Equals(ChannelMode other)
        {
            return this.Channel == other.Channel && this.Mode == other.Mode;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public override bool Equals(object? obj)
#else
        public override bool Equals(object obj)
#endif
        {
            if (obj == null) return false;
            if (obj is ChannelMode cm)
            {
                return cm.Equals(this);
            }
            return false;
        }

        public override string ToString()
        {
            return this.Channel;
        }

        public static bool operator ==(ChannelMode left, ChannelMode right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ChannelMode left, ChannelMode right)
        {
            return !left.Equals(right);
        }

        public static implicit operator string(in ChannelMode cm)
        {
            return cm.Channel;
        }
    }
}
