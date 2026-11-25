namespace SharpRedis.Enums;

internal enum RespType : sbyte
{
    PipelineOrTranEmpty = -2,

    Empty = -1,

    Unknown = 0,

    Exception = 1,

    /// <summary>
    /// ! Bulk errors
    /// </summary>
    BulkErrors = 33,

    /// <summary>
    /// + Simple strings
    /// </summary>
    SimpleString = 43,

    /// <summary>
    /// - Simple Errors
    /// </summary>
    SimpleErrors = 45,

    /// <summary>
    /// : Integers
    /// </summary>
    Integer = 58,

    /// <summary>
    /// $ Bulk strings
    /// </summary>
    BulkStrings = 36,

    /// <summary>
    /// * Array
    /// </summary>
    Array = 42,

    /// <summary>
    ///  _ null
    /// </summary>
    Null = 95,

    /// <summary>
    /// # boolean
    /// </summary>
    Boolean = 35,

    /// <summary>
    /// , double
    /// </summary>
    Double = 44,

    /// <summary>
    /// ( Big numbers
    /// </summary>
    BigNumber = 40,

    /// <summary>
    /// % Maps
    /// </summary>
    Maps = 37,

    /// <summary>
    /// ~ Sets
    /// </summary>
    Set = 126,

    /// <summary>
    /// = Verbatim strings
    /// </summary>
    VerbatimString = 61,

    /// <summary>
    /// > Pushes
    /// </summary>
    Push = 62,

    /// <summary>
    /// | Attributes
    /// </summary>
    Attributes = 124,
}
