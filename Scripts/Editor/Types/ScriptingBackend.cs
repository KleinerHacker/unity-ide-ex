using System;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Types
{
    public enum IL2CPPBackend
    {
        Debug = 0x10,
        Release = 0x11,
        Master = 0x12,
    }

    [Flags]
    public enum ScriptingBackend
    {
        None = 0x00,
        Mono = 0x01,
        IL2CPP = 0x02,
        All = Mono | IL2CPP,
    }
}