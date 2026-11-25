using SharpRedis;
using SharpRedis.Exceptions;

namespace NET10.Function;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class FunctionAsync(Redis redis)
{
    private readonly Redis _redis = redis;

    #region FUNCTION LOAD   FCALL
    [Test]
    public async Task FunctionLoadAsync()
    {
        const string functionCode = """
            #!lua name=sharpredis_test_load_async
            redis.register_function('sharpredis_test_load_hello_async', function(keys, args)
                return 'hello ' .. args[1]
            end)
            """;

        var result = await this._redis.Func.FunctionLoadAsync(functionCode, replace: true);

        Assert.That(result, Is.EqualTo("sharpredis_test_load_async"));
    }

    [Test]
    public async Task FCall_ReadOnlySpanAsync()
    {
        const string functionCode = """
            #!lua name=sharpredis_test_fcall_span_async
            redis.register_function('sharpredis_test_fcall_span_hello_async', function(keys, args)
                return 'hello ' .. args[1]
            end)
            """;

        await this._redis.Func.FunctionLoadAsync(functionCode, replace: true);

        var result = await this._redis.Func.FCallAsync(
            "sharpredis_test_fcall_span_hello_async",
            [],
            ["SharpRedis"]);

        Assert.That(result.ToString(), Is.EqualTo("hello SharpRedis"));
    }

    [Test]
    public async Task FCall_ICollectionAsync()
    {
        const string functionCode = """
            #!lua name=sharpredis_test_fcall_collection_async
            redis.register_function('sharpredis_test_fcall_collection_hello_async', function(keys, args)
                return 'hello ' .. args[1]
            end)
            """;

        await this._redis.Func.FunctionLoadAsync(functionCode, replace: true);

        var keys = new List<string>();
        var result = await this._redis.Func.FCallAsync(
            "sharpredis_test_fcall_collection_hello_async",
            keys,
            ["SharpRedis"]);
        Assert.That(result.ToString(), Is.EqualTo("hello SharpRedis"));

        keys = null!;
        result = await this._redis.Func.FCallAsync(
            "sharpredis_test_fcall_collection_hello_async",
            keys,
            ["SharpRedis"]);
        Assert.That(result.ToString(), Is.EqualTo("hello SharpRedis"));
    }

    [Test]
    public async Task FunctionLoad_ReplaceAsync()
    {
        const string functionCode1 = """
            #!lua name=sharpredis_test_replace_async
            redis.register_function('sharpredis_test_replace_hello_async', function(keys, args)
                return 'hello'
            end)
            """;

        const string functionCode2 = """
            #!lua name=sharpredis_test_replace_async
            redis.register_function('sharpredis_test_replace_hello_async', function(keys, args)
                return 'world'
            end)
            """;

        await this._redis.Func.FunctionLoadAsync(functionCode1, replace: true);
        await this._redis.Func.FunctionLoadAsync(functionCode2, replace: true);

        var result = await this._redis.Func.FCallAsync(
            "sharpredis_test_replace_hello_async");

        Assert.That(result.ToString(), Is.EqualTo("world"));
    }

    [Test]
    public async Task FunctionLoad_DuplicateLibraryAsync()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_duplicate_async
        redis.register_function('sharpredis_test_duplicate_hello_async', function(keys, args)
            return 'hello'
        end)
        """;

        await this._redis.Func.FunctionLoadAsync(functionCode, replace: true);

        Assert.That(
            () => this._redis.Func.FunctionLoadAsync(functionCode),
            Throws.TypeOf<RedisErrorException>()
                .And.Message.Contain("already exists")
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FUNCTION LOAD"));
    }

    [Test]
    public async Task FCall_FunctionNotFoundAsync()
    {
        Assert.That(
            () => this._redis.Func.FCallAsync(
                "sharpredis_test_function_not_found_async"),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FCALL"));
    }

    [Test]
    public async Task FCall_FunctionExecutionErrorAsync()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_execution_error_async
        redis.register_function('sharpredis_test_execution_error_throw_async', function(keys, args)
            error('SharpRedis function execution error')
        end)
        """;

