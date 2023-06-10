using Neo.IronLua;

namespace LuaScriptEngineLib
{
    internal class CancallationDebugEngine : LuaTraceLineDebugger
    {
        public CancallationDebugEngine(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
        }

        private readonly CancellationToken cancellationToken;

        protected override void OnTracePoint(LuaTraceLineEventArgs e)
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();
            base.OnTracePoint(e);
        }
    }
}
