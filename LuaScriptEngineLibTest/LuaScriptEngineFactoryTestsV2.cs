using LuaScriptEngineLib;
using Neo.IronLua;
using System.Diagnostics;
using System.Text;

namespace LuaScriptEngineLibTest
{
    [TestClass]
    public class LuaScriptEngineFactoryTestsV2
    {
        private sealed class TraceEmitter : ILuaScriptEngineOutputEmitter
        {
            public void Error(Exception e)
            {
                Trace.TraceError(e.Message);
            }

            public void Print(string message)
            {
                Trace.Write(message);
            }
        }

        [TestMethod]
        public async Task CreateEngineTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await engine.EvalAsync("print('Hello World!!');", TestContext.CancellationToken);
        }

        private sealed class TestFunc : AbstractLuaFunction
        {
            public override LuaResult? Invoke(params object[] args)
            {
                return new LuaResult(args[0]);
            }
        }

        [TestMethod]
        public async Task AddFunctionTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            factory.AddFunction("testFunc", new TestFunc());
            StringBuilder scriptBuilder = new StringBuilder();
            scriptBuilder.AppendLine("print('A');");
            scriptBuilder.AppendLine("local b = testFunc('A');");
            scriptBuilder.AppendLine("print(b);");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);
        }

        private sealed class TestLib : ILuaLibrary
        {
            public void Load(LuaGlobal g)
            {
                LuaTable tab = new LuaTable();
                tab.AddFunction("testFunc", new TestFunc());
                g.Add("testLib", tab);
            }
        }

        [TestMethod]
        public async Task AddLibraryTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            factory.AddLibrary(new TestLib());
            StringBuilder scriptBuilder = new StringBuilder();
            scriptBuilder.AppendLine("print('A');");
            scriptBuilder.AppendLine("local b = testLib.testFunc('A');");
            scriptBuilder.AppendLine("print(b);");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);
        }

        [TestMethod]
        public async Task CallTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            StringBuilder scriptBuilder = new StringBuilder();
            scriptBuilder.AppendLine("function call(str)");
            scriptBuilder.AppendLine("\tprint(str);");
            scriptBuilder.AppendLine("end");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);

            LuaFunction<string> callFunc = engine.GetFunction<string>("call");
            _ = callFunc.Invoke("Hello, World!!");
        }

        [TestMethod]
        public async Task CallTest2()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            StringBuilder scriptBuilder = new StringBuilder();
            scriptBuilder.AppendLine("function call(str1, str2)");
            scriptBuilder.AppendLine("\tprint(str1);");
            scriptBuilder.AppendLine("\tprint(str2);");
            scriptBuilder.AppendLine("end");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);

            LuaFunction<string, string> callFunc = engine.GetFunction<string, string>("call");
            _ = callFunc.Invoke("Hello, World!!", "Test");
        }

        [TestMethod]
        public async Task SendboxClrTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            StringBuilder scriptBuilder = new StringBuilder("clr.System.IO.File.WriteAllText(\"out.txt\", \"hello from lua\")");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await Assert.ThrowsExactlyAsync<LuaRuntimeException>(async () =>
            {
                await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);
            });
        }

        [TestMethod]
        public async Task SendboxIOTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            StringBuilder scriptBuilder = new StringBuilder("print(io)\nlocal f = io.open(\"hello.txt\", \"w\")\nf:close()");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await Assert.ThrowsExactlyAsync<AccessViolationException>(async () =>
            {
                await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);
            });
        }

        [TestMethod]
        public async Task SendboxOSTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            StringBuilder scriptBuilder = new StringBuilder("print(os.time())");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await Assert.ThrowsExactlyAsync<AccessViolationException>(async () =>
            {
                await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);
            });
        }

        [TestMethod]
        public async Task SendboxPackageTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            StringBuilder scriptBuilder = new StringBuilder("print(package.path)");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await Assert.ThrowsExactlyAsync<AccessViolationException>(async () =>
            {
                await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);
            });
        }

        [TestMethod]
        public async Task SendboxDebugTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            StringBuilder scriptBuilder = new StringBuilder("print(debug.traceback('stack dump'))");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await Assert.ThrowsExactlyAsync<AccessViolationException>(async () =>
            {
                await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);
            });
        }

        [TestMethod]
        public async Task SendboxDoFileTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            StringBuilder scriptBuilder = new StringBuilder("dofile('helper.lua')");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await Assert.ThrowsExactlyAsync<AccessViolationException>(async () =>
            {
                await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);
            });
        }

        [TestMethod]
        public async Task SendboxLoadFileTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            StringBuilder scriptBuilder = new StringBuilder("loadfile('helper.lua')");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await Assert.ThrowsExactlyAsync<AccessViolationException>(async () =>
            {
                await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);
            });
        }

        [TestMethod]
        public async Task SendboxRequireTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            StringBuilder scriptBuilder = new StringBuilder("require('mylib')");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await Assert.ThrowsExactlyAsync<AccessViolationException>(async () =>
            {
                await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);
            });
        }

        [TestMethod]
        public async Task SendboxLoadTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            StringBuilder scriptBuilder = new StringBuilder("load('return 1')");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await Assert.ThrowsExactlyAsync<AccessViolationException>(async () =>
            {
                await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);
            });
        }

        [TestMethod]
        public async Task SendboxDoChunkTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            StringBuilder scriptBuilder = new StringBuilder("dochunk('return 1')");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await Assert.ThrowsExactlyAsync<AccessViolationException>(async () =>
            {
                await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);
            });
        }

        [TestMethod]
        public async Task SendboxGetMetaTableTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            StringBuilder scriptBuilder = new StringBuilder("getmetatable({})");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await Assert.ThrowsExactlyAsync<AccessViolationException>(async () =>
            {
                await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);
            });
        }

        [TestMethod]
        public async Task SendboxSetMetaTableTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            StringBuilder scriptBuilder = new StringBuilder("setmetatable({}, {})");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await Assert.ThrowsExactlyAsync<AccessViolationException>(async () =>
            {
                await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);
            });
        }

        [TestMethod]
        public async Task SendboxRawGetTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            StringBuilder scriptBuilder = new StringBuilder("rawget({}, 'k')");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await Assert.ThrowsExactlyAsync<AccessViolationException>(async () =>
            {
                await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);
            });
        }

        [TestMethod]
        public async Task SendboxRawSetTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            StringBuilder scriptBuilder = new StringBuilder("rawset({}, 'k', 1)");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await Assert.ThrowsExactlyAsync<AccessViolationException>(async () =>
            {
                await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);
            });
        }

        [TestMethod]
        public async Task SendboxRawEqualTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            StringBuilder scriptBuilder = new StringBuilder("rawequal(1, 1)");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await Assert.ThrowsExactlyAsync<AccessViolationException>(async () =>
            {
                await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);
            });
        }

        [TestMethod]
        public async Task SendboxRawLenTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            StringBuilder scriptBuilder = new StringBuilder("rawlen({})");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await Assert.ThrowsExactlyAsync<AccessViolationException>(async () =>
            {
                await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);
            });
        }

        [TestMethod]
        public async Task SendboxCollectGarbageTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            StringBuilder scriptBuilder = new StringBuilder("collectgarbage('collect')");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await Assert.ThrowsExactlyAsync<AccessViolationException>(async () =>
            {
                await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);
            });
        }

        [TestMethod]
        public async Task SendboxRawMembersTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            StringBuilder scriptBuilder = new StringBuilder("rawmembers({})");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await Assert.ThrowsExactlyAsync<AccessViolationException>(async () =>
            {
                await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);
            });
        }

        [TestMethod]
        public async Task SendboxRawArrayTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            StringBuilder scriptBuilder = new StringBuilder("rawarray({})");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter(), TestContext.CancellationToken);
            await Assert.ThrowsExactlyAsync<AccessViolationException>(async () =>
            {
                await engine.EvalAsync(scriptBuilder.ToString(), TestContext.CancellationToken);
            });
        }

        public TestContext TestContext { get; set; }
    }
}
