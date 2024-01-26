using Neo.IronLua;

namespace LuaScriptEngineLib
{
    using Functions;
    using System.Threading.Tasks;

    public class LuaScriptEngineFactory
    {
        public void AddLibrary(ILuaLibrary library)
        {
            libSet.Add(library);
        }

        private readonly HashSet<ILuaLibrary> libSet = new HashSet<ILuaLibrary>();

        public void AddFunction(string functionName, ILuaFunction function)
        {
            if (!functionDic.TryAdd(functionName, function))
                functionDic[functionName] = function;
        }

        private readonly Dictionary<string, ILuaFunction> functionDic = new Dictionary<string, ILuaFunction>();

        public ILuaScriptEngine CreateEngine(ILuaScriptEngineOutputEmitter? emitter)
        {
            return CreateEngine(emitter, CancellationToken.None);
        }

        public ILuaScriptEngine CreateEngine(ILuaScriptEngineOutputEmitter? emitter, CancellationToken cancellationToken)
        {
            Lua lua = new Lua();
            LuaGlobal g = lua.CreateEnvironment();
            g.DefaultCompileOptions = new LuaCompileOptions
            {
                DebugEngine = new CancallationDebugEngine(cancellationToken)
            };
            if (emitter is not NoOutputEmitter)
                g.AddFunction("print", new PrintFunction(emitter));
            foreach (ILuaLibrary lib in libSet)
                g.AddLibrary(lib);
            foreach (KeyValuePair<string, ILuaFunction> pair in functionDic)
                g.AddFunction(pair.Key, pair.Value);
            LuaScriptEngine engine = new LuaScriptEngine();
            engine.Initialize(lua, g, emitter);
            return engine;
        }

        private sealed class LuaScriptEngine : ILuaScriptEngine
        {
            public void Initialize(Lua lua, LuaGlobal g, ILuaScriptEngineOutputEmitter? emitter)
            {
                this.lua = lua;
                this.Globals = g;
                this.Emitter = emitter ?? new NoOutputEmitter();
            }

            private Lua? lua;

            public ILuaScriptEngineOutputEmitter? Emitter { get; private set; }

            public LuaGlobal? Globals { get; private set; }

            public async Task EvalAsync(string source)
            {
                await Task.Run(() =>
                {
                    Globals?.DoChunk(source, "chunk");
                });
            }

            private bool disposedValue;

            private void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: 관리형 상태(관리형 개체)를 삭제합니다.
                        Globals?.Clear();
                        lua?.Dispose();
                    }

                    // TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
                    // TODO: 큰 필드를 null로 설정합니다.
                    disposedValue = true;
                }
            }

            // // TODO: 비관리형 리소스를 해제하는 코드가 'Dispose(bool disposing)'에 포함된 경우에만 종료자를 재정의합니다.
            // ~LuaScriptEngine()
            // {
            //     // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
            //     Dispose(disposing: false);
            // }

            public void Dispose()
            {
                // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }

        private sealed class NoOutputEmitter : ILuaScriptEngineOutputEmitter
        {
            public void Print(string s)
            {
            }

            public void Error(Exception e)
            {
            }
        }
    }
}
