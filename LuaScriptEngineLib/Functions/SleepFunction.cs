using Neo.IronLua;

namespace LuaScriptEngineLib.Functions
{
    public sealed class SleepFunction : ILuaFunction
    {
        public LuaResult? Invoke(params object[] args)
        {
            Thread.Sleep((int)args[0]);
            return null;
        }

        public void Load(string functionName, LuaTable tab)
        {
            LuaMethod m = new LuaMethod(this, typeof(SleepFunction).GetMethod("Invoke", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance));
            tab.Add(functionName, m);
        }
    }
}
