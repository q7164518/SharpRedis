using SharpRedis.Entities.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace SharpRedis.Modules;

public sealed partial class RedisSearch
{
    /// <summary>
    /// Represents configuration options for the Redis Search FT.CREATE command.
    /// <para>表示 Redis Search FT.CREATE 命令的索引创建配置</para>
    /// </summary>
    public sealed class CreateIndexOptions
    {
        private const IndexOptions _supportedOptions =
            IndexOptions.MaxTextFields |
            IndexOptions.NoOffsets |
            IndexOptions.NoHl |
            IndexOptions.NoFields |
            IndexOptions.NoFreqs |
            IndexOptions.SkipInitialScan;

        private readonly IndexOptions _options;

        /// <summary>
        /// Gets the indexed document data type.
        /// <para>获取索引文档的数据类型，默认为 Hash</para>
        /// </summary>
        public IndexDataType DataType { get; }

        /// <summary>
        /// Gets the key prefixes used to select documents for indexing.
        /// Null uses the Redis default, which matches all keys.
        /// <para>获取需要建立索引的 Key 前缀集合</para>
        /// <para>null 表示不指定 PREFIX，默认匹配所有 Key</para>
        /// </summary>
#if MODERN_NET
        public ReadOnlyCollection<string>? Prefixes { get; }
#else
        public ReadOnlyCollection<string> Prefixes { get; }
#endif

        /// <summary>
        /// Gets the filter expression used to select documents for indexing.
        /// <para>获取用于筛选待索引文档的过滤表达式</para>
        /// </summary>
#if MODERN_NET
        public string? Filter { get; }
#else
        public string Filter { get; }
#endif

        /// <summary>
        /// Gets the default language used for text indexing.
        /// The Redis default is English.
        /// <para>获取文本索引的默认语言，Redis 默认为 English</para>
        /// </summary>
#if MODERN_NET
        public string? Language { get; }
#else
        public string Language { get; }
#endif

        /// <summary>
        /// Gets the document attribute containing the language identifier.
        /// <para>获取用于指定文档语言的字段名称</para>
        /// </summary>
#if MODERN_NET
        public string? LanguageField { get; }
#else
        public string LanguageField { get; }
#endif

        /// <summary>
        /// Gets the default document score.
        /// Valid values are between 0.0 and 1.0.
        /// <para>获取文档的默认评分，有效范围为 0.0 到 1.0</para>
        /// <para>null 表示不指定 SCORE，Redis 默认值为 1.0</para>
        /// </summary>
        public double? Score { get; }

        /// <summary>
        /// Gets the document attribute containing the score.
        /// <para>获取用于指定文档评分的字段名称</para>
        /// </summary>
#if MODERN_NET
        public string? ScoreField { get; }
#else
        public string ScoreField { get; }
#endif

        /// <summary>
        /// Gets the document attribute containing the payload.
        /// <para>获取用于指定文档 Payload 数据的字段名称</para>
        /// <para>PAYLOAD_FIELD is deprecated since RediSearch 2.0.</para>
        /// <para>PAYLOAD_FIELD 从 RediSearch 2.0 开始已弃用</para>
        /// </summary>
#if MODERN_NET
        public byte[]? PayloadField { get; }
#else
        public byte[] PayloadField { get; }
#endif

        /// <summary>
        /// Gets the temporary index idle expiration time in seconds.
        /// Null creates a persistent index.
        /// <para>获取临时索引的闲置过期秒数</para>
        /// <para>null 表示不指定 TEMPORARY，创建普通持久索引</para>
        /// </summary>
        public int? TemporarySeconds { get; }

        /// <summary>
        /// Gets the custom stopword list.
        /// Null uses the default list; an empty list disables stopwords.
        /// <para>获取自定义停用词集合</para>
        /// <para>null 使用 Redis 默认停用词，空集合表示禁用停用词</para>
        /// </summary>
#if MODERN_NET
        public ReadOnlyCollection<string>? StopWords { get; }
#else
        public ReadOnlyCollection<string> StopWords { get; }
#endif

        /// <summary>
        /// Gets the INDEXALL configuration.
        /// <para>获取 INDEXALL 通配符查询优化配置</para>
        /// </summary>
        public IndexAllMode IndexAll { get; }

        /// <summary>
        /// Gets the index configuration flags.
        /// <para>获取索引配置标志，支持通过 | 组合多个选项</para>
        /// </summary>
        public IndexOptions Options => this._options;

        /// <summary>
        /// Gets whether MAXTEXTFIELDS is enabled.
        /// <para>获取是否启用 MAXTEXTFIELDS，以支持扩展更多 TEXT 字段</para>
        /// </summary>
        public bool MaxTextFields => (this._options & IndexOptions.MaxTextFields) == IndexOptions.MaxTextFields;

