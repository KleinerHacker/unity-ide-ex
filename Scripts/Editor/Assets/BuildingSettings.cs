using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditorEx.Runtime.editor_ex.Scripts.Runtime.Assets;
using UnityEngine;
using UnityEngine.Serialization;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Types;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Assets
{
    internal sealed class BuildingSettings : ProviderAsset<BuildingSettings>
    {
        #region Static Access

        public static BuildingSettings Singleton => GetSingleton("building", "building.asset", "Editor/Resources",
            BuildingSettingsFactory.Create);

        public static SerializedObject SerializedSingleton => GetSerializedSingleton("building", "building.asset",
            "Editor/Resources", BuildingSettingsFactory.Create);

        #endregion

        #region Inspector Data

        [SerializeField] private bool clean = true;

        [SerializeField] private bool showFolder = true;

        [SerializeField] private bool runTests = true;

        [SerializeField] private bool buildAssetBundles;

        [FormerlySerializedAs("targetName")] [SerializeField]
        private string appName;

        [SerializeField] private BuildingFlags buildingFlags = BuildingFlags.None;

        [SerializeField] private TargetPlatform selectedTargetPlatform = TargetPlatform.Windows;

        [SerializeField] private TargetArchitecture selectedTargetArchitecture = TargetArchitecture.ClientX64;

        [SerializeField] private ScriptingBackend selectedScriptingBackend = ScriptingBackend.Mono;

        [SerializeField] private int selectedGroup;

        [SerializeField] private BuildingGroupSettings<BuildingTargetSettingsWindows>[] windows =
            Array.Empty<BuildingGroupSettings<BuildingTargetSettingsWindows>>();

        [SerializeField] private BuildingGroupSettings<BuildingTargetSettingsLinux>[] linux =
            Array.Empty<BuildingGroupSettings<BuildingTargetSettingsLinux>>();

        [SerializeField] private BuildingGroupSettings<BuildingTargetSettingsMacOS>[] macOS =
            Array.Empty<BuildingGroupSettings<BuildingTargetSettingsMacOS>>();

        [SerializeField] private BuildingGroupSettings<BuildingTargetSettingsAndroid>[] android =
            Array.Empty<BuildingGroupSettings<BuildingTargetSettingsAndroid>>();

        [SerializeField] private BuildingGroupSettings<BuildingTargetSettingsIOS>[] ios =
            Array.Empty<BuildingGroupSettings<BuildingTargetSettingsIOS>>();

        [SerializeField] private BuildingGroupSettings<BuildingTargetSettingsWebGL>[] webGL =
            Array.Empty<BuildingGroupSettings<BuildingTargetSettingsWebGL>>();

        #endregion

        #region Properties

        internal bool Initialized { get; set; }

        public string AppName => appName;

        public bool Clean
        {
            get => clean;
            internal set => clean = value;
        }

        public bool ShowFolder
        {
            get => showFolder;
            internal set => showFolder = value;
        }

        public bool RunTests
        {
            get => runTests;
            internal set => runTests = value;
        }

        public bool BuildAssetBundles
        {
            get => buildAssetBundles;
            internal set => buildAssetBundles = value;
        }

        public BuildingFlags BuildingFlags
        {
            get => buildingFlags;
            internal set => buildingFlags = value;
        }

        public TargetPlatform SelectedTargetPlatform
        {
            get => selectedTargetPlatform;
            internal set => selectedTargetPlatform = value;
        }

        public TargetArchitecture SelectedTargetArchitecture
        {
            get => selectedTargetArchitecture;
            internal set => selectedTargetArchitecture = value;
        }

        public ScriptingBackend SelectedScriptingBackend
        {
            get => selectedScriptingBackend;
            internal set => selectedScriptingBackend = value;
        }

        public int SelectedGroup
        {
            get => selectedGroup;
            internal set => selectedGroup = value;
        }

        public BuildingGroupSettings<BuildingTargetSettingsWindows>[] Windows
        {
            get => windows;
            internal set => windows = value;
        }

        public BuildingGroupSettings<BuildingTargetSettingsLinux>[] Linux
        {
            get => linux;
            internal set => linux = value;
        }

        public BuildingGroupSettings<BuildingTargetSettingsMacOS>[] MacOS
        {
            get => macOS;
            internal set => macOS = value;
        }

        public BuildingGroupSettings<BuildingTargetSettingsAndroid>[] Android
        {
            get => android;
            internal set => android = value;
        }

        public BuildingGroupSettings<BuildingTargetSettingsIOS>[] IOS
        {
            get => ios;
            internal set => ios = value;
        }

        public BuildingGroupSettings<BuildingTargetSettingsWebGL>[] WebGL
        {
            get => webGL;
            internal set => webGL = value;
        }

        #endregion

        #region Builtin Methods

        private void OnEnable()
        {
            if (string.IsNullOrWhiteSpace(appName))
            {
                appName = Application.productName;
            }
        }

        #endregion
    }

    [Serializable]
    public abstract class BuildingGroupSettings
    {
        #region Inspector Data

        [SerializeField] private string name;

        [SerializeField] private string path;

        #endregion

        #region Properties

        public string Name
        {
            get => name;
            internal set => name = value;
        }

        public string Path
        {
            get => path;
            internal set => path = value;
        }

        #endregion
    }

    [Serializable]
    public sealed class BuildingGroupSettings<T> : BuildingGroupSettings where T : BuildingTargetSettings
    {
        #region Inspector Data

        [SerializeField] private T settings;

        #endregion

        #region Properties

        public T Settings
        {
            get => settings;
            internal set => settings = value;
        }

        #endregion
    }

    [Serializable]
    public abstract class BuildingTargetSettings
    {
        #region Inspector Data

        [Header("Commons")] private bool compress;

        [Header("IL2CPP")] [SerializeField] private IL2CPPBackend scriptingBackend = IL2CPPBackend.Debug;

#if UNITY_2021_2_OR_NEWER
        [SerializeField] private Il2CppCodeGeneration il2CPPCodeGeneration = Il2CppCodeGeneration.OptimizeSize;
#endif

        [Header("Defined Symbols")] [SerializeField]
        private string[] additionalScriptingDefineSymbols = Array.Empty<string>();

        [SerializeField] private string[] additionalCompilerArguments = Array.Empty<string>();

        [Header("Debugging")] [SerializeField] private bool developmentBuild;

        [SerializeField] private bool insertDebuggingSymbols;

        #endregion

        #region Properties

        public abstract TargetPlatform Platform { get; }

        public bool Compress
        {
            get => compress;
            internal set => compress = value;
        }

        public IL2CPPBackend ScriptingBackend
        {
            get => scriptingBackend;
            internal set => scriptingBackend = value;
        }

#if UNITY_2021_2_OR_NEWER
        public Il2CppCodeGeneration IL2CPPCodeGeneration
        {
            get => il2CPPCodeGeneration;
            internal set => il2CPPCodeGeneration = value;
        }
#endif

        public string[] AdditionalScriptingDefineSymbols
        {
            get => additionalScriptingDefineSymbols;
            internal set => additionalScriptingDefineSymbols = value;
        }

        public string[] AdditionalCompilerArguments
        {
            get => additionalCompilerArguments;
            internal set => additionalCompilerArguments = value;
        }

        public bool DevelopmentBuild
        {
            get => developmentBuild;
            internal set => developmentBuild = value;
        }

        public bool InsertDebuggingSymbols
        {
            get => insertDebuggingSymbols;
            internal set => insertDebuggingSymbols = value;
        }

        #endregion
    }

    [Serializable]
    public abstract class BuildingTargetSettingsDesktop : BuildingTargetSettings
    {
        #region Inspector Data

        [Header("Mono")] [SerializeField] private ManagedStrippingLevel strippingLevel = ManagedStrippingLevel.Disabled;

        #endregion

        #region Properties

        public ManagedStrippingLevel StrippingLevel
        {
            get => strippingLevel;
            internal set => strippingLevel = value;
        }

        #endregion
    }

    [Serializable]
    public abstract class BuildingTargetSettingsMobile : BuildingTargetSettings
    {
        #region Inspector Data

        [Header("Mono")] [SerializeField] private ManagedStrippingLevel strippingLevel = ManagedStrippingLevel.Disabled;

        #endregion

        #region Properties

        public ManagedStrippingLevel StrippingLevel
        {
            get => strippingLevel;
            internal set => strippingLevel = value;
        }

        #endregion
    }
}