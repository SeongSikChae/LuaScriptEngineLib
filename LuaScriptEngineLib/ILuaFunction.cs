using Neo.IronLua;

namespace LuaScriptEngineLib
{
    public interface ILuaFunction
    {
        void Load(string functionName, LuaTable tab);

        LuaResult? Invoke(params object[] args);
    }

    public abstract class AbstractLuaFunction : ILuaFunction
    {
        public abstract LuaResult? Invoke(params object[] args);

        public virtual void Load(string functionName, LuaTable tab)
        {
            LuaMethod m = new LuaMethod(this, this.GetType().GetMethod("Invoke", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance));
            tab.Add(functionName, m);
        }
    }
}
