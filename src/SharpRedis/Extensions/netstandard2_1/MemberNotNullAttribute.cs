#if NETSTANDARD2_1
#pragma warning disable IDE0130
namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
internal sealed class MemberNotNullAttribute : Attribute
{
    private readonly string[] _members;

    public string[] Members => this._members;

    internal MemberNotNullAttribute(string member)
    {
        this._members = [member];
    }

    internal MemberNotNullAttribute(params string[] members)
    {
        this._members = members;
    }
}
#endif
