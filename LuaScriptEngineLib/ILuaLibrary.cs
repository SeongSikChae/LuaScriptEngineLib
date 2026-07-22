using Neo.IronLua;

namespace LuaScriptEngineLib
{
    /// <summary>
    /// Lua 전역 환경에 로드할 수 있는 라이브러리입니다.
    /// </summary>
    public interface ILuaLibrary
    {
        /// <summary>
        /// 라이브러리를 Lua 전역 환경에 로드합니다.
        /// </summary>
        /// <param name="action">라이브러리 이름과 <see cref="LuaTable"/>을 전역 환경에 등록하는 콜백입니다.</param>
        void Load(Action<string, LuaTable> action);
    }
}
