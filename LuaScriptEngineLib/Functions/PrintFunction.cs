using Neo.IronLua;

namespace LuaScriptEngineLib.Functions
{
    /// <summary>
    /// Lua <c>print</c> 함수를 구현하며, 출력을 <see cref="ILuaScriptEngineOutputEmitter"/>로 전달합니다.
    /// </summary>
    /// <param name="emitter">출력 메시지를 수신하는 Emitter입니다. <see langword="null"/>이면 출력을 무시합니다.</param>
    public sealed class PrintFunction(ILuaScriptEngineOutputEmitter? emitter) : AbstractLuaFunction
    {
        private readonly ILuaScriptEngineOutputEmitter? emitter = emitter;

        /// <summary>
        /// 함수를 Lua 테이블에 등록하고, 테이블의 <c>tostring</c>을 캐시합니다.
        /// </summary>
        /// <param name="functionName">등록할 함수 이름입니다.</param>
        /// <param name="tab">함수를 추가할 <see cref="LuaTable"/>입니다.</param>
        public override void Load(string functionName, LuaTable tab)
        {
            toString = tab["tostring"] as LuaMethod;
            LuaMethod m = new LuaMethod(this, typeof(PrintFunction).GetMethod("Invoke", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance));
            tab.SetValue(functionName, m);
        }

        private LuaMethod? toString;

        /// <summary>
        /// 인자를 문자열로 변환해 한 줄로 출력합니다.
        /// </summary>
        /// <param name="args">출력할 인자입니다.</param>
        /// <returns>항상 <see langword="null"/>입니다.</returns>
        public override LuaResult? Invoke(params object[] args)
        {
            StringWriter sw = new StringWriter();
            if (toString is not null)
            {
                foreach (object obj in args)
                {
                    LuaResult r = new LuaResult(toString.Delegate.DynamicInvoke([obj]));
                    sw.Write(r.ToString());
                }
                sw.WriteLine();
            }
            emitter?.Print(sw.ToString());
            return null;
        }
    }
}
