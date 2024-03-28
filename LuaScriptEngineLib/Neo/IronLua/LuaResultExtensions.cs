namespace Neo.IronLua
{
    public static class LuaResultExtensions
    {
        public static T? GetOptionalValue<T>(this LuaResult result, int index)
        {
            return result.GetValueOrDefault(index, default(T));
        }
    }
}
