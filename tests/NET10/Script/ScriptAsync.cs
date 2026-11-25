using SharpRedis;
using SharpRedis.Exceptions;

namespace NET10.Script;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class ScriptAsync(Redis redis)
{
    private readonly Redis _redis = redis;

    #region Eval
    [Test]
    public async Task Eval_StringAsync()
    {
        var result = await this._redis.Script.EvalAsync("return 'Hello SharpRedis'");
        Assert.That(result.ToString(), Is.EqualTo("Hello SharpRedis"));
    }

    [Test]
    public async Task Eval_IntegerAsync()
    {
        List<string> keys = null!;
        var result = await this._redis.Script.EvalAsync("return 123456", keys);
        Assert.That(result.ToString(), Is.EqualTo("123456"));
    }

    [Test]
    public async Task Eval_NilAsync()
    {
        var result = await this._redis.Script.EvalAsync("return nil");
        Assert.That(result.IsNull, Is.True);
    }

    [Test]
    public async Task Eval_KeysAndArgvAsync()
    {
        var result = await this._redis.Script.EvalAsync(
            "redis.call('SET', KEYS[1], ARGV[1]); return redis.call('GET', KEYS[1])",
            new List<string> { "Script:{EvalAsync}:Key" },
            ["hello"]);
        Assert.That(result.ToString(), Is.EqualTo("hello"));
    }

    [Test]
    public async Task Eval_MultipleKeysAndArgvAsync()
    {
        var result = await this._redis.Script.EvalAsync(
            @"redis.call('SET', KEYS[1], ARGV[1]);
            redis.call('SET', KEYS[2], ARGV[2]);
            return {redis.call('GET', KEYS[1]), redis.call('GET', KEYS[2])}",
            ["Script:{EvalAsync}:Key1", "Script:{EvalAsync}:Key2"],
            ["hello", "world"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.RawValue, Is.InstanceOf<RedisResult[]>());
            Assert.That(result.ToString(), Is.Not.Null);
            var arr = result.RawValue as RedisResult[];
            Assert.That(arr![0].ToString(), Is.EqualTo("hello"));
            Assert.That(arr![1].ToString(), Is.EqualTo("world"));
        }
    }

    [Test]
    public async Task Eval_BitmapAsync()
    {
        var result = await this._redis.Script.EvalAsync(
            @"redis.call('SETBIT', KEYS[1], 0, 1);
            redis.call('SETBIT', KEYS[1], 7, 1);
            redis.call('SETBIT', KEYS[1], 15, 1);
            return {
                redis.call('GETBIT', KEYS[1], 0),
                redis.call('GETBIT', KEYS[1], 7),
                redis.call('GETBIT', KEYS[1], 15),
                redis.call('BITCOUNT', KEYS[1])
            }",
            ["Script:{EvalAsync}:Bitmap"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.RawValue, Is.InstanceOf<RedisResult[]>());
            var arr = result.RawValue as RedisResult[];
            Assert.That((int)arr![0], Is.EqualTo(1));
            Assert.That((int)arr![1], Is.EqualTo(1));
            Assert.That((int)arr![2], Is.EqualTo(1));
            Assert.That((int)arr![3], Is.EqualTo(3));
        }
    }

    [Test]
    public async Task Eval_IncrByAsync()
    {
        var result = await this._redis.Script.EvalAsync(
            "return redis.call('INCRBY', KEYS[1], tonumber(ARGV[1]))",
            ["Script:{EvalAsync}:Counter"],
            ["5"]);
        Assert.That((int)result, Is.EqualTo(5));
    }

