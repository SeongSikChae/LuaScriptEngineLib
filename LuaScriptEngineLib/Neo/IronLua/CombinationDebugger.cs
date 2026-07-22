using System.Linq.Expressions;
using System.Reflection;

namespace Neo.IronLua
{
    internal sealed class CombinationDebugger(ILuaDebug cancallationDebugger, ILuaDebug debug) : ILuaDebug
    {
        public LuaDebugLevel Level => debug.Level;

        public LuaChunk CreateChunk(Lua lua, LambdaExpression expr)
        {
            LuaChunk chunk = cancallationDebugger.CreateChunk(lua, expr);
            LuaChunk chunk2 = debug.CreateChunk(lua, expr);

            PropertyInfo? chunkInfo = chunk.GetType().GetProperty("Chunk", BindingFlags.NonPublic | BindingFlags.Instance);
            ArgumentNullException.ThrowIfNull(chunkInfo);

            Delegate? delegate1 = chunkInfo.GetValue(chunk) as Delegate;
            Delegate? delegate2 = chunkInfo.GetValue(chunk2) as Delegate;

            return new CombinationLuaChunk(lua, expr.Name ?? string.Empty, Delegate.Combine(delegate1, delegate2) ?? expr.Compile());
        }

        class CombinationLuaChunk(Lua lua, string name, Delegate chunk) : LuaChunk(lua, name, chunk)
        {
        }
    }
}
