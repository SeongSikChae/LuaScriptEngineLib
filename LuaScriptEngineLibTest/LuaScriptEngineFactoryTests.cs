﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IronLua;
using System.Diagnostics;

namespace LuaScriptEngineLib.Tests
{
    using Functions;

    [TestClass]
    public class LuaScriptEngineFactoryTests
    {
        [TestMethod]
        public void CreateEngineTest()
        {
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter((message) =>
            {
                Assert.AreEqual("Hello World!!" + Environment.NewLine, message);
            }));
            engine.EvalAsync("print('Hello World!!');").Wait();
        }

        private sealed class TraceEmitter : ILuaScriptEngineOutputEmitter
        {
            public TraceEmitter(Action<string>? action = null)
            {
                this.action = action;
            }

            private readonly Action<string>? action;

            public void Print(string message)
            {
                Trace.Write(message);
                if (action is not null)
                    action(message);
            }

            public void Error(Exception e)
            {
            }
        }

        [TestMethod]
        public void CreateEngineTest2()
        {
            CountdownEvent e = new CountdownEvent(1);

            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            factory.AddLibrary(new TestLibrary(e));
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter());
            engine.EvalAsync("print(test.test());").Wait();
            Assert.IsTrue(e.Wait(TimeSpan.FromSeconds(5)));
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
                tab.AddFunction("test", function);
                g.Add("test", tab);
            }

            private sealed class TestFunction : AbstractLuaFunction
            {
                public TestFunction(CountdownEvent e)
                {
                    this.e = e;
                }

                private readonly CountdownEvent e;

                public override LuaResult? Invoke(params object[] args)
                {
                    e.Signal(1);
                    return null;
                }
            }
        }

        [TestMethod]
        public void StopScriptTest()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CountdownEvent e = new CountdownEvent(5);
            LuaScriptEngineFactory factory = new LuaScriptEngineFactory();
            factory.AddFunction("sleep", new SleepFunction());
            using ILuaScriptEngine engine = factory.CreateEngine(new TraceEmitter((message) =>
            {
                e.Signal();
            }), cancellationTokenSource.Token);
            Task t = engine.EvalAsync("while(true) do print('Hello'); sleep(1000); end");
            e.Wait();
            cancellationTokenSource.Cancel();
            Assert.ThrowsException<TaskCanceledException>(() =>
            {
                try
                {
                    t.Wait();
                }
                catch (AggregateException ex)
                {
                    TaskCanceledException? taskCanceledException = ex.InnerExceptions.OfType<TaskCanceledException>().FirstOrDefault();
                    if (taskCanceledException is not null)
                        throw taskCanceledException;
                }
            });
        }
    }
}