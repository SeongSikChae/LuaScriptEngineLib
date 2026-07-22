using System.Runtime.CompilerServices;

namespace Neo.IronLua
{
    internal static class LuaCompileOptionsExtensions
    {
        private sealed class SendBoxState
        {
            public bool Enabled { get; set; }
        }

        private static readonly ConditionalWeakTable<LuaCompileOptions, SendBoxState> sendboxState = new ConditionalWeakTable<LuaCompileOptions, SendBoxState>();

        extension(LuaCompileOptions options)
        {
            public bool SendboxEnabled 
            {
                get => sendboxState.GetOrAdd(options, static _ => new SendBoxState { Enabled = true }).Enabled;
                set => sendboxState.GetOrAdd(options, static _ => new SendBoxState()).Enabled = value;
            }
        }
    }
}
