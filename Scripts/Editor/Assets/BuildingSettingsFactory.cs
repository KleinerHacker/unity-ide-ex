using UnityEditor;
using UnityEngine;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Types;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Assets
{
    internal static class BuildingSettingsFactory
    {
        public static BuildingSettings Create()
        {
            var settings = ScriptableObject.CreateInstance<BuildingSettings>();
            CreateForWindows(settings);
            CreateForLinux(settings);
            CreateForMacOS(settings);
            CreateForAndroid(settings);
            CreateForIOS(settings);
            CreateForWebGL(settings);

            return settings;
        }

        private static void CreateForWindows(BuildingSettings settings)
        {
            settings.Windows = new[]
            {
                new BuildingGroupSettings<BuildingTargetSettingsWindows>
                {
                    Name = "Debug",
                    Path = "Debug",
                    Settings = new BuildingTargetSettingsWindows
                    {
                        ScriptingBackend = IL2CPPBackend.Debug,
                        StrippingLevel = ManagedStrippingLevel.Disabled,
                        DevelopmentBuild = true,
                        InsertDebuggingSymbols = true,
                        Compress = false,
                    }
                },
                new BuildingGroupSettings<BuildingTargetSettingsWindows>
                {
                    Name = "Release",
                    Path = "Release",
                    Settings = new BuildingTargetSettingsWindows
                    {
                        ScriptingBackend = IL2CPPBackend.Master,
                        StrippingLevel = ManagedStrippingLevel.Low,
                        DevelopmentBuild = false,
                        InsertDebuggingSymbols = false,
                        Compress = true,
                    }
                }
            };
        }

        private static void CreateForLinux(BuildingSettings settings)
        {
            settings.Linux = new[]
            {
                new BuildingGroupSettings<BuildingTargetSettingsLinux>
                {
                    Name = "Debug",
                    Path = "Debug",
                    Settings = new BuildingTargetSettingsLinux
                    {
                        ScriptingBackend = IL2CPPBackend.Debug,
                        StrippingLevel = ManagedStrippingLevel.Disabled,
                        DevelopmentBuild = true,
                        InsertDebuggingSymbols = true,
                        Compress = false,
                    }
                },
                new BuildingGroupSettings<BuildingTargetSettingsLinux>
                {
                    Name = "Release",
                    Path = "Release",
                    Settings = new BuildingTargetSettingsLinux
                    {
                        ScriptingBackend = IL2CPPBackend.Master,
                        StrippingLevel = ManagedStrippingLevel.Low,
                        DevelopmentBuild = false,
                        InsertDebuggingSymbols = false,
                        Compress = true,
                    }
                }
            };
        }

        private static void CreateForMacOS(BuildingSettings settings)
        {
            settings.MacOS = new[]
            {
                new BuildingGroupSettings<BuildingTargetSettingsMacOS>
                {
                    Name = "Debug",
                    Path = "Debug",
                    Settings = new BuildingTargetSettingsMacOS
                    {
                        ScriptingBackend = IL2CPPBackend.Debug,
                        StrippingLevel = ManagedStrippingLevel.Disabled,
                        DevelopmentBuild = true,
                        InsertDebuggingSymbols = true,
                        Compress = false,
                    }
                },
                new BuildingGroupSettings<BuildingTargetSettingsMacOS>
                {
                    Name = "Release",
                    Path = "Release",
                    Settings = new BuildingTargetSettingsMacOS
                    {
                        ScriptingBackend = IL2CPPBackend.Master,
                        StrippingLevel = ManagedStrippingLevel.Low,
                        DevelopmentBuild = false,
                        InsertDebuggingSymbols = false,
                        Compress = true,
                    }
                }
            };
        }

        private static void CreateForAndroid(BuildingSettings settings)
        {
            settings.Android = new[]
            {
                new BuildingGroupSettings<BuildingTargetSettingsAndroid>
                {
                    Name = "Debug (*.apk)",
                    Path = "Debug",
                    Settings = new BuildingTargetSettingsAndroid
                    {
                        ScriptingBackend = IL2CPPBackend.Debug,
                        StrippingLevel = ManagedStrippingLevel.Disabled,
                        TargetArchive = AndroidTargetArchive.ApplicationPackage,
                        TargetAndroidArchitecture = AndroidArchitecture.X86_64,
                        DevelopmentBuild = true,
                        InsertDebuggingSymbols = true,
                        Compress = false,
                    }
                },
                new BuildingGroupSettings<BuildingTargetSettingsAndroid>
                {
                    Name = "Release (*.aab)",
                    Path = "Release",
                    Settings = new BuildingTargetSettingsAndroid
                    {
                        ScriptingBackend = IL2CPPBackend.Master,
                        StrippingLevel = ManagedStrippingLevel.Low,
                        TargetArchive = AndroidTargetArchive.ApplicationBundle,
                        TargetAndroidArchitecture = AndroidArchitecture.All,
                        DevelopmentBuild = false,
                        InsertDebuggingSymbols = false,
                        Compress = true,
                    }
                }
            };
        }

        private static void CreateForIOS(BuildingSettings settings)
        {
            settings.IOS = new[]
            {
                new BuildingGroupSettings<BuildingTargetSettingsIOS>
                {
                    Name = "Debug",
                    Path = "Debug",
                    Settings = new BuildingTargetSettingsIOS
                    {
                        ScriptingBackend = IL2CPPBackend.Debug,
                        StrippingLevel = ManagedStrippingLevel.Disabled,
                        DevelopmentBuild = true,
                        InsertDebuggingSymbols = true,
                        Compress = false,
                    }
                },
                new BuildingGroupSettings<BuildingTargetSettingsIOS>
                {
                    Name = "Release",
                    Path = "Release",
                    Settings = new BuildingTargetSettingsIOS
                    {
                        ScriptingBackend = IL2CPPBackend.Master,
                        StrippingLevel = ManagedStrippingLevel.Low,
                        DevelopmentBuild = false,
                        InsertDebuggingSymbols = false,
                        Compress = true,
                    }
                }
            };
        }

        private static void CreateForWebGL(BuildingSettings settings)
        {
            settings.WebGL = new[]
            {
                new BuildingGroupSettings<BuildingTargetSettingsWebGL>
                {
                    Name = "Debug",
                    Path = "Debug",
                    Settings = new BuildingTargetSettingsWebGL
                    {
                        ScriptingBackend = IL2CPPBackend.Debug,
                        DevelopmentBuild = true,
                        InsertDebuggingSymbols = true,
                        Compress = false,
                    }
                },
                new BuildingGroupSettings<BuildingTargetSettingsWebGL>
                {
                    Name = "Release",
                    Path = "Release",
                    Settings = new BuildingTargetSettingsWebGL
                    {
                        ScriptingBackend = IL2CPPBackend.Master,
                        DevelopmentBuild = false,
                        InsertDebuggingSymbols = false,
                        Compress = true,
                    }
                }
            };
        }
    }
}