using Neo.IronLua;
using LuaScriptEngineLib.Functions;

namespace LuaScriptEngineLib
{
    /// <summary>
    /// 라이브러리·함수를 등록하고 <see cref="ILuaScriptEngine"/> 인스턴스를 생성합니다.
    /// </summary>
    public class LuaScriptEngineFactory
    {
        /// <summary>
        /// 엔진 생성 시 전역 환경에 로드할 라이브러리를 등록합니다.
        /// </summary>
        /// <param name="library">등록할 라이브러리입니다.</param>
        public void AddLibrary(ILuaLibrary library)
        {
            libSet.Add(library);
        }

        private readonly HashSet<ILuaLibrary> libSet = new HashSet<ILuaLibrary>();

        /// <summary>
        /// 엔진 생성 시 전역 환경에 등록할 함수를 추가합니다. 동일 이름이 있으면 덮어씁니다.
        /// </summary>
        /// <param name="functionName">등록할 함수 이름입니다.</param>
        /// <param name="function">등록할 함수입니다.</param>
        public void AddFunction(string functionName, ILuaFunction function)
        {
            if (!functionDic.TryAdd(functionName, function))
                functionDic[functionName] = function;
        }

        private readonly Dictionary<string, ILuaFunction> functionDic = new Dictionary<string, ILuaFunction>();

        /// <summary>
        /// 출력 Emitter를 지정하여 스크립트 엔진을 생성합니다.
        /// </summary>
        /// <param name="emitter">스크립트 출력을 수신하는 Emitter입니다.</param>
        /// <returns>생성된 스크립트 엔진입니다.</returns>
        public ILuaScriptEngine CreateEngine(ILuaScriptEngineOutputEmitter? emitter)
        {
            return CreateEngine(emitter, null, CancellationToken.None);
        }

        /// <summary>
        /// 출력 Emitter와 컴파일 옵션을 지정하여 스크립트 엔진을 생성합니다.
        /// </summary>
        /// <param name="emitter">스크립트 출력을 수신하는 Emitter입니다.</param>
        /// <param name="options">Lua 컴파일 옵션입니다. <see langword="null"/>이면 기본 옵션을 사용합니다.</param>
        /// <returns>생성된 스크립트 엔진입니다.</returns>
        public ILuaScriptEngine CreateEngine(ILuaScriptEngineOutputEmitter? emitter, LuaCompileOptions? options)
        {
            return CreateEngine(emitter, options, CancellationToken.None);
        }

        /// <summary>
        /// 출력 Emitter와 취소 토큰을 지정하여 스크립트 엔진을 생성합니다.
        /// </summary>
        /// <param name="emitter">스크립트 출력을 수신하는 Emitter입니다.</param>
        /// <param name="cancellationToken">스크립트 실행을 취소하는 데 사용하는 토큰입니다.</param>
        /// <returns>생성된 스크립트 엔진입니다.</returns>
        public ILuaScriptEngine CreateEngine(ILuaScriptEngineOutputEmitter? emitter, CancellationToken cancellationToken)
        {
            return CreateEngine(emitter, null, cancellationToken);
        }

        /// <summary>
        /// 출력 Emitter, 컴파일 옵션, 취소 토큰을 지정하여 스크립트 엔진을 생성합니다.
        /// </summary>
        /// <param name="emitter">스크립트 출력을 수신하는 Emitter입니다.</param>
        /// <param name="options">Lua 컴파일 옵션입니다. <see langword="null"/>이면 샌드박스·취소를 위한 기본 옵션을 사용합니다.</param>
        /// <param name="cancellationToken">스크립트 실행을 취소하는 데 사용하는 토큰입니다.</param>
        /// <returns>생성된 스크립트 엔진입니다.</returns>
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

        /// <summary>
        /// <see cref="ILuaScriptEngine"/>의 기본 구현입니다.
        /// </summary>
        /// <param name="cancellationToken">동기 실행 시 취소를 위해 사용하는 토큰입니다.</param>
        private sealed class LuaScriptEngine(CancellationToken cancellationToken) : ILuaScriptEngine
        {
            /// <summary>
            /// Lua 런타임과 전역 환경, 출력 Emitter를 초기화합니다.
            /// </summary>
            /// <param name="lua">사용할 <see cref="Lua"/> 인스턴스입니다.</param>
            /// <param name="g">전역 환경입니다.</param>
            /// <param name="emitter">출력 Emitter입니다. <see langword="null"/>이면 출력을 무시합니다.</param>
            public void Initialize(Lua lua, LuaGlobal g, ILuaScriptEngineOutputEmitter? emitter)
            {
                this.lua = lua;
                this.Globals = g;
                this.Emitter = emitter ?? new NoOutputEmitter();
            }

            private Lua? lua;

            /// <inheritdoc />
            public ILuaScriptEngineOutputEmitter? Emitter { get; private set; }

            private LuaGlobal? Globals { get; set; }

            /// <inheritdoc />
            public async Task<LuaResult?> EvalAsync(string source, CancellationToken cancellationToken)
            {
                return await Task.Run(() =>
                {
                    return Globals?.DoChunk(source, "chunk");
                }, cancellationToken);
            }

            /// <inheritdoc />
            public LuaResult? Eval(string source, TimeSpan timeout)
            {
                using Task<LuaResult?> task = EvalAsync(source, cancellationToken);
                if (task.Wait(timeout, cancellationToken))
                    return task.GetAwaiter().GetResult();
                throw new TimeoutException($"Script evaluation timed out after {timeout}.");
            }

            /// <inheritdoc />
            public LuaFunction<T> GetFunction<T>(string functionName)
            {
                LuaFunction<T>? function = Globals?.GetFunction<T>(functionName);
                ArgumentNullException.ThrowIfNull(function);
                return function;
            }

            /// <inheritdoc />
            public LuaFunction<T1, T2> GetFunction<T1, T2>(string functionName)
            {
                LuaFunction<T1, T2>? function = Globals?.GetFunction<T1, T2>(functionName);
                ArgumentNullException.ThrowIfNull(function);
                return function;
            }

            /// <inheritdoc />
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

            /// <inheritdoc />
            public void Dispose()
            {
                // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// 출력을 모두 무시하는 Emitter입니다.
        /// </summary>
        private sealed class NoOutputEmitter : ILuaScriptEngineOutputEmitter
        {
            /// <inheritdoc />
            public void Print(string s)
            {
            }

            /// <inheritdoc />
            public void Error(Exception e)
            {
            }
        }
    }
}
