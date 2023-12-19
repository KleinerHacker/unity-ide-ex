using System;
using UnityEditor;
using UnityEditor.Build;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Assets;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Types;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Utils.Extensions
{
    internal static class BuildingSettingsExtensions
    {
        public static BuildTarget ToBuildTarget(this BuildingSettings settings)
        {
            return settings.SelectedTargetPlatform switch
            {
                TargetPlatform.Windows => settings.SelectedTargetArchitecture switch
                {
                    TargetArchitecture.ServerX86 => BuildTarget.StandaloneWindows,
                    TargetArchitecture.ClientX86 => BuildTarget.StandaloneWindows,
                    TargetArchitecture.ClientX64 => BuildTarget.StandaloneWindows64,
                    TargetArchitecture.ServerX64 => BuildTarget.StandaloneWindows64,
                    _ => throw new ArgumentOutOfRangeException()
                },
                TargetPlatform.Linux => BuildTarget.StandaloneLinux64,
                TargetPlatform.MacOS => BuildTarget.StandaloneOSX,
                TargetPlatform.Android => BuildTarget.Android,
                TargetPlatform.IOS => BuildTarget.iOS,
                TargetPlatform.WebGL => BuildTarget.WebGL,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static BuildTargetGroup ToBuildTargetGroup(this BuildingSettings settings)
        {
            return settings.SelectedTargetPlatform switch
            {
                TargetPlatform.Windows => BuildTargetGroup.Standalone,
                TargetPlatform.Linux => BuildTargetGroup.Standalone,
                TargetPlatform.MacOS => BuildTargetGroup.Standalone,
                TargetPlatform.Android => BuildTargetGroup.Android,
                TargetPlatform.IOS => BuildTargetGroup.iOS,
                TargetPlatform.WebGL => BuildTargetGroup.WebGL,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static BuildingGroupSettings GetSelectedGroupSettings(this BuildingSettings settings)
        {
            return settings.SelectedTargetPlatform switch
            {
                TargetPlatform.Windows => settings.Windows[settings.SelectedGroup],
                TargetPlatform.Linux => settings.Linux[settings.SelectedGroup],
                TargetPlatform.MacOS => settings.MacOS[settings.SelectedGroup],
                TargetPlatform.Android => settings.Android[settings.SelectedGroup],
                TargetPlatform.IOS => settings.IOS[settings.SelectedGroup],
                TargetPlatform.WebGL => settings.WebGL[settings.SelectedGroup],
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static BuildingTargetSettings GetSelectedTargetSettings(this BuildingSettings settings)
        {
            return settings.SelectedTargetPlatform switch
            {
                TargetPlatform.Windows => settings.Windows[settings.SelectedGroup].Settings,
                TargetPlatform.Linux => settings.Linux[settings.SelectedGroup].Settings,
                TargetPlatform.MacOS => settings.MacOS[settings.SelectedGroup].Settings,
                TargetPlatform.Android => settings.Android[settings.SelectedGroup].Settings,
                TargetPlatform.IOS => settings.IOS[settings.SelectedGroup].Settings,
                TargetPlatform.WebGL => settings.WebGL[settings.SelectedGroup].Settings,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}