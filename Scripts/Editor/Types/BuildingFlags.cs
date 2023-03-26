using System;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Types
{
    [Flags]
    public enum BuildingFlags
    {
        None = 0x0000,
        CodeCoverage = 0x0001,
        UseProfiler = 0x0002,
        StrictMode = 0x0004,
        WaitForConnection = 0x0010,
        ConnectToHost = 0x0020,
        DetailedReport = 0x0040,
        SymlinkSources = 0x0080,
    }
}