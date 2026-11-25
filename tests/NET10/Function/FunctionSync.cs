using SharpRedis;
using SharpRedis.Exceptions;

namespace NET10.Function;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class FunctionSync(Redis redis)
{
    private readonly Redis _redis = redis;

    #region FUNCTION LOAD   FCALL
    [Test]
    public void FunctionLoad()
    {
        const string functionCode = """
            #!lua name=sharpredis_test_load
            redis.register_function('sharpredis_test_load_hello', function(keys, args)
                return 'hello ' .. args[1]
            end)
            """;

        var result = this._redis.Func.FunctionLoad(functionCode, replace: true);

        Assert.That(result, Is.EqualTo("sharpredis_test_load"));
    }

    [Test]
    public void FCall_ReadOnlySpan()
    {
        const string functionCode = """
            #!lua name=sharpredis_test_fcall_span
            redis.register_function('sharpredis_test_fcall_span_hello', function(keys, args)
                return 'hello ' .. args[1]
            end)
            """;

        this._redis.Func.FunctionLoad(functionCode, replace: true);

        var result = this._redis.Func.FCall(
            "sharpredis_test_fcall_span_hello",
            [],
            ["SharpRedis"]);

        Assert.That(result.ToString(), Is.EqualTo("hello SharpRedis"));
    }

    [Test]
    public void FCall_ICollection()
    {
        const string functionCode = """
            #!lua name=sharpredis_test_fcall_collection
            redis.register_function('sharpredis_test_fcall_collection_hello', function(keys, args)
                return 'hello ' .. args[1]
            end)
            """;

        this._redis.Func.FunctionLoad(functionCode, replace: true);

        var keys = new List<string>();
        var result = this._redis.Func.FCall(
            "sharpredis_test_fcall_collection_hello",
            keys,
            ["SharpRedis"]);
        Assert.That(result.ToString(), Is.EqualTo("hello SharpRedis"));

        keys = null!;
        result = this._redis.Func.FCall(
            "sharpredis_test_fcall_collection_hello",
            keys,
            ["SharpRedis"]);
        Assert.That(result.ToString(), Is.EqualTo("hello SharpRedis"));
    }

    [Test]
    public void FunctionLoad_Replace()
    {
        const string functionCode1 = """
            #!lua name=sharpredis_test_replace
            redis.register_function('sharpredis_test_replace_hello', function(keys, args)
                return 'hello'
            end)
            """;

        const string functionCode2 = """
            #!lua name=sharpredis_test_replace
            redis.register_function('sharpredis_test_replace_hello', function(keys, args)
                return 'world'
            end)
            """;

        this._redis.Func.FunctionLoad(functionCode1, replace: true);
        this._redis.Func.FunctionLoad(functionCode2, replace: true);

        var result = this._redis.Func.FCall(
            "sharpredis_test_replace_hello");

        Assert.That(result.ToString(), Is.EqualTo("world"));
    }

    [Test]
    public void FunctionLoad_DuplicateLibrary()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_duplicate
        redis.register_function('sharpredis_test_duplicate_hello', function(keys, args)
            return 'hello'
        end)
        """;

        this._redis.Func.FunctionLoad(functionCode, replace: true);

        Assert.That(
            () => this._redis.Func.FunctionLoad(functionCode),
            Throws.TypeOf<RedisErrorException>()
                .And.Message.Contain("already exists")
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FUNCTION LOAD"));
    }

    [Test]
    public void FCall_FunctionNotFound()
    {
        Assert.That(
            () => this._redis.Func.FCall(
                "sharpredis_test_function_not_found"),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FCALL"));
    }

    [Test]
    public void FCall_FunctionExecutionError()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_execution_error
        redis.register_function('sharpredis_test_execution_error_throw', function(keys, args)
            error('SharpRedis function execution error')
        end)
        """;

        this._redis.Func.FunctionLoad(functionCode, replace: true);

        Assert.That(
            () => this._redis.Func.FCall(
                "sharpredis_test_execution_error_throw"),
            Throws.TypeOf<RedisErrorException>()
                .And.Message.Contain("SharpRedis function execution error")
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FCALL"));
    }

