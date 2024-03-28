using LuaScriptEngineLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter());
            await engine.EvalAsync("print('Hello World!!');");
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
            scriptBuilder.AppendLine("local a = os.date();");
            scriptBuilder.AppendLine("print(a);");
            scriptBuilder.AppendLine("local b = testFunc(a);");
            scriptBuilder.AppendLine("print(b);");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter());
            await engine.EvalAsync(scriptBuilder.ToString());
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
            scriptBuilder.AppendLine("local a = os.date();");
            scriptBuilder.AppendLine("print(a);");
            scriptBuilder.AppendLine("local b = testLib.testFunc(a);");
            scriptBuilder.AppendLine("print(b);");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter());
            await engine.EvalAsync(scriptBuilder.ToString());
        }

        [TestMethod]
        public async Task CallTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            StringBuilder scriptBuilder = new StringBuilder();
            scriptBuilder.AppendLine("function call(str)");
            scriptBuilder.AppendLine("\tprint(str);");
            scriptBuilder.AppendLine("end");
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter());
            await engine.EvalAsync(scriptBuilder.ToString());

            Assert.IsNotNull(engine.Globals);

            LuaFunction<string> callFunc = engine.Globals.GetFunction<string>("call");
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
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter());
            await engine.EvalAsync(scriptBuilder.ToString());

            Assert.IsNotNull(engine.Globals);

            LuaFunction<string, string> callFunc = engine.Globals.GetFunction<string, string>("call");
            _ = callFunc.Invoke("Hello, World!!", "Test");
        }
    }
}
