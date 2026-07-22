namespace Neo.IronLua
{
    internal class LuaSecurityException(string name) : LuaTable
    {
        protected override LuaResult OnCall(object[] args)
        {
            throw new AccessViolationException($"Access to '{name}' library denied.");
        }

        protected override object OnIndex(object key)
        {
            throw new AccessViolationException($"Access to '{name}.{key}' or '{name}:{key}' denied.");
        }

        protected override bool OnNewIndex(object key, object value)
        {
            throw new AccessViolationException($"Assignment of '{value}' to '{name}.{key}' or '{name}:{key}' denied.");
        }
    }
}
