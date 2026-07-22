namespace Neo.IronLua
{
    /// <summary>
    /// 인자가 하나인 Lua 함수에 대한 형식화된 래퍼입니다.
    /// </summary>
    /// <typeparam name="T">첫 번째 인자의 형식입니다.</typeparam>
    /// <param name="func">호출할 Lua 함수 대리자입니다.</param>
    public class LuaFunction<T>(Func<object?, LuaResult> func)
    {
        /// <summary>
        /// Lua 함수를 호출합니다.
        /// </summary>
        /// <param name="arg">전달할 인자입니다.</param>
        /// <returns>Lua 함수의 반환값입니다.</returns>
        public LuaResult Invoke(T arg)
        {
            return func.Invoke(arg);
        }
    }

    /// <summary>
    /// 인자가 두 개인 Lua 함수에 대한 형식화된 래퍼입니다.
    /// </summary>
    /// <typeparam name="T1">첫 번째 인자의 형식입니다.</typeparam>
    /// <typeparam name="T2">두 번째 인자의 형식입니다.</typeparam>
    /// <param name="func">호출할 Lua 함수 대리자입니다.</param>
    public class LuaFunction<T1, T2>(Func<object?, object?, LuaResult> func)
    {
        /// <summary>
        /// Lua 함수를 호출합니다.
        /// </summary>
        /// <param name="arg1">첫 번째 인자입니다.</param>
        /// <param name="arg2">두 번째 인자입니다.</param>
        /// <returns>Lua 함수의 반환값입니다.</returns>
        public LuaResult Invoke(T1 arg1, T2 arg2)
        {
            return func.Invoke(arg1, arg2);
        }
    }

    /// <summary>
    /// 인자가 세 개인 Lua 함수에 대한 형식화된 래퍼입니다.
    /// </summary>
    /// <typeparam name="T1">첫 번째 인자의 형식입니다.</typeparam>
    /// <typeparam name="T2">두 번째 인자의 형식입니다.</typeparam>
    /// <typeparam name="T3">세 번째 인자의 형식입니다.</typeparam>
    /// <param name="func">호출할 Lua 함수 대리자입니다.</param>
    public class LuaFunction<T1, T2, T3>(Func<object?, object?, object?, LuaResult> func)
    {
        /// <summary>
        /// Lua 함수를 호출합니다.
        /// </summary>
        /// <param name="arg1">첫 번째 인자입니다.</param>
        /// <param name="arg2">두 번째 인자입니다.</param>
        /// <param name="arg3">세 번째 인자입니다.</param>
        /// <returns>Lua 함수의 반환값입니다.</returns>
        public LuaResult Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            return func.Invoke(arg1, arg2, arg3);
        }
    }
}
