namespace Neo.IronLua
{
    /// <summary>
    /// <see cref="LuaResult"/>에 대한 확장 메서드를 제공합니다.
    /// </summary>
    public static class LuaResultExtensions
    {
        /// <summary>
        /// 지정한 인덱스의 값을 가져오며, 없거나 변환할 수 없으면 형식의 기본값을 반환합니다.
        /// </summary>
        /// <typeparam name="T">가져올 값의 형식입니다.</typeparam>
        /// <param name="result">값을 조회할 <see cref="LuaResult"/>입니다.</param>
        /// <param name="index">조회할 결과 인덱스입니다.</param>
        /// <returns>해당 인덱스의 값이거나, 없으면 <typeparamref name="T"/>의 기본값입니다.</returns>
        public static T? GetOptionalValue<T>(this LuaResult result, int index)
        {
            return result.GetValueOrDefault(index, default(T));
        }
    }
}
