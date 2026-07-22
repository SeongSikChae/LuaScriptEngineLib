using Neo.IronLua;

namespace LuaScriptEngineLib
{
    /// <summary>
    /// Lua 테이블에 등록하여 호출할 수 있는 함수입니다.
    /// </summary>
    public interface ILuaFunction
    {
        /// <summary>
        /// 함수를 Lua 테이블에 등록합니다.
        /// </summary>
        /// <param name="functionName">등록할 함수 이름입니다.</param>
        /// <param name="tab">함수를 추가할 <see cref="LuaTable"/>입니다.</param>
        void Load(string functionName, LuaTable tab);

        /// <summary>
        /// 함수를 인자와 함께 호출합니다.
        /// </summary>
        /// <param name="args">함수에 전달할 인자입니다.</param>
        /// <returns>함수 호출 결과입니다.</returns>
        LuaResult? Invoke(params object[] args);
    }

    /// <summary>
    /// <see cref="ILuaFunction"/>의 기본 구현을 제공하는 추상 클래스입니다.
    /// </summary>
    public abstract class AbstractLuaFunction : ILuaFunction
    {
        /// <inheritdoc />
        public abstract LuaResult? Invoke(params object[] args);

        /// <summary>
        /// <see cref="Invoke"/> 메서드를 Lua 테이블에 등록합니다.
        /// </summary>
        /// <param name="functionName">등록할 함수 이름입니다.</param>
        /// <param name="tab">함수를 추가할 <see cref="LuaTable"/>입니다.</param>
        public virtual void Load(string functionName, LuaTable tab)
        {
            LuaMethod m = new LuaMethod(this, this.GetType().GetMethod("Invoke", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance));
            tab.Add(functionName, m);
        }
    }
}
