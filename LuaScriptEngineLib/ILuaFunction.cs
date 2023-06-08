using Neo.IronLua;

namespace LuaScriptEngineLib
{
    public interface ILuaFunction
    {
        void Load(string functionName, LuaTable tab);

        LuaResult? Invoke(params object[] args);
    }
}
