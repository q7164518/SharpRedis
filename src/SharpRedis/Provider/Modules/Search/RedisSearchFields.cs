#if NET5_0_OR_GREATER
#pragma warning disable CA2208
#endif
using SharpRedis.Entities.Command;
using SharpRedis.Extensions;
using System;
using System.Globalization;

namespace SharpRedis.Modules;

public sealed partial class RedisSearch
{
    /// <summary>
    /// Represents the base class for a Redis Search schema field.
    /// <para>表示 Redis Search 索引 Schema 字段的基类</para>
    /// </summary>
#if NET11_0_OR_GREATER
    public closed class Field
#else
    public abstract class Field
#endif
    {
        private readonly FieldName _name;
        private readonly FieldType _type;

        /// <summary>
        /// Gets the field name and its optional alias.
        /// <para>获取字段名称及其可选别名</para>
        /// </summary>
        public FieldName Name => this._name;

        /// <summary>
        /// Gets the field type.
        /// <para>获取字段类型</para>
        /// </summary>
        public FieldType Type => this._type;

        /// <summary>
        /// Initializes a new instance of the Field class.
        /// <para>初始化 Field 类的新实例</para>
        /// </summary>
        /// <param name="name">
        /// The field name and its optional alias.
        /// <para>字段名称及其可选别名</para>
        /// </param>
        /// <param name="type">
        /// The field type.
        /// <para>字段类型</para>
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when the field name is null or empty.
        /// <para>当字段名称为 null 或空字符串时抛出</para>
        /// </exception>
        protected private Field(FieldName name, FieldType type)
        {
            if (name.IsNull) throw new ArgumentException("The field name cannot be null or empty.", nameof(name));
            this._name = name;
            this._type = type;
        }

        internal abstract CommandPacket AppendToCommand(ref readonly CommandPacket command);

        /// <summary>
        /// Schema field name
        /// <para>字段名称</para>
        /// </summary>
        /// <example>
        /// <code>
        /// FieldName name = new("fieldName");
        /// FieldName name = new("fieldName", "fieldAlias");
        /// FieldName name = "fieldName";
        /// FieldName name = ("fieldName", "fieldAlias");
        /// </code>
        /// </example>
        public readonly struct FieldName : IEquatable<FieldName>
        {
            private readonly string _name;

            /// <summary>
            /// Gets field name
            /// <para>获得字段名称</para>
            /// </summary>
            public readonly string Name => this._name;

            /// <summary>
            /// Gets field alias
            /// <para>获得字段别名</para>
            /// </summary>
#if MODERN_NET
            public readonly string? Alias => this._alias;
            private readonly string? _alias;
#else
            public readonly string Alias => this._alias;
            private readonly string _alias;
#endif

            public readonly bool IsNull
            {
                get
                {
                    return string.IsNullOrEmpty(this._name);
                }
            }

            /// <summary>
            /// Initialize a field-name
            /// <para>初始化一个字段名称</para>
            /// </summary>
            /// <param name="name">field name<para>字段名称</para></param>
            public FieldName(string name)
            {
                if (string.IsNullOrEmpty(name)) throw new ArgumentException("The field name is not empty or null.", nameof(name));
                this._name = name;
            }

            /// <summary>
            /// Initialize a field-name
            /// <para>初始化一个字段名称</para>
            /// </summary>
            /// <param name="name">field name<para>字段名称</para></param>
            /// <param name="alias">Field alias<para>字段别名</para></param>
            public FieldName(string name, string alias)
                : this(name)
            {
                this._alias = alias;
            }

            public bool Equals(FieldName other)
            {
                return this._name == other._name
                    && this._alias == other._alias;
            }

#if MODERN_NET
            public override bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] object? obj)
#else
            public override bool Equals(object obj)
#endif
            {
                if (obj is not FieldName other) return false;
                return this.Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 31 + (this._name?.GetHashCode() ?? 0);
                    hash = hash * 31 + (this._alias?.GetHashCode() ?? 0);
                    return hash;
                }
            }

            public static bool operator ==(FieldName left, FieldName right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(FieldName left, FieldName right)
            {
                return !left.Equals(right);
            }

            public static implicit operator FieldName(string name)
            {
                return new(name);
            }

            public static implicit operator FieldName((string name, string alias) nameAlias)
            {
                return new(name: nameAlias.name, alias: nameAlias.alias);
            }
        }

        /// <summary>
        /// Field types supported by Redis Search.
        /// <para>Redis Search 支持的字段类型</para>
        /// </summary>
        public enum FieldType : byte
        {
            /// <summary>
            /// Full-text field, supports tokenization, stemming, relevance scoring, and full-text search.
            /// <para>全文文本字段，支持分词、词干提取、相关性评分和全文搜索</para>
            /// </summary>
            Text = 0,

            /// <summary>
            /// Tag field, supports exact matching and filtering by one or more tags.
            /// <para>标签字段，支持精确匹配以及单个或多个标签的过滤查询</para>
            /// </summary>
            Tag = 1,

            /// <summary>
            /// Numeric field, supports numeric range queries, filtering, and sorting.
            /// <para>数值字段，支持数值范围查询、条件过滤和排序</para>
            /// </summary>
            Numeric = 2,

            /// <summary>
            /// Geographic coordinate field, supports geospatial queries based on longitude, latitude, and radius.
            /// <para>地理坐标字段，支持基于经度、纬度和半径的地理位置范围查询</para>
            /// </summary>
            Geo = 3,

            /// <summary>
            /// Vector field, supports vector similarity search and nearest-neighbor queries.
            /// <para>向量字段，支持向量相似度搜索和最近邻查询，适用于语义搜索、AI 检索等场景</para>
            /// </summary>
            Vector = 4,

            /// <summary>
            /// Geometric shape field, supports indexing and spatial queries on geometric shapes such as polygons.
            /// <para>几何形状字段，支持对多边形等几何图形建立索引并执行空间关系查询</para>
            /// </summary>
            GeoShape = 5,
        }

        /// <summary>
        /// Options for Redis Search schema fields.
        /// Multiple options can be combined using bitwise OR.
        /// <para>Redis Search Schema 字段配置选项，支持通过按位或运算符 | 组合多个选项</para>
        /// <para>不同字段类型支持的选项不同，具体由对应字段类型进行校验</para>
        /// </summary>
        [Flags]
        public enum FieldOptions : ushort
        {
            /// <summary>
            /// No additional field options.
            /// <para>不启用任何额外字段选项</para>
            /// </summary>
            None = 0,

            /// <summary>
            /// Optimizes sorting performance for TEXT, TAG, NUMERIC, and GEO fields.
            /// <para>为 TEXT、TAG、NUMERIC、GEO 字段建立排序优化结构，提高排序性能，但会增加内存开销</para>
            /// </summary>
            Sortable = 1 << 0,

            /// <summary>
            /// Preserves the original, unnormalized value for sorting.
            /// Requires SORTABLE.
            /// <para>保留未规范化的原始排序值，不进行大小写转换和重音符号移除等处理</para>
            /// <para>必须与 Sortable 配合使用</para>
            /// </summary>
            Unf = 1 << 1,

            /// <summary>
            /// Disables stemming for TEXT fields.
            /// <para>禁用 TEXT 字段的词干提取，不对单词进行词干归一化处理</para>
            /// </summary>
            NoStem = 1 << 2,

            /// <summary>
            /// Excludes the field from the search index.
            /// Can be combined with SORTABLE to retain sorting capabilities.
            /// <para>禁止为字段建立搜索索引，可与 Sortable 配合使用，仅保留排序能力</para>
            /// </summary>
            NoIndex = 1 << 3,

            /// <summary>
            /// Enables case-sensitive matching for TAG fields.
            /// <para>启用 TAG 字段的大小写敏感匹配，不将标签自动转换为小写</para>
            /// </summary>
            CaseSensitive = 1 << 4,

            /// <summary>
            /// Creates a suffix trie for TEXT and TAG fields.
            /// Optimizes suffix and contains queries.
            /// <para>为 TEXT 和 TAG 字段建立后缀 Trie 索引，优化后缀匹配和包含匹配查询</para>
            /// </summary>
            WithSuffixTrie = 1 << 5,

            /// <summary>
            /// Indexes empty string values for TEXT and TAG fields.
            /// <para>允许索引 TEXT 和 TAG 字段中的空字符串</para>
            /// <para>Available since RediSearch 2.10.</para>
            /// <para>支持此选项的 RediSearch 版本：2.10+</para>
            /// </summary>
            IndexEmpty = 1 << 6,

            /// <summary>
            /// Indexes documents that do not contain the specified field.
            /// <para>索引缺失指定字段的文档，以支持查询字段不存在的文档</para>
            /// <para>Available since RediSearch 2.10.</para>
            /// <para>支持此选项的 RediSearch 版本：2.10+</para>
            /// </summary>
            IndexMissing = 1 << 7,
        }
    }

    /// <summary>
    /// Represents a TEXT field in a Redis Search index schema.
    /// <para>表示 Redis Search 索引 Schema 中的 TEXT 全文搜索字段</para>
    /// </summary>
    public sealed class TextField : Field
    {
        private const FieldOptions _supportedOptions =
            FieldOptions.Sortable |
            FieldOptions.Unf |
            FieldOptions.NoStem |
            FieldOptions.NoIndex |
            FieldOptions.WithSuffixTrie |
            FieldOptions.IndexEmpty |
            FieldOptions.IndexMissing;

        private readonly FieldOptions _options;

        /// <summary>
        /// Gets the relevance scoring weight of this field.
        /// <para>获取字段的搜索相关性评分权重，默认为 1.0</para>
        /// </summary>
        public double Weight { get; }

        /// <summary>
        /// Gets the phonetic matching algorithm.
        /// <para>获取语音匹配算法，默认为 None（禁用语音匹配）</para>
        /// </summary>
        public PhoneticMatcher Phonetic { get; }

        /// <summary>
        /// Gets the field configuration options.
        /// <para>获取字段配置选项，支持通过按位或运算符 | 组合多个选项</para>
        /// </summary>
        public FieldOptions Options => this._options;

        /// <summary>
        /// Gets whether stemming is disabled for this field.
        /// <para>获取是否禁用词干提取，启用后不会对单词进行词干归一化处理</para>
        /// </summary>
        public bool NoStem => (this._options & FieldOptions.NoStem) == FieldOptions.NoStem;

        /// <summary>
        /// Gets whether the field is optimized for sorting.
        /// <para>获取是否启用排序优化，提高排序性能，但会增加内存开销</para>
        /// </summary>
        public bool Sortable => (this._options & FieldOptions.Sortable) == FieldOptions.Sortable;

        /// <summary>
        /// Gets whether the original, unnormalized field value is preserved for sorting.
        /// <para>获取是否保留未规范化的原始排序值，不进行大小写转换和重音符号移除等处理</para>
        /// <para>Requires SORTABLE. For JSON fields, UNF is implicit when SORTABLE is enabled.</para>
        /// <para>必须与 SORTABLE 配合使用。对于 JSON 字段，启用 SORTABLE 时默认采用 UNF 行为</para>
        /// </summary>
        public bool Unf => (this._options & FieldOptions.Unf) == FieldOptions.Unf;

        /// <summary>
        /// Gets whether the field is excluded from the search index.
        /// <para>获取是否禁止为字段建立搜索索引，可与 SORTABLE 配合使用，仅保留排序能力</para>
        /// </summary>
        public bool NoIndex => (this._options & FieldOptions.NoIndex) == FieldOptions.NoIndex;

        /// <summary>
        /// Gets whether a suffix trie is created for this field.
        /// <para>获取是否建立后缀 Trie 索引，以优化后缀匹配和包含匹配查询</para>
        /// </summary>
        public bool WithSuffixTrie => (this._options & FieldOptions.WithSuffixTrie) == FieldOptions.WithSuffixTrie;

        /// <summary>
        /// Gets whether documents missing this field are indexed.
        /// <para>获取是否索引缺失该字段的文档，以支持查询不包含该字段的文档</para>
        /// <para>Available since RediSearch 2.10.</para>
        /// <para>支持此选项的 RediSearch 版本：2.10+</para>
        /// </summary>
        public bool IndexMissing => (this._options & FieldOptions.IndexMissing) == FieldOptions.IndexMissing;

        /// <summary>
        /// Gets whether empty string values are indexed.
        /// <para>获取是否索引空字符串，以支持查询字段值为空字符串的文档</para>
        /// <para>Available since RediSearch 2.10.</para>
        /// <para>支持此选项的 RediSearch 版本：2.10+</para>
        /// </summary>
        public bool IndexEmpty => (this._options & FieldOptions.IndexEmpty) == FieldOptions.IndexEmpty;

        /// <summary>
        /// Initializes a new instance of the TextField class.
        /// <para>初始化 TextField 全文搜索字段的新实例</para>
        /// </summary>
        /// <param name="name">
        /// The field name and its optional alias.
        /// <para>字段名称及其可选别名</para>
        /// </param>
        /// <param name="weight">
        /// The relevance scoring weight. The default is 1.0.
        /// <para>搜索相关性评分权重，默认为 1.0，必须为有效正数</para>
        /// </param>
        /// <param name="phonetic">
        /// The phonetic matching algorithm. The default is None.
        /// <para>语音匹配算法，默认为 None（禁用语音匹配）</para>
        /// </param>
        /// <param name="options">
        /// Field options that can be combined using bitwise OR.
        /// Only options supported by TEXT fields are allowed.
        /// <para>字段配置选项，可通过按位或运算符 | 组合多个选项，默认为 None</para>
        /// <para>仅允许使用 TEXT 字段支持的选项</para>
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when weight, phonetic, or options contains an invalid value.
        /// <para>当 weight、phonetic 或 options 包含无效值时抛出</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when Unf is enabled without Sortable.
        /// <para>当启用 Unf 但未启用 Sortable 时抛出</para>
        /// </exception>
        public TextField(
            FieldName name,
            double weight = 1.0,
            PhoneticMatcher phonetic = PhoneticMatcher.None,
            FieldOptions options = FieldOptions.None
        ) : base(name, FieldType.Text)
        {
            if (double.IsNaN(weight) || double.IsInfinity(weight) || weight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(weight), "The field weight must be a finite positive number.");
            }

            if (phonetic > PhoneticMatcher.Spanish)
            {
                throw new ArgumentOutOfRangeException(nameof(phonetic), "The phonetic matcher is invalid.");
            }

            if ((options & ~TextField._supportedOptions) != FieldOptions.None)
            {
                throw new ArgumentOutOfRangeException(nameof(options), "The TEXT field contains unsupported options.");
            }

            if ((options & FieldOptions.Unf) == FieldOptions.Unf && (options & FieldOptions.Sortable) == FieldOptions.None)
            {
                throw new ArgumentException("UNF requires SORTABLE to be enabled.", nameof(options));
            }

            this.Weight = weight;
            this.Phonetic = phonetic;
            this._options = options;
        }

        internal sealed override CommandPacket AppendToCommand(ref readonly CommandPacket command)
        {
            _ = command.WriteArguments(base.Name.Name);

            if (!string.IsNullOrEmpty(base.Name.Alias))
            {
                _ = command
                    .WriteArguments("AS")
                    .WriteArguments(base.Name.Alias);
            }

            _ = command.WriteArguments("TEXT");

            if (this.Weight != 1.0)
            {
                _ = command
                    .WriteArguments("WEIGHT")
                    .WriteArguments(this.Weight.ToBytes());
            }

            _ = command.WriteArguments(this.NoStem, "NOSTEM");

            if (this.Phonetic != PhoneticMatcher.None)
            {
                _ = command.WriteArguments("PHONETIC");

                _ = this.Phonetic switch
                {
                    PhoneticMatcher.English => command.WriteArguments("dm:en"),
                    PhoneticMatcher.French => command.WriteArguments("dm:fr"),
                    PhoneticMatcher.Portuguese => command.WriteArguments("dm:pt"),
                    PhoneticMatcher.Spanish => command.WriteArguments("dm:es"),
                    _ => command,
                };
            }

            _ = command
                .WriteArguments(this.Sortable, "SORTABLE")
                .WriteArguments(this.Unf, "UNF")
                .WriteArguments(this.NoIndex, "NOINDEX")
                .WriteArguments(this.WithSuffixTrie, "WITHSUFFIXTRIE")
                .WriteArguments(this.IndexMissing, "INDEXMISSING")
                .WriteArguments(this.IndexEmpty, "INDEXEMPTY");

            return command;
        }

        /// <summary>
        /// Phonetic matching algorithms supported by Redis Search.
        /// <para>Redis Search 支持的语音匹配算法</para>
        /// </summary>
        public enum PhoneticMatcher : byte
        {
            /// <summary>
            /// Disables phonetic matching.
            /// <para>禁用语音匹配，不生成 PHONETIC 参数</para>
            /// </summary>
            None = 0,

            /// <summary>
            /// Double Metaphone phonetic matching for English (dm:en).
            /// <para>英语 Double Metaphone 语音匹配算法（dm:en）</para>
            /// </summary>
            English = 1,

            /// <summary>
            /// Double Metaphone phonetic matching for French (dm:fr).
            /// <para>法语 Double Metaphone 语音匹配算法（dm:fr）</para>
            /// </summary>
            French = 2,

            /// <summary>
            /// Double Metaphone phonetic matching for Portuguese (dm:pt).
            /// <para>葡萄牙语 Double Metaphone 语音匹配算法（dm:pt）</para>
            /// </summary>
            Portuguese = 3,

            /// <summary>
            /// Double Metaphone phonetic matching for Spanish (dm:es).
            /// <para>西班牙语 Double Metaphone 语音匹配算法（dm:es）</para>
            /// </summary>
            Spanish = 4,
        }
    }

    /// <summary>
    /// Represents a TAG field in a Redis Search index schema.
    /// <para>表示 Redis Search 索引 Schema 中的 TAG 标签字段</para>
    /// </summary>
    public sealed class TagField : Field
    {
        private const FieldOptions _supportedOptions =
            FieldOptions.Sortable |
            FieldOptions.Unf |
            FieldOptions.NoIndex |
            FieldOptions.CaseSensitive |
            FieldOptions.WithSuffixTrie |
            FieldOptions.IndexEmpty |
            FieldOptions.IndexMissing;
        private const char _defaultSeparator = ',';

        private readonly FieldOptions _options;

        /// <summary>
        /// Gets the tag separator. Null uses the Redis default behavior.
        /// <para>获取标签分隔符，null 表示使用 Redis 默认行为</para>
        /// <para>HASH defaults to comma; JSON strings are not split by default.</para>
        /// <para>HASH 默认使用逗号，JSON 字符串默认不进行分隔</para>
        /// </summary>
        public char Separator { get; }

        /// <summary>
        /// Gets the field configuration options.
        /// <para>获取字段配置选项</para>
        /// </summary>
        public FieldOptions Options => this._options;

        /// <summary>
        /// Gets whether sorting optimization is enabled.
        /// <para>获取是否启用排序优化</para>
        /// </summary>
        public bool Sortable => (this._options & FieldOptions.Sortable) == FieldOptions.Sortable;

        /// <summary>
        /// Gets whether the original value is preserved for sorting.
        /// <para>获取是否保留未规范化的原始排序值</para>
        /// </summary>
        public bool Unf => (this._options & FieldOptions.Unf) == FieldOptions.Unf;

        /// <summary>
        /// Gets whether the field is excluded from the search index.
        /// <para>获取是否禁止为字段建立搜索索引</para>
        /// </summary>
        public bool NoIndex => (this._options & FieldOptions.NoIndex) == FieldOptions.NoIndex;

        /// <summary>
        /// Gets whether tag matching is case-sensitive.
        /// <para>获取标签匹配是否区分大小写</para>
        /// </summary>
        public bool CaseSensitive => (this._options & FieldOptions.CaseSensitive) == FieldOptions.CaseSensitive;

        /// <summary>
        /// Gets whether a suffix trie is created.
        /// <para>获取是否建立后缀 Trie 索引</para>
        /// </summary>
        public bool WithSuffixTrie => (this._options & FieldOptions.WithSuffixTrie) == FieldOptions.WithSuffixTrie;

        /// <summary>
        /// Gets whether missing field values are indexed.
        /// <para>获取是否索引缺失字段，支持 RediSearch 2.10+</para>
        /// </summary>
        public bool IndexMissing => (this._options & FieldOptions.IndexMissing) == FieldOptions.IndexMissing;

        /// <summary>
        /// Gets whether empty string values are indexed.
        /// <para>获取是否索引空字符串，支持 RediSearch 2.10+</para>
        /// </summary>
        public bool IndexEmpty => (this._options & FieldOptions.IndexEmpty) == FieldOptions.IndexEmpty;

        /// <summary>
        /// Initializes a new instance of the TagField class.
        /// <para>初始化 TagField 标签字段的新实例</para>
        /// </summary>
        /// <param name="name">The field name and optional alias.<para>字段名称及可选别名</para></param>
        /// <param name="separator">Optional printable ASCII separator.<para>可选的 ASCII 可打印字符分隔符</para></param>
        /// <param name="options">Field configuration options.<para>字段配置选项，支持按位组合</para></param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The separator or field options are invalid.
        /// <para>分隔符或字段选项无效时抛出</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// UNF is enabled without SORTABLE.
        /// <para>启用 UNF 但未启用 SORTABLE 时抛出</para>
        /// </exception>
        public TagField(
            FieldName name,
            char separator = TagField._defaultSeparator,
            FieldOptions options = FieldOptions.None
        ) : base(name, FieldType.Tag)
        {
            if (separator < '!' || separator > '~')
            {
                throw new ArgumentOutOfRangeException(nameof(separator), "The TAG separator must be a printable ASCII character.");
            }

            if ((options & ~TagField._supportedOptions) != FieldOptions.None)
            {
                throw new ArgumentOutOfRangeException(nameof(options), "The TAG field contains unsupported options.");
            }

            if ((options & FieldOptions.Unf) == FieldOptions.Unf && (options & FieldOptions.Sortable) == FieldOptions.None)
            {
                throw new ArgumentException("UNF requires SORTABLE to be enabled.", nameof(options));
            }

            this.Separator = separator;
            this._options = options;
        }

        internal sealed override CommandPacket AppendToCommand(ref readonly CommandPacket command)
        {
            _ = command.WriteArguments(base.Name.Name);

            if (!string.IsNullOrEmpty(base.Name.Alias))
            {
                _ = command
                    .WriteArguments("AS")
                    .WriteArguments(base.Name.Alias);
            }

            _ = command.WriteArguments("TAG");

            if (this.Separator != TagField._defaultSeparator)
            {
                _ = command
                    .WriteArguments("SEPARATOR")
                    .WriteArguments(this.Separator.ToString());
            }

            _ = command
                .WriteArguments(this.CaseSensitive, "CASESENSITIVE")
                .WriteArguments(this.Sortable, "SORTABLE")
                .WriteArguments(this.Unf, "UNF")
                .WriteArguments(this.NoIndex, "NOINDEX")
                .WriteArguments(this.WithSuffixTrie, "WITHSUFFIXTRIE")
                .WriteArguments(this.IndexMissing, "INDEXMISSING")
                .WriteArguments(this.IndexEmpty, "INDEXEMPTY");

            return command;
        }
    }

    /// <summary>
    /// Represents a NUMERIC field in a Redis Search index schema.
    /// <para>表示 Redis Search 索引 Schema 中的 NUMERIC 数值字段</para>
    /// </summary>
    public sealed class NumericField : Field
    {
        private const FieldOptions _supportedOptions =
            FieldOptions.Sortable |
            FieldOptions.NoIndex |
            FieldOptions.IndexMissing;

        private readonly FieldOptions _options;

        /// <summary>
        /// Gets the field configuration options.
        /// <para>获取字段配置选项</para>
        /// </summary>
        public FieldOptions Options => this._options;

        /// <summary>
        /// Gets whether sorting optimization is enabled.
        /// <para>获取是否启用数值排序优化</para>
        /// </summary>
        public bool Sortable => (this._options & FieldOptions.Sortable) == FieldOptions.Sortable;

        /// <summary>
        /// Gets whether the field is excluded from the search index.
        /// <para>获取是否禁止为数值字段建立搜索索引</para>
        /// </summary>
        public bool NoIndex => (this._options & FieldOptions.NoIndex) == FieldOptions.NoIndex;

        /// <summary>
        /// Gets whether documents missing this field are indexed.
        /// <para>获取是否索引缺失该字段的文档，支持 RediSearch 2.10+</para>
        /// </summary>
        public bool IndexMissing => (this._options & FieldOptions.IndexMissing) == FieldOptions.IndexMissing;

        /// <summary>
        /// Initializes a new instance of the NumericField class.
        /// <para>初始化 NumericField 数值字段的新实例</para>
        /// </summary>
        /// <param name="name">The field name and optional alias.<para>字段名称及可选别名</para></param>
        /// <param name="options">Field configuration options.<para>字段配置选项，支持按位组合</para></param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The options contain unsupported flags.
        /// <para>当选项包含不支持的标志位时抛出</para>
        /// </exception>
        public NumericField(
            FieldName name,
            FieldOptions options = FieldOptions.None
        ) : base(name, FieldType.Numeric)
        {
            if ((options & ~NumericField._supportedOptions) != FieldOptions.None)
            {
                throw new ArgumentOutOfRangeException(nameof(options), "The NUMERIC field contains unsupported options.");
            }

            this._options = options;
        }

        internal sealed override CommandPacket AppendToCommand(ref readonly CommandPacket command)
        {
            _ = command.WriteArguments(base.Name.Name);

            if (!string.IsNullOrEmpty(base.Name.Alias))
            {
                _ = command
                    .WriteArguments("AS")
                    .WriteArguments(base.Name.Alias);
            }

            _ = command
                .WriteArguments("NUMERIC")
                .WriteArguments(this.Sortable, "SORTABLE")
                .WriteArguments(this.NoIndex, "NOINDEX")
                .WriteArguments(this.IndexMissing, "INDEXMISSING");

            return command;
        }
    }

    /// <summary>
    /// Represents a GEO field in a Redis Search index schema.
    /// Supports geographical radius queries.
    /// <para>表示 Redis Search 索引 Schema 中的 GEO 地理坐标字段</para>
    /// <para>支持基于经纬度和半径的地理范围查询</para>
    /// </summary>
    public sealed class GeoField : Field
    {
        private const FieldOptions _supportedOptions =
            FieldOptions.Sortable |
            FieldOptions.NoIndex |
            FieldOptions.IndexMissing;

        private readonly FieldOptions _options;

        /// <summary>
        /// Gets the field configuration options.
        /// <para>获取字段配置选项</para>
        /// </summary>
        public FieldOptions Options => this._options;

        /// <summary>
        /// Gets whether sorting optimization is enabled.
        /// <para>获取是否启用地理坐标字段的排序优化</para>
        /// </summary>
        public bool Sortable => (this._options & FieldOptions.Sortable) == FieldOptions.Sortable;

        /// <summary>
        /// Gets whether the field is excluded from the search index.
        /// <para>获取是否禁止为地理坐标字段建立搜索索引</para>
        /// </summary>
        public bool NoIndex => (this._options & FieldOptions.NoIndex) == FieldOptions.NoIndex;

        /// <summary>
        /// Gets whether documents missing this field are indexed.
        /// <para>获取是否索引缺失该字段的文档，支持 RediSearch 2.10+</para>
        /// </summary>
        public bool IndexMissing => (this._options & FieldOptions.IndexMissing) == FieldOptions.IndexMissing;

        /// <summary>
        /// Initializes a new instance of the GeoField class.
        /// <para>初始化 GeoField 地理坐标字段的新实例</para>
        /// </summary>
        /// <param name="name">The field name and optional alias.<para>字段名称及可选别名</para></param>
        /// <param name="options">Field configuration options.<para>字段配置选项，支持按位组合</para></param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The options contain unsupported flags.
        /// <para>当选项包含不支持的标志位时抛出</para>
        /// </exception>
        public GeoField(
            FieldName name,
            FieldOptions options = FieldOptions.None
        ) : base(name, FieldType.Geo)
        {
            if ((options & ~GeoField._supportedOptions) != FieldOptions.None)
            {
                throw new ArgumentOutOfRangeException(nameof(options), "The GEO field contains unsupported options.");
            }

            this._options = options;
        }

        internal sealed override CommandPacket AppendToCommand(ref readonly CommandPacket command)
        {
            _ = command.WriteArguments(base.Name.Name);

            if (!string.IsNullOrEmpty(base.Name.Alias))
            {
                _ = command
                    .WriteArguments("AS")
                    .WriteArguments(base.Name.Alias);
            }

            _ = command
                .WriteArguments("GEO")
                .WriteArguments(this.Sortable, "SORTABLE")
                .WriteArguments(this.NoIndex, "NOINDEX")
                .WriteArguments(this.IndexMissing, "INDEXMISSING");

            return command;
        }
    }

    /// <summary>
    /// Represents a GEOSHAPE field in a Redis Search index schema.
    /// <para>表示 Redis Search 索引 Schema 中的 GEOSHAPE 几何形状字段</para>
    /// </summary>
    public sealed class GeoShapeField : Field
    {
        private const FieldOptions _supportedOptions =
            FieldOptions.NoIndex |
            FieldOptions.IndexMissing;

        private readonly FieldOptions _options;

        /// <summary>
        /// Gets the coordinate system used by this field.
        /// <para>获取字段使用的坐标系统，默认为 Spherical</para>
        /// </summary>
        public GeoShapeCoordinateSystem CoordinateSystem { get; }

        /// <summary>
        /// Gets the field configuration options.
        /// <para>获取字段配置选项</para>
        /// </summary>
        public FieldOptions Options => this._options;

        /// <summary>
        /// Gets whether the field is excluded from the search index.
        /// <para>获取是否禁止为几何形状字段建立搜索索引</para>
        /// </summary>
        public bool NoIndex => (this._options & FieldOptions.NoIndex) == FieldOptions.NoIndex;

        /// <summary>
        /// Gets whether documents missing this field are indexed.
        /// <para>获取是否索引缺失该字段的文档，支持 RediSearch 2.10+</para>
        /// </summary>
        public bool IndexMissing => (this._options & FieldOptions.IndexMissing) == FieldOptions.IndexMissing;

        /// <summary>
        /// Initializes a new instance of the GeoShapeField class.
        /// <para>初始化 GeoShapeField 几何形状字段的新实例</para>
        /// </summary>
        /// <param name="name">
        /// The field name and optional alias.
        /// <para>字段名称及可选别名</para>
        /// </param>
        /// <param name="coordinateSystem">
        /// The coordinate system. The default is Spherical.
        /// <para>坐标系统，默认为 Spherical 球面坐标系统</para>
        /// </param>
        /// <param name="options">
        /// Field configuration options.
        /// <para>字段配置选项，支持按位组合</para>
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The coordinate system or field options are invalid.
        /// <para>当坐标系统或字段选项无效时抛出</para>
        /// </exception>
        public GeoShapeField(
            FieldName name,
            GeoShapeCoordinateSystem coordinateSystem = GeoShapeCoordinateSystem.Spherical,
            FieldOptions options = FieldOptions.None
        ) : base(name, FieldType.GeoShape)
        {
            if (coordinateSystem > GeoShapeCoordinateSystem.Flat)
            {
                throw new ArgumentOutOfRangeException(nameof(coordinateSystem), "The GEOSHAPE coordinate system is invalid.");
            }

            if ((options & ~GeoShapeField._supportedOptions) != FieldOptions.None)
            {
                throw new ArgumentOutOfRangeException(nameof(options), "The GEOSHAPE field contains unsupported options.");
            }

            this.CoordinateSystem = coordinateSystem;
            this._options = options;
        }

        internal sealed override CommandPacket AppendToCommand(ref readonly CommandPacket command)
        {
            _ = command.WriteArguments(base.Name.Name);

            if (!string.IsNullOrEmpty(base.Name.Alias))
            {
                _ = command
                    .WriteArguments("AS")
                    .WriteArguments(base.Name.Alias);
            }

            _ = command.WriteArguments("GEOSHAPE");

            // SPHERICAL is the Redis default.
            if (this.CoordinateSystem == GeoShapeCoordinateSystem.Flat)
            {
                _ = command.WriteArguments("FLAT");
            }

            _ = command
                .WriteArguments(this.NoIndex, "NOINDEX")
                .WriteArguments(this.IndexMissing, "INDEXMISSING");

            return command;
        }

        /// <summary>
        /// Coordinate systems supported by Redis Search GEOSHAPE fields.
        /// <para>Redis Search GEOSHAPE 字段支持的坐标系统</para>
        /// </summary>
        public enum GeoShapeCoordinateSystem : byte
        {
            /// <summary>
            /// Spherical geographic coordinates. This is the default.
            /// <para>球面地理坐标系统，适用于经纬度坐标，Redis 默认值</para>
            /// </summary>
            Spherical = 0,

            /// <summary>
            /// Flat Cartesian coordinates.
            /// <para>平面笛卡尔坐标系统，适用于二维平面几何图形</para>
            /// </summary>
            Flat
        }
    }

    #region Vector fields