    [Test]
    public async Task Eval_NestedArrayAsync()
    {
        var result = await this._redis.Script.EvalAsync("return {'hello', 123, {'redis', 'lua', 456}}");
        Assert.IsFalse(result.IsNull);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.RawValue, Is.InstanceOf<RedisResult[]>());
            var arr = result.RawValue as RedisResult[];
            Assert.That(arr, Has.Length.EqualTo(3));
            Assert.That(arr![0].ToString(), Is.EqualTo("hello"));
            Assert.That(arr![1].ToString(), Is.EqualTo("123"));
            Assert.That(arr[2].RawValue, Is.InstanceOf<RedisResult[]>());
            var arr1 = arr[2].RawValue as RedisResult[];
            Assert.That(arr1, Has.Length.EqualTo(3));
            Assert.That(arr1![0].ToString(), Is.EqualTo("redis"));
            Assert.That(arr1![1].ToString(), Is.EqualTo("lua"));
            Assert.That(arr1![2].ToString(), Is.EqualTo("456"));
        }
    }

    [Test]
    public async Task Eval_BinaryAsync()
    {
        var result = await this._redis.Script.EvalAsync(
            @"local value = string.char(0, 1, 2, 255, 65, 66);
            redis.call('SET', KEYS[1], value);
            return redis.call('GET', KEYS[1])",
            ["Script:{EvalAsync}:Binary"]);
        Assert.That(result.RawValue, Is.Not.Null);
    }

    [Test]
    public async Task Eval_CommandErrorAsync()
    {
        Assert.That(
            () => this._redis.Script.EvalAsync("return redis.call('NO_SUCH_COMMAND')"),
            Throws.TypeOf<RedisErrorException>().And.Message.Contain("ERR Unknown Redis command called from script")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVAL"));
    }

    [Test]
    public async Task Eval_RuntimeErrorAsync()
    {
        Assert.That(
            () => this._redis.Script.EvalAsync("local a = nil; return a.someField"),
            Throws.TypeOf<RedisErrorException>().And.Message.Contain("ERR user_script:1: attempt to index local 'a' (a nil value) script")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVAL"));
    }
    #endregion

    #region Eval_Ro
    [Test]
    public async Task Eval_Ro_StringAsync()
    {
        var result = await this._redis.Script.Eval_RoAsync("return 'Hello SharpRedis'");
        Assert.That(result.ToString(), Is.EqualTo("Hello SharpRedis"));
    }

    [Test]
    public async Task Eval_Ro_IntegerAsync()
    {
        List<string> keys = null!;
        var result = await this._redis.Script.Eval_RoAsync("return 123456", keys);
        Assert.That((int)result, Is.EqualTo(123456));
    }

    [Test]
    public async Task Eval_Ro_NilAsync()
    {
        var result = await this._redis.Script.Eval_RoAsync("return nil");
        Assert.That(result.IsNull, Is.True);
    }

    [Test]
    public async Task Eval_Ro_KeysAndArgvAsync()
    {
        _ = await this._redis.String.SetAsync("Script:{Eval_RoAsync}:Key", "hello");

        var result = await this._redis.Script.Eval_RoAsync(
            "return redis.call('GET', KEYS[1])",
            ["Script:{Eval_RoAsync}:Key"]);

        Assert.That(result.ToString(), Is.EqualTo("hello"));
    }

    [Test]
    public async Task Eval_Ro_MultipleKeysAndArgvAsync()
    {
        this._redis.String.Set("Script:{Eval_RoAsync}:Key1", "hello");
        this._redis.String.Set("Script:{Eval_RoAsync}:Key2", "world");

        var result = await this._redis.Script.Eval_RoAsync(
            @"return {
            redis.call('GET', KEYS[1]),
            redis.call('GET', KEYS[2])
        }",
            ["Script:{Eval_RoAsync}:Key1", "Script:{Eval_RoAsync}:Key2"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.RawValue, Is.InstanceOf<RedisResult[]>());
            var arr = result.RawValue as RedisResult[];
            Assert.That(arr, Has.Length.EqualTo(2));
            Assert.That(arr![0].ToString(), Is.EqualTo("hello"));
            Assert.That(arr[1].ToString(), Is.EqualTo("world"));
        }
    }

    [Test]
    public async Task Eval_Ro_BitmapAsync()
    {
        _ = await this._redis.Bitmap.SetBitAsync("Script:{Eval_RoAsync}:Bitmap", 0, true);
        _ = await this._redis.Bitmap.SetBitAsync("Script:{Eval_RoAsync}:Bitmap", 7, true);
        _ = await this._redis.Bitmap.SetBitAsync("Script:{Eval_RoAsync}:Bitmap", 15, true);

        var result = await this._redis.Script.Eval_RoAsync(
            @"return {
            redis.call('GETBIT', KEYS[1], 0),
            redis.call('GETBIT', KEYS[1], 7),
            redis.call('GETBIT', KEYS[1], 15),
            redis.call('BITCOUNT', KEYS[1])
        }",
            ["Script:{Eval_RoAsync}:Bitmap"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.RawValue, Is.InstanceOf<RedisResult[]>());
            var arr = result.RawValue as RedisResult[];
            Assert.That((int)arr![0], Is.EqualTo(1));
            Assert.That((int)arr[1], Is.EqualTo(1));
            Assert.That((int)arr[2], Is.EqualTo(1));
            Assert.That((int)arr[3], Is.EqualTo(3));
        }
    }

    [Test]
    public async Task Eval_Ro_NestedArrayAsync()
    {
        var result = await this._redis.Script.Eval_RoAsync(
            "return {'hello', 123, {'redis', 'lua', 456}}");

        Assert.That(result.IsNull, Is.False);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.RawValue, Is.InstanceOf<RedisResult[]>());
            var arr = result.RawValue as RedisResult[];
            Assert.That(arr, Has.Length.EqualTo(3));
            Assert.That(arr![0].ToString(), Is.EqualTo("hello"));
            Assert.That((int)arr[1], Is.EqualTo(123));

            Assert.That(arr[2].RawValue, Is.InstanceOf<RedisResult[]>());
            var arr1 = arr[2].RawValue as RedisResult[];
            Assert.That(arr1, Has.Length.EqualTo(3));
            Assert.That(arr1![0].ToString(), Is.EqualTo("redis"));
            Assert.That(arr1[1].ToString(), Is.EqualTo("lua"));
            Assert.That((int)arr1[2], Is.EqualTo(456));
        }
    }

    [Test]
    public async Task Eval_Ro_CommandErrorAsync()
    {
        Assert.That(
            () => this._redis.Script.Eval_RoAsync(
                "return redis.call('NO_SUCH_COMMAND')"),
            Throws.TypeOf<RedisErrorException>()
                .And.Message.Contain("ERR Unknown Redis command called from script")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVAL_RO"));
    }

    [Test]
    public async Task Eval_Ro_RuntimeErrorAsync()
    {
        Assert.That(
            () => this._redis.Script.Eval_RoAsync(
                "local a = nil; return a.someField"),
            Throws.TypeOf<RedisErrorException>()
                .And.Message.Contain("ERR user_script:1: attempt to index local 'a' (a nil value) script")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVAL_RO"));
    }

    [Test]
    public async Task Eval_Ro_WriteErrorAsync()
    {
        Assert.That(
            () => this._redis.Script.Eval_RoAsync(
                "return redis.call('SET', KEYS[1], ARGV[1])",
                ["Script:{Eval_RoAsync}:Write"],
                ["hello"]),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVAL_RO"));
    }

    [Test]
    public async Task Eval_Ro_ArgvAsync()
    {
        var result = await this._redis.Script.Eval_RoAsync(
            "return tonumber(ARGV[1]) + tonumber(ARGV[2])",
            argv: ["10", "20"]);

        Assert.That((int)result, Is.EqualTo(30));
    }
    #endregion

    #region EvalSHA and ScriptLoad
    [Test]
    public async Task ScriptLoad_EvalSHA_StringAsync()
    {
        const string script = "return 'Hello SharpRedis'";

        var sha1 = await this._redis.Script.ScriptLoadAsync(script);
        var result = await this._redis.Script.EvalSHAAsync(sha1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sha1, Is.Not.Null.And.Not.Empty);
            Assert.That(sha1, Has.Length.EqualTo(40));
            Assert.That(result.ToString(), Is.EqualTo("Hello SharpRedis"));
        }
    }

    [Test]
    public async Task ScriptLoad_EvalSHA_IntegerAsync()
    {
        const string script = "return 123456";

        List<string> keys = null!;
        var sha1 = await this._redis.Script.ScriptLoadAsync(script);
        var result = await this._redis.Script.EvalSHAAsync(sha1, keys);

        Assert.That((int)result, Is.EqualTo(123456));
    }

    [Test]
    public async Task ScriptLoad_EvalSHA_NilAsync()
    {
        const string script = "return nil";

        var sha1 = await this._redis.Script.ScriptLoadAsync(script);
        var result = await this._redis.Script.EvalSHAAsync(sha1);

        Assert.That(result.IsNull, Is.True);
    }

    [Test]
    public async Task ScriptLoad_EvalSHA_KeysAndArgvAsync()
    {
        const string script =
            "redis.call('SET', KEYS[1], ARGV[1]); return redis.call('GET', KEYS[1])";

        var sha1 = await this._redis.Script.ScriptLoadAsync(script);

        var result = await this._redis.Script.EvalSHAAsync(
            sha1,
            ["Script:{EvalSHAAsync}:Key"],
            ["hello"]);

        Assert.That(result.ToString(), Is.EqualTo("hello"));
    }

    [Test]
    public async Task ScriptLoad_EvalSHA_MultipleKeysAndArgvAsync()
    {
        const string script = @"
        redis.call('SET', KEYS[1], ARGV[1]);
        redis.call('SET', KEYS[2], ARGV[2]);
        return {
            redis.call('GET', KEYS[1]),
            redis.call('GET', KEYS[2])
        }";

        var sha1 = await this._redis.Script.ScriptLoadAsync(script);

        var result = await this._redis.Script.EvalSHAAsync(
            sha1,
            ["Script:{EvalSHAAsync}:Key1", "Script:{EvalSHAAsync}:Key2"],
            ["hello", "world"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.RawValue, Is.InstanceOf<RedisResult[]>());

            var arr = result.RawValue as RedisResult[];
            Assert.That(arr, Has.Length.EqualTo(2));
            Assert.That(arr![0].ToString(), Is.EqualTo("hello"));
            Assert.That(arr[1].ToString(), Is.EqualTo("world"));
        }
    }

    [Test]
    public async Task ScriptLoad_EvalSHA_BitmapAsync()
    {
        const string script = @"
        redis.call('SETBIT', KEYS[1], 0, 1);
        redis.call('SETBIT', KEYS[1], 7, 1);
        redis.call('SETBIT', KEYS[1], 15, 1);
        return {
            redis.call('GETBIT', KEYS[1], 0),
            redis.call('GETBIT', KEYS[1], 7),
            redis.call('GETBIT', KEYS[1], 15),
            redis.call('BITCOUNT', KEYS[1])
        }";

        var sha1 = await this._redis.Script.ScriptLoadAsync(script);

        var result = await this._redis.Script.EvalSHAAsync(
            sha1,
            ["Script:{EvalSHAAsync}:Bitmap"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.RawValue, Is.InstanceOf<RedisResult[]>());

            var arr = result.RawValue as RedisResult[];
            Assert.That(arr, Has.Length.EqualTo(4));
            Assert.That((int)arr![0], Is.EqualTo(1));
            Assert.That((int)arr[1], Is.EqualTo(1));
            Assert.That((int)arr[2], Is.EqualTo(1));
            Assert.That((int)arr[3], Is.EqualTo(3));
        }
    }

    [Test]
    public async Task ScriptLoad_EvalSHA_NestedArrayAsync()
    {
        const string script = "return {'hello', 123, {'redis', 'lua', 456}}";

        var sha1 = await this._redis.Script.ScriptLoadAsync(script);
        var result = await this._redis.Script.EvalSHAAsync(sha1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.RawValue, Is.InstanceOf<RedisResult[]>());

            var arr = result.RawValue as RedisResult[];
            Assert.That(arr, Has.Length.EqualTo(3));
            Assert.That(arr![0].ToString(), Is.EqualTo("hello"));
            Assert.That((int)arr[1], Is.EqualTo(123));

            Assert.That(arr[2].RawValue, Is.InstanceOf<RedisResult[]>());

            var nested = arr[2].RawValue as RedisResult[];
            Assert.That(nested, Has.Length.EqualTo(3));
            Assert.That(nested![0].ToString(), Is.EqualTo("redis"));
            Assert.That(nested[1].ToString(), Is.EqualTo("lua"));
            Assert.That((int)nested[2], Is.EqualTo(456));
        }
    }

    [Test]
    public async Task ScriptLoad_EvalSHA_CommandErrorAsync()
    {
        const string script = "return redis.call('NO_SUCH_COMMAND')";

        var sha1 = await this._redis.Script.ScriptLoadAsync(script);

        Assert.That(
            () => this._redis.Script.EvalSHAAsync(sha1),
            Throws.TypeOf<RedisErrorException>()
                .And.Message.Contain("ERR Unknown Redis command called from script")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVALSHA"));
    }

    [Test]
    public async Task ScriptLoad_EvalSHA_RuntimeErrorAsync()
    {
        const string script = "local a = nil; return a.someField";

        var sha1 = await this._redis.Script.ScriptLoadAsync(script);

        Assert.That(
            () => this._redis.Script.EvalSHAAsync(sha1),
            Throws.TypeOf<RedisErrorException>()
                .And.Message.Contain("user_script:1:")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVALSHA"));
    }

    [Test]
    public async Task EvalSHA_NoSuchScriptAsync()
    {
        Assert.That(
            () => this._redis.Script.EvalSHAAsync(
                "0000000000000000000000000000000000000000"),
            Throws.TypeOf<RedisErrorException>()
                .And.Message.Contain("NOSCRIPT")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVALSHA"));
    }
    #endregion

    #region EvalSHA_Ro
    [Test]
    public async Task ScriptLoad_EvalSHA_Ro_StringAsync()
    {
        const string script = "return 'Hello SharpRedis'";
        List<string> keys = null!;

        var sha1 = await this._redis.Script.ScriptLoadAsync(script);
        var result = await this._redis.Script.EvalSHA_RoAsync(sha1, keys);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sha1, Has.Length.EqualTo(40));
            Assert.That(result.ToString(), Is.EqualTo("Hello SharpRedis"));
        }
    }

    [Test]
    public async Task ScriptLoad_EvalSHA_Ro_IntegerAsync()
    {
        var sha1 = await this._redis.Script.ScriptLoadAsync("return 123456");
        var result = await this._redis.Script.EvalSHA_RoAsync(sha1);

        Assert.That((int)result, Is.EqualTo(123456));
    }

    [Test]
    public async Task ScriptLoad_EvalSHA_Ro_NilAsync()
    {
        var sha1 = await this._redis.Script.ScriptLoadAsync("return nil");
        var result = await this._redis.Script.EvalSHA_RoAsync(sha1);

        Assert.That(result.IsNull, Is.True);
    }

    [Test]
    public async Task ScriptLoad_EvalSHA_Ro_KeysAndArgvAsync()
    {
        const string key = "Script:{EvalSHA_ROAsync}:Key";

        await this._redis.String.SetAsync(key, "hello");

        var sha1 = await this._redis.Script.ScriptLoadAsync(
            "return redis.call('GET', KEYS[1]) .. ARGV[1]");

        var result = await this._redis.Script.EvalSHA_RoAsync(
            sha1,
            [key],
            [" world"]);

        Assert.That(result.ToString(), Is.EqualTo("hello world"));
    }

    [Test]
    public async Task ScriptLoad_EvalSHA_Ro_MultipleKeysAndArgvAsync()
    {
        const string script = @"
        return {
            redis.call('GET', KEYS[1]),
            redis.call('GET', KEYS[2]),
            ARGV[1]
        }";

        const string key1 = "Script:{EvalSHA_ROAsync}:Key1";
        const string key2 = "Script:{EvalSHA_ROAsync}:Key2";

        await this._redis.String.SetAsync(key1, "hello");
        await this._redis.String.SetAsync(key2, "world");

        var sha1 = await this._redis.Script.ScriptLoadAsync(script);

        var result = await this._redis.Script.EvalSHA_RoAsync(
            sha1,
            [key1, key2],
            ["SharpRedis"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.RawValue, Is.InstanceOf<RedisResult[]>());

            var arr = result.RawValue as RedisResult[];
            Assert.That(arr, Has.Length.EqualTo(3));
            Assert.That(arr![0].ToString(), Is.EqualTo("hello"));
            Assert.That(arr[1].ToString(), Is.EqualTo("world"));
            Assert.That(arr[2].ToString(), Is.EqualTo("SharpRedis"));
        }
    }

    [Test]
    public async Task ScriptLoad_EvalSHA_Ro_BitmapAsync()
    {
        const string key = "Script:{EvalSHA_ROAsync}:Bitmap";

        await this._redis.Bitmap.SetBitAsync(key, 0, true);
        await this._redis.Bitmap.SetBitAsync(key, 7, true);
        await this._redis.Bitmap.SetBitAsync(key, 15, true);

        const string script = @"
        return {
            redis.call('GETBIT', KEYS[1], 0),
            redis.call('GETBIT', KEYS[1], 7),
            redis.call('GETBIT', KEYS[1], 15),
            redis.call('BITCOUNT', KEYS[1])
        }";

        var sha1 = await this._redis.Script.ScriptLoadAsync(script);
        var result = await this._redis.Script.EvalSHA_RoAsync(sha1, [key]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.RawValue, Is.InstanceOf<RedisResult[]>());

            var arr = result.RawValue as RedisResult[];
            Assert.That(arr, Has.Length.EqualTo(4));
            Assert.That((int)arr![0], Is.EqualTo(1));
            Assert.That((int)arr[1], Is.EqualTo(1));
            Assert.That((int)arr[2], Is.EqualTo(1));
            Assert.That((int)arr[3], Is.EqualTo(3));
        }
    }

    [Test]
    public async Task ScriptLoad_EvalSHA_Ro_NestedArrayAsync()
    {
        var sha1 = await this._redis.Script.ScriptLoadAsync("return {'hello', 123, {'redis', 'lua', 456}}");

        var result = await this._redis.Script.EvalSHA_RoAsync(sha1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.RawValue, Is.InstanceOf<RedisResult[]>());

            var arr = result.RawValue as RedisResult[];
            Assert.That(arr, Has.Length.EqualTo(3));
            Assert.That(arr![0].ToString(), Is.EqualTo("hello"));
            Assert.That((int)arr[1], Is.EqualTo(123));

            Assert.That(arr[2].RawValue, Is.InstanceOf<RedisResult[]>());

            var nested = arr[2].RawValue as RedisResult[];
            Assert.That(nested, Has.Length.EqualTo(3));
            Assert.That(nested![0].ToString(), Is.EqualTo("redis"));
            Assert.That(nested[1].ToString(), Is.EqualTo("lua"));
            Assert.That((int)nested[2], Is.EqualTo(456));
        }
    }

    [Test]
    public async Task ScriptLoad_EvalSHA_Ro_CommandErrorAsync()
    {
        var sha1 = await this._redis.Script.ScriptLoadAsync("return redis.call('NO_SUCH_COMMAND')");

        Assert.That(
            () => this._redis.Script.EvalSHA_RoAsync(sha1),
            Throws.TypeOf<RedisErrorException>()
                .And.Message.Contain("ERR Unknown Redis command called from script")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVALSHA_RO"));
    }

    [Test]
    public async Task ScriptLoad_EvalSHA_Ro_RuntimeErrorAsync()
    {
        var sha1 = await this._redis.Script.ScriptLoadAsync(
            "local a = nil; return a.someField");

        Assert.That(
            () => this._redis.Script.EvalSHA_RoAsync(sha1),
            Throws.TypeOf<RedisErrorException>()
                .And.Message.Contain("user_script:1:")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVALSHA_RO"));
    }

    [Test]
    public async Task EvalSHA_Ro_NoSuchScriptAsync()
    {
        Assert.That(
            () => this._redis.Script.EvalSHA_RoAsync(
                "0000000000000000000000000000000000000000"),
            Throws.TypeOf<RedisErrorException>()
                .And.Message.Contain("NOSCRIPT")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVALSHA_RO"));
    }

    [Test]
    public async Task ScriptLoad_EvalSHA_Ro_WriteErrorAsync()
    {
        var sha1 = await this._redis.Script.ScriptLoadAsync("return redis.call('SET', KEYS[1], ARGV[1])");
        Assert.That(
            () => this._redis.Script.EvalSHA_Ro(
                sha1,
                ["Script:{EvalSHA_ROAsync}:Write"],
                ["hello"]),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVALSHA_RO"));
    }
    #endregion

    #region ScriptExists
    [Test]
    public async Task ScriptExists_SingleAsync()
    {
        const string script = "return 'ScriptExists_Single'";

        var sha1 = await this._redis.Script.ScriptLoadAsync(script);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await this._redis.Script.ScriptExistsAsync(sha1), Is.True);
            Assert.That(
                await this._redis.Script.ScriptExistsAsync(
                    "0000000000000000000000000000000000000000"),
                Is.False);
        }
    }

    [Test]
    public async Task ScriptExists_ReadOnlySpanAsync()
    {
        const string script1 = "return 'ScriptExists_Span_1'";
        const string script2 = "return 'ScriptExists_Span_2'";

        var sha1 = await this._redis.Script.ScriptLoadAsync(script1);
        var sha2 = await this._redis.Script.ScriptLoadAsync(script2);
        const string missingSha = "0000000000000000000000000000000000000000";

        var result = await this._redis.Script.ScriptExistsAsync([sha1, missingSha, sha2]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(3));
            Assert.That(result[0], Is.True);
            Assert.That(result[1], Is.False);
            Assert.That(result[2], Is.True);
        }
    }

    [Test]
    public async Task ScriptExists_ICollectionAsync()
    {
        const string script1 = "return 'ScriptExists_Collection_1'";
        const string script2 = "return 'ScriptExists_Collection_2'";

        var sha1 = await this._redis.Script.ScriptLoadAsync(script1);
        var sha2 = await this._redis.Script.ScriptLoadAsync(script2);
        const string missingSha = "0000000000000000000000000000000000000000";

        var sha1s = new List<string>
        {
            sha1,
            missingSha,
            sha2
        };

        var result = await this._redis.Script.ScriptExistsAsync(sha1s);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(3));
            Assert.That(result[0], Is.True);
            Assert.That(result[1], Is.False);
            Assert.That(result[2], Is.True);
        }
    }
    #endregion

    [Test]
    public async Task ScriptFlushAsync()
    {
        const string script = "return 'ScriptFlush_Test'";

        var sha1 = await this._redis.Script.ScriptLoadAsync(script);
        Assert.That(await this._redis.Script.ScriptExistsAsync(sha1), Is.True);
        var result = await this._redis.Script.ScriptFlushAsync();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(await this._redis.Script.ScriptExistsAsync(sha1), Is.False);
        }

        sha1 = await this._redis.Script.ScriptLoadAsync(script);
        Assert.That(await this._redis.Script.ScriptExistsAsync(sha1), Is.True);
        result = await this._redis.Script.ScriptFlushAsync(AsyncSync.Sync);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(await this._redis.Script.ScriptExistsAsync(sha1), Is.False);
        }

        sha1 = await this._redis.Script.ScriptLoadAsync(script);
        Assert.That(await this._redis.Script.ScriptExistsAsync(sha1), Is.True);
        result = await this._redis.Script.ScriptFlushAsync(AsyncSync.Async);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(await this._redis.Script.ScriptExistsAsync(sha1), Is.False);
        }
    }
}
