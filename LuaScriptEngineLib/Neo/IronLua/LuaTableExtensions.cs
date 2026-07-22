using LuaScriptEngineLib;

namespace Neo.IronLua
{
    /// <summary>
    /// <see cref="LuaTable"/>에 대한 확장 메서드를 제공합니다.
    /// </summary>
    public static class LuaTableExtensions
    {
        /// <summary>
        /// <see cref="ILuaFunction"/>을 테이블에 등록합니다.
        /// </summary>
        /// <param name="g">함수를 등록할 <see cref="LuaTable"/>입니다.</param>
        /// <param name="functionName">등록할 함수 이름입니다.</param>
        /// <param name="function">등록할 함수입니다.</param>
        public static void AddFunction(this LuaTable g, string functionName, ILuaFunction function)
        {
            function.Load(functionName, g);
        }

        /// <summary>
        /// 지정한 이름의 값을 지정한 형식으로 가져옵니다.
        /// </summary>
        /// <typeparam name="T">가져올 값의 형식입니다.</typeparam>
        /// <param name="table">값을 조회할 <see cref="LuaTable"/>입니다.</param>
        /// <param name="name">조회할 키 이름입니다.</param>
        /// <param name="rawGet"><see langword="true"/>이면 메타테이블을 거치지 않고 직접 조회합니다.</param>
        /// <returns>해당 키의 값입니다.</returns>
        public static T GetValue<T>(this LuaTable table, string name, bool rawGet = false)
        {
            return (T) table.GetValue(name, rawGet);
        }

        /// <summary>
        /// 지정한 이름의 값을 가져오며, 없거나 변환할 수 없으면 형식의 기본값을 반환합니다.
        /// </summary>
        /// <typeparam name="T">가져올 값의 형식입니다.</typeparam>
        /// <param name="table">값을 조회할 <see cref="LuaTable"/>입니다.</param>
        /// <param name="name">조회할 키 이름입니다.</param>
        /// <param name="rawGet"><see langword="true"/>이면 메타테이블을 거치지 않고 직접 조회합니다.</param>
        /// <returns>해당 키의 값이거나, 없으면 <typeparamref name="T"/>의 기본값입니다.</returns>
        public static T? GetOptionalValue<T>(this LuaTable table, string name, bool rawGet = false)
        {
            return table.GetOptionalValue<T?>(name, default(T));
        }

        /// <summary>
        /// 인자가 하나인 Lua 함수를 가져옵니다.
        /// </summary>
        /// <typeparam name="T">첫 번째 인자의 형식입니다.</typeparam>
        /// <param name="table">함수를 조회할 <see cref="LuaTable"/>입니다.</param>
        /// <param name="functionName">가져올 함수 이름입니다.</param>
        /// <returns>형식화된 Lua 함수 래퍼입니다.</returns>
        public static LuaFunction<T> GetFunction<T>(this LuaTable table, string functionName)
        {
            return new LuaFunction<T>(table.GetValue<Func<object?, LuaResult>>(functionName));
        }

        /// <summary>
        /// 인자가 두 개인 Lua 함수를 가져옵니다.
        /// </summary>
        /// <typeparam name="T1">첫 번째 인자의 형식입니다.</typeparam>
        /// <typeparam name="T2">두 번째 인자의 형식입니다.</typeparam>
        /// <param name="table">함수를 조회할 <see cref="LuaTable"/>입니다.</param>
        /// <param name="functionName">가져올 함수 이름입니다.</param>
        /// <returns>형식화된 Lua 함수 래퍼입니다.</returns>
        public static LuaFunction<T1, T2> GetFunction<T1, T2>(this LuaTable table, string functionName)
        {
            return new LuaFunction<T1, T2>(table.GetValue<Func<object?, object?, LuaResult>>(functionName));
        }

        /// <summary>
        /// 인자가 세 개인 Lua 함수를 가져옵니다.
        /// </summary>
        /// <typeparam name="T1">첫 번째 인자의 형식입니다.</typeparam>
        /// <typeparam name="T2">두 번째 인자의 형식입니다.</typeparam>
        /// <typeparam name="T3">세 번째 인자의 형식입니다.</typeparam>
        /// <param name="table">함수를 조회할 <see cref="LuaTable"/>입니다.</param>
        /// <param name="functionName">가져올 함수 이름입니다.</param>
        /// <returns>형식화된 Lua 함수 래퍼입니다.</returns>
        public static LuaFunction<T1, T2, T3> GetFunction<T1, T2, T3>(this LuaTable table, string functionName)
        {
            return new LuaFunction<T1, T2, T3>(table.GetValue<Func<object?, object?, object?, LuaResult>>(functionName));
        }
    }
}
