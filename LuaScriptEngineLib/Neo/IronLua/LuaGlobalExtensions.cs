using LuaScriptEngineLib;

namespace Neo.IronLua
{
    /// <summary>
    /// <see cref="LuaGlobal"/>에 대한 확장 메서드를 제공합니다.
    /// </summary>
    public static class LuaGlobalExtensions
    {
        /// <summary>
        /// <see cref="ILuaLibrary"/>를 Lua 전역 환경에 로드합니다.
        /// </summary>
        /// <param name="g">라이브러리를 등록할 <see cref="LuaGlobal"/>입니다.</param>
        /// <param name="library">로드할 라이브러리입니다.</param>
        public static void AddLibrary(this LuaGlobal g, ILuaLibrary library)
        {
            library.Load(g.Add);
        }
    }
}
