using System;
using UnityEditor;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Types
{
    [Flags]
    public enum TargetArchitecture
    {
        None = 0x00,

        ClientX86 = 0x01,
        ClientX64 = 0x02,
        ClientAll = ClientX86 | ClientX64,

        ServerX86 = 0x10,
        ServerX64 = 0x20,
        ServerAll = ServerX86 | ServerX64,

        All = ClientAll | ServerAll,
    }

    public static class TargetArchitectureExtensions
    {
        public static TargetArchitecture ToTargetArchitecture(this BuildTarget buildTarget, StandaloneBuildSubtarget subtarget)
        {
            return buildTarget switch
            {
                BuildTarget.StandaloneWindows64 or BuildTarget.StandaloneLinux64 or BuildTarget.StandaloneOSX =>
                    subtarget == StandaloneBuildSubtarget.Server ? TargetArchitecture.ServerX64 : TargetArchitecture.ClientX64,
                BuildTarget.StandaloneWindows =>
                    subtarget == StandaloneBuildSubtarget.Server ? TargetArchitecture.ServerX86 : TargetArchitecture.ClientX86,
                BuildTarget.Android or BuildTarget.iOS or BuildTarget.WebGL =>
                    TargetArchitecture.ClientX64,

                _ => TargetArchitecture.ClientX86
            };
        }
    }
}