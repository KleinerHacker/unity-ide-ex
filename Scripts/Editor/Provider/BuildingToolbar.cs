using System;
using System.Linq;
using System.Reflection;
using Unity.CodeEditor;
using UnityCommonEx.Runtime.common_ex.Scripts.Runtime.Utils.Extensions;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Assets;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Types;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Utils;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Windows;
using UnityToolbarExtender;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Provider
{
    [InitializeOnLoad]
    public static class BuildingToolbar
    {
        private static readonly GenericMenu BuildMenu = new GenericMenu();

        private static readonly BuildingSettings BuildingSettings;
        private static readonly SerializedObject BuildingSerializedObject;

        private static readonly AssetBundleSettings AssetBundleSettings;
        private static readonly SerializedObject AssetBundleSerializedObject;

        private static bool _blockRecompile;

        static BuildingToolbar()
        {
            BuildingSettings = BuildingSettings.Singleton;
            BuildingSerializedObject = BuildingSettings.SerializedSingleton;

            AssetBundleSettings = AssetBundleSettings.Singleton;
            AssetBundleSerializedObject = AssetBundleSettings.SerializedSingleton;

            ToolbarExtender.LeftToolbarGUI.Add(OnLeftToolbarGUI);
            ToolbarExtender.RightToolbarGUI.Insert(0, OnRightToolbarGUI);

            BuildMenu.AddItem(new GUIContent("Build"), false, () => Build(UnityBuilding.BuildBehavior.BuildOnly));
            BuildMenu.AddItem(new GUIContent("Build and Run"), false, () => Build(UnityBuilding.BuildBehavior.BuildAndRun));
            BuildMenu.AddSeparator(null);
            BuildMenu.AddItem(new GUIContent("Build only all Asset Bundles"), false, () => Build(UnityBuilding.BuildBehavior.BuildAssetBundleOnly));
            BuildMenu.AddItem(new GUIContent("Build only selected Asset Bundles..."), false, () => SelectAssetBundles(UnityBuilding.BuildBehavior.BuildAssetBundleOnly));
            BuildMenu.AddSeparator(null);
            BuildMenu.AddItem(new GUIContent("Run Tests"), false, RunTests);

            CompilationPipeline.compilationStarted += _ => _blockRecompile = true;
            CompilationPipeline.compilationFinished += _ => _blockRecompile = false;
        }

        private static void OnLeftToolbarGUI()
        {
            BuildingSerializedObject.Update();

            GUILayout.FlexibleSpace();

            GUILayout.Space(25f);

            if (!BuildingSettings.Initialized)
            {
                ResetSelectedTargets();
                BuildingSettings.Initialized = true;
            }

            LayoutTargetPlatform();
            var targetFieldInfo = typeof(TargetPlatform).GetField(BuildingSettings.SelectedTargetPlatform.ToString());
            var targetSettings = targetFieldInfo.GetCustomAttribute<SupportedSettingsAttribute>();

            LayoutTargetArchitecture();
            LayoutScriptingBackend();
            LayoutGroup();

            if (GUILayout.Button(new GUIContent("", (Texture2D)EditorGUIUtility.IconContent("d_Refresh").image, "Reset to active target"), ToolbarStyles.commandButtonStyle))
            {
                ResetSelectedTargets();
            }

            GUILayout.Space(10f);

            BuildingSettings.BuildingFlags = (BuildingFlags)EditorGUILayout.EnumFlagsField(BuildingSettings.BuildingFlags, ToolbarStyles.popupStyle, ToolbarLayouts.popupSmallLayout);

            GUILayout.Space(10f);

            var newClean = GUILayout.Toggle(BuildingSettings.Clean, new GUIContent(EditorGUIUtility.IconContent("Grid.EraserTool").image, "Clean complete build cache"), ToolbarStyles.commandButtonStyle);
            if (newClean != BuildingSettings.Clean)
            {
                BuildingSettings.Clean = newClean;
                EditorUtility.SetDirty(BuildingSettings);
            }

            var newShowFolder = GUILayout.Toggle(BuildingSettings.ShowFolder, new GUIContent(EditorGUIUtility.IconContent("d_FolderOpened Icon").image, "Open the build folder"), ToolbarStyles.commandButtonStyle);
            if (newShowFolder != BuildingSettings.ShowFolder)
            {
                BuildingSettings.ShowFolder = newShowFolder;
                EditorUtility.SetDirty(BuildingSettings);
            }

            var newBuildAssetBundles = GUILayout.Toggle(BuildingSettings.BuildAssetBundles, new GUIContent(EditorGUIUtility.IconContent("d_Profiler.NetworkOperations").image, "Build Asset Bundles"), ToolbarStyles.commandButtonStyle);
            if (newBuildAssetBundles != BuildingSettings.BuildAssetBundles)
            {
                BuildingSettings.BuildAssetBundles = newBuildAssetBundles;
                EditorUtility.SetDirty(BuildingSettings);
            }

            var newRunTests = GUILayout.Toggle(BuildingSettings.RunTests, new GUIContent(EditorGUIUtility.IconContent("FilterSelectedOnly").image, "Run tests before build starts"), ToolbarStyles.commandButtonStyle);
            if (newRunTests != BuildingSettings.RunTests)
            {
                BuildingSettings.RunTests = newRunTests;
                EditorUtility.SetDirty(BuildingSettings);
            }

            BuildingSerializedObject.ApplyModifiedProperties();

            void LayoutTargetPlatform()
            {
                var targetPlatforms = Enum.GetValues(typeof(TargetPlatform))
                    .Cast<TargetPlatform>().ToArray();

                var targetPlatformIndex = targetPlatforms.IndexOf(x => x == BuildingSettings.SelectedTargetPlatform);
                var newTargetPlatformIndex = EditorGUILayout.Popup(targetPlatformIndex, targetPlatforms.Select(x => x.ToString()).ToArray(),
                    ToolbarStyles.popupStyle, ToolbarLayouts.popupSmallLayout);
                if (targetPlatformIndex != newTargetPlatformIndex)
                {
                    BuildingSettings.SelectedTargetPlatform = newTargetPlatformIndex >= 0 ? targetPlatforms[newTargetPlatformIndex] : TargetPlatform.Windows;
                    EditorUtility.SetDirty(BuildingSettings);
                }
            }

            void LayoutTargetArchitecture()
            {
                var targetArchitectures = Enum.GetValues(typeof(TargetArchitecture))
                    .Cast<TargetArchitecture>()
                    .Where(x => x is not (TargetArchitecture.All or TargetArchitecture.None or TargetArchitecture.ClientAll or TargetArchitecture.ServerAll))
                    .Where(x => targetSettings.TargetArchitecture.HasFlag(x))
                    .ToArray();

                var architectureIndex = targetArchitectures.IndexOf(x => x == BuildingSettings.SelectedTargetArchitecture);
                var newArchitectureIndex = EditorGUILayout.Popup(architectureIndex, targetArchitectures.Select(x => x.ToString()).ToArray(),
                    ToolbarStyles.popupStyle, ToolbarLayouts.popupSmallLayout);
                if (architectureIndex != newArchitectureIndex)
                {
                    BuildingSettings.SelectedTargetArchitecture = newArchitectureIndex >= 0 ? targetArchitectures[newArchitectureIndex] : TargetArchitecture.ClientX64;
                    EditorUtility.SetDirty(BuildingSettings);
                }
            }

            void LayoutScriptingBackend()
            {
                var scriptingBackends = Enum.GetValues(typeof(ScriptingBackend))
                    .Cast<ScriptingBackend>()
                    .Where(x => x is not (ScriptingBackend.All or ScriptingBackend.None))
                    .Where(x => targetSettings.ScriptingBackend.HasFlag(x))
                    .ToArray();

                var scriptingBackendIndex = scriptingBackends.IndexOf(x => x == BuildingSettings.SelectedScriptingBackend);
                var newScriptingBackendIndex = EditorGUILayout.Popup(scriptingBackendIndex, scriptingBackends.Select(x => x.ToString()).ToArray(),
                    ToolbarStyles.popupStyle, ToolbarLayouts.popupSmallLayout);
                if (scriptingBackendIndex != newScriptingBackendIndex)
                {
                    BuildingSettings.SelectedScriptingBackend = newScriptingBackendIndex >= 0 ? scriptingBackends[newScriptingBackendIndex] : ScriptingBackend.Mono;
                    EditorUtility.SetDirty(BuildingSettings);
                }
            }

            void LayoutGroup()
            {
                var names = BuildingSettings.SelectedTargetPlatform switch
                {
                    TargetPlatform.Windows => BuildingSettings.Windows.Select(x => x.Name).ToArray(),
                    TargetPlatform.Linux => BuildingSettings.Linux.Select(x => x.Name).ToArray(),
                    TargetPlatform.MacOS => BuildingSettings.MacOS.Select(x => x.Name).ToArray(),
                    TargetPlatform.Android => BuildingSettings.Android.Select(x => x.Name).ToArray(),
                    TargetPlatform.IOS => BuildingSettings.IOS.Select(x => x.Name).ToArray(),
                    TargetPlatform.WebGL => BuildingSettings.WebGL.Select(x => x.Name).ToArray(),
                    _ => throw new ArgumentOutOfRangeException()
                };

                var newIndex = EditorGUILayout.Popup(BuildingSettings.SelectedGroup, names,
                    ToolbarStyles.popupStyle, ToolbarLayouts.popupSmallLayout);
                if (newIndex != BuildingSettings.SelectedGroup)
                {
                    BuildingSettings.SelectedGroup = newIndex;
                    EditorUtility.SetDirty(BuildingSettings);
                }
            }

            void ResetSelectedTargets()
            {
                BuildingSettings.SelectedTargetPlatform = EditorUserBuildSettings.activeBuildTarget.ToTargetPlatform();
                BuildingSettings.SelectedTargetArchitecture = EditorUserBuildSettings.activeBuildTarget
                    .ToTargetArchitecture(EditorUserBuildSettings.standaloneBuildSubtarget);
                BuildingSettings.SelectedScriptingBackend = PlayerSettings.GetIncrementalIl2CppBuild(EditorUserBuildSettings.selectedBuildTargetGroup)
                    ? ScriptingBackend.IL2CPP
                    : ScriptingBackend.Mono;
                EditorUtility.SetDirty(BuildingSettings);
            }
        }

        private static void OnRightToolbarGUI()
        {
            BuildingSerializedObject.Update();

            if (GUILayout.Button(new GUIContent("", (Texture2D)EditorGUIUtility.IconContent("winbtn_win_restore").image, "Open Project"), ToolbarStyles.commandButtonStyle))
            {
                CodeEditor.CurrentEditor.OpenProject();
            }

            GUILayout.Space(10f);

            EditorGUI.BeginDisabledGroup(_blockRecompile);
            if (GUILayout.Button(new GUIContent("", (Texture2D)EditorGUIUtility.IconContent("preAudioLoopOff").image, "Rebuild Scripts"), ToolbarStyles.commandButtonStyle))
            {
                _blockRecompile = true;
                CompilationPipeline.RequestScriptCompilation(RequestScriptCompilationOptions.CleanBuildCache);
            }

            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button(new GUIContent("", (Texture2D)EditorGUIUtility.IconContent("d_Settings").image, "Build the project"), ToolbarStyles.commandButtonStyle))
            {
                BuildMenu.ShowAsContext();
            }

            BuildingSerializedObject.ApplyModifiedProperties();
        }

        private static void Build(UnityBuilding.BuildBehavior behavior)
        {
            AssetDatabase.SaveAssets();
            switch (BuildingSettings.SelectedTargetPlatform)
            {
                case TargetPlatform.Windows:
                    UnityBuilding.Build(behavior, BuildingSettings.Windows[BuildingSettings.SelectedGroup]);
                    break;
                case TargetPlatform.Linux:
                    UnityBuilding.Build(behavior, BuildingSettings.Linux[BuildingSettings.SelectedGroup]);
                    break;
                case TargetPlatform.MacOS:
                    UnityBuilding.Build(behavior, BuildingSettings.MacOS[BuildingSettings.SelectedGroup]);
                    break;
                case TargetPlatform.Android:
                    UnityBuilding.Build(behavior, BuildingSettings.Android[BuildingSettings.SelectedGroup]);
                    break;
                case TargetPlatform.IOS:
                    UnityBuilding.Build(behavior, BuildingSettings.IOS[BuildingSettings.SelectedGroup]);
                    break;
                case TargetPlatform.WebGL:
                    UnityBuilding.Build(behavior, BuildingSettings.WebGL[BuildingSettings.SelectedGroup]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void RunTests()
        {
            AssetDatabase.SaveAssets();

            var buildingSettings = BuildingSettings.Singleton;
            switch (buildingSettings.SelectedTargetPlatform)
            {
                case TargetPlatform.Windows:
                    UnityTesting.RunTests(buildingSettings.Windows[buildingSettings.SelectedGroup], true);
                    break;
                case TargetPlatform.Linux:
                    UnityTesting.RunTests(buildingSettings.Linux[buildingSettings.SelectedGroup], true);
                    break;
                case TargetPlatform.MacOS:
                    UnityTesting.RunTests(buildingSettings.MacOS[buildingSettings.SelectedGroup], true);
                    break;
                case TargetPlatform.Android:
                    UnityTesting.RunTests(buildingSettings.Android[buildingSettings.SelectedGroup], true);
                    break;
                case TargetPlatform.IOS:
                    UnityTesting.RunTests(buildingSettings.IOS[buildingSettings.SelectedGroup], true);
                    break;
                case TargetPlatform.WebGL:
                    UnityTesting.RunTests(buildingSettings.WebGL[buildingSettings.SelectedGroup], true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void SelectAssetBundles(UnityBuilding.BuildBehavior behavior)
        {
            var window = ScriptableObject.CreateInstance<AssetBundleBuildWindow>();
            window.ShowModalUtility();
            if (!window.Result)
                return;

            AssetDatabase.SaveAssets();
            //UnityBuilding.Build(behavior, assetData: new AssetData { BuildStates = window.BuildStates });
        }

        private static class ToolbarLayouts
        {
            public static readonly GUILayoutOption[] popupLayout;
            public static readonly GUILayoutOption[] popupSmallLayout;

            static ToolbarLayouts()
            {
                popupLayout = new[]
                {
                    GUILayout.Width(150f)
                };
                popupSmallLayout = new[]
                {
                    GUILayout.Width(100f)
                };
            }
        }

        private static class ToolbarStyles
        {
            public static readonly GUIStyle commandButtonStyle;
            public static readonly GUIStyle popupStyle;
            public static readonly GUIStyle labelStyle;
            public static readonly GUIStyle toggleStyle;

            static ToolbarStyles()
            {
                commandButtonStyle = "AppCommand";

                popupStyle = new GUIStyle("Popup")
                {
                    fontSize = 12,
                    alignment = TextAnchor.MiddleLeft,
                    imagePosition = ImagePosition.TextOnly,
                    fontStyle = FontStyle.Normal,
                    stretchWidth = false,
                    fixedHeight = 20f,
                    margin = new RectOffset(5, 5, 5, 5)
                };

                labelStyle = new GUIStyle("Label")
                {
                    fontSize = 12,
                    alignment = TextAnchor.MiddleLeft,
                    imagePosition = ImagePosition.TextOnly,
                    fontStyle = FontStyle.Normal,
                    fixedHeight = 20f,
                    wordWrap = false,
                    margin = new RectOffset(5, 5, 5, 5)
                };

                toggleStyle = new GUIStyle("Toggle")
                {
                    fontSize = 12,
                    alignment = TextAnchor.MiddleLeft,
                    imagePosition = ImagePosition.TextOnly,
                    fontStyle = FontStyle.Normal,
                    fixedHeight = 20f,
                    wordWrap = false,
                    margin = new RectOffset(5, 5, 5, 5)
                };
            }
        }

        private sealed class EditorToolDelegate : EditorTool
        {
            private Action _action;
            private GUIContent _guiContent = new GUIContent();

            public override GUIContent toolbarIcon => _guiContent;

            public void Setup(Texture2D icon, String tooltip, Action action)
            {
                _guiContent = new GUIContent(icon, tooltip);
                _action = action;
            }

            public override void OnActivated() => _action?.Invoke();
        }
    }
}