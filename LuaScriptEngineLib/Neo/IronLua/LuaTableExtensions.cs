using LuaScriptEngineLib;

namespace Neo.IronLua
{
    public static class LuaTableExtensions
    {
        public static void AddFunction(this LuaTable g, string functionName, ILuaFunction function)
        {
            function.Load(functionName, g);
        }
    }
}