        /// <summary>
        /// Gets whether storage of term offsets is disabled.
        /// <para>获取是否禁用词项偏移信息的存储</para>
        /// </summary>
        public bool NoOffsets => (this._options & IndexOptions.NoOffsets) == IndexOptions.NoOffsets;

        /// <summary>
        /// Gets whether NOHL is explicitly enabled.
        /// NOOFFSETS also implicitly disables highlighting.
        /// <para>获取是否明确启用 NOHL</para>
        /// <para>注意 NOOFFSETS 也会隐式禁用高亮功能</para>
        /// </summary>
        public bool NoHl => (this._options & IndexOptions.NoHl) == IndexOptions.NoHl;

        /// <summary>
        /// Gets whether storage of field information is disabled.
        /// <para>获取是否禁用词项所属字段信息的存储</para>
        /// </summary>
        public bool NoFields => (this._options & IndexOptions.NoFields) == IndexOptions.NoFields;

        /// <summary>
        /// Gets whether storage of term frequencies is disabled.
        /// <para>获取是否禁用词项频率信息的存储</para>
        /// </summary>
        public bool NoFreqs => (this._options & IndexOptions.NoFreqs) == IndexOptions.NoFreqs;

        /// <summary>
        /// Gets whether the initial document scan is skipped.
        /// <para>获取创建索引时是否跳过对现有文档的初始扫描</para>
        /// </summary>
        public bool SkipInitialScan => (this._options & IndexOptions.SkipInitialScan) == IndexOptions.SkipInitialScan;

        /// <summary>
        /// Initializes a new instance of the CreateIndexOptions class.
        /// <para>初始化 CreateIndexOptions 索引配置类的新实例</para>
        /// </summary>
        /// <param name="dataType">Document data type.<para>文档数据类型，默认为 Hash</para></param>
        /// <param name="prefixes">Document key prefixes.<para>需要建立索引的 Key 前缀集合</para></param>
        /// <param name="filter">Index filter expression.<para>索引过滤表达式</para></param>
        /// <param name="language">Default indexing language.<para>默认索引语言</para></param>
        /// <param name="languageField">Document language attribute.<para>文档语言字段名称</para></param>
        /// <param name="score">Default document score, from 0.0 to 1.0.<para>默认文档评分，范围为 0.0 到 1.0</para></param>
        /// <param name="scoreField">Document score attribute.<para>文档评分字段名称</para></param>
        /// <param name="payloadField">Document payload attribute.<para>文档 Payload 字段名称</para></param>
        /// <param name="temporarySeconds">Temporary index idle expiration in seconds.<para>临时索引闲置过期秒数</para></param>
        /// <param name="stopWords">Custom stopwords; an empty collection disables stopwords.<para>自定义停用词，空集合表示禁用停用词</para></param>
        /// <param name="indexAll">Wildcard query index optimization mode.<para>INDEXALL 通配符查询优化模式</para></param>
        /// <param name="options">Bitwise index configuration options.<para>支持按位组合的索引配置选项</para></param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A numeric value, enum value, or option flag is invalid.
        /// <para>数值、枚举值或选项标志无效时抛出</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// A string or collection argument is invalid.
        /// <para>字符串或集合参数无效时抛出</para>
        /// </exception>
        public CreateIndexOptions(
            IndexDataType dataType = IndexDataType.Hash,
#if MODERN_NET
            IEnumerable<string>? prefixes = null,
            string? filter = null,
            string? language = null,
            string? languageField = null,
#else
            IEnumerable<string> prefixes = null,
            string filter = null,
            string language = null,
            string languageField = null,
#endif
            double? score = null,
#if MODERN_NET
            string? scoreField = null,
            byte[]? payloadField = null,
#else
            string scoreField = null,
            byte[] payloadField = null,
#endif
            int? temporarySeconds = null,
#if MODERN_NET
            IEnumerable<string>? stopWords = null,
#else
            IEnumerable<string> stopWords = null,
#endif
            IndexAllMode indexAll = IndexAllMode.Default,
            IndexOptions options = IndexOptions.None
        )
        {
            if (dataType > IndexDataType.Json) throw new ArgumentOutOfRangeException(nameof(dataType));
            if (indexAll > IndexAllMode.Disable) throw new ArgumentOutOfRangeException(nameof(indexAll));
            if ((options & ~CreateIndexOptions._supportedOptions) != IndexOptions.None)
            {
                throw new ArgumentOutOfRangeException(nameof(options), "The index contains unsupported options.");
            }

            if (score.HasValue
                && (double.IsNaN(score.Value)
                    || double.IsInfinity(score.Value)
                    || score.Value < 0
                    || score.Value > 1))
            {
                throw new ArgumentOutOfRangeException(nameof(score), "The index score must be between 0.0 and 1.0.");
            }

            if (temporarySeconds.HasValue && temporarySeconds.Value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(temporarySeconds), "The temporary index expiration must be greater than zero.");
            }