        await this._redis.Func.FunctionLoadAsync(functionCode, replace: true);

        Assert.That(
            () => this._redis.Func.FCallAsync(
                "sharpredis_test_execution_error_throw_async"),
            Throws.TypeOf<RedisErrorException>()
                .And.Message.Contain("SharpRedis function execution error")
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FCALL"));
    }

    [Test]
    public async Task FCall_InvalidArgumentAsync()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_invalid_argument_async
        redis.register_function('sharpredis_test_invalid_argument_number_async', function(keys, args)
            return tonumber(args[1]) + 1
        end)
        """;

        await this._redis.Func.FunctionLoadAsync(functionCode, replace: true);

        Assert.That(
            () => this._redis.Func.FCallAsync(
                "sharpredis_test_invalid_argument_number_async",
                ReadOnlySpan<string>.Empty,
                ["not-a-number"]),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FCALL"));
    }

    [Test]
    public async Task FCall_WriteKeyAsync()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_write_key_async
        redis.register_function('sharpredis_test_write_key_set_async', function(keys, args)
            redis.call('SET', keys[1], args[1])
            return redis.call('GET', keys[1])
        end)
        """;

        await this._redis.Func.FunctionLoadAsync(functionCode, replace: true);

        const string key = "Function:{FCall_WriteKeyAsync}:Key";

