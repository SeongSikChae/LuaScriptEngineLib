namespace LuaScriptEngineLib
{
    public interface ILuaScriptEngineOutputEmitter
    {
        void Print(string message);

        void Error(Exception e);
    }
}
