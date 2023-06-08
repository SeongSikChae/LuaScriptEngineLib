using LuaScriptEngineLib;

namespace Neo.IronLua
{
    internal static class LuaGlobalExtensions
    {
        public static void AddFunction(this LuaGlobal g, string functionName, ILuaFunction function)
        {
            function.Load(functionName, g);
        }

        public static void AddLibrary(this LuaGlobal g, ILuaLibrary library)
        {
            library.Load(g);
        }
    }
}