/// <summary>
/// Represents the base class for Redis Search VECTOR fields.
/// <para>表示 Redis Search VECTOR 向量字段的基类</para>
/// </summary>
#if NET11_0_OR_GREATER
    public closed class VectorField : Field
#else
    public abstract class VectorField : Field
#endif
    {
        private const FieldOptions _supportedOptions =
            FieldOptions.NoIndex |
            FieldOptions.IndexMissing;

        private readonly FieldOptions _options;

        /// <summary>
        /// Gets the vector indexing algorithm.
        /// <para>获取向量索引算法</para>
        /// </summary>
        public VectorAlgorithm Algorithm { get; }

        /// <summary>
        /// Gets the vector element data type.
        /// <para>获取向量元素的数据类型</para>
        /// </summary>
        public VectorDataType DataType { get; }

        /// <summary>
        /// Gets the number of vector dimensions.
        /// <para>获取向量维度</para>
        /// </summary>
        public int Dimensions { get; }

        /// <summary>
        /// Gets the vector distance metric.
        /// <para>获取向量距离计算方式</para>
        /// </summary>
        public VectorDistanceMetric DistanceMetric { get; }

        /// <summary>
        /// Gets the field configuration options.
        /// <para>获取字段配置选项</para>
        /// </summary>
        public FieldOptions Options => this._options;

        /// <summary>
        /// Gets whether the field is excluded from indexing.
        /// <para>获取是否禁用字段搜索索引</para>
        /// </summary>
        public bool NoIndex => (this._options & FieldOptions.NoIndex) == FieldOptions.NoIndex;

        /// <summary>
        /// Gets whether missing field values are indexed.
        /// <para>获取是否允许索引缺失字段的文档</para>
        /// </summary>
        public bool IndexMissing => (this._options & FieldOptions.IndexMissing) == FieldOptions.IndexMissing;

        /// <summary>
        /// Gets the number of additional algorithm attribute tokens.
        /// <para>获取算法专属参数的 Token 数量，每组参数名称和值计为 2</para>
        /// </summary>
        protected abstract int AdditionalArgumentCount { get; }

        /// <summary>
        /// Initializes a VECTOR field.
        /// <para>初始化 VECTOR 向量字段</para>
        /// </summary>
        /// <param name="name">Field name and optional alias.<para>字段名称及可选别名</para></param>
        /// <param name="algorithm">Vector indexing algorithm.<para>向量索引算法</para></param>
        /// <param name="dataType">Vector element data type.<para>向量元素类型</para></param>
        /// <param name="dimensions">Vector dimensions.<para>向量维度，必须大于零</para></param>
        /// <param name="distanceMetric">Distance calculation metric.<para>向量距离计算方式</para></param>
        /// <param name="options">Field configuration options.<para>字段配置选项</para></param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when a parameter is invalid.
        /// <para>参数值无效时抛出</para>
        /// </exception>
        protected private VectorField(
            FieldName name,
            VectorAlgorithm algorithm,
            VectorDataType dataType,
            int dimensions,
            VectorDistanceMetric distanceMetric,
            FieldOptions options
        ) : base(name, FieldType.Vector)
        {
            if (algorithm > VectorAlgorithm.SvsVamana) throw new ArgumentOutOfRangeException(nameof(algorithm));
            if (dataType > VectorDataType.UInt8) throw new ArgumentOutOfRangeException(nameof(dataType));
            if (distanceMetric > VectorDistanceMetric.Cosine) throw new ArgumentOutOfRangeException(nameof(distanceMetric));
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(dimensions);
#else
            if (dimensions <= 0) throw new ArgumentOutOfRangeException(nameof(dimensions));
#endif

            if ((options & ~VectorField._supportedOptions) != FieldOptions.None)
                throw new ArgumentOutOfRangeException(nameof(options), "The VECTOR field contains unsupported options.");

            if (algorithm == VectorAlgorithm.SvsVamana
                && dataType != VectorDataType.Float16
                && dataType != VectorDataType.Float32)
            {
                throw new ArgumentException(
                    "SVS-VAMANA only supports FLOAT16 and FLOAT32.",
                    nameof(dataType));
            }

            this.Algorithm = algorithm;
            this.DataType = dataType;
            this.Dimensions = dimensions;
            this.DistanceMetric = distanceMetric;
            this._options = options;
        }

        private protected abstract void AppendAlgorithmArguments(ref readonly CommandPacket command);

        internal sealed override CommandPacket AppendToCommand(ref readonly CommandPacket command)
        {
            _ = command.WriteArguments(base.Name.Name);

            if (!string.IsNullOrEmpty(base.Name.Alias))
            {
                _ = command
                    .WriteArguments("AS")
                    .WriteArguments(base.Name.Alias);
            }

            _ = command.WriteArguments("VECTOR");

            string algorithm = this.Algorithm switch
            {
                VectorAlgorithm.Flat => "FLAT",
                VectorAlgorithm.Hnsw => "HNSW",
                VectorAlgorithm.SvsVamana => "SVS-VAMANA",
                _ => throw new ArgumentOutOfRangeException(nameof(this.Algorithm))
            };

            string dataType = this.DataType switch
            {
                VectorDataType.Float32 => "FLOAT32",
                VectorDataType.Float64 => "FLOAT64",
                VectorDataType.Float16 => "FLOAT16",
                VectorDataType.BFloat16 => "BFLOAT16",
                VectorDataType.Int8 => "INT8",
                VectorDataType.UInt8 => "UINT8",
                _ => throw new ArgumentOutOfRangeException(nameof(this.DataType))
            };

            string metric = this.DistanceMetric switch
            {
                VectorDistanceMetric.L2 => "L2",
                VectorDistanceMetric.IP => "IP",
                VectorDistanceMetric.Cosine => "COSINE",
                _ => throw new ArgumentOutOfRangeException(nameof(this.DistanceMetric))
            };

            // Required attributes: TYPE, DIM, DISTANCE_METRIC.
            // Each attribute occupies two RESP arguments.
            int argumentCount = 6 + this.AdditionalArgumentCount;

            _ = command
                .WriteArguments(algorithm)
                .WriteArguments(argumentCount.ToString(CultureInfo.InvariantCulture))
                .WriteArguments("TYPE")
                .WriteArguments(dataType)
                .WriteArguments("DIM")
                .WriteArguments(this.Dimensions.ToString(CultureInfo.InvariantCulture))
                .WriteArguments("DISTANCE_METRIC")
                .WriteArguments(metric);

            this.AppendAlgorithmArguments(in command);

            return command
                .WriteArguments(this.NoIndex, "NOINDEX")
                .WriteArguments(this.IndexMissing, "INDEXMISSING");
        }

        /// <summary>
        /// Vector indexing algorithms supported by Redis Search.
        /// <para>Redis Search 支持的向量索引算法</para>
        /// </summary>
        public enum VectorAlgorithm : byte
        {
            /// <summary>
            /// Exact nearest-neighbor search using a flat index.
            /// <para>FLAT 扁平索引，进行精确最近邻搜索</para>
            /// </summary>
            Flat = 0,

            /// <summary>
            /// Approximate nearest-neighbor search using HNSW.
            /// <para>HNSW 分层导航小世界图索引，进行近似最近邻搜索</para>
            /// </summary>
            Hnsw,

            /// <summary>
            /// Compressed approximate nearest-neighbor indexing, available since Redis 8.2.
            /// <para>SVS-VAMANA 压缩向量近似索引，支持 Redis 8.2+</para>
            /// </summary>
            SvsVamana,
        }

        /// <summary>
        /// Data types supported by Redis Search vector fields.
        /// <para>Redis Search 向量字段支持的数据类型</para>
        /// </summary>
        public enum VectorDataType : byte
        {
            /// <summary>
            /// 32-bit floating-point values.
            /// <para>32 位浮点数</para>
            /// </summary>
            Float32 = 0,

            /// <summary>
            /// 64-bit floating-point values.
            /// <para>64 位浮点数</para>
            /// </summary>
            Float64,

            /// <summary>
            /// 16-bit floating-point values, supported since RediSearch 2.10.
            /// <para>16 位浮点数，支持 RediSearch 2.10+</para>
            /// </summary>
            Float16,

            /// <summary>
            /// Brain floating-point 16-bit values, supported since RediSearch 2.10.
            /// <para>BFLOAT16 浮点数，支持 RediSearch 2.10+</para>
            /// </summary>
            BFloat16,

            /// <summary>
            /// Signed 8-bit integers, supported since Redis 8.0.
            /// <para>8 位有符号整数，支持 Redis 8.0+</para>
            /// </summary>
            Int8,

            /// <summary>
            /// Unsigned 8-bit integers, supported since Redis 8.0.
            /// <para>8 位无符号整数，支持 Redis 8.0+</para>
            /// </summary>
            UInt8,
        }

        /// <summary>
        /// Distance metrics for Redis Search vector similarity queries.
        /// <para>Redis Search 向量相似度查询的距离计算方式</para>
        /// </summary>
        public enum VectorDistanceMetric : byte
        {
            /// <summary>
            /// Euclidean distance.
            /// <para>欧几里得距离</para>
            /// </summary>
            L2 = 0,

            /// <summary>
            /// Inner product distance.
            /// <para>内积距离</para>
            /// </summary>
            IP,

            /// <summary>
            /// Cosine distance.
            /// <para>余弦距离</para>
            /// </summary>
            Cosine,
        }

        /// <summary>
        /// Compression algorithms for SVS-VAMANA vector indexes.
        /// <para>SVS-VAMANA 向量索引支持的压缩算法</para>
        /// </summary>
        public enum VectorCompression : byte
        {
            /// <summary>
            /// No explicit compression.
            /// <para>不指定压缩算法</para>
            /// </summary>
            None = 0,

            /// <summary>8-bit locally adaptive vector quantization.<para>8 位局部自适应向量量化</para></summary>
            LVQ8,

            /// <summary>4-bit locally adaptive vector quantization.<para>4 位局部自适应向量量化</para></summary>
            LVQ4,

            /// <summary>LVQ4x4 compression.<para>LVQ4x4 压缩</para></summary>
            LVQ4x4,

            /// <summary>LVQ4x8 compression.<para>LVQ4x8 压缩</para></summary>
            LVQ4x8,

            /// <summary>LeanVec4x8 compression.<para>LeanVec4x8 压缩</para></summary>
            LeanVec4x8,

            /// <summary>LeanVec8x8 compression.<para>LeanVec8x8 压缩</para></summary>
            LeanVec8x8,
        }
    }

    /// <summary>
    /// Represents a FLAT vector field for exact nearest-neighbor search.
    /// <para>表示使用 FLAT 算法进行精确最近邻搜索的向量字段</para>
    /// </summary>
    public sealed class FlatVectorField : VectorField
    {
        /// <summary>
        /// Gets the initial capacity of the vector index.
        /// <para>获取向量索引的初始容量，null 表示使用 Redis 默认值</para>
        /// </summary>
        public int? InitialCapacity { get; }

        /// <summary>
        /// Gets the vector index block size. The Redis default is 1024.
        /// <para>获取向量索引块大小，Redis 默认值为 1024</para>
        /// </summary>
        public int? BlockSize { get; }

        /// <summary>
        /// Gets the number of additional algorithm argument tokens.
        /// <para>获取算法专属参数的 Token 数量</para>
        /// </summary>
        protected override int AdditionalArgumentCount =>
            (this.InitialCapacity.HasValue ? 2 : 0) +
            (this.BlockSize.HasValue ? 2 : 0);

        /// <summary>
        /// Initializes a new instance of the FlatVectorField class.
        /// <para>初始化 FlatVectorField 精确向量索引字段的新实例</para>
        /// </summary>
        /// <param name="name">The field name and optional alias.<para>字段名称及可选别名</para></param>
        /// <param name="dataType">The vector element data type.<para>向量元素的数据类型</para></param>
        /// <param name="dimensions">The number of dimensions.<para>向量维度，必须大于零</para></param>
        /// <param name="distanceMetric">The vector distance metric.<para>向量距离计算方式</para></param>
        /// <param name="initialCapacity">Optional initial index capacity.<para>可选的索引初始容量，必须大于零</para></param>
        /// <param name="blockSize">Optional block size.<para>可选的索引块大小，必须大于零</para></param>
        /// <param name="options">Field configuration options.<para>字段配置选项，支持按位组合</para></param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The initial capacity or block size is invalid.
        /// <para>初始容量或块大小无效时抛出</para>
        /// </exception>
        public FlatVectorField(
            FieldName name,
            VectorDataType dataType,
            int dimensions,
            VectorDistanceMetric distanceMetric,
            int? initialCapacity = null,
            int? blockSize = null,
            FieldOptions options = FieldOptions.None
        ) : base(
            name: name,
            algorithm: VectorAlgorithm.Flat,
            dataType: dataType,
            dimensions: dimensions,
            distanceMetric: distanceMetric,
            options: options)
        {
            if (initialCapacity.HasValue && initialCapacity.Value <= 0) throw new ArgumentOutOfRangeException(nameof(initialCapacity));
            if (blockSize.HasValue && blockSize.Value <= 0) throw new ArgumentOutOfRangeException(nameof(blockSize));

            this.InitialCapacity = initialCapacity;
            this.BlockSize = blockSize;
        }

        /// <summary>
        /// Appends FLAT-specific attributes to the command.
        /// <para>将 FLAT 算法专属参数追加到命令中</para>
        /// </summary>
        private protected sealed override void AppendAlgorithmArguments(ref readonly CommandPacket command)
        {
            if (this.InitialCapacity.HasValue)
            {
                _ = command
                    .WriteArguments("INITIAL_CAP")
                    .WriteArguments(this.InitialCapacity.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (this.BlockSize.HasValue)
            {
                _ = command
                    .WriteArguments("BLOCK_SIZE")
                    .WriteArguments(this.BlockSize.Value.ToString(CultureInfo.InvariantCulture));
            }
        }
    }

    /// <summary>
    /// Represents an HNSW vector field for approximate nearest-neighbor search.
    /// <para>表示使用 HNSW 算法进行近似最近邻搜索的向量字段</para>
    /// </summary>
    public sealed class HnswVectorField : VectorField
    {
        /// <summary>
        /// Gets the initial capacity of the vector index.
        /// <para>获取向量索引的初始容量</para>
        /// </summary>
        public int? InitialCapacity { get; }

        /// <summary>
        /// Gets the maximum number of graph connections per node.
        /// The Redis default is 16.
        /// <para>获取图中每个节点的最大连接数参数 M，Redis 默认值为 16</para>
        /// </summary>
        public int? M { get; }

        /// <summary>
        /// Gets the maximum candidate count during graph construction.
        /// The Redis default is 200.
        /// <para>获取构建索引时的最大候选节点数量，Redis 默认值为 200</para>
        /// </summary>
        public int? EfConstruction { get; }

        /// <summary>
        /// Gets the maximum candidate count during KNN search.
        /// The Redis default is 10.
        /// <para>获取 KNN 搜索时的最大候选节点数量，Redis 默认值为 10</para>
        /// </summary>
        public int? EfRuntime { get; }

        /// <summary>
        /// Gets the relative range-search tolerance.
        /// The Redis default is 0.01.
        /// <para>获取向量范围查询的相对容差系数，Redis 默认值为 0.01</para>
        /// </summary>
        public double? Epsilon { get; }

        /// <summary>
        /// Gets the number of additional algorithm argument tokens.
        /// <para>获取算法专属参数的 Token 数量</para>
        /// </summary>
        protected override int AdditionalArgumentCount =>
            (this.InitialCapacity.HasValue ? 2 : 0) +
            (this.M.HasValue ? 2 : 0) +
            (this.EfConstruction.HasValue ? 2 : 0) +
            (this.EfRuntime.HasValue ? 2 : 0) +
            (this.Epsilon.HasValue ? 2 : 0);

        /// <summary>
        /// Initializes a new instance of the HnswVectorField class.
        /// <para>初始化 HnswVectorField 近似向量索引字段的新实例</para>
        /// </summary>
        /// <param name="name">Field name and optional alias.<para>字段名称及可选别名</para></param>
        /// <param name="dataType">Vector element data type.<para>向量元素数据类型</para></param>
        /// <param name="dimensions">Number of vector dimensions.<para>向量维度，必须大于零</para></param>
        /// <param name="distanceMetric">Vector distance metric.<para>向量距离计算方式</para></param>
        /// <param name="initialCapacity">Initial vector index capacity.<para>索引初始容量</para></param>
        /// <param name="m">Maximum graph connections per node.<para>图节点最大连接数参数 M</para></param>
        /// <param name="efConstruction">Construction candidate count.<para>构建索引时的候选数量</para></param>
        /// <param name="efRuntime">Search candidate count.<para>搜索时的候选数量</para></param>
        /// <param name="epsilon">Range search tolerance factor.<para>范围查询的相对容差系数</para></param>
        /// <param name="options">Field configuration options.<para>字段配置选项</para></param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A numeric parameter is invalid.
        /// <para>数值参数无效时抛出</para>
        /// </exception>
        public HnswVectorField(
            FieldName name,
            VectorDataType dataType,
            int dimensions,
            VectorDistanceMetric distanceMetric,
            int? initialCapacity = null,
            int? m = null,
            int? efConstruction = null,
            int? efRuntime = null,
            double? epsilon = null,
            FieldOptions options = FieldOptions.None
        ) : base(
            name: name,
            algorithm: VectorAlgorithm.Hnsw,
            dataType: dataType,
            dimensions: dimensions,
            distanceMetric: distanceMetric,
            options: options)
        {
            if (initialCapacity.HasValue && initialCapacity.Value <= 0) throw new ArgumentOutOfRangeException(nameof(initialCapacity));
            if (m.HasValue && m.Value <= 0) throw new ArgumentOutOfRangeException(nameof(m));
            if (efConstruction.HasValue && efConstruction.Value <= 0) throw new ArgumentOutOfRangeException(nameof(efConstruction));
            if (efRuntime.HasValue && efRuntime.Value <= 0) throw new ArgumentOutOfRangeException(nameof(efRuntime));

            if (epsilon.HasValue
                && (double.IsNaN(epsilon.Value)
                    || double.IsInfinity(epsilon.Value)
                    || epsilon.Value < 0))
            {
                throw new ArgumentOutOfRangeException(nameof(epsilon));
            }

            this.InitialCapacity = initialCapacity;
            this.M = m;
            this.EfConstruction = efConstruction;
            this.EfRuntime = efRuntime;
            this.Epsilon = epsilon;
        }

        /// <summary>
        /// Appends HNSW-specific attributes to the command.
        /// <para>将 HNSW 算法专属参数追加到命令中</para>
        /// </summary>
        private protected sealed override void AppendAlgorithmArguments(ref readonly CommandPacket command)
        {
            if (this.InitialCapacity.HasValue)
            {
                _ = command
                    .WriteArguments("INITIAL_CAP")
                    .WriteArguments(this.InitialCapacity.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (this.M.HasValue)
            {
                _ = command
                    .WriteArguments("M")
                    .WriteArguments(this.M.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (this.EfConstruction.HasValue)
            {
                _ = command
                    .WriteArguments("EF_CONSTRUCTION")
                    .WriteArguments(this.EfConstruction.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (this.EfRuntime.HasValue)
            {
                _ = command
                    .WriteArguments("EF_RUNTIME")
                    .WriteArguments(this.EfRuntime.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (this.Epsilon.HasValue)
            {
                _ = command
                    .WriteArguments("EPSILON")
                    .WriteArguments(this.Epsilon.Value.ToString("R", CultureInfo.InvariantCulture));
            }
        }
    }

    /// <summary>
    /// Represents an SVS-VAMANA vector field for approximate nearest-neighbor search.
    /// <para>表示使用 SVS-VAMANA 算法进行近似最近邻搜索的向量字段</para>
    /// <para>Available since Redis 8.2.</para>
    /// <para>支持此算法的 Redis 版本：8.2+</para>
    /// </summary>
    public sealed class SvsVamanaVectorField : VectorField
    {
        /// <summary>
        /// Gets the vector compression algorithm.
        /// <para>获取向量压缩算法，None 表示不指定压缩</para>
        /// </summary>
        public VectorCompression Compression { get; }

        /// <summary>
        /// Gets the graph construction window size.
        /// The Redis default is 200.
        /// <para>获取构建索引时的搜索窗口大小，Redis 默认值为 200</para>
        /// </summary>
        public int? ConstructionWindowSize { get; }

        /// <summary>
        /// Gets the maximum number of edges per graph node.
        /// The Redis default is 32.
        /// <para>获取图中每个节点的最大连接度，Redis 默认值为 32</para>
        /// </summary>
        public int? GraphMaxDegree { get; }

        /// <summary>
        /// Gets the search window size.
        /// The Redis default is 10.
        /// <para>获取搜索窗口大小，Redis 默认值为 10</para>
        /// </summary>
        public int? SearchWindowSize { get; }

        /// <summary>
        /// Gets the relative tolerance for vector range queries.
        /// The Redis default is 0.01.
        /// <para>获取向量范围查询的相对容差系数，Redis 默认值为 0.01</para>
        /// </summary>
        public double? Epsilon { get; }

        /// <summary>
        /// Gets the number of vectors required for compression training.
        /// Requires a compression algorithm.
        /// <para>获取压缩训练所需的向量数量，必须启用压缩算法</para>
        /// <para>When specified, the value must be less than 102400.</para>
        /// <para>显式指定时，参数值必须小于 102400</para>
        /// </summary>
        public int? TrainingThreshold { get; }

        /// <summary>
        /// Gets the reduced vector dimension for LeanVec compression.
        /// <para>获取 LeanVec 压缩的降维目标维度</para>
        /// <para>Only available with LeanVec4x8 or LeanVec8x8.</para>
        /// <para>仅适用于 LeanVec4x8 或 LeanVec8x8，且必须小于原始维度</para>
        /// </summary>
        public int? Reduce { get; }

        /// <summary>
        /// Gets the number of additional algorithm argument tokens.
        /// <para>获取算法专属参数的 Token 数量</para>
        /// </summary>
        protected override int AdditionalArgumentCount =>
            (this.Compression != VectorCompression.None ? 2 : 0) +
            (this.ConstructionWindowSize.HasValue ? 2 : 0) +
            (this.GraphMaxDegree.HasValue ? 2 : 0) +
            (this.SearchWindowSize.HasValue ? 2 : 0) +
            (this.Epsilon.HasValue ? 2 : 0) +
            (this.TrainingThreshold.HasValue ? 2 : 0) +
            (this.Reduce.HasValue ? 2 : 0);

        /// <summary>
        /// Initializes a new instance of the SvsVamanaVectorField class.
        /// <para>初始化 SvsVamanaVectorField 向量索引字段的新实例</para>
        /// </summary>
        /// <param name="name">Field name and optional alias.<para>字段名称及可选别名</para></param>
        /// <param name="dataType">Vector data type; FLOAT16 or FLOAT32.<para>向量元素类型，仅支持 FLOAT16 或 FLOAT32</para></param>
        /// <param name="dimensions">Vector dimensions.<para>向量维度，必须大于零</para></param>
        /// <param name="distanceMetric">Vector distance metric.<para>向量距离计算方式</para></param>
        /// <param name="compression">Compression algorithm.<para>向量压缩算法，默认为 None</para></param>
        /// <param name="constructionWindowSize">Graph construction window size.<para>建图窗口大小</para></param>
        /// <param name="graphMaxDegree">Maximum graph degree.<para>图节点最大连接度</para></param>
        /// <param name="searchWindowSize">Search window size.<para>搜索窗口大小</para></param>
        /// <param name="epsilon">Range search tolerance factor.<para>范围查询容差系数</para></param>
        /// <param name="trainingThreshold">Compression training threshold.<para>压缩训练阈值，仅适用于启用压缩时</para></param>
        /// <param name="reduce">Reduced dimension for LeanVec.<para>LeanVec 降维后的维度</para></param>
        /// <param name="options">Field configuration options.<para>字段配置选项</para></param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A numeric value or compression algorithm is invalid.
        /// <para>当数值参数或压缩算法无效时抛出</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Compression-related parameters are incompatible.
        /// <para>压缩相关参数不兼容时抛出</para>
        /// </exception>
        public SvsVamanaVectorField(
            FieldName name,
            VectorDataType dataType,
            int dimensions,
            VectorDistanceMetric distanceMetric,
            VectorCompression compression = VectorCompression.None,
            int? constructionWindowSize = null,
            int? graphMaxDegree = null,
            int? searchWindowSize = null,
            double? epsilon = null,
            int? trainingThreshold = null,
            int? reduce = null,
            FieldOptions options = FieldOptions.None
        ) : base(
            name: name,
            algorithm: VectorAlgorithm.SvsVamana,
            dataType: dataType,
            dimensions: dimensions,
            distanceMetric: distanceMetric,
            options: options)
        {
            if (compression > VectorCompression.LeanVec8x8) throw new ArgumentOutOfRangeException(nameof(compression));
            if (constructionWindowSize.HasValue && constructionWindowSize.Value <= 0) throw new ArgumentOutOfRangeException(nameof(constructionWindowSize));
            if (graphMaxDegree.HasValue && graphMaxDegree.Value <= 0) throw new ArgumentOutOfRangeException(nameof(graphMaxDegree));
            if (searchWindowSize.HasValue && searchWindowSize.Value <= 0) throw new ArgumentOutOfRangeException(nameof(searchWindowSize));

            if (epsilon.HasValue
                && (double.IsNaN(epsilon.Value)
                    || double.IsInfinity(epsilon.Value)
                    || epsilon.Value < 0))
            {
                throw new ArgumentOutOfRangeException(nameof(epsilon));
            }

            if (trainingThreshold.HasValue &&
                (trainingThreshold.Value <= 0 || trainingThreshold.Value >= 102400))
            {
                throw new ArgumentOutOfRangeException(nameof(trainingThreshold));
            }

            if (reduce.HasValue
                && (reduce.Value <= 0 || reduce.Value >= dimensions))
            {
                throw new ArgumentOutOfRangeException(nameof(reduce));
            }

            if (trainingThreshold.HasValue && compression == VectorCompression.None)
            {
                throw new ArgumentException("TRAINING_THRESHOLD requires COMPRESSION.", nameof(trainingThreshold));
            }

            if (reduce.HasValue
                && compression != VectorCompression.LeanVec4x8
                && compression != VectorCompression.LeanVec8x8)
            {
                throw new ArgumentException("REDUCE requires LeanVec4x8 or LeanVec8x8 compression.", nameof(reduce));
            }

            this.Compression = compression;
            this.ConstructionWindowSize = constructionWindowSize;
            this.GraphMaxDegree = graphMaxDegree;
            this.SearchWindowSize = searchWindowSize;
            this.Epsilon = epsilon;
            this.TrainingThreshold = trainingThreshold;
            this.Reduce = reduce;
        }

        /// <summary>
        /// Appends SVS-VAMANA-specific attributes to the command.
        /// <para>将 SVS-VAMANA 算法专属参数追加到命令中</para>
        /// </summary>
        private protected sealed override void AppendAlgorithmArguments(ref readonly CommandPacket command)
        {
            if (this.Compression != VectorCompression.None)
            {
                string compression = this.Compression switch
                {
                    VectorCompression.LVQ8 => "LVQ8",
                    VectorCompression.LVQ4 => "LVQ4",
                    VectorCompression.LVQ4x4 => "LVQ4x4",
                    VectorCompression.LVQ4x8 => "LVQ4x8",
                    VectorCompression.LeanVec4x8 => "LeanVec4x8",
                    VectorCompression.LeanVec8x8 => "LeanVec8x8",
                    _ => throw new ArgumentOutOfRangeException(nameof(this.Compression))
                };

                _ = command
                    .WriteArguments("COMPRESSION")
                    .WriteArguments(compression);
            }

            if (this.ConstructionWindowSize.HasValue)
            {
                _ = command
                    .WriteArguments("CONSTRUCTION_WINDOW_SIZE")
                    .WriteArguments(this.ConstructionWindowSize.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (this.GraphMaxDegree.HasValue)
            {
                _ = command
                    .WriteArguments("GRAPH_MAX_DEGREE")
                    .WriteArguments(this.GraphMaxDegree.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (this.SearchWindowSize.HasValue)
            {
                _ = command
                    .WriteArguments("SEARCH_WINDOW_SIZE")
                    .WriteArguments(this.SearchWindowSize.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (this.Epsilon.HasValue)
            {
                _ = command
                    .WriteArguments("EPSILON")
                    .WriteArguments(this.Epsilon.Value.ToString("R", CultureInfo.InvariantCulture));
            }

            if (this.TrainingThreshold.HasValue)
            {
                _ = command
                    .WriteArguments("TRAINING_THRESHOLD")
                    .WriteArguments(this.TrainingThreshold.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (this.Reduce.HasValue)
            {
                _ = command
                    .WriteArguments("REDUCE")
                    .WriteArguments(this.Reduce.Value.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
#endregion
}
