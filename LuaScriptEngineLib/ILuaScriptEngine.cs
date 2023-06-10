using Neo.IronLua;

namespace LuaScriptEngineLib
{
    public interface ILuaScriptEngine : IDisposable
    {
        Task EvalAsync(string source);

        ILuaScriptEngineOutputEmitter? Emitter { get; }

        LuaGlobal? Globals { get; }
    }
}
