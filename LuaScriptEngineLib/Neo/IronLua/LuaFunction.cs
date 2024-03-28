namespace Neo.IronLua
{
    public class LuaFunction<T>(Func<object?, LuaResult> func)
    {
        public LuaResult Invoke(T arg)
        {
            return func.Invoke(arg);
        }
    }

    public class LuaFunction<T1, T2>(Func<object?, object?, LuaResult> func)
    {
        public LuaResult Invoke(T1 arg1, T2 arg2)
        {
            return func.Invoke(arg1, arg2);
        }
    }

    public class LuaFunction<T1, T2, T3>(Func<object?, object?, object?, LuaResult> func)
    {
        public LuaResult Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            return func.Invoke(arg1, arg2, arg3);
        }
    }
}