        var result = await this._redis.Func.FCallAsync(
            "sharpredis_test_write_key_set_async",
            new List<string> { key },
            ["SharpRedis"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.ToString(), Is.EqualTo("SharpRedis"));
            Assert.That((await this._redis.String.GetAsync(key)).ToString(), Is.EqualTo("SharpRedis"));
        }
    }

    [Test]
    public async Task FCall_ReadAndIncrementKeyAsync()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_read_increment_async
        redis.register_function('sharpredis_test_read_increment_value_async', function(keys, args)
            local oldValue = redis.call('GET', keys[1])
            local newValue = redis.call('INCRBY', keys[1], args[1])
            return {oldValue, newValue}
        end)
        """;

        await this._redis.Func.FunctionLoadAsync(functionCode, replace: true);

        const string key = "Function:{FCall_ReadAndIncrementKeyAsync}:Key";

        await this._redis.String.SetAsync(key, "10");

        var result = await this._redis.Func.FCallAsync(
            "sharpredis_test_read_increment_value_async",
            [key],
            ["5"]);

        var values = result.RawValue as RedisResult[];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(values, Is.Not.Null);
            Assert.That(values, Has.Length.EqualTo(2));
            Assert.That(values![0].ToString(), Is.EqualTo("10"));
            Assert.That(values[1].ToString(), Is.EqualTo("15"));
            Assert.That((await this._redis.String.GetAsync(key)).ToString(), Is.EqualTo("15"));
        }
    }
    #endregion

    #region FCall_Ro
    [Test]
    public async Task FCall_Ro_ReadOnlySpanAsync()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_fcall_ro_span_async
        redis.register_function{
            function_name = 'sharpredis_test_fcall_ro_span_get_async',
            callback = function(keys, args)
                return redis.call('GET', keys[1])
            end,
            flags = {'no-writes'}
        }
        """;

        await this._redis.Func.FunctionLoadAsync(functionCode, replace: true);

        const string key = "Function:{FCall_Ro_Span_Async}:Key";
        await this._redis.String.SetAsync(key, "hello");

        var result = await this._redis.Func.FCall_RoAsync(
            "sharpredis_test_fcall_ro_span_get_async",
            [key]);

        Assert.That(result.ToString(), Is.EqualTo("hello"));
    }

    [Test]
    public async Task FCall_Ro_ICollectionAsync()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_fcall_ro_collection_async
        redis.register_function{
            function_name = 'sharpredis_test_fcall_ro_collection_get_async',
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

        await this._redis.Func.FunctionLoadAsync(functionCode, replace: true);

        const string key1 = "Function:{FCall_Ro_Collection_Async}:Key1";
        const string key2 = "Function:{FCall_Ro_Collection_Async}:Key2";

        await this._redis.String.SetAsync(key1, "value1");
        await this._redis.String.SetAsync(key2, "value2");

        var keys = new List<string>
        {
            key1,
            key2
        };

        var result = await this._redis.Func.FCall_RoAsync(
            "sharpredis_test_fcall_ro_collection_get_async",
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
    public async Task FCall_Ro_WithoutKeysAsync()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_fcall_ro_no_keys_async
        redis.register_function{
            function_name = 'sharpredis_test_fcall_ro_no_keys_echo_async',
            callback = function(keys, args)
                return args[1]
            end,
            flags = {'no-writes'}
        }
        """;

        await this._redis.Func.FunctionLoadAsync(functionCode, replace: true);

        var result = await this._redis.Func.FCall_RoAsync(
            "sharpredis_test_fcall_ro_no_keys_echo_async",
            args: ["SharpRedis"]);

        Assert.That(result.ToString(), Is.EqualTo("SharpRedis"));
    }

    [Test]
    public async Task FCall_Ro_FunctionNotFoundAsync()
    {
        Assert.That(
            () => this._redis.Func.FCall_RoAsync(
                "sharpredis_test_function_not_found"),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FCALL_RO"));
    }

    [Test]
    public async Task FCall_Ro_WriteCommandRejectedAsync()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_fcall_ro_write_async
        redis.register_function{
            function_name = 'sharpredis_test_fcall_ro_write_set_async',
            callback = function(keys, args)
                return redis.call('SET', keys[1], args[1])
            end,
            flags = {'no-writes'}
        }
        """;

        await this._redis.Func.FunctionLoadAsync(functionCode, replace: true);

        const string key = "Function:{FCall_Ro_Write_Async}:Key";
        await this._redis.String.SetAsync(key, "original");

        Assert.That(
            () => this._redis.Func.FCall_RoAsync(
                "sharpredis_test_fcall_ro_write_set_async",
                [key],
                ["modified"]),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FCALL_RO"));

        Assert.That((await this._redis.String.GetAsync(key)).ToString(), Is.EqualTo("original"));
    }

    [Test]
    public async Task FCall_Ro_FunctionWithoutNoWritesFlagAsync()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_fcall_ro_flag_async
        redis.register_function(
            'sharpredis_test_fcall_ro_flag_get_async',
            function(keys, args)
                return redis.call('GET', keys[1])
            end
        )
        """;

        await this._redis.Func.FunctionLoadAsync(functionCode, replace: true);

        const string key = "Function:{FCall_Ro_Flag_Async}:Key";
        await this._redis.String.SetAsync(key, "hello");

        Assert.That(
            () => this._redis.Func.FCall_RoAsync(
                "sharpredis_test_fcall_ro_flag_get_async",
                [key]),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FCALL_RO"));
    }
    #endregion

    #region FunctionDelete
    [Test]
    public async Task FunctionDeleteAsync()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_delete_async
        redis.register_function('sharpredis_test_delete_hello_async', function(keys, args)
            return 'hello'
        end)
        """;

        await this._redis.Func.FunctionLoadAsync(functionCode, replace: true);

        var result = await this._redis.Func.FCallAsync(
            "sharpredis_test_delete_hello_async");

        Assert.That(result.ToString(), Is.EqualTo("hello"));

        var deleted = await this._redis.Func.FunctionDeleteAsync(
            "sharpredis_test_delete_async");
        Assert.That(deleted, Is.True);

        Assert.That(
            () => this._redis.Func.FCallAsync("sharpredis_test_delete_hello_async"),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FCALL"));
    }

    [Test]
    public async Task FunctionDelete_LibraryNotFoundAsync()
    {
        Assert.That(
            () => this._redis.Func.FunctionDeleteAsync(
                "sharpredis_test_delete_not_found"),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FUNCTION DELETE"));
    }
    #endregion

    #region FUNCTION DUMP
    [Test]
    public async Task FunctionDumpAsync()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_dump_async
        redis.register_function('sharpredis_test_dump_hello_async', function(keys, args)
            return 'hello ' .. args[1]
        end)
        """;

        await this._redis.Func.FunctionFlushAsync();
        await this._redis.Func.FunctionLoadAsync(functionCode, replace: true);

        var dump = await this._redis.Func.FunctionDumpAsync();

        Assert.That(dump, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(dump, Is.Not.Empty);

            Assert.That(
                await this._redis.Func.FunctionDeleteAsync("sharpredis_test_dump_async"),
                Is.True);

            Assert.That(await this._redis.Func.FunctionRestoreAsync(dump), Is.True);
        }

        var result = await this._redis.Func.FCallAsync(
            "sharpredis_test_dump_hello_async",
            args: ["SharpRedis"]);

        Assert.That(result.ToString(), Is.EqualTo("hello SharpRedis"));
    }
    #endregion

    #region FUNCTION RESTORE
    [Test]
    public async Task FunctionRestore_DuplicateLibraryAsync()
    {
        const string functionCode = """
        #!lua name=sharpredis_test_restore_duplicate_async
        redis.register_function('sharpredis_test_restore_duplicate_hello_async', function(keys, args)
            return 'hello'
        end)
        """;

        await this._redis.Func.FunctionLoadAsync(functionCode, replace: true);

        var dump = await this._redis.Func.FunctionDumpAsync();

        Assert.That(
            () => this._redis.Func.FunctionRestoreAsync(dump),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FUNCTION RESTORE"));
    }

    [Test]
    public async Task FunctionRestore_ReplaceAsync()
    {
        const string functionCode1 = """
        #!lua name=sharpredis_test_restore_replace_async
        redis.register_function('sharpredis_test_restore_replace_hello_async', function(keys, args)
            return 'old'
        end)
        """;

        const string functionCode2 = """
        #!lua name=sharpredis_test_restore_replace_async
        redis.register_function('sharpredis_test_restore_replace_hello_async', function(keys, args)
            return 'new'
        end)
        """;

        await this._redis.Func.FunctionLoadAsync(functionCode1, replace: true);

        var dump = await this._redis.Func.FunctionDumpAsync();

        await this._redis.Func.FunctionLoadAsync(functionCode2, replace: true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                    (await this._redis.Func.FCallAsync("sharpredis_test_restore_replace_hello_async")).ToString(),
                    Is.EqualTo("new"));

            Assert.That(
                await this._redis.Func.FunctionRestoreAsync(dump, RedisFunction.RestoreHandling.Replace),
                Is.True);

            Assert.That(
                (await this._redis.Func.FCallAsync("sharpredis_test_restore_replace_hello_async")).ToString(),
                Is.EqualTo("old"));
        }
    }

    [Test]
    public async Task FunctionRestore_FlushAsync()
    {
        const string functionCode1 = """
        #!lua name=sharpredis_test_restore_flush_a_async
        redis.register_function('sharpredis_test_restore_flush_a_hello_async', function(keys, args)
            return 'library-a'
        end)
        """;

        const string functionCode2 = """
        #!lua name=sharpredis_test_restore_flush_b_async
        redis.register_function('sharpredis_test_restore_flush_b_hello_async', function(keys, args)
            return 'library-b'
        end)
        """;

        Assert.That(
            await this._redis.Func.FunctionFlushAsync(AsyncSync.Sync),
            Is.True);

        await this._redis.Func.FunctionLoadAsync(functionCode1, replace: true);

        var dump = await this._redis.Func.FunctionDumpAsync();

        await this._redis.Func.FunctionLoadAsync(functionCode2, replace: true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                    (await this._redis.Func.FCallAsync("sharpredis_test_restore_flush_b_hello_async")).ToString(),
                    Is.EqualTo("library-b"));

            Assert.That(
                await this._redis.Func.FunctionRestoreAsync(dump, RedisFunction.RestoreHandling.Flush),
                Is.True);

            Assert.That(
                (await this._redis.Func.FCallAsync("sharpredis_test_restore_flush_a_hello_async")).ToString(),
                Is.EqualTo("library-a"));

            Assert.That(
                () => this._redis.Func.FCallAsync("sharpredis_test_restore_flush_b_hello_async"),
                Throws.TypeOf<RedisErrorException>()
                    .And.Property(nameof(RedisErrorException.Command))
                    .EqualTo("FCALL"));
        }
    }

    [Test]
    public async Task FunctionRestore_InvalidDataAsync()
    {
        byte[] invalidData = [1, 2, 3, 4, 5, 6];

        Assert.That(
            () => this._redis.Func.FunctionRestoreAsync(invalidData),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FUNCTION RESTORE"));
    }
    #endregion

    #region FUNCTION FLUSH
    private Task<string> LoadFlushTestLibraryAsync(string libraryName, string functionName)
    {
        var functionCode = $"""
        #!lua name={libraryName}
        redis.register_function('{functionName}', function(keys, args)
            return '{functionName}'
        end)
        """;

        return this._redis.Func.FunctionLoadAsync(functionCode, replace: true);
    }

    [Test]
    public async Task FunctionFlushAsync()
    {
        await this.LoadFlushTestLibraryAsync(
            "sharpredis_test_flush_default_async",
            "sharpredis_test_flush_default_hello_async");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                    (await this._redis.Func.FCallAsync("sharpredis_test_flush_default_hello_async")).ToString(),
                    Is.EqualTo("sharpredis_test_flush_default_hello_async"));

            Assert.That(await this._redis.Func.FunctionFlushAsync(), Is.True);

            Assert.That(
                () => this._redis.Func.FCallAsync("sharpredis_test_flush_default_hello_async"),
                Throws.TypeOf<RedisErrorException>()
                    .And.Property(nameof(RedisErrorException.Command))
                    .EqualTo("FCALL"));
        }
    }

    [Test]
    public async Task FunctionFlush_SyncAsync()
    {
        await this.LoadFlushTestLibraryAsync(
            "sharpredis_test_flush_sync_async",
            "sharpredis_test_flush_sync_hello_async");

        Assert.That(
            await this._redis.Func.FunctionFlushAsync(AsyncSync.Sync),
            Is.True);

        Assert.That(
            () => this._redis.Func.FCallAsync("sharpredis_test_flush_sync_hello_async"),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FCALL"));
    }

    [Test]
    public async Task FunctionFlush_AsyncAsync()
    {
        await this.LoadFlushTestLibraryAsync(
            "sharpredis_test_flush_async_async",
            "sharpredis_test_flush_async_hello_async");

        Assert.That(
            await this._redis.Func.FunctionFlushAsync(AsyncSync.Async),
            Is.True);

        Assert.That(
            () => this._redis.Func.FCallAsync("sharpredis_test_flush_async_hello_async"),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command))
                .EqualTo("FCALL"));
    }
    #endregion

    #region FunctionList
    [Test]
    public async Task FunctionListAsync()
    {
        const string libraryName = "sharpredis_test_list_basic_async";
        const string functionName = "sharpredis_test_list_basic_hello_async";

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

        await this._redis.Func.FunctionLoadAsync(functionCode, replace: true);

        var libraries = await this._redis.Func.FunctionListAsync();

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
    public async Task FunctionList_LibraryNamePatternAsync()
    {
        const string libraryName = "sharpredis_test_list_pattern_async";
        const string functionName = "sharpredis_test_list_pattern_hello_async";

        var functionCode = $$"""
        #!lua name={{libraryName}}
        redis.register_function('{{functionName}}', function(keys, args)
            return 'hello'
        end)
        """;

        await this._redis.Func.FunctionLoadAsync(functionCode, replace: true);

        var libraries = await this._redis.Func.FunctionListAsync(libraryName);

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
    public async Task FunctionList_WithCodeAsync()
    {
        const string libraryName = "sharpredis_test_list_withcode_async";
        const string functionName = "sharpredis_test_list_withcode_hello_async";

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

        await this._redis.Func.FunctionLoadAsync(functionCode, replace: true);

        var libraries = await this._redis.Func.FunctionListAsync(libraryName, withCode: true);

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
    public async Task FunctionList_NoMatchAsync()
    {
        var libraries = await this._redis.Func.FunctionListAsync(
            "sharpredis_test_list_no_such_library_*");

        Assert.That(libraries, Is.Null);
    }
    #endregion
}
