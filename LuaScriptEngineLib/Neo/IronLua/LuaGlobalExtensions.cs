using LuaScriptEngineLib;

namespace Neo.IronLua
{
    public static class LuaGlobalExtensions
    {
        public static void AddLibrary(this LuaGlobal g, ILuaLibrary library)
        {
            library.Load(g);
        }
    }
}
