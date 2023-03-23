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
                    Settings = new BuildingTargetSettingsWindows
                    {
                        ScriptingBackend = IL2CPPBackend.Debug,
                        StrippingLevel = ManagedStrippingLevel.Disabled
                    }
                },
                new BuildingGroupSettings<BuildingTargetSettingsWindows>
                {
                    Name = "Release",
                    Settings = new BuildingTargetSettingsWindows
                    {
                        ScriptingBackend = IL2CPPBackend.Master,
                        StrippingLevel = ManagedStrippingLevel.Low
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
                    Settings = new BuildingTargetSettingsLinux
                    {
                        ScriptingBackend = IL2CPPBackend.Debug,
                        StrippingLevel = ManagedStrippingLevel.Disabled
                    }
                },
                new BuildingGroupSettings<BuildingTargetSettingsLinux>
                {
                    Name = "Release",
                    Settings = new BuildingTargetSettingsLinux
                    {
                        ScriptingBackend = IL2CPPBackend.Master,
                        StrippingLevel = ManagedStrippingLevel.Low
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
                    Settings = new BuildingTargetSettingsMacOS
                    {
                        ScriptingBackend = IL2CPPBackend.Debug,
                        StrippingLevel = ManagedStrippingLevel.Disabled
                    }
                },
                new BuildingGroupSettings<BuildingTargetSettingsMacOS>
                {
                    Name = "Release",
                    Settings = new BuildingTargetSettingsMacOS
                    {
                        ScriptingBackend = IL2CPPBackend.Master,
                        StrippingLevel = ManagedStrippingLevel.Low
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
                    Name = "Debug",
                    Settings = new BuildingTargetSettingsAndroid
                    {
                        ScriptingBackend = IL2CPPBackend.Debug,
                        StrippingLevel = ManagedStrippingLevel.Disabled
                    }
                },
                new BuildingGroupSettings<BuildingTargetSettingsAndroid>
                {
                    Name = "Release",
                    Settings = new BuildingTargetSettingsAndroid
                    {
                        ScriptingBackend = IL2CPPBackend.Master,
                        StrippingLevel = ManagedStrippingLevel.Low
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
                    Settings = new BuildingTargetSettingsIOS
                    {
                        ScriptingBackend = IL2CPPBackend.Debug,
                        StrippingLevel = ManagedStrippingLevel.Disabled
                    }
                },
                new BuildingGroupSettings<BuildingTargetSettingsIOS>
                {
                    Name = "Release",
                    Settings = new BuildingTargetSettingsIOS
                    {
                        ScriptingBackend = IL2CPPBackend.Master,
                        StrippingLevel = ManagedStrippingLevel.Low
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
                    Settings = new BuildingTargetSettingsWebGL
                    {
                        ScriptingBackend = IL2CPPBackend.Debug
                    }
                },
                new BuildingGroupSettings<BuildingTargetSettingsWebGL>
                {
                    Name = "Release",
                    Settings = new BuildingTargetSettingsWebGL
                    {
                        ScriptingBackend = IL2CPPBackend.Master
                    }
                }
            };
        }
    }
}