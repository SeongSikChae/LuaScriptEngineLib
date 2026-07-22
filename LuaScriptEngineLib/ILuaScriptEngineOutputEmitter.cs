namespace LuaScriptEngineLib
{
    /// <summary>
    /// Lua 스크립트 엔진의 출력(표준 출력·오류)을 수신하는 Emitter입니다.
    /// </summary>
    public interface ILuaScriptEngineOutputEmitter
    {
        /// <summary>
        /// 스크립트의 일반 출력 메시지를 내보냅니다.
        /// </summary>
        /// <param name="message">출력할 메시지입니다.</param>
        void Print(string message);

        /// <summary>
        /// 스크립트 실행 중 발생한 예외를 내보냅니다.
        /// </summary>
        /// <param name="e">발생한 예외입니다.</param>
        void Error(Exception e);
    }
}
