using System;
using UnityEditor;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Types
{
    public enum TargetPlatform
    {
        [SupportedSettings(ScriptingBackend.All, TargetArchitecture.All)]
        Windows,

        [SupportedSettings(ScriptingBackend.All, TargetArchitecture.ClientX64)]
        Linux,

        [SupportedSettings(ScriptingBackend.All, TargetArchitecture.ClientX64)]
        MacOS,

        [SupportedSettings(ScriptingBackend.All, TargetArchitecture.ClientX64)]
        Android,

        [SupportedSettings(ScriptingBackend.All, TargetArchitecture.ClientX64)]
        IOS,

        [SupportedSettings(ScriptingBackend.IL2CPP, TargetArchitecture.ClientX64)]
        WebGL
    }

    public class SupportedSettingsAttribute : Attribute
    {
        public ScriptingBackend ScriptingBackend { get; }
        public TargetArchitecture TargetArchitecture { get; }

        public SupportedSettingsAttribute(ScriptingBackend scriptingBackend, TargetArchitecture targetArchitecture)
        {
            ScriptingBackend = scriptingBackend;
            TargetArchitecture = targetArchitecture;
        }
    }

    public static class TargetPlatformExtensions
    {
        public static TargetPlatform ToTargetPlatform(this BuildTarget buildTarget)
        {
            return buildTarget switch
            {
                BuildTarget.StandaloneWindows or BuildTarget.StandaloneWindows64 => TargetPlatform.Windows,
                BuildTarget.StandaloneLinux64 => TargetPlatform.Linux,
                BuildTarget.StandaloneOSX => TargetPlatform.MacOS,
                BuildTarget.Android => TargetPlatform.Android,
                BuildTarget.iOS => TargetPlatform.IOS,
                BuildTarget.WebGL => TargetPlatform.WebGL,
                _ => TargetPlatform.Windows,
            };
        }
    }
}