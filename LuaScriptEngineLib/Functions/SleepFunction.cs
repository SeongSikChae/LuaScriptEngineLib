using Neo.IronLua;

namespace LuaScriptEngineLib.Functions
{
    /// <summary>
    /// 지정한 밀리초 동안 현재 스레드를 대기시키는 Lua 함수입니다.
    /// </summary>
    public sealed class SleepFunction : AbstractLuaFunction
    {
        /// <summary>
        /// 첫 번째 인자(밀리초)만큼 현재 스레드를 대기합니다.
        /// </summary>
        /// <param name="args">대기 시간(밀리초)이 담긴 인자 배열입니다. <c>args[0]</c>을 사용합니다.</param>
        /// <returns>항상 <see langword="null"/>입니다.</returns>
        public override LuaResult? Invoke(params object[] args)
        {
            Thread.Sleep((int)args[0]);
            return null;
        }
    }
}
