using Neo.IronLua;

namespace LuaScriptEngineLib
{
    /// <summary>
    /// Lua 스크립트를 실행하고 전역 함수에 접근하는 엔진입니다.
    /// </summary>
    public interface ILuaScriptEngine : IDisposable
    {
        /// <summary>
        /// Lua 소스 코드를 비동기로 실행합니다.
        /// </summary>
        /// <param name="source">실행할 Lua 소스 코드입니다.</param>
        /// <param name="cancellationToken">실행을 취소하는 데 사용하는 토큰입니다.</param>
        /// <returns>스크립트 실행 결과입니다.</returns>
        Task<LuaResult?> EvalAsync(string source, CancellationToken cancellationToken);

        /// <summary>
        /// Lua 소스 코드를 동기적으로 실행합니다.
        /// </summary>
        /// <param name="source">실행할 Lua 소스 코드입니다.</param>
        /// <param name="timeout">최대 대기 시간입니다. 초과 시 <see cref="TimeoutException"/>이 발생합니다.</param>
        /// <returns>스크립트 실행 결과입니다.</returns>
        /// <exception cref="TimeoutException"><paramref name="timeout"/> 내에 실행이 완료되지 않은 경우 발생합니다.</exception>
        LuaResult? Eval(string source, TimeSpan timeout);

        /// <summary>
        /// 스크립트 출력을 수신하는 Emitter입니다.
        /// </summary>
        ILuaScriptEngineOutputEmitter? Emitter { get; }

        /// <summary>
        /// 인자가 하나인 Lua 전역 함수를 가져옵니다.
        /// </summary>
        /// <typeparam name="T">첫 번째 인자의 형식입니다.</typeparam>
        /// <param name="functionName">가져올 함수 이름입니다.</param>
        /// <returns>형식화된 Lua 함수 래퍼입니다.</returns>
        LuaFunction<T> GetFunction<T>(string functionName);

        /// <summary>
        /// 인자가 두 개인 Lua 전역 함수를 가져옵니다.
        /// </summary>
        /// <typeparam name="T1">첫 번째 인자의 형식입니다.</typeparam>
        /// <typeparam name="T2">두 번째 인자의 형식입니다.</typeparam>
        /// <param name="functionName">가져올 함수 이름입니다.</param>
        /// <returns>형식화된 Lua 함수 래퍼입니다.</returns>
        LuaFunction<T1, T2> GetFunction<T1, T2>(string functionName);

        /// <summary>
        /// 인자가 세 개인 Lua 전역 함수를 가져옵니다.
        /// </summary>
        /// <typeparam name="T1">첫 번째 인자의 형식입니다.</typeparam>
        /// <typeparam name="T2">두 번째 인자의 형식입니다.</typeparam>
        /// <typeparam name="T3">세 번째 인자의 형식입니다.</typeparam>
        /// <param name="functionName">가져올 함수 이름입니다.</param>
        /// <returns>형식화된 Lua 함수 래퍼입니다.</returns>
        LuaFunction<T1, T2, T3> GetFunction<T1, T2, T3>(string functionName);
    }
}
