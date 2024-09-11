#pragma warning disable IDE0130
using SharpRedis.Extensions;
using System;
using System.Text;

namespace SharpRedis
{
    /// <summary>
    /// SortedSet member: value
    /// </summary>
    /// <typeparam name="TMember">Member type</typeparam>
    public readonly struct MemberScoreValue<TMember>
        where TMember : class
    {
        private readonly TMember _member;
        private readonly NumberValue _score;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        /// <summary>
        /// Get member
        /// <para>获得元素</para>
        /// </summary>
        public readonly TMember Member => this._member;

        /// <summary>
        /// Get member score
        /// <para>获得元素排序分数</para>
        /// </summary>
        public readonly NumberValue Score => this._score;
#else
        /// <summary>
        /// Get member
        /// <para>获得元素</para>
        /// </summary>
        public TMember Member => this._member;

        /// <summary>
        /// Get member score
        /// <para>获得元素排序分数</para>
        /// </summary>
        public NumberValue Score => this._score;
#endif

        internal MemberScoreValue(object data, Encoding encoding, ResultType memberType)
        {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            object? scoreObject = null, memberObject = null;
#else
            object scoreObject = null, memberObject = null;
#endif
            if (data is object[] array)
            {
                if (array[0] is object[] arr && arr.Length == 2 && array.Length == 1)
                {
                    scoreObject = arr[1];
                    memberObject = arr[0];
                }
                else
                {
                    if (array.Length == 2)
                    {
                        scoreObject = array[1];
                        memberObject = array[0];
                    }
                }
            }

            if (scoreObject != null && memberObject != null)
            {
                var score = ConvertExtensions.To<NumberValue>(scoreObject, ResultType.Number, encoding)
                        ?? throw new FormatException($"The data is not a valid MemberScoreValue, The actual type is {data.GetType().FullName}");
                var member = ConvertExtensions.To<TMember>(memberObject, memberType, encoding)
                        ?? throw new FormatException($"The data is not a valid MemberScoreValue, The actual type is {data.GetType().FullName}");

                this._member = member;
                this._score = score;
            }
            else
            {
                throw new FormatException($"The data is not a valid MemberScoreValue, The actual type is {data.GetType().FullName}");
            }
        }

        public override string ToString()
        {
            return $"{this._member}: {this._score}";
        }

        /// <summary>
        /// Deconstruct, var (member, score) = this
        /// <para>解构函数, var (member, score) = this</para>
        /// </summary>
        /// <param name="member">member</param>
        /// <param name="score">score<para>排序分数</para></param>
        public void Deconstruct(out TMember member, out NumberValue score)
        {
            member = this.Member;
            score = this.Score;
        }
    }
}
