using Neo.IronLua;

namespace LuaScriptEngineLib
{
    public interface ILuaLibrary
    {
        void Load(LuaGlobal g);
    }
}
