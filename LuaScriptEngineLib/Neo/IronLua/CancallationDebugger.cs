namespace Neo.IronLua
{
    internal sealed class CancallationDebugger(CancellationToken cancellationToken) : LuaTraceLineDebugger
    {
        protected override void OnTracePoint(LuaTraceLineEventArgs e)
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();
            base.OnTracePoint(e);
        }
    }
}
