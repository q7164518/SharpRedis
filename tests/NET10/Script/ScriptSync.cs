using SharpRedis;
using SharpRedis.Exceptions;

namespace NET10.Script;

[TestFixtureSource(typeof(Factory), nameof(Factory.CreateInstance))]
internal class ScriptSync(Redis redis)
{
    private readonly Redis _redis = redis;

    #region Eval
    [Test]
    public void Eval_String()
    {
        var result = this._redis.Script.Eval("return 'Hello SharpRedis'");
        Assert.That(result.ToString(), Is.EqualTo("Hello SharpRedis"));
    }

    [Test]
    public void Eval_Integer()
    {
        List<string> keys = null!;
        var result = this._redis.Script.Eval("return 123456", keys);
        Assert.That(result.ToString(), Is.EqualTo("123456"));
    }

    [Test]
    public void Eval_Nil()
    {
        var result = this._redis.Script.Eval("return nil");
        Assert.That(result.IsNull, Is.True);
    }

    [Test]
    public void Eval_KeysAndArgv()
    {
        var result = this._redis.Script.Eval(
            "redis.call('SET', KEYS[1], ARGV[1]); return redis.call('GET', KEYS[1])",
            ["Script:{Eval}:Key"],
            ["hello"]);
        Assert.That(result.ToString(), Is.EqualTo("hello"));
    }

