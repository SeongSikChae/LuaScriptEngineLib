using LuaScriptEngineLib;

namespace Neo.IronLua
{
    public static class LuaTableExtensions
    {
        public static void AddFunction(this LuaTable g, string functionName, ILuaFunction function)
        {
            function.Load(functionName, g);
        }

        public static T GetValue<T>(this LuaTable table, string name, bool rawGet = false)
        {
            return (T) table.GetValue(name, rawGet);
        }

        public static T? GetOptionalValue<T>(this LuaTable table, string name, bool rawGet = false)
        {
            return table.GetOptionalValue<T?>(name, default(T));
        }

        public static LuaFunction<T> GetFunction<T>(this LuaTable table, string functionName)
        {
            return new LuaFunction<T>(table.GetValue<Func<object?, LuaResult>>(functionName));
        }

        public static LuaFunction<T1, T2> GetFunction<T1, T2>(this LuaTable table, string functionName)
        {
            return new LuaFunction<T1, T2>(table.GetValue<Func<object?, object?, LuaResult>>(functionName));
        }

        public static LuaFunction<T1, T2, T3> GetFunction<T1, T2, T3>(this LuaTable table, string functionName)
        {
            return new LuaFunction<T1, T2, T3>(table.GetValue<Func<object?, object?, object?, LuaResult>>(functionName));
        }
    }
}
