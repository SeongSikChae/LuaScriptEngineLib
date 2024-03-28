using Neo.IronLua;

namespace LuaScriptEngineLib.Functions
{
    public sealed class PrintFunction(ILuaScriptEngineOutputEmitter? emitter) : AbstractLuaFunction
    {
        private readonly ILuaScriptEngineOutputEmitter? emitter = emitter;

        public override void Load(string functionName, LuaTable tab)
        {
            toString = tab["tostring"] as LuaMethod;
            LuaMethod m = new LuaMethod(this, typeof(PrintFunction).GetMethod("Invoke", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance));
            tab.SetValue(functionName, m);
        }

        private LuaMethod? toString;

        public override LuaResult? Invoke(params object[] args)
        {
            StringWriter sw = new StringWriter();
            if (toString is not null)
            {
                foreach (object obj in args)
                {
                    LuaResult r = new LuaResult(toString.Delegate.DynamicInvoke(new object[] { obj }));
                    sw.Write(r.ToString());
                }
                sw.WriteLine();
            }
            emitter?.Print(sw.ToString());
            return null;
        }
    }
}