            if (filter is not null && string.IsNullOrWhiteSpace(filter)) throw new ArgumentException("The filter cannot be empty.", nameof(filter));
            if (language is not null && string.IsNullOrWhiteSpace(language)) throw new ArgumentException("The language cannot be empty.", nameof(language));
            if (languageField is not null && string.IsNullOrWhiteSpace(languageField)) throw new ArgumentException("The language field cannot be empty.", nameof(languageField));
            if (scoreField is not null && string.IsNullOrWhiteSpace(scoreField)) throw new ArgumentException("The score field cannot be empty.", nameof(scoreField));
            if (payloadField is not null && payloadField.Length is 0) throw new ArgumentException("The payload field cannot be empty.", nameof(payloadField));
            if (prefixes is not null)
            {
                this.Prefixes = CreateIndexOptions.CopyValues(prefixes, nameof(prefixes), false, true);
            }

            if (stopWords is not null)
            {
                this.StopWords = CreateIndexOptions.CopyValues(stopWords, nameof(stopWords), true, false);
            }

            this.DataType = dataType;
            this.Filter = filter;
            this.Language = language;
            this.LanguageField = languageField;
            this.Score = score;
            this.ScoreField = scoreField;
            this.PayloadField = payloadField;
            this.TemporarySeconds = temporarySeconds;
            this.IndexAll = indexAll;
            this._options = options;
        }

        /// <summary>
        /// Copies and validates a collection of string arguments.
        /// <para>复制并校验字符串参数集合</para>
        /// </summary>
        private static ReadOnlyCollection<string> CopyValues(
            IEnumerable<string> source,
            string parameterName,
            bool allowEmptyCollection,
            bool allowEmptyValue)
        {
            var values = new List<string>();

            foreach (var value in source)
            {
                if (value is null || (!allowEmptyValue && value.Length == 0))
                {
                    throw new ArgumentException("The collection contains an invalid string.", parameterName);
                }
                values.Add(value);
            }

            if (!allowEmptyCollection && values.Count == 0)
            {
                throw new ArgumentException("The collection must contain at least one value.", parameterName);
            }

            return values.AsReadOnly();
        }

