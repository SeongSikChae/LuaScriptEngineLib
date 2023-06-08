using Neo.IronLua;

namespace LuaScriptEngineLib
{
    public interface ILuaScriptEngine : IDisposable
    {
        void Eval(string source);

        ILuaScriptEngineOutputEmitter? Emitter { get; }

        LuaGlobal? Globals { get; }
    }
}
