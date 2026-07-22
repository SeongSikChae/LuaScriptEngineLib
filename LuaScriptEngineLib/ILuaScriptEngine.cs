using Neo.IronLua;

namespace LuaScriptEngineLib
{
    public interface ILuaScriptEngine : IDisposable
    {
        Task<LuaResult?> EvalAsync(string source, CancellationToken cancellationToken);

        LuaResult? Eval(string source, TimeSpan timeout);

        ILuaScriptEngineOutputEmitter? Emitter { get; }

        LuaFunction<T> GetFunction<T>(string functionName);

        LuaFunction<T1, T2> GetFunction<T1, T2>(string functionName);

        LuaFunction<T1, T2, T3> GetFunction<T1, T2, T3>(string functionName);
    }
}