        internal CommandPacket AppendToCommand(ref readonly CommandPacket command)
        {
            // ON HASH is the Redis default.
            if (this.DataType == IndexDataType.Json)
            {
                _ = command
                    .WriteArguments("ON")
                    .WriteArguments("JSON");
            }

            if (this.Prefixes is not null)
            {
                _ = command
                    .WriteArguments("PREFIX")
                    .WriteArguments(this.Prefixes.Count.ToString(CultureInfo.InvariantCulture));

                for (int i = 0; i < this.Prefixes.Count; i++)
                {
                    _ = command.WriteArguments(this.Prefixes[i]);
                }
            }

            if (this.Filter is not null)
            {
                _ = command
                    .WriteArguments("FILTER")
                    .WriteArguments(this.Filter);
            }

            if (this.Language is not null)
            {
                _ = command
                    .WriteArguments("LANGUAGE")
                    .WriteArguments(this.Language);
            }

            if (this.LanguageField is not null)
            {
                _ = command
                    .WriteArguments("LANGUAGE_FIELD")
                    .WriteArguments(this.LanguageField);
            }

            if (this.Score.HasValue)
            {
                _ = command
                    .WriteArguments("SCORE")
                    .WriteArguments(this.Score.Value.ToString("R", CultureInfo.InvariantCulture));
            }

            if (this.ScoreField is not null)
            {
                _ = command
                    .WriteArguments("SCORE_FIELD")
                    .WriteArguments(this.ScoreField);
            }

            if (this.PayloadField is not null)
            {
                _ = command
                    .WriteArguments("PAYLOAD_FIELD")
                    .WriteArguments(this.PayloadField);
            }

            _ = command.WriteArguments(this.MaxTextFields, "MAXTEXTFIELDS");

            if (this.TemporarySeconds.HasValue)
            {
                _ = command
                    .WriteArguments("TEMPORARY")
                    .WriteArguments(this.TemporarySeconds.Value.ToString(CultureInfo.InvariantCulture));
            }

            _ = command
                .WriteArguments(this.NoOffsets, "NOOFFSETS")
                .WriteArguments(this.NoHl, "NOHL")
                .WriteArguments(this.NoFields, "NOFIELDS")
                .WriteArguments(this.NoFreqs, "NOFREQS");

            if (this.StopWords is not null)
            {
                _ = command
                    .WriteArguments("STOPWORDS")
                    .WriteArguments(this.StopWords.Count.ToString(CultureInfo.InvariantCulture));

                for (int i = 0; i < this.StopWords.Count; i++)
                {
                    _ = command.WriteArguments(this.StopWords[i]);
                }
            }

            _ = command.WriteArguments(this.SkipInitialScan, "SKIPINITIALSCAN");

            switch (this.IndexAll)
            {
                case IndexAllMode.Enable:
                    _ = command
                        .WriteArguments("INDEXALL")
                        .WriteArguments("ENABLE");
                    break;

                case IndexAllMode.Disable:
                    _ = command
                        .WriteArguments("INDEXALL")
                        .WriteArguments("DISABLE");
                    break;
            }

            return command;
        }
    }

    /// <summary>
    /// Options for the Redis Search FT.CREATE command.
    /// Multiple options can be combined using bitwise OR.
    /// <para>Redis Search FT.CREATE 命令的索引配置选项，支持通过 | 组合多个选项</para>
    /// <para>这些选项作用于整个索引，而不是单个 Schema 字段</para>
    /// </summary>
    [Flags]
    public enum IndexOptions : byte
    {
        /// <summary>
        /// No additional index options.
        /// <para>不启用任何额外索引选项</para>
        /// </summary>
        None = 0,

        /// <summary>
        /// Enables an index encoding that supports adding more than 32 TEXT fields.
        /// <para>启用支持超过 32 个 TEXT 字段的索引编码方式，便于后续通过 FT.ALTER 增加字段</para>
        /// </summary>
        MaxTextFields = 1 << 0,

        /// <summary>
        /// Disables storage of term offsets, reducing memory usage.
        /// Also disables highlighting and implies NOHL.
        /// <para>不保存词项偏移信息，减少内存占用，同时禁用高亮功能，并隐含 NOHL</para>
        /// </summary>
        NoOffsets = 1 << 1,

        /// <summary>
        /// Disables highlighting support to reduce index memory usage.
        /// <para>禁用搜索结果高亮功能，以减少索引内存占用</para>
        /// </summary>
        NoHl = 1 << 2,

        /// <summary>
        /// Disables storage of field information for indexed terms.
        /// Field-specific text searches will not be supported.
        /// <para>不保存词项所属字段的信息，减少内存占用，但不支持针对特定 TEXT 字段的搜索</para>
        /// </summary>
        NoFields = 1 << 3,

        /// <summary>
        /// Disables storage of term frequencies.
        /// <para>不保存词项出现频率，减少内存占用，但会影响依赖词频的相关性评分</para>
        /// </summary>
        NoFreqs = 1 << 4,

        /// <summary>
        /// Skips the initial scan of existing documents when creating the index.
        /// <para>创建索引时跳过对现有文档的初始扫描，仅在后续文档更新时进行索引</para>
        /// </summary>
        SkipInitialScan = 1 << 5,
    }

    /// <summary>
    /// Data types supported by Redis Search indexes.
    /// <para>Redis Search 索引支持的数据类型</para>
    /// </summary>
    public enum IndexDataType : byte
    {
        /// <summary>
        /// Indexes Redis Hash documents. This is the default.
        /// <para>对 Redis Hash 文档建立索引，Redis 默认类型</para>
        /// </summary>
        Hash = 0,

        /// <summary>
        /// Indexes Redis JSON documents.
        /// <para>对 Redis JSON 文档建立索引，需要服务器支持 RedisJSON</para>
        /// </summary>
        Json,
    }

    /// <summary>
    /// Controls the INDEXALL option of FT.CREATE.
    /// <para>控制 FT.CREATE 命令的 INDEXALL 配置</para>
    /// </summary>
    public enum IndexAllMode : byte
    {
        /// <summary>
        /// Does not specify INDEXALL. Uses the Redis default behavior.
        /// <para>不指定 INDEXALL，使用 Redis 默认行为</para>
        /// </summary>
        Default = 0,

        /// <summary>
        /// Enables document ID indexing to optimize wildcard queries.
        /// <para>启用文档 ID 索引，优化 FT.SEARCH 中的通配符查询</para>
        /// <para>Available since Redis 8.0.</para>
        /// <para>支持 Redis 8.0+</para>
        /// </summary>
        Enable,

        /// <summary>
        /// Explicitly disables document ID indexing for wildcard queries.
        /// <para>明确禁用通配符查询的文档 ID 索引优化</para>
        /// <para>Available since Redis 8.0.</para>
        /// <para>支持 Redis 8.0+</para>
        /// </summary>
        Disable,
    }
}
