using Neo.IronLua;
using LuaScriptEngineLib.Functions;

namespace LuaScriptEngineLib
{
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
            return CreateEngine(emitter, null, CancellationToken.None);
        }

        public ILuaScriptEngine CreateEngine(ILuaScriptEngineOutputEmitter? emitter, LuaCompileOptions? options)
        {
            return CreateEngine(emitter, options, CancellationToken.None);
        }

        public ILuaScriptEngine CreateEngine(ILuaScriptEngineOutputEmitter? emitter, CancellationToken cancellationToken)
        {
            return CreateEngine(emitter, null, cancellationToken);
        }

        public ILuaScriptEngine CreateEngine(ILuaScriptEngineOutputEmitter? emitter, LuaCompileOptions? options, CancellationToken cancellationToken)
        {
            Lua lua = new Lua();
            LuaGlobal g = lua.CreateEnvironment();
            lua.CreateEnvironment();
            if (options != null)
            {
                if (options.DebugEngine is not null)
                    options.DebugEngine = new CombinationDebugger(new CancallationDebugger(cancellationToken), options.DebugEngine);
                else
                    options.DebugEngine = new CancallationDebugger(cancellationToken);
            }
            else 
            {
                options = new LuaCompileOptions
                {
                    DebugEngine = new CancallationDebugger(cancellationToken),
                    ClrEnabled = false
                };
            }

            if (options.SendboxEnabled)
            {
                g["io"] = new LuaSecurityException("io");
                g["os"] = new LuaSecurityException("os");
                g["package"] = new LuaSecurityException("package");
                g["debug"] = new LuaSecurityException("debug");
                g["dofile"] = new LuaSecurityException("dofile");
                g["loadfile"] = new LuaSecurityException("loadfile");
                g["require"] = new LuaSecurityException("require");
                g["load"] = new LuaSecurityException("load");
                g["dochunk"] = new LuaSecurityException("dochunk");
                g["getmetatable"] = new LuaSecurityException("getmetatable");
                g["setmetatable"] = new LuaSecurityException("setmetatable");
                g["rawget"] = new LuaSecurityException("rawget");
                g["rawset"] = new LuaSecurityException("rawset");
                g["rawequal"] = new LuaSecurityException("rawequal");
                g["rawlen"] = new LuaSecurityException("rawlen");
                g["collectgarbage"] = new LuaSecurityException("collectgarbage");
                g["rawmembers"] = new LuaSecurityException("rawmembers");
                g["rawarray"] = new LuaSecurityException("rawarray");
            }

            g.DefaultCompileOptions = options;
            if (emitter is not NoOutputEmitter)
                g.AddFunction("print", new PrintFunction(emitter));
            foreach (ILuaLibrary lib in libSet)
                g.AddLibrary(lib);
            foreach (KeyValuePair<string, ILuaFunction> pair in functionDic)
                g.AddFunction(pair.Key, pair.Value);
            LuaScriptEngine engine = new LuaScriptEngine(cancellationToken);
            engine.Initialize(lua, g, emitter);
            return engine;
        }

        private sealed class LuaScriptEngine(CancellationToken cancellationToken) : ILuaScriptEngine
        {
            public void Initialize(Lua lua, LuaGlobal g, ILuaScriptEngineOutputEmitter? emitter)
            {
                this.lua = lua;
                this.Globals = g;
                this.Emitter = emitter ?? new NoOutputEmitter();
            }

            private Lua? lua;

            public ILuaScriptEngineOutputEmitter? Emitter { get; private set; }

            private LuaGlobal? Globals { get; set; }

            public async Task<LuaResult?> EvalAsync(string source, CancellationToken cancellationToken)
            {
                return await Task.Run(() =>
                {
                    return Globals?.DoChunk(source, "chunk");
                }, cancellationToken);
            }

            public LuaResult? Eval(string source, TimeSpan timeout)
            {
                using Task<LuaResult?> task = EvalAsync(source, cancellationToken);
                if (task.Wait(timeout, cancellationToken))
                    return task.GetAwaiter().GetResult();
                throw new TimeoutException($"Script evaluation timed out after {timeout}.");
            }

            public LuaFunction<T> GetFunction<T>(string functionName)
            {
                LuaFunction<T>? function = Globals?.GetFunction<T>(functionName);
                ArgumentNullException.ThrowIfNull(function);
                return function;
            }

            public LuaFunction<T1, T2> GetFunction<T1, T2>(string functionName)
            {
                LuaFunction<T1, T2>? function = Globals?.GetFunction<T1, T2>(functionName);
                ArgumentNullException.ThrowIfNull(function);
                return function;
            }

            public LuaFunction<T1, T2, T3> GetFunction<T1, T2, T3>(string functionName)
            {
                LuaFunction<T1, T2, T3>? function = Globals?.GetFunction<T1, T2, T3>(functionName);
                ArgumentNullException.ThrowIfNull(function);
                return function;
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
