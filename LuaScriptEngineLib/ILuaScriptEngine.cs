using Neo.IronLua;

namespace LuaScriptEngineLib
{
    public interface ILuaScriptEngine : IDisposable
    {
        Task<LuaResult?> EvalAsync(string source);

        LuaResult? Eval(string source, TimeSpan timeout);

        ILuaScriptEngineOutputEmitter? Emitter { get; }

        LuaGlobal? Globals { get; }
    }
}
