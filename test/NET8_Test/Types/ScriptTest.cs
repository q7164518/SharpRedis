using SharpRedis;

namespace NET8_Test.Types;

public class ScriptTest
{
    [Theory, ClassData(typeof(RedisProvider))]
    public void Eval(Redis redis)
    {
        var result = redis.Script.Eval("return ARGV[1]", null, ["hello"]);
        Assert.Equal("hello", result);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task EvalAsync(Redis redis)
    {
        var result = await redis.Script.EvalAsync("return ARGV[1]", null, ["hello"]);
        Assert.Equal("hello", result);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void FunctionLoad(Redis redis)
    {
        var name = redis.Script.FunctionLoad(@"#!lua name=mylib
redis.register_function('myfunc', function(keys, args) return args[1] end)
", true);
        Assert.Equal("mylib", name);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task FunctionLoadAsync(Redis redis)
    {
        var name = await redis.Script.FunctionLoadAsync(@"#!lua name=mylib
redis.register_function('myfunc', function(keys, args) return args[1] end)
", true);
        Assert.Equal("mylib", name);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void FCall(Redis redis)
    {
        var name = redis.Script.FunctionLoad(@"#!lua name=mylib
redis.register_function('myfunc', function(keys, args) return args[1] end)
", true);
        Assert.Equal("mylib", name);

        var call = redis.Script.FCall("myfunc", null, ["hello"]);
        Assert.Equal("hello", call);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task FCallAsync(Redis redis)
    {
        var name = await redis.Script.FunctionLoadAsync(@"#!lua name=mylib
redis.register_function('myfunc', function(keys, args) return args[1] end)
", true);
        Assert.Equal("mylib", name);

        var call = await redis.Script.FCallAsync("myfunc", null, ["hello"]);
        Assert.Equal("hello", call);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void FunctionDelete(Redis redis)
    {
        var name = redis.Script.FunctionLoad(@"#!lua name=dellib
redis.register_function('delfunc', function(keys, args) return args[1] end)
", true);
        Assert.Equal("dellib", name);

        var ok = redis.Script.FunctionDelete("dellib");
        Assert.True(ok);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task FunctionDeleteAsync(Redis redis)
    {
        var name = await redis.Script.FunctionLoadAsync(@"#!lua name=dellib_async
redis.register_function('delfunc_async', function(keys, args) return args[1] end)
", true);
        Assert.Equal("dellib_async", name);

        var ok = await redis.Script.FunctionDeleteAsync("dellib_async");
        Assert.True(ok);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void FunctionRestore(Redis redis)
    {
        var name = redis.Script.FunctionLoad(@"#!lua name=mylib_restore
redis.register_function('myfunc_restore', function(keys, args) return args[1] end)
", true);
        Assert.Equal("mylib_restore", name);

        var call = redis.Script.FCall("myfunc_restore", null, ["hello"]);
        Assert.Equal("hello", call);

        var ser = redis.Script.FunctionDump();

        redis.Script.FunctionFlush(FlushingMode.Sync);

        call = redis.Script.FCall("myfunc_restore", null, ["hello"]);
        Assert.True(call is RedisException);

        var restore = redis.Script.FunctionRestore(ser);
        Assert.True(restore);
        call = redis.Script.FCall("myfunc_restore", null, ["hello"]);
        Assert.Equal("hello", call);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task FunctionRestoreAsync(Redis redis)
    {
        var name = await redis.Script.FunctionLoadAsync(@"#!lua name=mylib_restore_async
redis.register_function('myfunc_restore_async', function(keys, args) return args[1] end)
", true);
        Assert.Equal("mylib_restore_async", name);

        var call = await redis.Script.FCallAsync("myfunc_restore_async", null, ["hello"]);
        Assert.Equal("hello", call);

        var ser = await redis.Script.FunctionDumpAsync();

        await redis.Script.FunctionFlushAsync(FlushingMode.Sync);

        call = await redis.Script.FCallAsync("myfunc_restore", null, ["hello"]);
        Assert.True(call is RedisException);

        var restore = await redis.Script.FunctionRestoreAsync(ser, FunctionRestoreMode.Replace);
        Assert.True(restore);
        call = await redis.Script.FCallAsync("myfunc_restore", null, ["hello"]);
        Assert.Equal("hello", call);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public void FunctionList(Redis redis)
    {
        var load = redis.Script.FunctionLoad(@"#!lua name=mylib_funs

local function my_hset(keys, args)
  local hash = keys[1]
  local time = redis.call('TIME')[1]
  return redis.call('HSET', hash, '_last_modified_', time, unpack(args))
end

local function my_hgetall(keys, args)
  redis.setresp(3)
  local hash = keys[1]
  local res = redis.call('HGETALL', hash)
  res['map']['_last_modified_'] = nil
  return res
end

local function my_hlastmodified(keys, args)
  local hash = keys[1]
  return redis.call('HGET', hash, '_last_modified_')
end

redis.register_function('my_hset', my_hset)
redis.register_function{
  function_name='my_hgetall',
  callback=my_hgetall,
  flags={ 'no-writes' }
}
redis.register_function{
  function_name='my_hlastmodified',
  callback=my_hlastmodified,
  flags={ 'no-writes' }
}
", true);
        Assert.Equal("mylib_funs", load);

        var list = redis.Script.FunctionList("mylib_funs", true);
        Assert.NotNull(list);
        Assert.Single(list);
        Assert.Equal("mylib_funs", list[0].LibraryName);
        Assert.Equal(3, list[0].Functions.Length);
        Assert.NotNull(list[0].LibraryCode);
    }

    [Theory, ClassData(typeof(RedisProvider))]
    public async Task FunctionListAsync(Redis redis)
    {
        var load = await redis.Script.FunctionLoadAsync(@"#!lua name=mylib_funs

local function my_hset(keys, args)
  local hash = keys[1]
  local time = redis.call('TIME')[1]
  return redis.call('HSET', hash, '_last_modified_', time, unpack(args))
end

local function my_hgetall(keys, args)
  redis.setresp(3)
  local hash = keys[1]
  local res = redis.call('HGETALL', hash)
  res['map']['_last_modified_'] = nil
  return res
end

local function my_hlastmodified(keys, args)
  local hash = keys[1]
  return redis.call('HGET', hash, '_last_modified_')
end

redis.register_function('my_hset', my_hset)
redis.register_function{
  function_name='my_hgetall',
  callback=my_hgetall,
  flags={ 'no-writes' }
}
redis.register_function{
  function_name='my_hlastmodified',
  callback=my_hlastmodified,
  flags={ 'no-writes' }
}
", true);
        Assert.Equal("mylib_funs", load);

        var list = await redis.Script.FunctionListAsync("mylib_funs", true);
        Assert.NotNull(list);
        Assert.Single(list);
        Assert.Equal("mylib_funs", list[0].LibraryName);
        Assert.Equal(3, list[0].Functions.Length);
        Assert.NotNull(list[0].LibraryCode);
    }
}