    [Test]
    public void Eval_MultipleKeysAndArgv()
    {
        var result = this._redis.Script.Eval(
            @"redis.call('SET', KEYS[1], ARGV[1]);
            redis.call('SET', KEYS[2], ARGV[2]);
            return {redis.call('GET', KEYS[1]), redis.call('GET', KEYS[2])}",
            ["Script:{Eval}:Key1", "Script:{Eval}:Key2"],
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
    public void Eval_Bitmap()
    {
        var result = this._redis.Script.Eval(
            @"redis.call('SETBIT', KEYS[1], 0, 1);
            redis.call('SETBIT', KEYS[1], 7, 1);
            redis.call('SETBIT', KEYS[1], 15, 1);
            return {
                redis.call('GETBIT', KEYS[1], 0),
                redis.call('GETBIT', KEYS[1], 7),
                redis.call('GETBIT', KEYS[1], 15),
                redis.call('BITCOUNT', KEYS[1])
            }",
            ["Script:{Eval}:Bitmap"]);
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
    public void Eval_IncrBy()
    {
        var result = this._redis.Script.Eval(
            "return redis.call('INCRBY', KEYS[1], tonumber(ARGV[1]))",
            ["Script:{Eval}:Counter"],
            ["5"]);
        Assert.That((int)result, Is.EqualTo(5));
    }

    [Test]
    public void Eval_NestedArray()
    {
        var result = this._redis.Script.Eval("return {'hello', 123, {'redis', 'lua', 456}}");
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
    public void Eval_Binary()
    {
        var result = this._redis.Script.Eval(
            @"local value = string.char(0, 1, 2, 255, 65, 66);
            redis.call('SET', KEYS[1], value);
            return redis.call('GET', KEYS[1])",
            ["Script:{Eval}:Binary"]);
        Assert.That(result.RawValue, Is.Not.Null);
    }

    [Test]
    public void Eval_CommandError()
    {
        Assert.That(
            () => this._redis.Script.Eval("return redis.call('NO_SUCH_COMMAND')"),
            Throws.TypeOf<RedisErrorException>().And.Message.Contain("ERR Unknown Redis command called from script")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVAL"));
    }

    [Test]
    public void Eval_RuntimeError()
    {
        Assert.That(
            () => this._redis.Script.Eval("local a = nil; return a.someField"),
            Throws.TypeOf<RedisErrorException>().And.Message.Contain("ERR user_script:1: attempt to index local 'a' (a nil value) script")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVAL"));
    }
    #endregion

    #region Eval_Ro
    [Test]
    public void Eval_Ro_String()
    {
        var result = this._redis.Script.Eval_Ro("return 'Hello SharpRedis'");
        Assert.That(result.ToString(), Is.EqualTo("Hello SharpRedis"));
    }

    [Test]
    public void Eval_Ro_Integer()
    {
        List<string> keys = null!;
        var result = this._redis.Script.Eval_Ro("return 123456", keys);
        Assert.That((int)result, Is.EqualTo(123456));
    }

    [Test]
    public void Eval_Ro_Nil()
    {
        var result = this._redis.Script.Eval_Ro("return nil");
        Assert.That(result.IsNull, Is.True);
    }

    [Test]
    public void Eval_Ro_KeysAndArgv()
    {
        this._redis.String.Set("Script:{Eval_Ro}:Key", "hello");

        var result = this._redis.Script.Eval_Ro(
            "return redis.call('GET', KEYS[1])",
            ["Script:{Eval_Ro}:Key"]);

        Assert.That(result.ToString(), Is.EqualTo("hello"));
    }

    [Test]
    public void Eval_Ro_MultipleKeysAndArgv()
    {
        this._redis.String.Set("Script:{Eval_Ro}:Key1", "hello");
        this._redis.String.Set("Script:{Eval_Ro}:Key2", "world");

        var result = this._redis.Script.Eval_Ro(
            @"return {
            redis.call('GET', KEYS[1]),
            redis.call('GET', KEYS[2])
        }",
            ["Script:{Eval_Ro}:Key1", "Script:{Eval_Ro}:Key2"]);

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
    public void Eval_Ro_Bitmap()
    {
        this._redis.Bitmap.SetBit("Script:{Eval_Ro}:Bitmap", 0, true);
        this._redis.Bitmap.SetBit("Script:{Eval_Ro}:Bitmap", 7, true);
        this._redis.Bitmap.SetBit("Script:{Eval_Ro}:Bitmap", 15, true);

        var result = this._redis.Script.Eval_Ro(
            @"return {
            redis.call('GETBIT', KEYS[1], 0),
            redis.call('GETBIT', KEYS[1], 7),
            redis.call('GETBIT', KEYS[1], 15),
            redis.call('BITCOUNT', KEYS[1])
        }",
            ["Script:{Eval_Ro}:Bitmap"]);

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
    public void Eval_Ro_NestedArray()
    {
        var result = this._redis.Script.Eval_Ro(
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
    public void Eval_Ro_CommandError()
    {
        Assert.That(
            () => this._redis.Script.Eval_Ro(
                "return redis.call('NO_SUCH_COMMAND')"),
            Throws.TypeOf<RedisErrorException>()
                .And.Message.Contain("ERR Unknown Redis command called from script")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVAL_RO"));
    }

    [Test]
    public void Eval_Ro_RuntimeError()
    {
        Assert.That(
            () => this._redis.Script.Eval_Ro(
                "local a = nil; return a.someField"),
            Throws.TypeOf<RedisErrorException>()
                .And.Message.Contain("ERR user_script:1: attempt to index local 'a' (a nil value) script")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVAL_RO"));
    }

    [Test]
    public void Eval_Ro_WriteError()
    {
        Assert.That(
            () => this._redis.Script.Eval_Ro(
                "return redis.call('SET', KEYS[1], ARGV[1])",
                ["Script:{Eval_Ro}:Write"],
                ["hello"]),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVAL_RO"));
    }

    [Test]
    public void Eval_Ro_Argv()
    {
        var result = this._redis.Script.Eval_Ro(
            "return tonumber(ARGV[1]) + tonumber(ARGV[2])",
            argv: ["10", "20"]);

        Assert.That((int)result, Is.EqualTo(30));
    }
    #endregion

    #region EvalSHA and ScriptLoad
    [Test]
    public void ScriptLoad_EvalSHA_String()
    {
        const string script = "return 'Hello SharpRedis'";
        List<string> keys = null!;

        var sha1 = this._redis.Script.ScriptLoad(script);
        var result = this._redis.Script.EvalSHA(sha1, keys);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sha1, Is.Not.Null.And.Not.Empty);
            Assert.That(sha1, Has.Length.EqualTo(40));
            Assert.That(result.ToString(), Is.EqualTo("Hello SharpRedis"));
        }
    }

    [Test]
    public void ScriptLoad_EvalSHA_Integer()
    {
        const string script = "return 123456";

        var sha1 = this._redis.Script.ScriptLoad(script);
        var result = this._redis.Script.EvalSHA(sha1);

        Assert.That((int)result, Is.EqualTo(123456));
    }

    [Test]
    public void ScriptLoad_EvalSHA_Nil()
    {
        const string script = "return nil";

        var sha1 = this._redis.Script.ScriptLoad(script);
        var result = this._redis.Script.EvalSHA(sha1);

        Assert.That(result.IsNull, Is.True);
    }

    [Test]
    public void ScriptLoad_EvalSHA_KeysAndArgv()
    {
        const string script =
            "redis.call('SET', KEYS[1], ARGV[1]); return redis.call('GET', KEYS[1])";

        var sha1 = this._redis.Script.ScriptLoad(script);

        var result = this._redis.Script.EvalSHA(
            sha1,
            ["Script:{EvalSHA}:Key"],
            ["hello"]);

        Assert.That(result.ToString(), Is.EqualTo("hello"));
    }

    [Test]
    public void ScriptLoad_EvalSHA_MultipleKeysAndArgv()
    {
        const string script = @"
        redis.call('SET', KEYS[1], ARGV[1]);
        redis.call('SET', KEYS[2], ARGV[2]);
        return {
            redis.call('GET', KEYS[1]),
            redis.call('GET', KEYS[2])
        }";

        var sha1 = this._redis.Script.ScriptLoad(script);

        var result = this._redis.Script.EvalSHA(
            sha1,
            ["Script:{EvalSHA}:Key1", "Script:{EvalSHA}:Key2"],
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
    public void ScriptLoad_EvalSHA_Bitmap()
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

        var sha1 = this._redis.Script.ScriptLoad(script);

        var result = this._redis.Script.EvalSHA(
            sha1,
            ["Script:{EvalSHA}:Bitmap"]);

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
    public void ScriptLoad_EvalSHA_NestedArray()
    {
        const string script = "return {'hello', 123, {'redis', 'lua', 456}}";

        var sha1 = this._redis.Script.ScriptLoad(script);
        var result = this._redis.Script.EvalSHA(sha1);

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
    public void ScriptLoad_EvalSHA_CommandError()
    {
        const string script = "return redis.call('NO_SUCH_COMMAND')";

        var sha1 = this._redis.Script.ScriptLoad(script);

        Assert.That(
            () => this._redis.Script.EvalSHA(sha1),
            Throws.TypeOf<RedisErrorException>()
                .And.Message.Contain("ERR Unknown Redis command called from script")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVALSHA"));
    }

    [Test]
    public void ScriptLoad_EvalSHA_RuntimeError()
    {
        const string script = "local a = nil; return a.someField";

        var sha1 = this._redis.Script.ScriptLoad(script);

        Assert.That(
            () => this._redis.Script.EvalSHA(sha1),
            Throws.TypeOf<RedisErrorException>()
                .And.Message.Contain("user_script:1:")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVALSHA"));
    }

    [Test]
    public void EvalSHA_NoSuchScript()
    {
        Assert.That(
            () => this._redis.Script.EvalSHA(
                "0000000000000000000000000000000000000000"),
            Throws.TypeOf<RedisErrorException>()
                .And.Message.Contain("NOSCRIPT")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVALSHA"));
    }
    #endregion

    #region EvalSHA_Ro
    [Test]
    public void ScriptLoad_EvalSHA_Ro_String()
    {
        const string script = "return 'Hello SharpRedis'";
        List<string> keys = null!;

        var sha1 = this._redis.Script.ScriptLoad(script);
        var result = this._redis.Script.EvalSHA_Ro(sha1, keys);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sha1, Has.Length.EqualTo(40));
            Assert.That(result.ToString(), Is.EqualTo("Hello SharpRedis"));
        }
    }

    [Test]
    public void ScriptLoad_EvalSHA_Ro_Integer()
    {
        var sha1 = this._redis.Script.ScriptLoad("return 123456");
        var result = this._redis.Script.EvalSHA_Ro(sha1);

        Assert.That((int)result, Is.EqualTo(123456));
    }

    [Test]
    public void ScriptLoad_EvalSHA_Ro_Nil()
    {
        var sha1 = this._redis.Script.ScriptLoad("return nil");
        var result = this._redis.Script.EvalSHA_Ro(sha1);

        Assert.That(result.IsNull, Is.True);
    }

    [Test]
    public void ScriptLoad_EvalSHA_Ro_KeysAndArgv()
    {
        const string key = "Script:{EvalSHA_RO}:Key";

        this._redis.String.Set(key, "hello");

        var sha1 = this._redis.Script.ScriptLoad(
            "return redis.call('GET', KEYS[1]) .. ARGV[1]");

        var result = this._redis.Script.EvalSHA_Ro(
            sha1,
            [key],
            [" world"]);

        Assert.That(result.ToString(), Is.EqualTo("hello world"));
    }

    [Test]
    public void ScriptLoad_EvalSHA_Ro_MultipleKeysAndArgv()
    {
        const string script = @"
        return {
            redis.call('GET', KEYS[1]),
            redis.call('GET', KEYS[2]),
            ARGV[1]
        }";

        const string key1 = "Script:{EvalSHA_RO}:Key1";
        const string key2 = "Script:{EvalSHA_RO}:Key2";

        this._redis.String.Set(key1, "hello");
        this._redis.String.Set(key2, "world");

        var sha1 = this._redis.Script.ScriptLoad(script);

        var result = this._redis.Script.EvalSHA_Ro(
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
    public void ScriptLoad_EvalSHA_Ro_Bitmap()
    {
        const string key = "Script:{EvalSHA_RO}:Bitmap";

        this._redis.Bitmap.SetBit(key, 0, true);
        this._redis.Bitmap.SetBit(key, 7, true);
        this._redis.Bitmap.SetBit(key, 15, true);

        const string script = @"
        return {
            redis.call('GETBIT', KEYS[1], 0),
            redis.call('GETBIT', KEYS[1], 7),
            redis.call('GETBIT', KEYS[1], 15),
            redis.call('BITCOUNT', KEYS[1])
        }";

        var sha1 = this._redis.Script.ScriptLoad(script);
        var result = this._redis.Script.EvalSHA_Ro(sha1, [key]);

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
    public void ScriptLoad_EvalSHA_Ro_NestedArray()
    {
        var sha1 = this._redis.Script.ScriptLoad("return {'hello', 123, {'redis', 'lua', 456}}");

        var result = this._redis.Script.EvalSHA_Ro(sha1);

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
    public void ScriptLoad_EvalSHA_Ro_CommandError()
    {
        var sha1 = this._redis.Script.ScriptLoad("return redis.call('NO_SUCH_COMMAND')");

        Assert.That(
            () => this._redis.Script.EvalSHA_Ro(sha1),
            Throws.TypeOf<RedisErrorException>()
                .And.Message.Contain("ERR Unknown Redis command called from script")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVALSHA_RO"));
    }

    [Test]
    public void ScriptLoad_EvalSHA_Ro_RuntimeError()
    {
        var sha1 = this._redis.Script.ScriptLoad(
            "local a = nil; return a.someField");

        Assert.That(
            () => this._redis.Script.EvalSHA_Ro(sha1),
            Throws.TypeOf<RedisErrorException>()
                .And.Message.Contain("user_script:1:")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVALSHA_RO"));
    }

    [Test]
    public void EvalSHA_Ro_NoSuchScript()
    {
        Assert.That(
            () => this._redis.Script.EvalSHA_Ro(
                "0000000000000000000000000000000000000000"),
            Throws.TypeOf<RedisErrorException>()
                .And.Message.Contain("NOSCRIPT")
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVALSHA_RO"));
    }

    [Test]
    public void ScriptLoad_EvalSHA_Ro_WriteError()
    {
        var sha1 = this._redis.Script.ScriptLoad(
            "return redis.call('SET', KEYS[1], ARGV[1])");

        Assert.That(
            () => this._redis.Script.EvalSHA_Ro(
                sha1,
                ["Script:{EvalSHA_RO}:Write"],
                ["hello"]),
            Throws.TypeOf<RedisErrorException>()
                .And.Property(nameof(RedisErrorException.Command)).EqualTo("EVALSHA_RO"));
    }
    #endregion

    #region ScriptExists
    [Test]
    public void ScriptExists_Single()
    {
        const string script = "return 'ScriptExists_Single'";

        var sha1 = this._redis.Script.ScriptLoad(script);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this._redis.Script.ScriptExists(sha1), Is.True);
            Assert.That(
                this._redis.Script.ScriptExists(
                    "0000000000000000000000000000000000000000"),
                Is.False);
        }
    }

    [Test]
    public void ScriptExists_ReadOnlySpan()
    {
        const string script1 = "return 'ScriptExists_Span_1'";
        const string script2 = "return 'ScriptExists_Span_2'";

        var sha1 = this._redis.Script.ScriptLoad(script1);
        var sha2 = this._redis.Script.ScriptLoad(script2);
        const string missingSha = "0000000000000000000000000000000000000000";

        var result = this._redis.Script.ScriptExists([sha1, missingSha, sha2]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(3));
            Assert.That(result[0], Is.True);
            Assert.That(result[1], Is.False);
            Assert.That(result[2], Is.True);
        }
    }

    [Test]
    public void ScriptExists_ICollection()
    {
        const string script1 = "return 'ScriptExists_Collection_1'";
        const string script2 = "return 'ScriptExists_Collection_2'";

        var sha1 = this._redis.Script.ScriptLoad(script1);
        var sha2 = this._redis.Script.ScriptLoad(script2);
        const string missingSha = "0000000000000000000000000000000000000000";

        var sha1s = new List<string>
        {
            sha1,
            missingSha,
            sha2
        };

        var result = this._redis.Script.ScriptExists(sha1s);

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
    public void ScriptFlush()
    {
        const string script = "return 'ScriptFlush_Test'";

        var sha1 = this._redis.Script.ScriptLoad(script);
        Assert.That(this._redis.Script.ScriptExists(sha1), Is.True);
        var result = this._redis.Script.ScriptFlush();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(this._redis.Script.ScriptExists(sha1), Is.False);
        }

        sha1 = this._redis.Script.ScriptLoad(script);
        Assert.That(this._redis.Script.ScriptExists(sha1), Is.True);
        result = this._redis.Script.ScriptFlush(AsyncSync.Sync);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(this._redis.Script.ScriptExists(sha1), Is.False);
        }

        sha1 = this._redis.Script.ScriptLoad(script);
        Assert.That(this._redis.Script.ScriptExists(sha1), Is.True);
        result = this._redis.Script.ScriptFlush(AsyncSync.Async);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(this._redis.Script.ScriptExists(sha1), Is.False);
        }
    }
}