    [Test]
    public void FCall_InvalidArgument()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_invalid_argument
        redis.register_function('sharpredis_test_invalid_argument_number', function(keys, args)
            return tonumber(args[1]) + 1
        end)
        """;

        this._redis.Func.FunctionLoad(functionCode, replace: true);

        Assert.That(
            () => this._redis.Func.FCall(
                "sharpredis_test_invalid_argument_number",
                ReadOnlySpan<string>.Empty,
                ["not-a-number"]),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FCALL"));
    }

    [Test]
    public void FCall_WriteKey()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_write_key
        redis.register_function('sharpredis_test_write_key_set', function(keys, args)
            redis.call('SET', keys[1], args[1])
            return redis.call('GET', keys[1])
        end)
        """;

        this._redis.Func.FunctionLoad(functionCode, replace: true);

        const string key = "Function:{FCall_WriteKey}:Key";

        var result = this._redis.Func.FCall(
            "sharpredis_test_write_key_set",
            new List<string> { key },
            ["SharpRedis"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.ToString(), Is.EqualTo("SharpRedis"));
            Assert.That(this._redis.String.Get(key).ToString(), Is.EqualTo("SharpRedis"));
        }
    }

    [Test]
    public void FCall_ReadAndIncrementKey()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_read_increment
        redis.register_function('sharpredis_test_read_increment_value', function(keys, args)
            local oldValue = redis.call('GET', keys[1])
            local newValue = redis.call('INCRBY', keys[1], args[1])
            return {oldValue, newValue}
        end)
        """;

        this._redis.Func.FunctionLoad(functionCode, replace: true);

        const string key = "Function:{FCall_ReadAndIncrementKey}:Key";

        this._redis.String.Set(key, "10");

        var result = this._redis.Func.FCall(
            "sharpredis_test_read_increment_value",
            [key],
            ["5"]);

        var values = result.RawValue as RedisResult[];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(values, Is.Not.Null);
            Assert.That(values, Has.Length.EqualTo(2));
            Assert.That(values![0].ToString(), Is.EqualTo("10"));
            Assert.That(values[1].ToString(), Is.EqualTo("15"));
            Assert.That(this._redis.String.Get(key).ToString(), Is.EqualTo("15"));
        }
    }
    #endregion

    #region FCall_Ro
    [Test]
    public void FCall_Ro_ReadOnlySpan()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_fcall_ro_span
        redis.register_function{
            function_name = 'sharpredis_test_fcall_ro_span_get',
            callback = function(keys, args)
                return redis.call('GET', keys[1])
            end,
            flags = {'no-writes'}
        }
        """;

        this._redis.Func.FunctionLoad(functionCode, replace: true);

        const string key = "Function:{FCall_Ro_Span}:Key";
        this._redis.String.Set(key, "hello");

        var result = this._redis.Func.FCall_Ro(
            "sharpredis_test_fcall_ro_span_get",
            [key]);

        Assert.That(result.ToString(), Is.EqualTo("hello"));
    }

    [Test]
    public void FCall_Ro_ICollection()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_fcall_ro_collection
        redis.register_function{
            function_name = 'sharpredis_test_fcall_ro_collection_get',
            callback = function(keys, args)
                return {
                    redis.call('GET', keys[1]),
                    redis.call('GET', keys[2]),
                    args[1]
                }
            end,
            flags = {'no-writes'}
        }
        """;

        this._redis.Func.FunctionLoad(functionCode, replace: true);

        const string key1 = "Function:{FCall_Ro_Collection}:Key1";
        const string key2 = "Function:{FCall_Ro_Collection}:Key2";

        this._redis.String.Set(key1, "value1");
        this._redis.String.Set(key2, "value2");

        var keys = new List<string>
        {
            key1,
            key2
        };

        var result = this._redis.Func.FCall_Ro(
            "sharpredis_test_fcall_ro_collection_get",
            keys,
            ["hello"]);

        var values = result.RawValue as RedisResult[];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(values, Is.Not.Null);
            Assert.That(values, Has.Length.EqualTo(3));
            Assert.That(values![0].ToString(), Is.EqualTo("value1"));
            Assert.That(values[1].ToString(), Is.EqualTo("value2"));
            Assert.That(values[2].ToString(), Is.EqualTo("hello"));
        }
    }

    [Test]
    public void FCall_Ro_WithoutKeys()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_fcall_ro_no_keys
        redis.register_function{
            function_name = 'sharpredis_test_fcall_ro_no_keys_echo',
            callback = function(keys, args)
                return args[1]
            end,
            flags = {'no-writes'}
        }
        """;

        this._redis.Func.FunctionLoad(functionCode, replace: true);

        var result = this._redis.Func.FCall_Ro(
            "sharpredis_test_fcall_ro_no_keys_echo",
            args: ["SharpRedis"]);

        Assert.That(result.ToString(), Is.EqualTo("SharpRedis"));
    }

    [Test]
    public void FCall_Ro_FunctionNotFound()
    {
        Assert.That(
            () => this._redis.Func.FCall_Ro(
                "sharpredis_test_function_not_found"),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FCALL_RO"));
    }

    [Test]
    public void FCall_Ro_WriteCommandRejected()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_fcall_ro_write
        redis.register_function{
            function_name = 'sharpredis_test_fcall_ro_write_set',
            callback = function(keys, args)
                return redis.call('SET', keys[1], args[1])
            end,
            flags = {'no-writes'}
        }
        """;

        this._redis.Func.FunctionLoad(functionCode, replace: true);

        const string key = "Function:{FCall_Ro_Write}:Key";
        this._redis.String.Set(key, "original");

        Assert.That(
            () => this._redis.Func.FCall_Ro(
                "sharpredis_test_fcall_ro_write_set",
                [key],
                ["modified"]),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FCALL_RO"));

        Assert.That(this._redis.String.Get(key).ToString(), Is.EqualTo("original"));
    }

    [Test]
    public void FCall_Ro_FunctionWithoutNoWritesFlag()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_fcall_ro_flag
        redis.register_function(
            'sharpredis_test_fcall_ro_flag_get',
            function(keys, args)
                return redis.call('GET', keys[1])
            end
        )
        """;

        this._redis.Func.FunctionLoad(functionCode, replace: true);

        const string key = "Function:{FCall_Ro_Flag}:Key";
        this._redis.String.Set(key, "hello");

        Assert.That(
            () => this._redis.Func.FCall_Ro(
                "sharpredis_test_fcall_ro_flag_get",
                [key]),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FCALL_RO"));
    }
    #endregion

    #region FunctionDelete
    [Test]
    public void FunctionDelete()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_delete
        redis.register_function('sharpredis_test_delete_hello', function(keys, args)
            return 'hello'
        end)
        """;

        this._redis.Func.FunctionLoad(functionCode, replace: true);

        var result = this._redis.Func.FCall(
            "sharpredis_test_delete_hello");

        Assert.That(result.ToString(), Is.EqualTo("hello"));

        var deleted = this._redis.Func.FunctionDelete(
            "sharpredis_test_delete");
        Assert.That(deleted, Is.True);

        Assert.That(
            () => this._redis.Func.FCall("sharpredis_test_delete_hello"),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FCALL"));
    }

    [Test]
    public void FunctionDelete_LibraryNotFound()
    {
        Assert.That(
            () => this._redis.Func.FunctionDelete(
                "sharpredis_test_delete_not_found"),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FUNCTION DELETE"));
    }
    #endregion

    #region FUNCTION DUMP
    [Test]
    public void FunctionDump()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_dump
        redis.register_function('sharpredis_test_dump_hello', function(keys, args)
            return 'hello ' .. args[1]
        end)
        """;

        this._redis.Func.FunctionFlush();
        this._redis.Func.FunctionLoad(functionCode, replace: true);

        var dump = this._redis.Func.FunctionDump();

        Assert.That(dump, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(dump, Is.Not.Empty);

            Assert.That(
                this._redis.Func.FunctionDelete("sharpredis_test_dump"),
                Is.True);

            Assert.That(this._redis.Func.FunctionRestore(dump), Is.True);
        }

        var result = this._redis.Func.FCall(
            "sharpredis_test_dump_hello",
            args: ["SharpRedis"]);

        Assert.That(result.ToString(), Is.EqualTo("hello SharpRedis"));
    }
    #endregion

    #region FUNCTION RESTORE
    [Test]
    public void FunctionRestore_DuplicateLibrary()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_restore_duplicate
        redis.register_function('sharpredis_test_restore_duplicate_hello', function(keys, args)
            return 'hello'
        end)
        """;

        this._redis.Func.FunctionLoad(functionCode, replace: true);

        var dump = this._redis.Func.FunctionDump();

        Assert.That(
            () => this._redis.Func.FunctionRestore(dump),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FUNCTION RESTORE"));
    }

    [Test]
    public void FunctionRestore_Replace()
    {
        const string functionCode1 = """
        #!lua name=sharpredis_test_restore_replace
        redis.register_function('sharpredis_test_restore_replace_hello', function(keys, args)
            return 'old'
        end)
        """;

        const string functionCode2 = """
        #!lua name=sharpredis_test_restore_replace
        redis.register_function('sharpredis_test_restore_replace_hello', function(keys, args)
            return 'new'
        end)
        """;

        this._redis.Func.FunctionLoad(functionCode1, replace: true);

        var dump = this._redis.Func.FunctionDump();

        this._redis.Func.FunctionLoad(functionCode2, replace: true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                    this._redis.Func.FCall("sharpredis_test_restore_replace_hello").ToString(),
                    Is.EqualTo("new"));

            Assert.That(
                this._redis.Func.FunctionRestore(dump, RedisFunction.RestoreHandling.Replace),
                Is.True);

            Assert.That(
                this._redis.Func.FCall("sharpredis_test_restore_replace_hello").ToString(),
                Is.EqualTo("old"));
        }
    }

    [Test]
    public void FunctionRestore_Flush()
    {
        const string functionCode1 = """
        #!lua name=sharpredis_test_restore_flush_a
        redis.register_function('sharpredis_test_restore_flush_a_hello', function(keys, args)
            return 'library-a'
        end)
        """;

        const string functionCode2 = """
        #!lua name=sharpredis_test_restore_flush_b
        redis.register_function('sharpredis_test_restore_flush_b_hello', function(keys, args)
            return 'library-b'
        end)
        """;

        Assert.That(
            this._redis.Func.FunctionFlush(AsyncSync.Sync),
            Is.True);

        this._redis.Func.FunctionLoad(functionCode1, replace: true);

        var dump = this._redis.Func.FunctionDump();

        this._redis.Func.FunctionLoad(functionCode2, replace: true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                    this._redis.Func.FCall("sharpredis_test_restore_flush_b_hello").ToString(),
                    Is.EqualTo("library-b"));

            Assert.That(
                this._redis.Func.FunctionRestore(dump, RedisFunction.RestoreHandling.Flush),
                Is.True);

            Assert.That(
                this._redis.Func.FCall("sharpredis_test_restore_flush_a_hello").ToString(),
                Is.EqualTo("library-a"));

            Assert.That(
                () => this._redis.Func.FCall("sharpredis_test_restore_flush_b_hello"),
                Throws.TypeOf<RedisErrorException>()
                    .And.Property(nameof(RedisErrorException.Command))
                    .EqualTo("FCALL"));
        }
    }

    [Test]
    public void FunctionRestore_InvalidData()
    {
        byte[] invalidData = [1, 2, 3, 4, 5, 6];

        Assert.That(
            () => this._redis.Func.FunctionRestore(invalidData),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FUNCTION RESTORE"));
    }
    #endregion

    #region FUNCTION FLUSH
    private void LoadFlushTestLibrary(string libraryName, string functionName)
    {
        var functionCode = $"""
        #!lua name={libraryName}
        redis.register_function('{functionName}', function(keys, args)
            return '{functionName}'
        end)
        """;

        this._redis.Func.FunctionLoad(functionCode, replace: true);
    }

    [Test]
    public void FunctionFlush()
    {
        this.LoadFlushTestLibrary(
            "sharpredis_test_flush_default",
            "sharpredis_test_flush_default_hello");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                    this._redis.Func.FCall("sharpredis_test_flush_default_hello").ToString(),
                    Is.EqualTo("sharpredis_test_flush_default_hello"));

            Assert.That(this._redis.Func.FunctionFlush(), Is.True);

            Assert.That(
                () => this._redis.Func.FCall("sharpredis_test_flush_default_hello"),
                Throws.TypeOf<RedisErrorException>()
                    .And.Property(nameof(RedisErrorException.Command))
                    .EqualTo("FCALL"));
        }
    }

    [Test]
    public void FunctionFlush_Sync()
    {
        this.LoadFlushTestLibrary(
            "sharpredis_test_flush_sync",
            "sharpredis_test_flush_sync_hello");

        Assert.That(
            this._redis.Func.FunctionFlush(AsyncSync.Sync),
            Is.True);

        Assert.That(
            () => this._redis.Func.FCall("sharpredis_test_flush_sync_hello"),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FCALL"));
    }

    [Test]
    public void FunctionFlush_Async()
    {
        this.LoadFlushTestLibrary(
            "sharpredis_test_flush_async",
            "sharpredis_test_flush_async_hello");

        Assert.That(
            this._redis.Func.FunctionFlush(AsyncSync.Async),
            Is.True);

        Assert.That(
            () => this._redis.Func.FCall("sharpredis_test_flush_async_hello"),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FCALL"));
    }
    #endregion

    #region FunctionList
    [Test]
    public void FunctionList()
    {
        const string libraryName = "sharpredis_test_list_basic";
        const string functionName = "sharpredis_test_list_basic_hello";

        var functionCode = $$"""
        #!lua name={{libraryName}}
        redis.register_function{
            function_name = '{{functionName}}',
            callback = function(keys, args)
                return 'hello ' .. args[1]
            end,
            description = 'SharpRedis FunctionList test',
            flags = {'no-writes'}
        }
        """;

        this._redis.Func.FunctionLoad(functionCode, replace: true);

        var libraries = this._redis.Func.FunctionList();

        Assert.That(libraries, Is.Not.Null);

        var library = Array.Find(
            libraries!,
            item => item.LibraryName == libraryName);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(library.LibraryName, Is.EqualTo(libraryName));
            Assert.That(library.Engine, Is.EqualTo("LUA"));
            Assert.That(library.Functions, Has.Length.EqualTo(1));
        }
        Assert.That(library.Functions[0].Name, Is.EqualTo(functionName));
    }

    [Test]
    public void FunctionList_LibraryNamePattern()
    {
        const string libraryName = "sharpredis_test_list_pattern";
        const string functionName = "sharpredis_test_list_pattern_hello";

        var functionCode = $$"""
        #!lua name={{libraryName}}
        redis.register_function('{{functionName}}', function(keys, args)
            return 'hello'
        end)
        """;

        this._redis.Func.FunctionLoad(functionCode, replace: true);

        var libraries = this._redis.Func.FunctionList(libraryName);

        Assert.That(libraries, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(libraries, Has.Length.EqualTo(1));
            Assert.That(libraries![0].LibraryName, Is.EqualTo(libraryName));
            Assert.That(libraries[0].Functions, Has.Length.EqualTo(1));
            Assert.That(libraries[0].Functions[0].Name, Is.EqualTo(functionName));
        }
    }

    [Test]
    public void FunctionList_WithCode()
    {
        const string libraryName = "sharpredis_test_list_withcode";
        const string functionName = "sharpredis_test_list_withcode_hello";

        var functionCode = $$"""
        #!lua name={{libraryName}}
        redis.register_function{
            function_name = '{{functionName}}',
            callback = function(keys, args)
                return 'hello'
            end,
            description = 'FunctionList WITHCODE test',
            flags = {'no-writes'}
        }
        """;

        this._redis.Func.FunctionLoad(functionCode, replace: true);

        var libraries = this._redis.Func.FunctionList(libraryName, withCode: true);

        Assert.That(libraries, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(libraries, Has.Length.EqualTo(1));

            var library = libraries![0];

            Assert.That(library.LibraryName, Is.EqualTo(libraryName));
            Assert.That(library.Engine, Is.EqualTo("LUA"));
            Assert.That(library.LibraryCode, Is.Not.Null.And.Not.Empty);

            Assert.That(library.Functions, Has.Length.EqualTo(1));

            var function = library.Functions[0];

            Assert.That(function.Name, Is.EqualTo(functionName));
            Assert.That(function.Description, Is.EqualTo("FunctionList WITHCODE test"));
            Assert.That(function.Flags, Does.Contain("no-writes"));
        }
    }

    [Test]
    public void FunctionList_NoMatch()
    {
        var libraries = this._redis.Func.FunctionList(
            "sharpredis_test_list_no_such_library_*");

        Assert.That(libraries, Is.Null);
    }
    #endregion
}
