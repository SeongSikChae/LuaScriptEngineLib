using Neo.IronLua;

namespace LuaScriptEngineLib.Functions
{
    public sealed class SleepFunction : AbstractLuaFunction
    {
        public override LuaResult? Invoke(params object[] args)
        {
            Thread.Sleep((int)args[0]);
            return null;
        }
    }
}
