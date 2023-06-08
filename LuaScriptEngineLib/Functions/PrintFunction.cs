using Neo.IronLua;

namespace LuaScriptEngineLib.Functions
{
    public sealed class PrintFunction : ILuaFunction
    {
        public PrintFunction(ILuaScriptEngineOutputEmitter? emitter)
        {
            this.emitter = emitter;
        }

        private readonly ILuaScriptEngineOutputEmitter? emitter;

        public void Load(string functionName, LuaTable tab)
        {
            toString = tab["tostring"] as LuaMethod;
            LuaMethod m = new LuaMethod(this, typeof(PrintFunction).GetMethod("Invoke", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance));
            tab.SetValue(functionName, m);
        }

        private LuaMethod? toString;

        public LuaResult? Invoke(params object[] args)
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
