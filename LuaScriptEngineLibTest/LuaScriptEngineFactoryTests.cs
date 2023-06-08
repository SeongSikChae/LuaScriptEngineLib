using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IronLua;
using System.Diagnostics;

namespace LuaScriptEngineLib.Tests
{
    [TestClass]
    public class LuaScriptEngineFactoryTests
    {
        [TestMethod]
        public void CreateEngineTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter());
            engine.Eval("print('Hello World!!');");
        }

        private sealed class TraceEmitter : ILuaScriptEngineOutputEmitter
        {
            public void Print(string message)
            {
                Trace.Write(message);
                Assert.AreEqual("Hello World!!" + Environment.NewLine, message);
            }
        }

        [TestMethod]
        public void CreateEngineTest2()
        {
            CountdownEvent e = new CountdownEvent(1);

            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            factory.AddLibrary(new TestLibrary(e));
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter2());
            engine.Eval("print(test.test());");
            Assert.IsTrue(e.Wait(TimeSpan.FromSeconds(5)));
        }

        private sealed class TraceEmitter2 : ILuaScriptEngineOutputEmitter
        {
            public void Print(string message)
            {
                Trace.Write(message);
            }
        }

        private sealed class TestLibrary : ILuaLibrary
        {
            public TestLibrary(CountdownEvent e)
            {
                this.e = e;
            }

            private readonly CountdownEvent e;

            public void Load(LuaGlobal g)
            {
                LuaTable tab = new LuaTable();
                TestFunction function = new TestFunction(e);
                function.Load("test", tab);
                g.Add("test", tab);
            }

            private sealed class TestFunction : ILuaFunction
            {
                public TestFunction(CountdownEvent e)
                {
                    this.e = e;
                }

                private readonly CountdownEvent e;

                public LuaResult? Invoke(params object[] args)
                {
                    e.Signal(1);
                    return null;
                }

                public void Load(string functionName, LuaTable tab)
                {
                    LuaMethod m = new LuaMethod(this, typeof(TestFunction).GetMethod("Invoke", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance));
                    tab.Add(functionName, m);
                }
            }
        }
    }
}